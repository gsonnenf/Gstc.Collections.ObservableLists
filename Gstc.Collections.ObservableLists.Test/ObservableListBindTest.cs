﻿using System;
using System.Collections.Specialized;
using Gstc.Collections.ObservableLists.Binding;
using Gstc.Collections.ObservableLists.Test.Fakes;
using Gstc.Utility.UnitTest.Event;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListBindTest {

    #region Datasources
    public static object[] DataSource_Empty => new object[] {
        ()=> new ObservableListBind_ItemAB(new ObservableList<ItemA>(), new ObservableList<ItemB>()),

        ()=> new ObservableListBindFunc<ItemA, ItemB>(
            ObservableListBind_ItemAB.ConvertItemAToB,
            ObservableListBind_ItemAB.ConvertItemBToA,
            new ObservableList<ItemA>(),
            new ObservableList<ItemB>()),

        ()=> new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(), new ObservableList<ItemB>() ),

        ()=> new ObservableListBindPropertyFunc<ItemA,ItemB>(
           ObservableListBindProperty_ItemAB.ConvertItemAToB,
           ObservableListBindProperty_ItemAB.ConvertItemBToA,
           PropertyBindType.UpdateCollectionNotify,
           new ObservableList<ItemA>(),
           new ObservableList<ItemB>()),

        ()=> new ObservableListBindPropertyFunc<ItemA,ItemB>(
           ObservableListBindProperty_ItemAB.ConvertItemAToB,
           ObservableListBindProperty_ItemAB.ConvertItemBToA,
           new CustomPropertyMapItemAB(),
           new ObservableList<ItemA>(),
           new ObservableList<ItemB>()
           )
    };

    //Generator methods resolve issue with NUnit ValueSource using same object for each run
    public static object[] DataSource_Prepopulated => new object[] {
        () => new ObservableListBind_ItemAB(
           new ObservableList<ItemA> {ItemA1},
           new ObservableList<ItemB> {ItemB1}
           ),

        () => new ObservableListBindFunc<ItemA, ItemB>(
             ObservableListBind_ItemAB.ConvertItemAToB,
             ObservableListBind_ItemAB.ConvertItemBToA,
             new ObservableList<ItemA> {ItemA1},
             new ObservableList<ItemB> {ItemB1}
            ),

       () => new ObservableListBindProperty_ItemAB(
           new ObservableList<ItemA> {ItemA1},
           new ObservableList<ItemB> {ItemB1}
           ),

        ()=> new ObservableListBindPropertyFunc<ItemA,ItemB>(
           ObservableListBindProperty_ItemAB.ConvertItemAToB,
           ObservableListBindProperty_ItemAB.ConvertItemBToA,
           PropertyBindType.UpdateCollectionNotify,
           new ObservableList<ItemA> {ItemA1},
           new ObservableList<ItemB> {ItemB1}),

        () => new ObservableListBindPropertyFunc<ItemA,ItemB>(
           ObservableListBindProperty_ItemAB.ConvertItemAToB,
           ObservableListBindProperty_ItemAB.ConvertItemBToA,
           new CustomPropertyMapItemAB(),
           new ObservableList<ItemA> {ItemA1},
           new ObservableList<ItemB> {ItemB1}
           )
    };

    public static object[] DataSource_CopyInConstructor => new object[] {
        new ObservableListBind_ItemAB(ItemA.GetSampleSourceItemAList(), new ObservableList<ItemB>()),

        new ObservableListBindFunc<ItemA, ItemB>(
            ObservableListBind_ItemAB.ConvertItemAToB,
            ObservableListBind_ItemAB.ConvertItemBToA,
            ItemA.GetSampleSourceItemAList(),
            new ObservableList<ItemB>()
        ),

        new ObservableListBindProperty_ItemAB( ItemA.GetSampleSourceItemAList(), new ObservableList<ItemB>()),

        new ObservableListBindPropertyFunc<ItemA,ItemB>(
           ObservableListBindProperty_ItemAB.ConvertItemAToB,
           ObservableListBindProperty_ItemAB.ConvertItemBToA,
           PropertyBindType.UpdateCollectionNotify,
           ItemA.GetSampleSourceItemAList(),
           new ObservableList<ItemB>()),

        new ObservableListBindPropertyFunc<ItemA,ItemB>(
           ObservableListBindProperty_ItemAB.ConvertItemAToB,
           ObservableListBindProperty_ItemAB.ConvertItemBToA,
           new CustomPropertyMapItemAB(),
           ItemA.GetSampleSourceItemAList(),
           new ObservableList<ItemB>()
           )
    };
    #endregion

    #region TestFixture
    public static ItemA ItemA1 => new() { MyNum = 1, MyStringLower = "test string 1" };
    public static ItemB ItemB1 => new() { MyNum = "1", MyStringUpper = "TEST STRING 1" };
    public static ItemA ItemA2 => new() { MyNum = 2, MyStringLower = "test string 2" };
    public static ItemB ItemB2 => new() { MyNum = "2", MyStringUpper = "TEST STRING 2" };
    public static ItemA ItemA3 => new() { MyNum = 3, MyStringLower = "test string 3" };
    public static ItemB ItemB3 => new() { MyNum = "3", MyStringUpper = "TEST STRING 3" };
    #endregion

    [Test, Description("Tests initialization from constructor for ObservableListBind")]
    [TestCaseSource(nameof(DataSource_CopyInConstructor))]
    public void ReplaceList_UsingConstructor_ListAreSynced(IObservableListBind<ItemA, ItemB> obvListBind) {
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(obvListBind.ObservableListB.Count));
            for (int index = 0; index < 3; index++) {
                Assert.That(obvListBind.ObservableListB[index].MyNum, Is.EqualTo(obvListBind.ObservableListA[index].MyNum.ToString()));
                Assert.That(obvListBind.ObservableListB[index].MyStringUpper, Is.EqualTo(obvListBind.ObservableListA[index].MyStringLower.ToUpper()));
            }
        });
    }

    [Test, Description("Tests that list replace copies over properly")]
    public void ReplaceList_UsingReplace_IsInitializedFromSource(
            [ValueSource(nameof(DataSource_Empty))] Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] ListIdentifier listOrder) {
        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
        if (sourceList != ListIdentifier.ListA && obvListBind is IObservableListBindProperty<ItemA, ItemB>) {
            _ = Assert.Throws<OneWayBindingException>(() => obvListBind.SourceList = sourceList);
            return;
        }

        IObservableList<ItemA> compareA = ItemA.GetSampleSourceItemAList();
        IObservableList<ItemB> compareB = ItemB.GetSampleDestItemAList();
        obvListBind.SourceList = sourceList;

        //Tests order of assignment.
        if (listOrder == sourceList) {
            obvListBind.ObservableListA = ItemA.GetSampleSourceItemAList();
            obvListBind.ObservableListB = ItemB.GetSampleDestItemAList();
        }
        if (listOrder != sourceList) {
            obvListBind.ObservableListB = ItemB.GetSampleDestItemAList();
            obvListBind.ObservableListA = ItemA.GetSampleSourceItemAList();
        }

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(obvListBind.ObservableListA.Count));
            for (int index = 0; index < 3; index++) {
                //Checks correct list was preserved
                if (sourceList == ListIdentifier.ListA) {
                    Assert.That(obvListBind.ObservableListA[index].MyNum, Is.EqualTo(compareA[index].MyNum));
                    Assert.That(obvListBind.ObservableListA[index].MyStringLower, Is.EqualTo(compareA[index].MyStringLower));
                }
                if (sourceList == ListIdentifier.ListB) {
                    Assert.That(obvListBind.ObservableListB[index].MyNum, Is.EqualTo(compareB[index].MyNum));
                    Assert.That(obvListBind.ObservableListB[index].MyStringUpper, Is.EqualTo(compareB[index].MyStringUpper));
                }
                //Checks lists match
                Assert.That(obvListBind.ObservableListB[index].MyNum, Is.EqualTo(obvListBind.ObservableListA[index].MyNum.ToString()));
                Assert.That(obvListBind.ObservableListB[index].MyStringUpper, Is.EqualTo(obvListBind.ObservableListA[index].MyStringLower.ToUpper()));
            }
        });
    }

    [Test, Description("Tests that list doesn't fail when null.")]
    public void ReplaceList_WithNull_NoExceptions(
           [ValueSource(nameof(DataSource_Prepopulated))] Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator,
           [Values] ListIdentifier testList
       ) {
        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();

        if (testList == ListIdentifier.ListA) obvListBind.ObservableListA = null;
        if (testList == ListIdentifier.ListB) obvListBind.ObservableListB = null;

        obvListBind.ObservableListA?.Add(ItemA1);
        _ = (obvListBind.ObservableListA?.Remove(ItemA1));
        obvListBind.ObservableListA?.Clear();

        obvListBind.ObservableListB?.Add(ItemB1);
        _ = (obvListBind.ObservableListB?.Remove(ItemB1));
        obvListBind.ObservableListB?.Clear();

    }

    [Test, Description("Ensures correct initialization of prepopulated lists used in other tests")]
    [TestCaseSource(nameof(DataSource_Prepopulated))]
    public void SyncList_PrepopulatedDataIsConsistent(Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator) {
        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(1));
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(1));
            Assert.That(ItemA1, Is.EqualTo(obvListBind.ObservableListA[0]));
            Assert.That(ItemB1, Is.EqualTo(obvListBind.ObservableListB[0]));

        });
    }

    [Test, Description("Tests that bidirectional add works properly.")]
    public void AddItem_ListsAreSynced_BidirectionalSuccess_UnidirectionalTargetThrowsError(
            [ValueSource(nameof(DataSource_Empty))] Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {
        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();

        //ObservableListBindProperty doesn't support changing source list.
        if (sourceList != ListIdentifier.ListA && obvListBind is IObservableListBindProperty<ItemA, ItemB>) {
            _ = Assert.Throws<OneWayBindingException>(() => obvListBind.SourceList = sourceList);
            return;
        }

        obvListBind.SourceList = sourceList;
        obvListBind.IsBidirectional = isBidirectional;

        if (testList != sourceList && !isBidirectional) {
            _ = Assert.Throws<OneWayBindingException>(testList switch {
                ListIdentifier.ListA => () => obvListBind.ObservableListA.Add(ItemA1),
                ListIdentifier.ListB => () => obvListBind.ObservableListB.Add(ItemB1)
            });
            return;
        }

        if (testList == ListIdentifier.ListA) obvListBind.ObservableListA.Add(ItemA1);
        if (testList == ListIdentifier.ListB) obvListBind.ObservableListB.Add(ItemB1);

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(1));
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(1));
            Assert.That(ItemA1, Is.EqualTo(obvListBind.ObservableListA[0]));
            Assert.That(ItemB1, Is.EqualTo(obvListBind.ObservableListB[0]));
        });
    }

    [Test, Description("Tests that remove works properly")]
    public void ClearItem_ListsAreSynced_BidirectionalSuccess_UnidirectionalTargetThrowsError(
            [ValueSource(nameof(DataSource_Prepopulated))] Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {
        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();

        //ObservableListBindProperty doesn't support changing source list.
        if (sourceList != ListIdentifier.ListA && obvListBind is IObservableListBindProperty<ItemA, ItemB>) {
            _ = Assert.Throws<OneWayBindingException>(() => obvListBind.SourceList = sourceList);
            return;
        }
        obvListBind.IsBidirectional = isBidirectional;
        obvListBind.SourceList = sourceList;

        if (testList != sourceList && !isBidirectional) {
            _ = Assert.Throws<OneWayBindingException>(testList switch {
                ListIdentifier.ListA => () => obvListBind.ObservableListA.Clear(),
                ListIdentifier.ListB => () => obvListBind.ObservableListB.Clear()
            });
            return;
        }

        if (testList == ListIdentifier.ListA) obvListBind.ObservableListA.Clear();
        if (testList == ListIdentifier.ListB) obvListBind.ObservableListB.Clear();
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(0));
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(0));
        });
    }

    [Test, Description("Tests that move works properly")]
    public void MoveItem_ListsAreSynced_BidirectionalSuccess_UnidirectionalTargetThrowsError(
            [ValueSource(nameof(DataSource_Prepopulated))] Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {

        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();

        //ObservableListBindProperty doesn't support changing source list.
        if (sourceList != ListIdentifier.ListA && obvListBind is IObservableListBindProperty<ItemA, ItemB>) {
            _ = Assert.Throws<OneWayBindingException>(() => obvListBind.SourceList = sourceList);
            return;
        }

        obvListBind.IsBidirectional = isBidirectional;
        obvListBind.SourceList = sourceList;

        //ensures add works which is used in arrangment.
        if (sourceList == ListIdentifier.ListA) obvListBind.ObservableListA.Add(ItemA2);
        if (sourceList == ListIdentifier.ListB) obvListBind.ObservableListB.Add(ItemB2);
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(2));
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(2));
            Assert.That(ItemA1, Is.EqualTo(obvListBind.ObservableListA[0]));
            Assert.That(ItemB1, Is.EqualTo(obvListBind.ObservableListB[0]));
            Assert.That(ItemA2, Is.EqualTo(obvListBind.ObservableListA[1]));
            Assert.That(ItemB2, Is.EqualTo(obvListBind.ObservableListB[1]));
        });

        if (sourceList != testList && !isBidirectional) {
            _ = Assert.Throws<OneWayBindingException>(testList switch {
                ListIdentifier.ListA => () => obvListBind.ObservableListA.Move(0, 1),
                ListIdentifier.ListB => () => obvListBind.ObservableListB.Move(0, 1)
            });
            return;
        }

        if (testList == ListIdentifier.ListA) obvListBind.ObservableListA.Move(0, 1);
        if (testList == ListIdentifier.ListB) obvListBind.ObservableListB.Move(0, 1);

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(2));
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(2));
            Assert.That(ItemA1, Is.EqualTo(obvListBind.ObservableListA[1]));
            Assert.That(ItemB1, Is.EqualTo(obvListBind.ObservableListB[1]));
            Assert.That(ItemA2, Is.EqualTo(obvListBind.ObservableListA[0]));
            Assert.That(ItemB2, Is.EqualTo(obvListBind.ObservableListB[0]));
        });
    }

    [Test, Description("Tests that remove works properly")]
    public void RemoveItem_ListsAreSynced_BidirectionalSuccess_UnidirectionalTargetThrowsError(
            [ValueSource(nameof(DataSource_Prepopulated))] Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {
        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();

        //ObservableListBindProperty doesn't support changing source list.
        if (sourceList != ListIdentifier.ListA && obvListBind is IObservableListBindProperty<ItemA, ItemB>) {
            _ = Assert.Throws<OneWayBindingException>(() => obvListBind.SourceList = sourceList);
            return;
        }

        obvListBind.IsBidirectional = isBidirectional;
        obvListBind.SourceList = sourceList;

        if (sourceList != testList && !isBidirectional) {
            _ = Assert.Throws<OneWayBindingException>(testList switch {
                ListIdentifier.ListA => () => obvListBind.ObservableListA.RemoveAt(0),
                ListIdentifier.ListB => () => obvListBind.ObservableListB.RemoveAt(0)
            });
            return;
        }

        if (testList == ListIdentifier.ListA) obvListBind.ObservableListA.RemoveAt(0);
        if (testList == ListIdentifier.ListB) obvListBind.ObservableListB.RemoveAt(0);
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(0));
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(0));
        });
    }

    [Test, Description("Tests that replace works properly")]
    public void ReplaceItem_ListsAreSynced_BidirectionalSuccess_UnidirectionalTargetThrowsError(
            [ValueSource(nameof(DataSource_Prepopulated))] Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {
        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();

        //ObservableListBindProperty doesn't support changing source list.
        if (sourceList != ListIdentifier.ListA && obvListBind is IObservableListBindProperty<ItemA, ItemB>) {
            _ = Assert.Throws<OneWayBindingException>(() => obvListBind.SourceList = sourceList);
            return;
        }

        obvListBind.IsBidirectional = isBidirectional;
        obvListBind.SourceList = sourceList;

        if (testList != sourceList && !isBidirectional) {
            _ = Assert.Throws<OneWayBindingException>(testList switch {
                ListIdentifier.ListA => () => obvListBind.ObservableListA[0] = ItemA2,
                ListIdentifier.ListB => () => obvListBind.ObservableListB[0] = ItemB2
            });
            return;
        }

        if (testList == ListIdentifier.ListA) obvListBind.ObservableListA[0] = ItemA2;
        if (testList == ListIdentifier.ListB) obvListBind.ObservableListB[0] = ItemB2;
        Assert.Multiple(() => {
            Assert.That(ItemA2, Is.EqualTo(obvListBind.ObservableListA[0]));
            Assert.That(ItemB2, Is.EqualTo(obvListBind.ObservableListB[0]));
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(1));
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(1));
        });
    }

    [Test]
    [TestCaseSource(nameof(DataSource_Prepopulated))]
    public void ReleaseAll_ClearsAllEventsListsItemsSuccessfully(Func<IObservableListBind<ItemA, ItemB>> obvListBindGenerator) {

        IObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
        IObservableList<ItemA> obvListA = obvListBind.ObservableListA;
        IObservableList<ItemB> obvListB = obvListBind.ObservableListB;

        obvListBind.ReleaseAll();
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Is.Null);
            Assert.That(obvListBind.ObservableListB, Is.Null);
            Assert.That(AssertEvent.NumberOfEvents_NotifyCollection(obvListA, nameof(obvListA.CollectionChanged)), Is.EqualTo(0));
            Assert.That(AssertEvent.NumberOfEvents_NotifyCollection(obvListB, nameof(obvListB.CollectionChanged)), Is.EqualTo(0));
            Assert.That(AssertEvent.NumberOfEvents_NotifyCollection(obvListA, nameof(obvListA.PropertyChanged)), Is.EqualTo(0));
            Assert.That(AssertEvent.NumberOfEvents_NotifyCollection(obvListB, nameof(obvListB.PropertyChanged)), Is.EqualTo(0));
        });
    }

    #region Bug Fix
    [Test]
    [Description("https://github.com/gsonnenf/Gstc.Collections.ObservableLists/issues/5")]
    public void InProgressFlagExceptionTest_InProgressFlagShouldNotGetStuckInTheOnStateWhenAnExceptionIsThrown() {
        ObservableListBind_ItemAB obvListBind = new(
            new ObservableList<ItemA>(),
            new ObservableList<ItemB>(),
            isBidirectional: true);

        ObservableListBindProperty_ItemAB obvListBindProperty = new(
            new ObservableList<ItemA>(),
            new ObservableList<ItemB>(),
            isBidirectional: true);

        static void ExceptionCallback(object sender, NotifyCollectionChangedEventArgs args) => throw new NotImplementedException();

        // ListB exception is called the ListChanged(...) synchronization event. The end of the codeblock is not reached.
        obvListBind.ObservableListB.Adding += ExceptionCallback;
        Assert.That(obvListBind.Syncing.InProgress, Is.False);
        Assert.Throws<NotImplementedException>(() => obvListBind.ObservableListA.Add(new ItemA()));
        Assert.That(obvListBind.Syncing.InProgress, Is.False);

        obvListBindProperty.ObservableListB.Adding += ExceptionCallback;
        Assert.That(obvListBindProperty.Syncing.InProgress, Is.False);
        Assert.Throws<NotImplementedException>(() => obvListBindProperty.ObservableListA.Add(new ItemA()));
        Assert.That(obvListBindProperty.Syncing.InProgress, Is.False);

        //Todo: The same fixes were applied to propertybinder, but are not listed here.
    }
    #endregion
}

