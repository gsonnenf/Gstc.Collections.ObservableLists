using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Multithread;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using Gstc.Collections.ObservableLists.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListTest : CollectionTestBase<TestItem> {

    [SetUp]
    public new void TestInit() => base.TestInit();

    [Test]
    [Description("Test constructor initialization with list")]
    public void TestMethod_Constructor() {
        var list = new List<TestItem> { Item1, Item2 };

        var obvList = new ObservableList<TestItem>(list);
        Assert.That(obvList.Count == 2);
        Assert.That(obvList[0] == Item1);
        Assert.That(obvList[1] == Item2);

        var obvList2 = new ObservableIList<TestItem, List<TestItem>>(list);
        Assert.That(obvList2.Count == 2);
        Assert.That(obvList2[0] == Item1);
        Assert.That(obvList2[1] == Item2);

        var obvList3 = new ObservableIListLocking<TestItem, List<TestItem>>(list);
        Assert.That(obvList3.Count == 2);
        Assert.That(obvList3[0] == Item1);
        Assert.That(obvList3[1] == Item2);
    }

    [Test]
    [Description("Tests event triggers on replace list.")]
    public void TestMethod_InternalList_Replace() {
        var list = new List<TestItem> { Item1, Item2 };

        //Tests ObservableList
        var obvList = new ObservableList<TestItem>();
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Reset);
        obvList.List = list;
        Assert.That(obvList.Count == 2);
        Assert.That(obvList[0] == Item1);
        Assert.That(obvList[1] == Item2);
        AssertPropertyCollectionTest();

        //Tests ObservableIList
        var obvList2 = new ObservableIList<TestItem, List<TestItem>>();
        InitPropertyCollectionTest(obvList2, AssertArgs.OnCollectionChanged_Reset);
        obvList2.List = list;
        Assert.That(obvList2.Count == 2);
        Assert.That(obvList2[0] == Item1);
        Assert.That(obvList2[1] == Item2);
        AssertPropertyCollectionTest();

        //Tests ObservableIListLocking
        var obvList3 = new ObservableIListLocking<TestItem, List<TestItem>>();
        InitPropertyCollectionTest(obvList3, AssertArgs.OnCollectionChanged_Reset);
        obvList3.List = list;
        Assert.That(obvList3.Count == 2);
        Assert.That(obvList3[0] == Item1);
        Assert.That(obvList3[1] == Item2);
        AssertPropertyCollectionTest();
    }

    [Test, Description("Test AddRange only on the ObservableList<>")]
    public void TestMethod_AddRange() {
        var obvList = new ObservableList<TestItem> { DefaultTestItem };
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_AddRange3(1, Item1, Item2, Item3));

        obvList.AddRange(new List<TestItem>() { Item1, Item2, Item3 });

        AssertPropertyCollectionTest();
        Assert.That(obvList.Count, Is.EqualTo(4));
        Assert.That(obvList[0], Is.EqualTo(DefaultTestItem));
        Assert.That(obvList[1], Is.EqualTo(Item1));
        Assert.That(obvList[2], Is.EqualTo(Item2));
        Assert.That(obvList[3], Is.EqualTo(Item3));
    }

    [Test, Description("Tests the reset event for WPF data binding")]
    public void TestMethod_AddRange2() {
        var obvList = new ObservableList<TestItem> { IsWpfDataBinding = true };
        obvList.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Reset);
        obvList.AddRange(new List<TestItem>() { Item1, Item2, Item3 });
        AssertPropertyCollectionTest();
    }

    [Test, Description("Tests that move generated the appropriate events.")]
    public void TestMethod_Move() {
        var obvList = new ObservableList<TestItem>() { Item1, Item2, Item3 };
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Moved(Item2, 2, 1));
        obvList.Move(1, 2);
        AssertPropertyCollectionTest(1, 0, 1);
        Assert.That(obvList[2], Is.EqualTo(Item2));

        var obvList2 = new ObservableIList<TestItem, List<TestItem>> { Item1, Item2, Item3 };
        InitPropertyCollectionTest(obvList2, AssertArgs.OnCollectionChanged_Moved(Item2, 2, 1));
        obvList2.Move(1, 2);
        AssertPropertyCollectionTest(1, 0, 1);
        Assert.That(obvList[2], Is.EqualTo(Item2));

        var obvList3 = new ObservableIListLocking<TestItem, List<TestItem>>() { Item1, Item2, Item3 };
        InitPropertyCollectionTest(obvList3, AssertArgs.OnCollectionChanged_Moved(Item2, 2, 1));
        obvList3.Move(1, 2);
        AssertPropertyCollectionTest(1, 0, 1);
        Assert.That(obvList[2], Is.EqualTo(Item2));
    }
}
