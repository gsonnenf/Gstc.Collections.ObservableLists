using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Multithread;
using Gstc.Collections.ObservableLists.Test.Fakes;
using Gstc.Collections.ObservableLists.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListTest : CollectionTestBase<TestItem> {

    [SetUp]
    public new void TestInit() => base.TestInit();

    [Test]
    [Description("Test constructor initialization with list")]
    public void ConstructorInitialization_ObservableListInitializes() {
        List<TestItem> list = new() { Item1, Item2 };

        ObservableList<TestItem> obvList = new(list);
        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(2));
            Assert.That(obvList[0], Is.EqualTo(Item1));
            Assert.That(obvList[1], Is.EqualTo(Item2));
        });
        ObservableIList<TestItem, List<TestItem>> obvList2 = new(list);
        Assert.Multiple(() => {
            Assert.That(obvList2, Has.Count.EqualTo(2));
            Assert.That(obvList2[0], Is.EqualTo(Item1));
            Assert.That(obvList2[1], Is.EqualTo(Item2));
        });
        ObservableIListLocking<TestItem, List<TestItem>> obvList3 = new(list);
        Assert.Multiple(() => {
            Assert.That(obvList3, Has.Count.EqualTo(2));
            Assert.That(obvList3[0], Is.EqualTo(Item1));
            Assert.That(obvList3[1], Is.EqualTo(Item2));
        });
    }

    [Test]
    [Description("Tests event triggers on replace list.")]
    public void ReplaceInternalList_ListIsReplaced_TriggerNotifyEvents() {
        List<TestItem> list = new() { Item1, Item2 };

        ObservableList<TestItem> obvList = new();
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Reset);
        obvList.List = list;
        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(2));
            Assert.That(obvList[0], Is.EqualTo(Item1));
            Assert.That(obvList[1], Is.EqualTo(Item2));
            AssertPropertyCollectionTest();
        });

        ObservableIList<TestItem, List<TestItem>> obvList2 = new();
        InitPropertyCollectionTest(obvList2, AssertArgs.OnCollectionChanged_Reset);
        obvList2.List = list;
        Assert.Multiple(() => {
            Assert.That(obvList2, Has.Count.EqualTo(2));
            Assert.That(obvList2[0], Is.EqualTo(Item1));
            Assert.That(obvList2[1], Is.EqualTo(Item2));
            AssertPropertyCollectionTest();
        });

        ObservableIListLocking<TestItem, List<TestItem>> obvList3 = new();
        InitPropertyCollectionTest(obvList3, AssertArgs.OnCollectionChanged_Reset);
        obvList3.List = list;
        Assert.Multiple(() => {
            Assert.That(obvList3, Has.Count.EqualTo(2));
            Assert.That(obvList3[0], Is.EqualTo(Item1));
            Assert.That(obvList3[1], Is.EqualTo(Item2));
            AssertPropertyCollectionTest();
        });
    }

    [Test, Description("Tests that addRange triggers reset event instead of add event. Used for compatibility with WPF data binding.")]
    public void AddRangeWithIsResetForAddRangeTrue_TriggersResetEventInsteadOfAddEvent() {
        ObservableList<TestItem> obvList = new() { IsAddRangeResetEvent = true };
        obvList.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Reset);
        obvList.AddRange(new List<TestItem>() { Item1, Item2, Item3 });
        AssertPropertyCollectionTest();

        ObservableIList<TestItem, List<TestItem>> obvList2 = new() { IsAddRangeResetEvent = true };
        obvList2.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList2, AssertArgs.OnCollectionChanged_Reset);
        obvList2.AddRange(new List<TestItem>() { Item1, Item2, Item3 });
        AssertPropertyCollectionTest();

        ObservableIListLocking<TestItem, List<TestItem>> obvList3 = new() { IsAddRangeResetEvent = true };
        obvList3.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList3, AssertArgs.OnCollectionChanged_Reset);
        obvList3.AddRange(new List<TestItem>() { Item1, Item2, Item3 });
        AssertPropertyCollectionTest();
    }
}
