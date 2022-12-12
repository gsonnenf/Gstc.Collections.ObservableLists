using System;
using System.Collections.Specialized;
using System.Collections.Generic;

using NUnit.Framework;

using Gstc.Collections.ObservableLists.Interface;
using System.Threading;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using Gstc.Collections.ObservableLists.Multithread;

namespace Gstc.Collections.ObservableLists.Test {

    [TestFixture]
    public class ReentrancyTest {

        /// <summary>
        /// An array of lists that will be tested 
        /// </summary>
        public static object[] StaticDataSource => new object[] {
            new ObservableList<string>(),
            new ObservableIList<string, List<string>>(),
            new ObservableIListLocking<string,List<string>>()
        };

        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        [Description("Tests that reentrancy is only allowed if AllowReentrancy is set to true")]
        public void ReentrancySucceedTest(IObservableCollection<string> list) {

            int reentrancyCounter = 0;
            //Demonstrates single reentrancy success or notImplementedException
            try {
                list.AllowReentrancy = true;
                list.CollectionChanged += (sender, args) => {
                   
                    if (list.Count > 10) return;
                    if (args.Action == NotifyCollectionChangedAction.Add) list.Add("String Object");
                    reentrancyCounter++;
                };
                list.Add("My reentrancy trigger");
            } catch (NotSupportedException e) { Console.WriteLine(e.Message); }; //allows not supported exception for Allow Reentrancy
            Console.WriteLine(reentrancyCounter);
        }

        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        [Description("Tests that reentrancy triggers an error if it is set to false")]
        public void ReentrancyFailTest(IObservableCollection<string> list) {
            list.AllowReentrancy = false;          

            list.CollectionChanged += (sender, args) => {
                if (list.Count > 10) return;
                if (args.Action == NotifyCollectionChangedAction.Add) list.Add("This add should cause reentrancy.");
            };
            Assert.Throws<InvalidOperationException>(() => list.Add("This add should start reentrancy."));
        }


        [Test]
        public void JoinRecursiveChainTest() {
            int count = 0;
            void ThreadFunc() {
                var newCount = Interlocked.Increment(ref count);
                if (count < 100) {
                    var thread = new Thread(ThreadFunc);
                    Console.WriteLine("Starting new thread:" + newCount);
                    thread.Start();
                    Console.WriteLine("Joining Thread:" + newCount);
                    thread.Join();
                    Console.WriteLine("Ended Thread:" + newCount);
                }
            }
            ThreadFunc();
        }

        //TODO: File a bug report with NUnit for Assert.Inconclusive() not working correctly in child threads.
        [Test]
        [Description("Test for locking on multithread reentrancy.")]
        public void ReentrancyMultithreadTest() {
            var list = new ObservableIListLocking<Thread, List<Thread>>();
            var callCount = 0;

            //creates stack of threads to access 
            void AddList() {
                if (callCount > 10) return;
                Interlocked.Increment(ref callCount);
                var newThread = new Thread(AddList);
                Console.WriteLine("Approaching lock:" + Thread.CurrentThread.ManagedThreadId);
                list.Add(newThread);
                newThread.Join();
                Console.WriteLine("Thread exiting:" + Thread.CurrentThread.ManagedThreadId);
            }

            list.CollectionChanged += (sender, args) => {
                Console.WriteLine("Enter Lock:" + Thread.CurrentThread.ManagedThreadId);
                if (args.NewItems[0] is not Thread thread) throw new NullReferenceException("Not a thread");
                thread.Start();
                Thread.Sleep(50); //Allows started thread to hit lock before releasing lock.
                Console.WriteLine("Exit Lock:" + Thread.CurrentThread.ManagedThreadId);
            };

            AddList();
            Console.WriteLine("Count of Threads: " + callCount);
           
        }
    }
}
