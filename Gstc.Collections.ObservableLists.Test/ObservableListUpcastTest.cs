using System;
using System.Collections;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Interface;
using Gstc.Collections.ObservableLists.Multithread;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using Gstc.Collections.ObservableLists.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListUpcastTest : CollectionTestBase<TestItem> {

    public static object[] StaticDataSource => new object[] {
        new ObservableList<TestItem>(),
        new ObservableIList<TestItem, List<TestItem>>(),
        new ObservableIListLocking<TestItem,List<TestItem>>()
    };

    [SetUp]
    public new void TestInit() => base.TestInit();

    #region generic interface
    [Test]
    [TestCaseSource(nameof(StaticDataSource))]
    [Description("Tests functionality when upcast to ICollection<> interface.")]
    public void CollectionInterfaceGenericTest(ICollection<TestItem> collection) {
        //Staging
        if (collection is not IObservableList<TestItem> obvList) throw new InvalidCastException("Collection does not support the IObservableList<TestItem> Interface");

        InitPropertyCollectionTest(obvList);
        //Add and count Test
        collection.Add(Item1);
        Assert.AreEqual(1, collection.Count);
        AssertPropertyCollectionTest();

        var a = PropertyTest;
        //Remove Test
        collection.Remove(Item1);
        Assert.AreEqual(0, collection.Count);
        AssertPropertyCollectionTest();

        //Clear Test
        collection.Add(Item1);
        collection.Add(Item2);
        collection.Add(Item3);
        Assert.AreEqual(3, collection.Count);
        AssertPropertyCollectionTest(3, 3, 3);

        collection.Clear();
        Assert.AreEqual(0, collection.Count);
        AssertPropertyCollectionTest();

        //Contains Test
        collection.Add(Item1);
        Assert.IsTrue(collection.Contains(Item1));
        Assert.IsFalse(collection.Contains(Item2));

        //SyncRoot Test
        Assert.IsFalse(collection.IsReadOnly);

        //IEnumerator/ EnumeratorGeneric test
        collection.Clear();
        collection.Add(Item1);
        collection.Add(Item2);
        collection.Add(Item3);
        Assert.AreEqual(3, collection.Count);

        IEnumerator enumerator = collection.GetEnumerator();
        Assert.IsNotNull(enumerator);
        enumerator.MoveNext();
        Assert.AreEqual(enumerator.Current, Item1);

        IEnumerator<TestItem> enumeratorGeneric = collection.GetEnumerator();
        Assert.IsNotNull(enumeratorGeneric);
        enumeratorGeneric.MoveNext();
        Assert.AreEqual(enumeratorGeneric.Current, Item1);

        //CopyTo test
        var array = new TestItem[3];
        collection.CopyTo(array, 0);
        Assert.AreEqual(array[0], Item1);
        Assert.AreEqual(array[1], Item2);
        Assert.AreEqual(array[2], Item3);
    }


    [Test]
    [TestCaseSource(nameof(StaticDataSource))]
    [Description("Tests functionality when upcast to IList<> interface (those methods not found in ICollection<>)")]
    public void ListInterfaceGenericTest(IList<TestItem> list) {
        if (list is not IObservableList<TestItem> obvCollection) throw new InvalidCastException("Collection does not support the IObservable Interface");
        InitPropertyCollectionTest(obvCollection);

        list.Add(Item1);
        AssertPropertyCollectionTest();

        //Index
        list[0] = Item2;
        Assert.AreEqual(Item2, list[0]);
        AssertPropertyCollectionTest(1, 0, 1);

        //IndexOf test
        Assert.AreEqual(0, list.IndexOf(Item2));

        //Contains test
        Assert.IsTrue(list.Contains(Item2));
        Assert.IsFalse(list.Contains(Item1));
    }
    #endregion

    #region Non-generic interface
    [Test]
    [TestCaseSource(nameof(StaticDataSource))]
    [Description("Tests functionality when upcast to ICollection interface.")]
    public void CollectionInterfaceTest(ICollection collection) {
        if (collection is not IObservableList<TestItem> obvList) throw new InvalidCastException("Collection does not support the IObservable Interface");
        obvList.Add(Item1);
        obvList.Add(Item2);
        obvList.Add(Item3);

        //Count Test
        Assert.AreEqual(3, collection.Count);

        //SyncRoot Test
        try { Assert.IsNotNull(collection.SyncRoot); }
        catch (NotSupportedException e) { Console.WriteLine(e.Message); }//Concurrent Collections will not support sync root.

        //isSynchronized Test
        Assert.IsNotNull(collection.IsSynchronized);

        //IEnumerator and CopyTo test
        var enumerator = collection.GetEnumerator();
        Assert.IsNotNull(enumerator);
        foreach (var item in collection) {
            enumerator.MoveNext();
            Assert.AreEqual(item, enumerator.Current);
            Assert.IsTrue(item != null);
        }

        //Array Test
        var array = new object[3];
        collection.CopyTo(array, 0);

        Assert.AreEqual(array[0], Item1);
        Assert.AreEqual(array[1], Item2);
        Assert.AreEqual(array[2], Item3);
    }


    public static object[] StaticDataSourceIList => new object[] {
        new ObservableList<TestItem>(),
        new ObservableIList<TestItem, List<TestItem>>(),
    };
    [Test]
    [TestCaseSource(nameof(StaticDataSourceIList))]
    [Description("Tests functionality when upcast to IList interface.")]
    public void ListInterfaceTest(IList list) {
        if (list is not IObservableList<TestItem> obvCollection) throw new InvalidCastException("Collection does not support the IObservable Interface");

        //Add test
        InitPropertyCollectionTest(obvCollection);
        list.Add(Item1);
        Assert.AreEqual(Item1, list[0]);
        AssertPropertyCollectionTest();

        //Contains test
        Assert.IsTrue(list.Contains(Item1));
        Assert.IsFalse(list.Contains(new TestItem()));

        //Index Test
        InitPropertyCollectionTest(obvCollection);
        list[0] = Item2;
        Assert.AreEqual(Item2, list[0]);
        AssertPropertyCollectionTest(1, 0, 1);

        //Index of test
        Assert.AreEqual(0, list.IndexOf(Item2));

        //Insert(,)
        list.Insert(0, Item3);
        Assert.AreEqual(Item3, list[0]);
        AssertPropertyCollectionTest();

        //RemoveAt()
        list.RemoveAt(0);
        Assert.AreEqual(Item2, list[0]);
        AssertPropertyCollectionTest();
    }
    #endregion



}
