using System;
using System.Collections.Generic;
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
    public void ReentrancySuccess_AddAllowReentrancyTrue_ReentrancyIsAllowed(IObservableCollection<string> obvList) {
        int reentrancyCounter = 0;

        //ObservableIListLocking does not support single thread reentrnacy
        if (obvList is ObservableIListLocking<string, List<string>>) {
            NotSupportedException e = Assert.Throws<NotSupportedException>(() => obvList.AllowReentrancy = true);
            Console.WriteLine(e.Message);
            return;
        }
        obvList.AllowReentrancy = true;
        obvList.CollectionChanged += (_, args) => {
            if (obvList.Count > 10) return;
            obvList.Add("Reentrancy trigger");
            reentrancyCounter++;
        };

        obvList.Add("Event trigger");

        Assert.That(reentrancyCounter, Is.EqualTo(10));
        Console.WriteLine(reentrancyCounter);
    }

    [Test, Description("Tests that reentrancy triggers an error if AllowReentrancy is set to false")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void ReentrancyFailure_AddAllowReentrancyFalse_ReentrancyThrowsException(IObservableCollection<string> obvList) {

        obvList.AllowReentrancy = false;

        obvList.CollectionChanged += (_, args) => {
            if (obvList.Count > 10) return;
            obvList.Add("Reentrancy trigger");
        };
        InvalidOperationException e = Assert.Throws<InvalidOperationException>(() => obvList.Add("Event trigger"));
        Console.WriteLine(e.Message);
    }

    [Test, Description("Tests that ObservableIListLocking allows multithread access to list without an error.")]
    [Timeout(1000)]
    public void ReentrancyMultithreadSucess_MultipleThreadsRaisingEvents_Success() {
        List<Thread> threadList = new();
        ObservableIListLocking<int, List<int>> obvList = new();
        bool lastThreadExecuted = false;
        int collectionChangedCounter = 0;

        obvList.CollectionChanged += (_, _) => {
            while (!lastThreadExecuted) Thread.Sleep(1); //Spinlock until all events queued behind lock.
            collectionChangedCounter++;
        };

        for (int index = 0; index < 10; index++) { //Queues multiple adds up to lock.
            Thread newThread = new(() => obvList.Add(index));
            newThread.Start();
            threadList.Add(newThread);
        }

        Thread.Sleep(100);
        Assert.That(collectionChangedCounter, Is.EqualTo(0));
        lastThreadExecuted = true; //Releases spin lock.
        threadList.ForEach(thread => thread.Join()); //Keeps test running until all threads release.
        Assert.That(collectionChangedCounter, Is.EqualTo(10));
    }

    [Test, Description("Tests that ObservableIListLocking allows multithread access to list without an error.")]
    [Timeout(1000)]
    public void ReentrancyMultithreadSucess_MultipleTasksRaisingEvents_Success() {
        List<Task> taskList = new();
        ObservableIListLocking<int, List<int>> obvList = new();
        bool lastThreadExecuted = false;
        int collectionChangedCounter = 0;

        obvList.CollectionChanged += (_, _) => {
            while (!lastThreadExecuted) Thread.Sleep(1); //Spinlock until all events queued behind lock.
            collectionChangedCounter++;
        };

        for (int index = 0; index < 10; index++) { //Queues multiple adds up to lock.
            Task task = Task.Run(() => obvList.Add(index));
            taskList.Add(task);
        }

        Thread.Sleep(100);
        Assert.That(collectionChangedCounter, Is.EqualTo(0));
        lastThreadExecuted = true; //Releases spin lock.
        Task.WaitAll(taskList.ToArray());//Keeps test running until all threads release.
        Assert.That(collectionChangedCounter, Is.EqualTo(10));
    }

    [Test, Description("Tests the locking of the ObservableIListLocking.")]
    public void MultiThread_AddOperationWithLocks_AddsAreAtomic() {
        ObservableIListLocking<string, List<string>> obvList = new();

        List<Task> taskList = new();
        Random rand = new(0);

        //Event demonstrates that count of list does not change while locked.
        obvList.Adding += (sender, args) => {
            int initialCount = obvList.Count;
            Thread.Sleep(rand.Next(10));
            int finalCount = obvList.Count;
            if (initialCount != finalCount) throw new TimeoutException("Race condition detected.");
        };

        for (int index = 0; index < 10; index++) {
            Task task = Task.Run(() => obvList.Add("a"));
            taskList.Add(task);
        }

        Task.WaitAll(taskList.ToArray());

    }
}
