using System;
using Gstc.Collections.ObservableLists.Binding;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListBindTest { //todo: validate all these tests for new form

    public ObservableList<ItemASource> SourceObvListA;
    public ObservableList<ItemADest> DestObvListA;
    public ObservableListBind<ItemASource, ItemADest> ObvListSyncA;

    [Test, Description("Creates a sync and tests initialization copying from source item to dest item.")]
    public void TestMethod_CopyOnInitialize() {
        SourceObvListA = ItemASource.GetSampleSourceItemAList();
        DestObvListA = new ObservableList<ItemADest>();

        ObvListSyncA = new ObservableListBindFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
            SourceObvListA,
            DestObvListA
        );

        Assert.Multiple(() => {
            Assert.That(DestObvListA, Has.Count.EqualTo(SourceObvListA.Count));
            for (int index = 0; index < 3; index++) {
                Assert.That(DestObvListA[index].MyNum, Is.EqualTo(SourceObvListA[index].MyNum.ToString()));
                Assert.That(DestObvListA[index].MyStringUpper, Is.EqualTo(SourceObvListA[index].MyStringLower.ToUpper()));
            }
        });
    }

    [Description("Creates a sync and tests assignment copying from source item to dest item.")]
    [TestCase("SourceFirst")]
    [TestCase("DestFirst")]
    public void TestMethod_TestAfterInitialize(string order) {
        SourceObvListA = ItemASource.GetSampleSourceItemAList();
        DestObvListA = new ObservableList<ItemADest>();

        ObvListSyncA = new ObservableListBindFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() }
        );

        //Tests several order with same boiler plate code.
        if (order == "SourceFirst") {
            ObvListSyncA.ObservableListA = SourceObvListA;
            ObvListSyncA.ObservableListB = DestObvListA;
        }

        if (order == "DestFirst") {
            ObvListSyncA.ObservableListB = DestObvListA;
            ObvListSyncA.ObservableListA = SourceObvListA;
        }
        Assert.Multiple(() => {
            Assert.That(DestObvListA, Has.Count.EqualTo(SourceObvListA.Count));
            for (int index = 0; index < 3; index++) {
                Assert.That(DestObvListA[index].MyNum, Is.EqualTo(SourceObvListA[index].MyNum.ToString()));
                Assert.That(DestObvListA[index].MyStringUpper, Is.EqualTo(SourceObvListA[index].MyStringLower.ToUpper()));
            }
        });
    }

    //todo: fix this test
    /*
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

        string sourceItemCheck = SourceObvListA[0].MyStringLower;
        string destItemCheck = DestObvListA[0].MyStringUpper;

        ObvListSyncA = new ObservableListBindingFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },



            );

        List<Action> actionList = new();

        actionList.Insert(0, () => ObvListSyncA.ReplaceListA(SourceObvListA));
        actionList.Insert(1, () => ObvListSyncA.ReplaceSource_SyncFromDestination(SourceObvListA));

        actionList.Insert(2, () => ObvListSyncA.ReplaceDestination_SyncToSource(DestObvListA));
        actionList.Insert(3, () => ObvListSyncA.ReplaceDestination_SyncFromSource(DestObvListA));

        actionList[firstCommand].Invoke();
        actionList[secondCommand].Invoke();

        //Both lists are cleared
        if (result == "Clear") {
            Assert.Multiple(() => {
                Assert.That(DestObvListA, Is.Empty);
                Assert.That(SourceObvListA, Is.Empty);
            });
            return;
        }

        //Test that lists are synced
        Assert.Multiple(() => {
            Assert.That(DestObvListA, Has.Count.EqualTo(SourceObvListA.Count));
            for (int index = 0; index < 3; index++) {
                Assert.That(DestObvListA[index].MyNum, Is.EqualTo(SourceObvListA[index].MyNum.ToString()));
                Assert.That(DestObvListA[index].MyStringUpper, Is.EqualTo(SourceObvListA[index].MyStringLower.ToUpper()));
                Console.WriteLine("Source: " + SourceObvListA[index].MyStringLower + "  Dest: " + DestObvListA[index].MyStringUpper);
            }

            if (result == "Source") Assert.That(SourceObvListA[0].MyStringLower, Is.EqualTo(sourceItemCheck)); // Source list is preserved
            if (result == "Dest") Assert.That(DestObvListA[0].MyStringUpper, Is.EqualTo(destItemCheck)); // Dest list is preserved
        });
    }
    */
    [Test, Description("Tests synchronization when adding and removing items for a two way sync.")]
    public void TestMethod_AddSubtractTwoWaySync() {
        SourceObvListA = new ObservableList<ItemASource>();
        DestObvListA = new ObservableList<ItemADest>();

        ObvListSyncA = new ObservableListBindFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
            SourceObvListA,
            DestObvListA
        );

        ItemASource item1 = new() { MyNum = 10, MyStringLower = "x" };
        ItemASource item2 = new() { MyNum = 15, MyStringLower = "y" };

        SourceObvListA.Add(item1);
        SourceObvListA.Add(item2);

        ItemADest item3 = new() { MyNum = "1000", MyStringUpper = "A" };
        ItemADest item4 = new() { MyNum = "2000", MyStringUpper = "B" };
        DestObvListA.Add(item3);
        DestObvListA.Add(item4);

        Assert.Multiple(() => {
            Assert.That(DestObvListA, Has.Count.EqualTo(SourceObvListA.Count));
            Assert.That(SourceObvListA, Has.Count.EqualTo(4));

            for (int index = 0; index < SourceObvListA.Count; index++) {
                Assert.That(DestObvListA[index].MyNum, Is.EqualTo(SourceObvListA[index].MyNum.ToString()));
                Assert.That(DestObvListA[index].MyStringUpper, Is.EqualTo(SourceObvListA[index].MyStringLower.ToUpper()));
                Console.WriteLine("Source: " + SourceObvListA[index].MyStringLower + "  Dest: " + DestObvListA[index].MyStringUpper);
            }

            Assert.That(item1, Is.EqualTo(SourceObvListA[0]));
            Assert.That(item2, Is.EqualTo(SourceObvListA[1]));
            Assert.That(item3, Is.EqualTo(DestObvListA[2]));
            Assert.That(item4, Is.EqualTo(DestObvListA[3]));
        });
    }

    //todo: fix this test
    /*
    [Test, Description("Tests synchronization when adding and removing items for a one way sync.")]
    public void TestMethod_AddSubtractOneWaySync() {
        SourceObvListA = new ObservableList<ItemASource>();
        DestObvListA = new ObservableList<ItemADest>();

        ObvListSyncA = new ObservableListBindingFunc<ItemASource, ItemADest>(
            (sourceItem) => new ItemADest { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemASource { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
            SourceObvListA,
            DestObvListA) {
            IsSyncListAToListB = true,
            IsSyncListBToListA = false
        };

        ItemASource item1 = new() { MyNum = 10, MyStringLower = "x" };
        ItemASource item2 = new() { MyNum = 15, MyStringLower = "y" };
        ItemADest item3 = new() { MyNum = "1000", MyStringUpper = "A" };
        ItemADest item4 = new() { MyNum = "2000", MyStringUpper = "B" };

        SourceObvListA.Add(item1);
        SourceObvListA.Add(item2);
        DestObvListA.Add(item3);
        DestObvListA.Add(item4);

        Assert.Multiple(() => {
            Assert.That(SourceObvListA, Has.Count.EqualTo(2));
            Assert.That(DestObvListA, Has.Count.EqualTo(4));

            for (int index = 0; index < SourceObvListA.Count; index++) {
                Assert.That(DestObvListA[index].MyNum, Is.EqualTo(SourceObvListA[index].MyNum.ToString()));
                Assert.That(DestObvListA[index].MyStringUpper, Is.EqualTo(SourceObvListA[index].MyStringLower.ToUpper()));
                Console.WriteLine("Source: " + SourceObvListA[index].MyStringLower + "  Dest: " + DestObvListA[index].MyStringUpper);
            }

            Assert.That(item1, Is.EqualTo(SourceObvListA[0]));
            Assert.That(item2, Is.EqualTo(SourceObvListA[1]));

            Assert.That(ObvListSyncA.ConvertItem(item1), Is.EqualTo(DestObvListA[0]));
            Assert.That(ObvListSyncA.ConvertItem(item2), Is.EqualTo(DestObvListA[1]));

            Assert.That(item3, Is.EqualTo(DestObvListA[2]));
            Assert.That(item4, Is.EqualTo(DestObvListA[3]));
        });
    }
    */
}
