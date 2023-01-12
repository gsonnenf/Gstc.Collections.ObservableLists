using System;
using Gstc.Collections.ObservableLists.Binding;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListBindTest {

    #region Datasources
    public static object[] DataSource_ObvBind => new object[] {
       ()=> new ObservableListBind_Item1(new ObservableList<ItemA>(), new ObservableList<ItemB>()),
       ()=> new ObservableListBindFunc<ItemA, ItemB>(
            (sourceItem) => new ItemB { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemA { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
            new ObservableList<ItemA>(),
            new ObservableList<ItemB>())
    };

    public static object[] DataSource_ObvBind_Prepopulated => new object[] {
       ()=> new ObservableListBind_Item1(
           new ObservableList<ItemA>() {ItemA1},
           new ObservableList<ItemB>() {ItemB1}
           ),
       ()=>new ObservableListBindFunc<ItemA, ItemB>(
            (sourceItem) => new ItemB { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemA { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
             new ObservableList<ItemA>() {ItemA1 },
             new ObservableList<ItemB>() {ItemB1 }
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
    public void CopyOnInitialize_ObservableListBind() {
        ObservableListBind<ItemA, ItemB> obvListBind =
            new ObservableListBind_Item1(ItemA.GetSampleSourceItemAList(), new ObservableList<ItemB>());

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(obvListBind.ObservableListB.Count));
            for (int index = 0; index < 3; index++) {
                Assert.That(obvListBind.ObservableListB[index].MyNum, Is.EqualTo(obvListBind.ObservableListA[index].MyNum.ToString()));
                Assert.That(obvListBind.ObservableListB[index].MyStringUpper, Is.EqualTo(obvListBind.ObservableListA[index].MyStringLower.ToUpper()));
            }
        });
    }

    [Test, Description("Tests initialization from constructor for ObservableListBindFunc")]
    public void CopyOnInitialize_ObservableListBindFunc() {
        //Func
        ObservableListBind<ItemA, ItemB> obvListBind2 =
        new ObservableListBindFunc<ItemA, ItemB>(
            (sourceItem) => new ItemB { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
            (destItem) => new ItemA { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
            ItemA.GetSampleSourceItemAList(),
            new ObservableList<ItemB>()
        );

        Assert.Multiple(() => {
            Assert.That(obvListBind2.ObservableListA, Has.Count.EqualTo(obvListBind2.ObservableListB.Count));
            for (int index = 0; index < 3; index++) {
                Assert.That(obvListBind2.ObservableListB[index].MyNum, Is.EqualTo(obvListBind2.ObservableListA[index].MyNum.ToString()));
                Assert.That(obvListBind2.ObservableListB[index].MyStringUpper, Is.EqualTo(obvListBind2.ObservableListA[index].MyStringLower.ToUpper()));
            }
        });
    }

    [Test, Description("Ensures correct initialization of prepopulated")]
    [TestCaseSource(nameof(DataSource_ObvBind_Prepopulated))]
    public void Prepopulated(Func<ObservableListBind<ItemA, ItemB>> obvListBindGenerator) {
        ObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(1));
            Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(1));
            Assert.That(ItemA1, Is.EqualTo(obvListBind.ObservableListA[0]));
            Assert.That(ItemB1, Is.EqualTo(obvListBind.ObservableListB[0]));

        });
    }

    [Test, Description("Tests that list replace copies over properly")]
    public void CopyAfterInitialize(
            [ValueSource(nameof(DataSource_ObvBind))] Func<ObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] ListIdentifier listOrder) {
        ObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
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

    [Test, Description("Tests that bidirectional add works properly.")]
    public void Add(
            [ValueSource(nameof(DataSource_ObvBind))] Func<ObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {

        ObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
        obvListBind.IsBidirectional = isBidirectional;
        obvListBind.SourceList = sourceList;

        if (testList != sourceList && !isBidirectional) {
            _ = Assert.Throws<InvalidOperationException>(testList switch {
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
    public void Clear(
            [ValueSource(nameof(DataSource_ObvBind_Prepopulated))] Func<ObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {

        ObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
        obvListBind.IsBidirectional = isBidirectional;
        obvListBind.SourceList = sourceList;

        if (testList != sourceList && !isBidirectional) {
            _ = Assert.Throws<InvalidOperationException>(testList switch {
                ListIdentifier.ListA => () => obvListBind.ObservableListA.Clear(),
                ListIdentifier.ListB => () => obvListBind.ObservableListB.Clear()
            });
            return;
        }

        if (testList == ListIdentifier.ListA) obvListBind.ObservableListA.Clear();
        if (testList == ListIdentifier.ListB) obvListBind.ObservableListB.Clear();
        Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(0));
        Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(0));
    }

    [Test, Description("Tests that move works properly")]
    public void Move(
            [ValueSource(nameof(DataSource_ObvBind_Prepopulated))] Func<ObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {

        ObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
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
            _ = Assert.Throws<InvalidOperationException>(testList switch {
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
    public void Remove(
            [ValueSource(nameof(DataSource_ObvBind_Prepopulated))] Func<ObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {

        ObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
        obvListBind.IsBidirectional = isBidirectional;
        obvListBind.SourceList = sourceList;

        if (sourceList != testList && !isBidirectional) {
            _ = Assert.Throws<InvalidOperationException>(testList switch {
                ListIdentifier.ListA => () => obvListBind.ObservableListA.Remove(ItemA1),
                ListIdentifier.ListB => () => obvListBind.ObservableListB.Remove(ItemB1)
            });
            return;
        }

        if (testList == ListIdentifier.ListA) _ = obvListBind.ObservableListA.Remove(ItemA1);
        if (testList == ListIdentifier.ListB) _ = obvListBind.ObservableListB.Remove(ItemB1);
        Assert.That(obvListBind.ObservableListA, Has.Count.EqualTo(0));
        Assert.That(obvListBind.ObservableListB, Has.Count.EqualTo(0));
    }

    [Test, Description("Tests that replace works properly")]
    public void Replace(
            [ValueSource(nameof(DataSource_ObvBind_Prepopulated))] Func<ObservableListBind<ItemA, ItemB>> obvListBindGenerator,
            [Values] ListIdentifier sourceList,
            [Values] bool isBidirectional,
            [Values] ListIdentifier testList) {
        ObservableListBind<ItemA, ItemB> obvListBind = obvListBindGenerator();
        obvListBind.IsBidirectional = isBidirectional;
        obvListBind.SourceList = sourceList;

        if (testList != sourceList && !isBidirectional) {
            _ = Assert.Throws<InvalidOperationException>(testList switch {
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
}

