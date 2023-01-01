using System;
using System.Collections;
using System.Collections.Generic;
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
        Assert.That(collection, Has.Count.EqualTo(1));
        AssertPropertyCollectionTest();

        //Remove Test
        _ = collection.Remove(Item1);
        Assert.That(collection, Is.Empty);
        AssertPropertyCollectionTest();

        //Clear Test
        collection.Add(Item1);
        collection.Add(Item2);
        collection.Add(Item3);
        Assert.That(collection, Has.Count.EqualTo(3));
        AssertPropertyCollectionTest(3, 3, 3);

        collection.Clear();
        Assert.That(collection, Is.Empty);
        AssertPropertyCollectionTest();

        //Contains Test
        collection.Add(Item1);
        Assert.Multiple(() => {
            Assert.That(collection.Contains(Item1), Is.True);
            Assert.That(collection.Contains(Item2), Is.False);
            Assert.That(collection.IsReadOnly, Is.False);
        });

        //IEnumerator/ EnumeratorGeneric test
        collection.Clear();
        collection.Add(Item1);
        collection.Add(Item2);
        collection.Add(Item3);
        Assert.That(collection, Has.Count.EqualTo(3));

        IEnumerator enumerator = collection.GetEnumerator();
        Assert.That(enumerator, Is.Not.Null);
        _ = enumerator.MoveNext();
        Assert.That(Item1, Is.EqualTo(enumerator.Current));

        IEnumerator<TestItem> enumeratorGeneric = collection.GetEnumerator();
        Assert.That(enumeratorGeneric, Is.Not.Null);
        _ = enumeratorGeneric.MoveNext();
        Assert.That(Item1, Is.EqualTo(enumeratorGeneric.Current));

        //CopyTo test
        TestItem[] array = new TestItem[3];
        collection.CopyTo(array, 0);
        Assert.Multiple(() => {
            Assert.That(Item1, Is.EqualTo(array[0]));
            Assert.That(Item2, Is.EqualTo(array[1]));
            Assert.That(Item3, Is.EqualTo(array[2]));
        });
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

        Assert.Multiple(() => {
            AssertPropertyCollectionTest(1, 0, 1);
            Assert.That(list[0], Is.EqualTo(Item2));
            //IndexOf test
            Assert.That(list.IndexOf(Item2), Is.EqualTo(0));
            //Contains test
            Assert.That(list.Contains(Item2), Is.True);
            Assert.That(list.Contains(Item1), Is.False);
        });
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
        Assert.That(collection, Has.Count.EqualTo(3));

        //SyncRoot Test
        try { Assert.That(collection.SyncRoot, Is.Not.Null); }
        catch (NotSupportedException e) { Console.WriteLine(e.Message); }//Concurrent Collections will not support sync root.

        //isSynchronized Test
        Assert.That(() => collection.IsSynchronized, Throws.Nothing);

        //IEnumerator and CopyTo test
        IEnumerator enumerator = collection.GetEnumerator();

        Assert.Multiple(() => {
            Assert.That(enumerator, Is.Not.Null);
            foreach (object item in collection) {
                _ = enumerator.MoveNext();
                Assert.That(enumerator.Current, Is.EqualTo(item));
                Assert.That(item, Is.Not.Null);
            }
        });

        //Array Test
        object[] array = new object[3];
        collection.CopyTo(array, 0);
        Assert.Multiple(() => {
            Assert.That(Item1, Is.EqualTo(array[0]));
            Assert.That(Item2, Is.EqualTo(array[1]));
            Assert.That(Item3, Is.EqualTo(array[2]));
        });
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
        _ = list.Add(Item1);

        Assert.Multiple(() => {
            Assert.That(list[0], Is.EqualTo(Item1));
            //Contains test
            Assert.That(list.Contains(Item1), Is.True);
            Assert.That(list.Contains(new TestItem()), Is.False);
            AssertPropertyCollectionTest();
        });

        //Index Test
        InitPropertyCollectionTest(obvCollection);
        list[0] = Item2;
        Assert.Multiple(() => {
            Assert.That(list[0], Is.EqualTo(Item2));
            //Index of test
            Assert.That(list.IndexOf(Item2), Is.EqualTo(0));
            AssertPropertyCollectionTest(1, 0, 1);
        });

        //Insert(,)
        list.Insert(0, Item3);
        Assert.That(list[0], Is.EqualTo(Item3));
        AssertPropertyCollectionTest();

        //RemoveAt()
        list.RemoveAt(0);
        Assert.That(list[0], Is.EqualTo(Item2));
        AssertPropertyCollectionTest();
    }
    #endregion
}
