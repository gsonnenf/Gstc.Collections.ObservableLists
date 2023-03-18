using System;
using System.Collections.Generic;
using Gstc.Collections.ObservableDictionary.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;

[TestFixture]
public class BenchmarkObservableListVsObservableIList {

    //[Test]
    public void TestListVsObservableListVsObservableIList() {

        var numOfItems = -1;
        var listint = new List<int>();
        var obvlist = new ObservableList<int>();
        var obvIList = new ObservableList2<int>();
        var obvIListLocking = new ObservableIListLocking<int, List<int>>();

        var listArray = new (string description, IList<int> list)[] {
            ("Warmup", new List<int>()),
            (nameof(listint), listint),
            (nameof(obvlist), obvlist),
            (nameof(obvIList), obvIList),
            (nameof(obvIListLocking), obvIListLocking),
        };

        Console.WriteLine("\n\n index[] write \n");
        numOfItems = 10000000;
        foreach ((var description, var list) in listArray) {
            list.Add(1);
            using (ScopedStopwatch.Start("add:" + description))
                for (var i = 0; i < numOfItems; i++) list[0] = -1;
            list.Clear();
            GC.Collect();
        }

        Console.WriteLine("\n\n index[] read \n");
        numOfItems = 10000000;
        int temp = -1;
        foreach ((var description, var list) in listArray) {
            list.Add(1);
            using (ScopedStopwatch.Start("add:" + description))
                for (var i = 0; i < numOfItems; i++) temp = list[0];
            list.Clear();
            GC.Collect();
        }

        Console.WriteLine("\n\n Add \n");
        numOfItems = 10000000;
        foreach ((var description, var list) in listArray) {
            var counter = 0;
            using (ScopedStopwatch.Start("add:" + description))
                for (var i = 0; i < numOfItems; i++) list.Add(counter++);
            list.Clear();
            GC.Collect();
        }

        Console.WriteLine("\n\n Remove \n");
        numOfItems = 100000;
        foreach ((var description, var list) in listArray) {
            for (var i = 0; i < numOfItems; i++) list.Add(1);
            using (ScopedStopwatch.Start("Remove:" + description))
                for (var i = 0; i < numOfItems; i++) list.Remove(1);
            list.Clear();
            GC.Collect();
        }

        Console.WriteLine("\n\n RemoveAt \n");
        numOfItems = 100000;
        foreach ((var description, var list) in listArray) {
            for (var i = 0; i < numOfItems; i++) list.Add(1);
            using (ScopedStopwatch.Start("Remove:" + description))
                for (var i = 0; i < numOfItems; i++) list.RemoveAt(0);

            list.Clear();
            GC.Collect();
        }

        Console.WriteLine("\n\n Contains (true) O(1) \n");
        numOfItems = 100000;
        foreach ((var description, var list) in listArray) {
            for (var i = 0; i < numOfItems; i++) list.Add(1);
            using (ScopedStopwatch.Start("Remove:" + description))
                for (var i = 0; i < numOfItems; i++) list.Contains(1);

            list.Clear();
            GC.Collect();
        }

        Console.WriteLine("\n\n Contains (false) O(n) \n");
        numOfItems = 100000;
        foreach ((var description, var list) in listArray) {
            for (var i = 0; i < numOfItems; i++) list.Add(1);
            using (ScopedStopwatch.Start("Remove:" + description))
                for (var i = 0; i < numOfItems; i++) list.Contains(-1);

            list.Clear();
            GC.Collect();
        }

    }

    public class ObservableList2<TItem> : ObservableIList<TItem, List<TItem>> { }

}
