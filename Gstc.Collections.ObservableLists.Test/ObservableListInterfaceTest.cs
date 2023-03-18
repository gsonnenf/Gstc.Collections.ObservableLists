// ReSharper disable RedundantArgumentDefaultValue
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Test.Fakes;
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
    public void Add_ItemAdded_EventsTriggeredSuccessfully(IObservableList<TestItem> obvList) {
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Add(0, Item1));

        obvList.Add(Item1);

        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(1));
            Assert.That(obvList[0], Is.EqualTo(Item1));
            AssertPropertyCollectionTest();
        });
    }

    [TestCaseSource(nameof(StaticDataSource))]
    [Test, Description("Test AddRange")]
    public void AddRange_ItemsAdded_NotifyEventsInvoked(IObservableList<TestItem> obvList) {
        obvList.Add(DefaultTestItem);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_AddRange3(1, Item1, Item2, Item3));

        obvList.AddRange(new[] { Item1, Item2, Item3 });

        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(4));
            Assert.That(obvList[0], Is.EqualTo(DefaultTestItem));
            Assert.That(obvList[1], Is.EqualTo(Item1));
            Assert.That(obvList[2], Is.EqualTo(Item2));
            Assert.That(obvList[3], Is.EqualTo(Item3));
            AssertPropertyCollectionTest();
        });
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void Clear_ItemsCleared_NotifyEventsInvoked(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Reset);

        obvList.Clear();

        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(0));
            AssertPropertyCollectionTest();
        });
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void Index_ItemAtIndexReplaced_NotifyEventsInvoked(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        obvList.Add(Item2);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Replace(1, Item2, Item3));

        obvList[1] = Item3;

        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(2));
            Assert.That(obvList[1], Is.EqualTo(Item3));
            Assert.That(obvList.IndexOf(Item3), Is.EqualTo(1)); //IndexOf test
            AssertPropertyCollectionTest(1, 0, 1);
        });
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void Insert_ItemInserted_NotifyEventsInvoked(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        obvList.Add(Item3);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Add(1, Item2));

        obvList.Insert(1, Item2);

        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(3));
            Assert.That(obvList[1], Is.EqualTo(Item2));
            AssertPropertyCollectionTest();
        });
    }

    [Test, Description("Tests that move generated the appropriate events.")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void Move_ItemMoved_NotifyEventsInvoked(IObservableList<TestItem> obvList) {
        obvList.AddRange(new[] { Item1, Item2, Item3 });
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Moved(Item2, 2, 1));

        obvList.Move(1, 2);

        Assert.Multiple(() => {
            Assert.That(obvList[2], Is.EqualTo(Item2));
            AssertPropertyCollectionTest(1, 0, 1);
        });
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void RefreshIndex_NotifyEventInvokedAtIndex(IObservableList<TestItem> obvList) {
        obvList.AddRange(new[] { Item1 });
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Replace(0, Item1, Item1));

        obvList.RefreshIndex(0);
        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(1));
            Assert.That(obvList[0], Is.EqualTo(Item1));
            AssertPropertyCollectionTest(1, 0, 1);
        });
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void RefreshAll_NotifyEventInvoked(IObservableList<TestItem> obvList) {
        obvList.AddRange(new[] { Item1 });
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Reset);

        obvList.RefreshAll();

        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(1));
            Assert.That(obvList[0], Is.EqualTo(Item1));
            AssertPropertyCollectionTest();
        });
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void Remove_ItemRemoved_NotifyEventsInvoked(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        obvList.Add(Item2);
        obvList.Add(Item3);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Removed(1, Item2));

        _ = obvList.Remove(Item2);

        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(2));
            Assert.That(obvList[0], Is.EqualTo(Item1));
            Assert.That(obvList[1], Is.EqualTo(Item3));
            AssertPropertyCollectionTest();
        });
    }

    [Test, Description("")]
    [TestCaseSource(nameof(StaticDataSource))]
    public void RemoveAt_ItemRemovedAtIndex_NotifyEventsInvoked(IObservableList<TestItem> obvList) {
        obvList.Add(Item1);
        obvList.Add(Item2);
        obvList.Add(Item3);
        InitPropertyCollectionTest(obvList, AssertArgs.OnCollectionChanged_Removed(1, Item2));

        obvList.RemoveAt(1);

        Assert.Multiple(() => {
            Assert.That(obvList, Has.Count.EqualTo(2));
            Assert.That(obvList[0], Is.EqualTo(Item1));
            Assert.That(obvList[1], Is.EqualTo(Item3));
            AssertPropertyCollectionTest();
        });
    }
}
