using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Gstc.Collections.ObservableLists.Multithread;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListReentrancyTest {

    /// <summary>
    /// An array of lists that will be tested 
    /// </summary>
    public static object[] StaticDataSource => new object[] {
        new ObservableList<string>(),
        new ObservableIList<string, List<string>>(),
        new ObservableIListLocking<string,List<string>>()
    };

    [Test, Description("Tests that reentrancy is allowed if AllowReentrancy is set to true")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void ReentrancySucceedTest(IObservableCollection<string> list) {
        int reentrancyCounter = 0;
        try {
            list.AllowReentrancy = true;
            list.CollectionChanged += (_, args) => {
                if (list.Count > 10) return;
                if (args.Action == NotifyCollectionChangedAction.Add) list.Add("This add should cause reentrancy.");
                reentrancyCounter++;
            };
            list.Add("My reentrancy trigger");
        }
        catch (NotSupportedException e) { Console.WriteLine(e.Message); } //allows not NotSupportedException if your list isn't compatible with AllowReentrancy
        Console.WriteLine(reentrancyCounter);
    }

    [Test, Description("Tests that reentrancy triggers an error if AllowReentrancy is set to false")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void ReentrancyFailTest(IObservableCollection<string> list) {
        list.AllowReentrancy = false;
        list.CollectionChanged += (_, args) => {
            if (list.Count > 10) return;
            if (args.Action == NotifyCollectionChangedAction.Add) list.Add("This add should cause reentrancy.");
        };
        _ = Assert.Throws<InvalidOperationException>(() => list.Add("This add should start test."));
    }

    [Test, Description("Test for locking on multithread reentrancy when chaining thread calls.")]
    public void ReentrancyMultithreadTest() {
        ObservableIListLocking<Thread, List<Thread>> list = new();
        int callCount = 0;

        void AddList() { // Callbacks to create new threads recursively.
            if (callCount > 10) return;
            _ = Interlocked.Increment(ref callCount);
            Thread newThread = new(AddList);
            Console.WriteLine("Approaching lock:" + Environment.CurrentManagedThreadId);
            list.Add(newThread);
            newThread.Join();
            Console.WriteLine("Thread exiting:" + Environment.CurrentManagedThreadId);
        }

        list.CollectionChanged += (_, args) => {
            Console.WriteLine("Enter Lock:" + Environment.CurrentManagedThreadId);
            if (args.NewItems![0] is not Thread thread) throw new NullReferenceException("Not a thread");
            thread.Start();
            Thread.Sleep(50); //Allows started thread to hit lock before releasing lock.
            Console.WriteLine("Exit Lock:" + Environment.CurrentManagedThreadId);
        };
        AddList();
        Console.WriteLine("Count of Threads: " + callCount);
    }

    //TODO: Verify this works.
    [Test, Description("Test for locking on multithread reentrancy using Task")]
    public async Task ReentrancyMultithreadTest_Task() {
        ObservableIListLocking<Task, List<Task>> list = new();
        int callCount = 0;

        //creates stack of threads to access 
        async Task<(int start, int end)> AddListAsync() {
            //await Task.Delay(5000); //Allows started thread to hit lock before releasing lock.
            DateTime start = DateTime.Now;
            if (callCount > 10) return new(start.Millisecond, DateTime.Now.Millisecond);
            int currentCount = Interlocked.Increment(ref callCount);
            Console.WriteLine("currentCount:" + currentCount);
            Task<(int start, int end)> newTask = new(() => { return (0, 0); });
            //Console.WriteLine("Approaching lock:" + Environment.CurrentManagedThreadId);
            list.Add(newTask);
            (int start, int end) priorTime = await newTask;

            //Console.WriteLine("Thread exiting:" + Environment.CurrentManagedThreadId);
            return new(start.Millisecond, DateTime.Now.Millisecond);
        }

        list.CollectionChanged += (_, args) => {
            //Console.WriteLine("Enter Lock:" + Environment.CurrentManagedThreadId);
            if (args.NewItems![0] is not Task newTask) throw new NullReferenceException("Not a task.");
            newTask.Start();
            //Console.WriteLine("Exit Lock:" + Environment.CurrentManagedThreadId);
        };

        Task<(int start, int end)> time = AddListAsync();
        await time;
        Console.WriteLine("Count of Threads: " + callCount);
    }

    //Todo: finish this test and then delete.
    [Test]
    public async Task Temp() {
        static async Task Wait() {
            Console.WriteLine("start:" + DateTime.Now.Millisecond);
            await Task.Delay(500);
            Console.WriteLine("end:" + DateTime.Now.Millisecond);
        }

        Task task = new(() => Console.WriteLine("hi"));
        Task task2 = new(() => {
            Console.WriteLine("start:" + DateTime.Now.Millisecond);
            Task.Delay(500);
            Console.WriteLine("end:" + DateTime.Now.Millisecond);
        });
        //task2.Start();
        await Wait();
        Console.WriteLine("exit:" + DateTime.Now.Millisecond);
    }
    //TODO: Add unit test for Task instead similar to Thread.
}
