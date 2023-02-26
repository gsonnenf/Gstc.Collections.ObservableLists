using System;
using System.Collections;
using System.Collections.Generic;
using Gstc.Collections.ObservableDictionary.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;
[TestFixture]
public class BenchMarkIListSpecialProxy {

    //[Test, Sequential, Ignore("Benchmarks take alot of time. Unignore to run benchmark.")]
    [Description("Tests how much overhead is added for virtualizing the IList interface twice.")]
    public void ListSpecialProxy_Add_Benchmark(
           [Values(100000000)] int numOfItems) {
        Console.WriteLine(nameof(numOfItems) + ":" + numOfItems);

        var listArray = new (string description, IList list)[] {
            ("Warmup", new List<int>()),
            (nameof(List<int>), new List<int>()),
            (nameof(TestListNoAbstract<int>), new TestListNoAbstract<int>()),
            (nameof(TestListAbstractInherit<int>), new TestListAbstractInherit<int>()),
            (nameof(TestListAbstractOverrideInherit<int>), new TestListAbstractOverrideInherit<int>()),
            (nameof(TestListTwoMethod<int>), new TestListTwoMethod<int>()),
            (nameof(TestListVirtualTwoMethod<int>), new TestListVirtualTwoMethod<int>()),
            (nameof(TestListOverrideTwoMethod<int>), new TestListOverrideTwoMethod<int>()),
        };

        foreach ((var description, var list) in listArray) {
            using (ScopedStopwatch.Start(description))
                for (var i = 0; i < numOfItems; i++) list.Add(1);
            list.Clear();
            GC.Collect();
        }
    }

    private class TestListOverrideTwoMethod<TItem> : TestListVirtualTwoMethod<TItem> {
        public override int AddVirtual(object? value) {
            InternalList.Add((TItem)value);
            return 0;
        }
    }

    private class TestListVirtualTwoMethod<TItem> : TestListAbstract<TItem> {
        public override int Add(object? value) => AddVirtual(value);
        public virtual int AddVirtual(object? value) {
            InternalList.Add((TItem)value);
            return 0;
        }
    }

    private class TestListTwoMethod<TItem> : TestListAbstract<TItem> {
        public override int Add(object? value) => AddVirtual(value);
        public int AddVirtual(object? value) {
            InternalList.Add((TItem)value);
            return 0;
        }
    }

    private class TestListAbstractOverrideInherit<TItem> : TestListAbstractInherit<TItem> {
        public override int Add(object? value) {
            InternalList.Add((TItem)value);
            return 0;
        }
    }


    private class TestListAbstractInherit<TItem> : TestListAbstract<TItem> {
        public override int Add(object? value) {
            InternalList.Add((TItem)value);
            return 0;
        }
    }

    private abstract class TestListAbstract<TItem> : IList {
        public List<TItem> InternalList = new List<TItem>();
        public abstract int Add(object? value);
        public void Clear() => InternalList.Clear();
        public int Count => InternalList.Count;

        #region Not Implemented
        public object? this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFixedSize => throw new NotImplementedException();
        public bool IsReadOnly => throw new NotImplementedException();
        public bool IsSynchronized => throw new NotImplementedException();
        public object SyncRoot => throw new NotImplementedException();
        public bool Contains(object? value) => throw new NotImplementedException();
        public void CopyTo(Array array, int index) => throw new NotImplementedException();
        public IEnumerator GetEnumerator() => throw new NotImplementedException();
        public int IndexOf(object? value) => throw new NotImplementedException();
        public void Insert(int index, object? value) => throw new NotImplementedException();
        public void Remove(object? value) => throw new NotImplementedException();
        public void RemoveAt(int index) => throw new NotImplementedException();
        #endregion
    }

    private class TestListNoAbstract<TItem> : IList {
        public List<TItem> InternalList = new List<TItem>();

        public int Add(object? value) {
            InternalList.Add((TItem)value);
            return 0;
        }
        public void Clear() => InternalList.Clear();
        public int Count => InternalList.Count;

        #region Not Implemented
        public object? this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFixedSize => throw new NotImplementedException();
        public bool IsReadOnly => throw new NotImplementedException();
        public bool IsSynchronized => throw new NotImplementedException();
        public object SyncRoot => throw new NotImplementedException();
        public bool Contains(object? value) => throw new NotImplementedException();
        public void CopyTo(Array array, int index) => throw new NotImplementedException();
        public IEnumerator GetEnumerator() => throw new NotImplementedException();
        public int IndexOf(object? value) => throw new NotImplementedException();
        public void Insert(int index, object? value) => throw new NotImplementedException();
        public void Remove(object? value) => throw new NotImplementedException();
        public void RemoveAt(int index) => throw new NotImplementedException();
        #endregion
    }

}
