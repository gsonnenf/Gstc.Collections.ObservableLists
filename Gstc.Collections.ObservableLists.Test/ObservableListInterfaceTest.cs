using System;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Interface;
using Gstc.Collections.ObservableLists.Multithread;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using Gstc.Collections.ObservableLists.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListInterfaceTest : CollectionTestBase<TestItem> {
    public static object[] StaticDataSource => new object[] {
        new ObservableList<TestItem>(),
        new ObservableIList<TestItem, List<TestItem>>(),
        new ObservableIListLocking<TestItem,List<TestItem>>()
    };

    [SetUp]
    public new void TestInit() => base.TestInit();

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void TestMethod_Add(IObservableList<TestItem> obvList) {

        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Add(0, Item1));

        obvList.Add(Item1);

        AssertPropertyCollectionTest();
        Assert.That(obvList[0], Is.EqualTo(Item1));
        Assert.That(obvList, Has.Count.EqualTo(1));
    }

    [Test, Description("Ensure that IList is implemented as an explicit interface, and is not accessible unless cast.")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void TestMethod_Add_object(IObservableList<TestItem> obvList) {
        //TODO - BUG: IList, Work on this for all IList methods and/or move elsewhere
        Assert.Throws<InvalidCastException>(() => obvList.Add(new object()));
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void TestMethod_Clear(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Reset);

        obvList.Clear();

        AssertPropertyCollectionTest();
        Assert.That(obvList, Has.Count.EqualTo(0));
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void TestMethod_Index(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        obvList.Add(Item2);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Replace(1, Item2, Item3));

        obvList[1] = Item3;

        AssertPropertyCollectionTest(1, 0, 1);
        Assert.That(obvList[1], Is.EqualTo(Item3));
        Assert.That(obvList.IndexOf(Item3), Is.EqualTo(1)); //IndexOf test
        Assert.That(obvList, Has.Count.EqualTo(2));
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void TestMethod_Insert(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        obvList.Add(Item3);

        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Add(1, Item2));

        obvList.Insert(1, Item2);

        AssertPropertyCollectionTest();
        Assert.That(obvList[1], Is.EqualTo(Item2));
        Assert.That(obvList, Has.Count.EqualTo(3));
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void TestMethod_Remove(IObservableList<TestItem> obvList) {

        obvList.Add(Item1);
        obvList.Add(Item2);
        obvList.Add(Item3);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Removed(1, Item2));

        obvList.Remove(Item2);

        AssertPropertyCollectionTest();
        Assert.That(obvList[0], Is.EqualTo(Item1));
        Assert.That(obvList[1], Is.EqualTo(Item3));
        Assert.That(obvList, Has.Count.EqualTo(2));
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void TestMethod_RemoveAt(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        obvList.Add(Item2);
        obvList.Add(Item3);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Removed(1, Item2));

        obvList.RemoveAt(1);

        AssertPropertyCollectionTest();
        Assert.That(obvList[0], Is.EqualTo(Item1));
        Assert.That(obvList[1], Is.EqualTo(Item3));
        Assert.That(obvList, Has.Count.EqualTo(2));
    }

}
