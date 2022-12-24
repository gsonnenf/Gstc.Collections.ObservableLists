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

    [TestCaseSource(nameof(StaticDataSource))]
    [Test, Description("Test AddRange")]
    public void TestMethod_AddRange(IObservableList<TestItem> obvList) {
        obvList.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_AddRange3(1, Item1, Item2, Item3));

        obvList.AddRange(new[] { Item1, Item2, Item3 });

        AssertPropertyCollectionTest();
        Assert.That(obvList.Count, Is.EqualTo(4));
        Assert.That(obvList[0], Is.EqualTo(DefaultTestItem));
        Assert.That(obvList[1], Is.EqualTo(Item1));
        Assert.That(obvList[2], Is.EqualTo(Item2));
        Assert.That(obvList[3], Is.EqualTo(Item3));
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

    [Test, Description("Tests that move generated the appropriate events.")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void TestMethod_Move(IObservableList<TestItem> obvList) {
        obvList.AddRange(new[] { Item1, Item2, Item3 });
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Moved(Item2, 2, 1));
        obvList.Move(1, 2);
        AssertPropertyCollectionTest(1, 0, 1);
        Assert.That(obvList[2], Is.EqualTo(Item2));
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
