#pragma warning disable IDE0079
#pragma warning disable NUnit2002
#pragma warning disable NUnit2003
#pragma warning disable NUnit2004
#pragma warning disable NUnit2005
#pragma warning disable NUnit2019
using System;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Synchronizer;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListSynchronizerTest {

    public ObservableList<ItemASource> SourceObvListA;
    public ObservableList<ItemADest> DestObvListA;
    public ObservableListSynchronizer<ItemASource, ItemADest> ObvListSyncA;

    [Test, Description("Creates a sync and tests initialization copying from source item to dest item.")]
    public void TestMethod_CopyOnInitialize() {
        SourceObvListA = ItemASource.GetSampleSourceItemAList();
        DestObvListA = new ObservableList<ItemADest>();

        ObvListSyncA = new ObservableListSynchronizerFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
            SourceObvListA,
            DestObvListA
        );

        Assert.AreEqual(SourceObvListA.Count, DestObvListA.Count);
        for (var index = 0; index < 3; index++) {
            Assert.AreEqual(SourceObvListA[index].MyNum.ToString(), DestObvListA[index].MyNum);
            Assert.AreEqual(SourceObvListA[index].MyStringLower.ToUpper(), DestObvListA[index].MyStringUpper);
        }
    }

    [Description("Creates a sync and tests assignment copying from source item to dest item.")]
    [TestCase("SourceFirst")]
    [TestCase("DestFirst")]
    public void TestMethod_TestAfterInitialize(string order) {
        SourceObvListA = ItemASource.GetSampleSourceItemAList();
        DestObvListA = new ObservableList<ItemADest>();

        ObvListSyncA = new ObservableListSynchronizerFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() }
        );

        //Tests several order with same boiler plate code.
        if (order == "SourceFirst") {
            ObvListSyncA.SourceObservableList = SourceObvListA;
            ObvListSyncA.DestinationObservableList = DestObvListA;
        }

        if (order == "DestFirst") {
            ObvListSyncA.DestinationObservableList = DestObvListA;
            ObvListSyncA.SourceObservableList = SourceObvListA;
        }

        Assert.AreEqual(SourceObvListA.Count, DestObvListA.Count);
        for (var index = 0; index < 3; index++) {
            Assert.AreEqual(SourceObvListA[index].MyNum.ToString(), DestObvListA[index].MyNum);
            Assert.AreEqual(SourceObvListA[index].MyStringLower.ToUpper(), DestObvListA[index].MyStringUpper);
        }
    }

    [Description("Creates a sync and tests all combinations of assignment of lists.")]
    [TestCase(0, 2, "Dest")]
    [TestCase(0, 3, "Source")]
    [TestCase(1, 2, "Dest")]
    [TestCase(1, 3, "Clear")]
    [TestCase(2, 0, "Source")]
    [TestCase(2, 1, "Dest")]
    [TestCase(3, 0, "Source")]
    [TestCase(3, 1, "Clear")]

    public void TestMethod_TestAfterInitializeReplace(int firstCommand, int secondCommand, string result) {
        SourceObvListA = ItemASource.GetSampleSourceItemAList();
        DestObvListA = ItemADest.GetSampleDestItemAList();

        var sourceItemCheck = SourceObvListA[0].MyStringLower;
        var destItemCheck = DestObvListA[0].MyStringUpper;

        ObvListSyncA = new ObservableListSynchronizerFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() }
        );

        List<Action> actionList = new();

        actionList.Insert(0, () => ObvListSyncA.ReplaceSource_SyncToDestination(SourceObvListA));
        actionList.Insert(1, () => ObvListSyncA.ReplaceSource_SyncFromDestination(SourceObvListA));

        actionList.Insert(2, () => ObvListSyncA.ReplaceDestination_SyncToSource(DestObvListA));
        actionList.Insert(3, () => ObvListSyncA.ReplaceDestination_SyncFromSource(DestObvListA));

        actionList[firstCommand].Invoke();
        actionList[secondCommand].Invoke();

        //Both lists are cleared
        if (result == "Clear") {
            Assert.AreEqual(SourceObvListA.Count, DestObvListA.Count);
            Assert.AreEqual(0, SourceObvListA.Count);
            return;
        }

        //Test that lists are synced
        Assert.AreEqual(SourceObvListA.Count, DestObvListA.Count);
        for (var index = 0; index < 3; index++) {
            Assert.AreEqual(SourceObvListA[index].MyNum.ToString(), DestObvListA[index].MyNum);
            Assert.AreEqual(SourceObvListA[index].MyStringLower.ToUpper(), DestObvListA[index].MyStringUpper);
            Console.WriteLine("Source: " + SourceObvListA[index].MyStringLower + "  Dest: " + DestObvListA[index].MyStringUpper);
        }

        if (result == "Source") Assert.AreEqual(sourceItemCheck, SourceObvListA[0].MyStringLower); // Source list is preserved
        if (result == "Dest") Assert.AreEqual(destItemCheck, DestObvListA[0].MyStringUpper); // Dest list is preserved
    }

    [Test, Description("Tests synchronization when adding and removing items for a two way sync.")]
    public void TestMethod_AddSubtractTwoWaySync() {
        SourceObvListA = new ObservableList<ItemASource>();
        DestObvListA = new ObservableList<ItemADest>();

        ObvListSyncA = new ObservableListSynchronizerFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
            SourceObvListA,
            DestObvListA
        );

        var item1 = new ItemASource { MyNum = 10, MyStringLower = "x" };
        var item2 = new ItemASource { MyNum = 15, MyStringLower = "y" };

        SourceObvListA.Add(item1);
        SourceObvListA.Add(item2);

        var item3 = new ItemADest { MyNum = "1000", MyStringUpper = "A" };
        var item4 = new ItemADest { MyNum = "2000", MyStringUpper = "B" };
        DestObvListA.Add(item3);
        DestObvListA.Add(item4);

        Assert.AreEqual(SourceObvListA.Count, DestObvListA.Count);
        Assert.AreEqual(4, SourceObvListA.Count);

        for (var index = 0; index < SourceObvListA.Count; index++) {
            Assert.AreEqual(SourceObvListA[index].MyNum.ToString(), DestObvListA[index].MyNum);
            Assert.AreEqual(SourceObvListA[index].MyStringLower.ToUpper(), DestObvListA[index].MyStringUpper);
            Console.WriteLine("Source: " + SourceObvListA[index].MyStringLower + "  Dest: " + DestObvListA[index].MyStringUpper);
        }

        Assert.AreEqual(SourceObvListA[0], item1);
        Assert.AreEqual(SourceObvListA[1], item2);
        Assert.AreEqual(DestObvListA[2], item3);
        Assert.AreEqual(DestObvListA[3], item4);
    }

    [Test, Description("Tests synchronization when adding and removing items for a one way sync.")]
    public void TestMethod_AddSubtractOneWaySync() {
        SourceObvListA = new ObservableList<ItemASource>();
        DestObvListA = new ObservableList<ItemADest>();

        ObvListSyncA = new ObservableListSynchronizerFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
            SourceObvListA,
            DestObvListA) {
            IsSyncSourceToDestCollection = true,
            IsSyncDestToSourceCollection = false
        };

        var item1 = new ItemASource { MyNum = 10, MyStringLower = "x" };
        var item2 = new ItemASource { MyNum = 15, MyStringLower = "y" };
        var item3 = new ItemADest { MyNum = "1000", MyStringUpper = "A" };
        var item4 = new ItemADest { MyNum = "2000", MyStringUpper = "B" };

        SourceObvListA.Add(item1);
        SourceObvListA.Add(item2);

        DestObvListA.Add(item3);
        DestObvListA.Add(item4);

        Assert.AreEqual(2, SourceObvListA.Count);
        Assert.AreEqual(4, DestObvListA.Count);

        for (var index = 0; index < SourceObvListA.Count; index++) {
            Assert.AreEqual(SourceObvListA[index].MyNum.ToString(), DestObvListA[index].MyNum);
            Assert.AreEqual(SourceObvListA[index].MyStringLower.ToUpper(), DestObvListA[index].MyStringUpper);
            Console.WriteLine("Source: " + SourceObvListA[index].MyStringLower + "  Dest: " + DestObvListA[index].MyStringUpper);
        }

        Assert.AreEqual(SourceObvListA[0], item1);
        Assert.AreEqual(SourceObvListA[1], item2);

        Assert.AreEqual(DestObvListA[0], ObvListSyncA.ConvertSourceToDestination(item1));
        Assert.AreEqual(DestObvListA[1], ObvListSyncA.ConvertSourceToDestination(item2));

        Assert.AreEqual(DestObvListA[2], item3);
        Assert.AreEqual(DestObvListA[3], item4);
    }
}
