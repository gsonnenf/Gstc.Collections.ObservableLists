using System;
using System.Collections;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Test.Fakes;
using Gstc.Collections.ObservableLists.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListUpcastTest : CollectionTestBase<TestItem> {

    #region DataSource
    public static object[] StaticDataSource => new object[] {
        new ObservableList<TestItem>(),
        new ObservableIList<TestItem, List<TestItem>>(),
        new ObservableIListLocking<TestItem,List<TestItem>>()
    };

    //Special case for ensure List and IObservableLists have same functionality for casting issues.
    public static object[] StaticDataSourceWithList => new object[] {
        new List<TestItem>(), // Control group to ensure alignment of behavior between List<T> and ObservableList<T>.
        new ObservableList<TestItem>(),
        new ObservableIList<TestItem, List<TestItem>>(),
        new ObservableIListLocking<TestItem,List<TestItem>>()
    };

    //Special case for ensure List and IObservableLists have same functionality for casting issues.
    public static object[] StaticDataSourceWithListInt => new object[] {
        new List<int>(), // Control group to ensure alignment of beahvior between List<T> and ObservableList<T>.
        new ObservableList<int>(),
        new ObservableIList<int, List<int>>(),
        new ObservableIListLocking<int,List<int>>()
    };
    #endregion

    [SetUp]
    public new void TestInit() => base.TestInit();

    #region generic interface
    [Test]
    [TestCaseSource(nameof(StaticDataSource))]
    [Description("Tests functionality when upcast to ICollection<> interface.")]
    public void UpcastToICollectionGeneric_MethodsWorkCorrectlyAndTriggerEvents(ICollection<TestItem> collection) {
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
    public void UpcastToIListGeneric_MethodsStillWorkAndTriggerEvents(IList<TestItem> list) {
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
    public void UpcastToICollection_MethodsStillWork(ICollection collection) {
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

    [Test]
    [TestCaseSource(nameof(StaticDataSourceWithList))]
    [Description("Tests functionality when upcast to IList interface.")]
    public void UpcastToIList_MethodsStillWorkAndTriggerEvents(IList list) {
        if (list is List<TestItem> && list is not IObservableList<TestItem>) return; //Control case for different test not use here.
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

    [Test]
    [TestCaseSource(nameof(StaticDataSourceWithList))]
    public void IListCastingExceptionsReferenceTypes_HandlesCastingSimilarToListGeneric(IList list) {
        TestItem baseItem = new();
        DerivedItem derivedItem = new();

        Console.WriteLine(derivedItem is TestItem);
        list.Clear();
        list.Add(baseItem);
        //Derived case
        Assert.That(list.Add(new DerivedItem()), Is.EqualTo(1));
        Assert.DoesNotThrow(() => list[1] = derivedItem);
        Assert.That(list.Contains(derivedItem), Is.True);
        Assert.That(list.IndexOf(derivedItem), Is.EqualTo(1));
        Assert.DoesNotThrow(() => list.Insert(0, new DerivedItem()));
        Assert.DoesNotThrow(() => list.Remove(derivedItem));

        list.Clear();
        list.Add(baseItem);
        //Null case
        Assert.That(list.Add(null), Is.EqualTo(1));
        Assert.DoesNotThrow(() => list[1] = null);
        Assert.That(list.Contains(null), Is.True);
        Assert.That(list.IndexOf(null), Is.EqualTo(1));
        Assert.DoesNotThrow(() => list.Insert(0, null));
        Assert.DoesNotThrow(() => list.Remove(null));

        list.Clear();
        list.Add(baseItem);
        //Non-derived case
        Assert.Throws<ArgumentException>(() => list.Add(new NonDerivedItem()));
        Assert.Throws<ArgumentException>(() => list[0] = new NonDerivedItem());
        Assert.That(list.Contains(new NonDerivedItem()), Is.False);
        Assert.That(list.IndexOf(new NonDerivedItem()), Is.EqualTo(-1));
        Assert.Throws<ArgumentException>(() => list.Insert(0, new NonDerivedItem()));
        Assert.DoesNotThrow(() => list.Remove(new NonDerivedItem()));
    }

    [Test]
    [TestCaseSource(nameof(StaticDataSourceWithListInt))]
    public void IListCastingExceptionsValueTypes_HandlesCastingSimilarToListGeneric(IList list) {

        var num1 = 1;
        var num2 = 2;

        list.Clear();
        list.Add(num1);
        //Standard case
        Assert.That(list.Add(num2), Is.EqualTo(1));
        Assert.DoesNotThrow(() => list[1] = num2);
        Assert.That(list.Contains(num2), Is.True);
        Assert.That(list.IndexOf(num2), Is.EqualTo(1));
        Assert.DoesNotThrow(() => list.Insert(0, num2));
        Assert.DoesNotThrow(() => list.Remove(num2));

        list.Clear();
        list.Add(num1);
        //Null case
        Assert.Throws<ArgumentNullException>(() => list.Add(null));
        Assert.Throws<ArgumentNullException>(() => list[1] = null);
        Assert.That(list.Contains(null), Is.False);
        Assert.That(list.IndexOf(null), Is.EqualTo(-1));
        Assert.Throws<ArgumentNullException>(() => list.Insert(0, null));
        Assert.DoesNotThrow(() => list.Remove(null));

        list.Clear();
        list.Add(num1);
        //Non-derived case
        Assert.Throws<ArgumentException>(() => list.Add(new NonDerivedItem()));
        Assert.Throws<ArgumentException>(() => list[0] = new NonDerivedItem());
        Assert.That(list.Contains(new NonDerivedItem()), Is.False);
        Assert.That(list.IndexOf(new NonDerivedItem()), Is.EqualTo(-1));
        Assert.Throws<ArgumentException>(() => list.Insert(0, new NonDerivedItem()));
        Assert.DoesNotThrow(() => list.Remove(new NonDerivedItem()));
    }

    private class DerivedItem : TestItem { }
    private class NonDerivedItem { }//Used for testing failed IList casting
    #endregion

    #region IReadOnlyList
    [Test]
    [TestCaseSource(nameof(StaticDataSource))]
    [Description("Tests functionality when upcast to IReadOnlyList<T> interface.")]
    public void UpcastToIReadOnlyList_CastsSuccessfullyAndReadSuccess(IObservableList<TestItem> obvList) { //Todo: Move to IList
        obvList.Add(Item1);
        IReadOnlyList<TestItem> myReadOnlyList = obvList;
        Assert.That(myReadOnlyList[0], Is.EqualTo(Item1));
        Assert.That(myReadOnlyList, Has.Count.EqualTo(1));
        foreach (var indexItem in myReadOnlyList) Assert.That(indexItem, Is.EqualTo(Item1));
    }
    #endregion
}
