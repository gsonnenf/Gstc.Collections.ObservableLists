using System;
using Gstc.Collections.ObservableLists.Binding;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using Gstc.Utility.UnitTest.Event;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListBindPropertyTest {

    public static object[] DataSource_Empty => new object[] {
       new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(), new ObservableList<ItemB>(), PropertyBindType.UpdateCollectionNotify),
       new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(), new ObservableList<ItemB>(), customPropertyMap: new CustomPropertyMapItemAB())
    };

    public static object[] DataSource_Populated => new object[] {
        new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(){ ItemA1 }, new ObservableList<ItemB>(), PropertyBindType.UpdateCollectionNotify),
        new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(){ ItemA1 }, new ObservableList<ItemB>(), customPropertyMap: new CustomPropertyMapItemAB())
    };
    public static object[] DataSource_NullTest => new object[] {
       ()=> new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(), new ObservableList<ItemB>(), PropertyBindType.UpdatePropertyNotify),
       ()=> new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(), new ObservableList<ItemB>(), PropertyBindType.UpdateCollectionNotify),
       ()=> new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(), new ObservableList<ItemB>(), customPropertyMap: new CustomPropertyMapItemAB())
    };

    #region Test Fixture
    public static ItemA ItemA1 => new() { MyNum = 0, MyStringLower = "string number 0" };
    public static ItemB ItemB1 => new() { MyNum = "0", MyStringUpper = "STRING NUMBER 0" };
    public static ItemA ItemA2 => new() { MyNum = 1, MyStringLower = "string number 1" };
    public static ItemB ItemB2 => new() { MyNum = "1", MyStringUpper = "STRING NUMBER 1" };

    #endregion

    [Test, Description("Tests that sync is ignored properly when one of the lists is null.")]
    public void ReplaceList_WithNulls_DoesNotThrowExceptions(
     [ValueSource(nameof(DataSource_NullTest))] Func<ObservableListBindProperty_ItemAB> obvListBindGenerator,
     [Values] ListIdentifier testList) {
        ObservableListBindProperty_ItemAB obvListBind = obvListBindGenerator();
        if (testList == ListIdentifier.ListA) obvListBind.ObservableListA = null;
        if (testList == ListIdentifier.ListB) obvListBind.ObservableListB = null;

        obvListBind.ObservableListA?.Add(ItemA1);
        _ = (obvListBind.ObservableListA?.Remove(ItemA1));
        obvListBind.ObservableListA?.Clear();

        obvListBind.ObservableListB?.Add(ItemB1);
        _ = (obvListBind.ObservableListB?.Remove(ItemB1));
        obvListBind.ObservableListB?.Clear();

        if (testList == ListIdentifier.ListA) {
            obvListBind.ObservableListB.Add(ItemB1);
            obvListBind.ObservableListB[0].MyNum = "100";
            Assert.That(obvListBind.ObservableListA, Is.Null);
        }

        if (testList == ListIdentifier.ListB) {
            obvListBind.ObservableListA.Add(ItemA1);
            obvListBind.ObservableListA[0].MyNum = 100;
            Assert.That(obvListBind.ObservableListB, Is.Null);
        }
    }

    [Test, Description("Tests that property changes are propagated from one list item to the other for UpdateCollectionNotify and UpdateCustomNotify.")]
    [TestCaseSource(nameof(DataSource_Populated))]
    public void BidirectionalSync_ItemsSynchronizeAsExpected(ObservableListBindProperty_ItemAB obvListBind) {
        //Initialization
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(0));
            Assert.That(obvListBind.ObservableListB[0].MyNum, Is.EqualTo("0"));
        });

        //Tests directionality on assignment
        obvListBind.IsBidirectional = true;
        obvListBind.ObservableListB[0].MyNum = "10";
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(10));
            Assert.That(obvListBind.ObservableListB[0].MyNum, Is.EqualTo("10"));
        });

        // Tests directionality on false
        obvListBind.IsBidirectional = false;
        obvListBind.ObservableListB[0].MyNum = "20";
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(10));
            Assert.That(obvListBind.ObservableListB[0].MyNum, Is.EqualTo("20"));
        });
        obvListBind.ObservableListA[0].MyNum = 30;
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(30));
            Assert.That(obvListBind.ObservableListB[0].MyNum, Is.EqualTo("30"));
        });

        //Test directionality on toggle back to true
        obvListBind.IsBidirectional = true;
        obvListBind.ObservableListB[0].MyNum = "100";
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(100));
            Assert.That(obvListBind.ObservableListB[0].MyNum, Is.EqualTo("100"));
        });
        obvListBind.ObservableListA[0].MyNum = 200;
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(200));
            Assert.That(obvListBind.ObservableListB[0].MyNum, Is.EqualTo("200"));
        });
    }

    [Test, Description("Tests that property bind is disabled when IsPropertyBindEnabled set.")]
    [TestCaseSource(nameof(DataSource_Empty))]
    public void IsPropertyBindEnabled_PropertiesSyncWhenEnabled_DoNotSyncWhenDisabled(ObservableListBindProperty_ItemAB obvListBind) {

        //tests constructor enabled
        obvListBind.ObservableListA.Add(ItemA1);
        obvListBind.ObservableListB[0].MyNum = "10";
        Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(10));

        //tests disabled
        obvListBind.IsPropertyBindingEnabled = false;
        obvListBind.ObservableListB.Add(ItemB2);
        obvListBind.ObservableListB[0].MyNum = "100";
        obvListBind.ObservableListA[1].MyNum = 100;
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(10));
            Assert.That(obvListBind.ObservableListB[1].MyNum, Is.EqualTo("1"));
        });

        //Test re-enabled
        obvListBind.IsPropertyBindingEnabled = true;
        obvListBind.ObservableListB[0].MyNum = "1000";
        obvListBind.ObservableListA[1].MyNum = 1000;
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[1].MyNum, Is.EqualTo(1000));
            Assert.That(obvListBind.ObservableListB[1].MyNum, Is.EqualTo("1000"));
        });
    }

    #region UpdateProperty
    [Test, Description("Tests that property notify events are propogated from source to target on PropertyBindType.UpdatePropertyNotify using INotifyPropertyChanged and reflection.")]
    public void UpdatePropertyNotify_PropertiesSyncViaReflection() {
        ObservableListBindProperty_ItemMVM obvListBind = new(
            obvListA: new ObservableList<ItemModel>() { new ItemModel() { PhoneNumber = 8005551111 } },
            obvListB: new ObservableList<ItemViewModel>(),
            bindType: PropertyBindType.UpdatePropertyNotify
            );

        AssertNotifyProperty testItemM = new(obvListBind.ObservableListA[0]);
        AssertNotifyProperty testItemVM = new(obvListBind.ObservableListB[0]);
        ItemModel ItemM1 = obvListBind.ObservableListA[0];
        ItemViewModel ItemVM1 = obvListBind.ObservableListB[0];

        obvListBind.ObservableListA[0].PhoneNumber = 5055551111;

        Assert.Multiple(() => {
            Assert.That(testItemM.TestOnChangedTimesCalled(1), testItemM.ErrorMessages);
            Assert.That(testItemVM.TestOnChangedTimesCalled(1), testItemVM.ErrorMessages);
            Assert.That(obvListBind.ObservableListA[0], Is.SameAs(ItemM1));
            Assert.That(obvListBind.ObservableListB[0], Is.SameAs(ItemVM1));
            Assert.That(obvListBind.ObservableListA[0].PhoneNumber, Is.EqualTo(5055551111));
            Assert.That(obvListBind.ObservableListB[0].PhoneNumber, Is.EqualTo("505-555-1111"));

        });

        obvListBind.ObservableListB[0].PhoneNumber = "408-555-1111";
        obvListBind.ObservableListB[0].PhoneNumber = "415-555-1111";

        Assert.Multiple(() => {
            Assert.That(testItemM.TestOnChangedTimesCalled(2), testItemM.ErrorMessages);
            Assert.That(testItemVM.TestOnChangedTimesCalled(2), testItemVM.ErrorMessages);
            Assert.That(obvListBind.ObservableListA[0], Is.SameAs(ItemM1));
            Assert.That(obvListBind.ObservableListB[0], Is.SameAs(ItemVM1));
            Assert.That(obvListBind.ObservableListA[0].PhoneNumber, Is.EqualTo(4155551111));
            Assert.That(obvListBind.ObservableListB[0].PhoneNumber, Is.EqualTo("415-555-1111"));
        });
    }

    [Test, Description("Tests that property notify events are propagated from source to target on PropertyBindType.UpdatePropertyNotify using INotifyPropertyChangedHook.")]
    public void UpdatePropertyNotify_PropertiesSyncViaINotifyPropertyChangedHook() {
        ObservableListBindProperty_ItemMVMHook obvListBind = new(
            obvListA: new ObservableList<ItemModelHook>() { new ItemModelHook() { PhoneNumber = 8005551111 } },
            obvListB: new ObservableList<ItemViewModelHook>(),
            bindType: PropertyBindType.UpdatePropertyNotify
            );

        AssertNotifyProperty testItemM = new(obvListBind.ObservableListA[0]);
        AssertNotifyProperty testItemVM = new(obvListBind.ObservableListB[0]);
        ItemModelHook ItemM1 = obvListBind.ObservableListA[0];
        ItemViewModelHook ItemVM1 = obvListBind.ObservableListB[0];

        obvListBind.ObservableListA[0].PhoneNumber = 5055551111;

        Assert.Multiple(() => {
            Assert.That(testItemM.TestOnChangedTimesCalled(1), testItemM.ErrorMessages);
            Assert.That(testItemVM.TestOnChangedTimesCalled(1), testItemVM.ErrorMessages);
            Assert.That(obvListBind.ObservableListA[0], Is.SameAs(ItemM1));
            Assert.That(obvListBind.ObservableListB[0], Is.SameAs(ItemVM1));
            Assert.That(obvListBind.ObservableListA[0].PhoneNumber, Is.EqualTo(5055551111));
            Assert.That(obvListBind.ObservableListB[0].PhoneNumber, Is.EqualTo("505-555-1111"));
        });

        obvListBind.ObservableListB[0].PhoneNumber = "408-555-1111";
        obvListBind.ObservableListB[0].PhoneNumber = "415-555-1111";

        Assert.Multiple(() => {
            Assert.That(testItemM.TestOnChangedTimesCalled(2), testItemM.ErrorMessages);
            Assert.That(testItemVM.TestOnChangedTimesCalled(2), testItemVM.ErrorMessages);
            Assert.That(obvListBind.ObservableListA[0], Is.SameAs(ItemM1));
            Assert.That(obvListBind.ObservableListB[0], Is.SameAs(ItemVM1));
            Assert.That(obvListBind.ObservableListA[0].PhoneNumber, Is.EqualTo(4155551111));
            Assert.That(obvListBind.ObservableListB[0].PhoneNumber, Is.EqualTo("415-555-1111"));
        });
    }

    [Test, Description("Tests that property notify events are propogated from source to target on PropertyBindUpdateCustom with a custom map.")]
    public void UpdateCustomNotify_PropertiesSyncUsingCustomPropertyMap() {
        ObservableListBindProperty_ItemAB obvListBind = new(
            obvListA: new ObservableList<ItemA>() { ItemA1 },
            obvListB: new ObservableList<ItemB>(),
            customPropertyMap: new CustomPropertyMapItemAB()
            );

        ItemA ItemAInitial = obvListBind.ObservableListA[0];
        ItemB ItemBInitial = obvListBind.ObservableListB[0];

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(0));
            Assert.That(obvListBind.ObservableListB[0].MyNum, Is.EqualTo("0"));
        });

        obvListBind.ObservableListA[0].MyNum = 10;

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(10));
            Assert.That(obvListBind.ObservableListB[0].MyNum, Is.EqualTo("10"));
            Assert.That(obvListBind.ObservableListA[0], Is.SameAs(ItemAInitial));
            Assert.That(obvListBind.ObservableListB[0], Is.SameAs(ItemBInitial));
        });

        string testString = "NEW TEST STRING";
        obvListBind.ObservableListB[0].MyStringUpper = testString;

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0].MyStringLower, Is.EqualTo(testString.ToLower()));
            Assert.That(obvListBind.ObservableListB[0].MyStringUpper, Is.EqualTo(testString));
            Assert.That(obvListBind.ObservableListA[0], Is.SameAs(ItemAInitial));
            Assert.That(obvListBind.ObservableListB[0], Is.SameAs(ItemBInitial));
        });
    }
    #endregion

    [Test, Description("Tests and demonstrates behavior of uni-directional repeat cascade resulting from repeat items in the same source list.")]
    public void UpdateCollectionNotify_DemonstratesCascadeUpdateCollection_NewTargetItemsAreCreatedAsExpected() {
        ItemA itemA1 = ItemA1;

        ObservableListBindProperty_ItemAB obvListBind = new(
           obvListA: new ObservableList<ItemA>() { itemA1 },
           obvListB: new ObservableList<ItemB>()
           );

        obvListBind.ObservableListA.Add(itemA1);
        obvListBind.ObservableListA.Add(itemA1);

        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0], Is.SameAs(obvListBind.ObservableListA[1]));
            Assert.That(obvListBind.ObservableListB[0], Is.Not.SameAs(obvListBind.ObservableListB[1]));
        });

        obvListBind.ObservableListA[0].MyNum = 10;
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0], Is.SameAs(obvListBind.ObservableListA[1]));
            Assert.That(obvListBind.ObservableListB[0], Is.Not.SameAs(obvListBind.ObservableListB[1]));
            Assert.That(obvListBind.ObservableListB[0], Is.EqualTo(obvListBind.ObservableListB[1]));
        });

        obvListBind.ObservableListB[0].MyNum = "100";
        Assert.Multiple(() => {
            Assert.That(obvListBind.ObservableListA[0], Is.Not.SameAs(obvListBind.ObservableListA[1]));
            Assert.That(obvListBind.ObservableListB[0], Is.Not.SameAs(obvListBind.ObservableListB[1]));
            Assert.That(obvListBind.ObservableListB[0], Is.Not.EqualTo(obvListBind.ObservableListB[1]));
        });
    }
}
