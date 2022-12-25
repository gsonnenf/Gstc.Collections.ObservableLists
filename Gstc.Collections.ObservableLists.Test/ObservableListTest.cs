#pragma warning disable IDE0079
#pragma warning disable NUnit2002
#pragma warning disable NUnit2003
#pragma warning disable NUnit2004
#pragma warning disable NUnit2005
#pragma warning disable NUnit2010
#pragma warning disable NUnit2019
#pragma warning disable NUnit2045

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

    [Test, Description("Tests the reset event for WPF data binding")]
    public void TestMethod_AddRange_Wpf() {
        var obvList = new ObservableList<TestItem> { IsResetForAddRange = true };
        obvList.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Reset);
        obvList.AddRange(new List<TestItem>() { Item1, Item2, Item3 });
        AssertPropertyCollectionTest();

        var obvList2 = new ObservableIList<TestItem, List<TestItem>>() { IsResetForAddRange = true };
        obvList2.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList2, AssertArgs.OnCollectionChanged_Reset);
        obvList2.AddRange(new List<TestItem>() { Item1, Item2, Item3 });
        AssertPropertyCollectionTest();

        var obvList3 = new ObservableIListLocking<TestItem, List<TestItem>> { IsResetForAddRange = true };
        obvList3.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList3, AssertArgs.OnCollectionChanged_Reset);
        obvList3.AddRange(new List<TestItem>() { Item1, Item2, Item3 });
        AssertPropertyCollectionTest();
    }
}
