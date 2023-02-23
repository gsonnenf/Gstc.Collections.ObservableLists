using System;
using System.Collections;
using System.Collections.Generic;
using Gstc.Collections.ObservableDictionary.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;
[TestFixture]
public class BenchMarkListOverride {

    [Test, Sequential, Ignore("Benchmarks take alot of time. Unignore to run benchmark.")]
    [Description("Tests how much overhead is added for using an wrapper over the list class with various methods.")]
    public void ListOverride_Add_Benchmark(
           [Values(100000000)] int numOfItems) {
        Console.WriteLine(nameof(numOfItems) + ":" + numOfItems);

        var listArray = new (string description, IList<int> list)[] {
            ("Warmup", new List<int>()),
            (nameof(List<int>), new List<int>()),
            (nameof(TestListNoAbstract<int>), new TestListNoAbstract<int>()),
            (nameof(TestListAbstractOverride<int>), new TestListAbstractOverride<int>()),
             (nameof(TestListVirtual<int>), new TestListVirtual<int>()),
            (nameof(TestListVirtualOverride<int>), new TestListVirtualOverride<int>()),
        };

        foreach ((var description, var list) in listArray) {
            using (ScopedStopwatch.Start(description))
                for (var i = 0; i < numOfItems; i++) list.Add(1);
            list.Clear();
            GC.Collect();
        }
    }

    private class TestListVirtualOverride<TItem> : TestListVirtual<TItem> {
        public override void Add(TItem value) => InternalList.Add(value);
    }

    private class TestListVirtual<TItem> : IList<TItem> {
        public List<TItem> InternalList = new List<TItem>();
        public virtual void Add(TItem item) => InternalList.Add(item);
        public void Clear() => InternalList.Clear();
        public int Count => InternalList.Count;

        #region Not Implemented
        public TItem this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsReadOnly => throw new NotImplementedException();
        public bool Contains(TItem item) => throw new NotImplementedException();
        public void CopyTo(TItem[] array, int arrayIndex) => throw new NotImplementedException();
        public IEnumerator<TItem> GetEnumerator() => throw new NotImplementedException();
        public int IndexOf(TItem item) => throw new NotImplementedException();
        public void Insert(int index, TItem item) => throw new NotImplementedException();
        public bool Remove(TItem item) => throw new NotImplementedException();
        public void RemoveAt(int index) => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        #endregion
    }

    private class TestListAbstractOverride<TItem> : TestListAbstract<TItem> {
        public override void Add(TItem item) => InternalList.Add(item);
    }

    private abstract class TestListAbstract<TItem> : IList<TItem> {
        public List<TItem> InternalList = new List<TItem>();
        public abstract void Add(TItem item);
        public void Clear() => InternalList.Clear();
        public int Count => InternalList.Count;

        #region Not Implemented
        public TItem this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsReadOnly => throw new NotImplementedException();
        public bool Contains(TItem item) => throw new NotImplementedException();
        public void CopyTo(TItem[] array, int arrayIndex) => throw new NotImplementedException();
        public IEnumerator<TItem> GetEnumerator() => throw new NotImplementedException();
        public int IndexOf(TItem item) => throw new NotImplementedException();
        public void Insert(int index, TItem item) => throw new NotImplementedException();
        public bool Remove(TItem item) => throw new NotImplementedException();
        public void RemoveAt(int index) => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        #endregion
    }

    private class TestListNoAbstract<TItem> : IList<TItem> {
        public List<TItem> InternalList = new List<TItem>();
        public void Add(TItem item) => InternalList.Add(item);
        public void Clear() => InternalList.Clear();
        public int Count => InternalList.Count;

        #region Not Implemented
        public TItem this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsReadOnly => throw new NotImplementedException();
        public bool Contains(TItem item) => throw new NotImplementedException();
        public void CopyTo(TItem[] array, int arrayIndex) => throw new NotImplementedException();
        public IEnumerator<TItem> GetEnumerator() => throw new NotImplementedException();
        public int IndexOf(TItem item) => throw new NotImplementedException();
        public void Insert(int index, TItem item) => throw new NotImplementedException();
        public bool Remove(TItem item) => throw new NotImplementedException();
        public void RemoveAt(int index) => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        #endregion
    }

}
