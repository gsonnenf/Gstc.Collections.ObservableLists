using Gstc.Collections.ObservableLists.Test.MockObjects;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListBindPropertyTest {
    //TODO: Find a test situation where a two way notify might be used. It seems that any user written mapping between objects would trigger INotifyProperty without needing this.

    public static object[] DataSource_Empty => new object[] {
       new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(), new ObservableList<ItemB>()),
    };

    public static object[] DataSource_Populated => new object[] {
       new ObservableListBindProperty_ItemAB(new ObservableList<ItemA>(){ ItemA1 }, new ObservableList<ItemB>()),
    };

    #region Test Fixture
    public static ItemA ItemA1 => new() { MyNum = 0, MyStringLower = "string number 0" };
    public static ItemB ItemB1 => new() { MyNum = "0", MyStringUpper = "STRING NUMBER 0" };
    public static ItemA ItemA2 => new() { MyNum = 1, MyStringLower = "string number 1" };
    public static ItemB ItemB2 => new() { MyNum = "1", MyStringUpper = "STRING NUMBER 1" };

    #endregion

    [Test, Description("Tests that notify collection are triggered when property is set.")]
    [TestCaseSource(nameof(DataSource_Populated))]
    public void BidirectionalTest(ObservableListBindProperty_ItemAB obvListBind) {

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

    [Test, Description("Tests that property bind can be disabled.")]
    [TestCaseSource(nameof(DataSource_Empty))]
    public void IsPropertyBindEnabled(ObservableListBindProperty_ItemAB obvListBind) {
        //tests constructor enabled
        obvListBind.ObservableListA.Add(ItemA1);
        obvListBind.ObservableListB[0].MyNum = "10";
        Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(10));

        //tests disabled
        obvListBind.IsPropertyBindingEnabled = false;
        obvListBind.ObservableListB.Add(ItemB2);
        obvListBind.ObservableListB[0].MyNum = "100";
        obvListBind.ObservableListA[1].MyNum = 100;
        Assert.That(obvListBind.ObservableListA[0].MyNum, Is.EqualTo(10));
        Assert.That(obvListBind.ObservableListB[1].MyNum, Is.EqualTo("1"));

        //Test re-enabled
        obvListBind.IsPropertyBindingEnabled = true;
        obvListBind.ObservableListB[0].MyNum = "1000";
        obvListBind.ObservableListA[1].MyNum = 1000;
        Assert.That(obvListBind.ObservableListA[1].MyNum, Is.EqualTo(1000));
        Assert.That(obvListBind.ObservableListB[1].MyNum, Is.EqualTo("1000"));
    }
    /*
    [Test, Description("Test that changes in item properties of one list propagate to the other if INotifyPropertyChanged is implemented on the TItem")]
    public void TestMethod_PropertyNotify() {
        //Arrange
        ObservableList<ItemS> sourceObvListB = new();
        ObservableList<ItemBDest> destObvListB = new();

        ObservableListBindingFunc<ItemBSource, ItemBDest> obvListSyncB = new(
            (sourceItem) => new ItemBDest(sourceItem),
            (destItem) => destItem.ItemBSourceItem,
            sourceObvListB,
            destObvListB,
            true,
            true
        );

        sourceObvListB.Add(new ItemBSource { MyNum = 10, MyStringLower = "x" });
        destObvListB.Add(new ItemBDest { MyNum = "1000", MyStringUpper = "A" });
        const string string0 = "First Synchronized String";
        const string string1 = "Second Synchronized String";

        //Add event checks
        int sourceEventCount0 = 0;
        int destEventCount0 = 0;
        int sourceEventCount1 = 0;
        int destEventCount1 = 0;

        sourceObvListB[0].PropertyChanged += (_, _) => sourceEventCount0++;
        destObvListB[0].PropertyChanged += (_, _) => destEventCount0++;
        sourceObvListB[1].PropertyChanged += (_, _) => sourceEventCount1++;
        destObvListB[1].PropertyChanged += (_, _) => destEventCount1++;

        //Act
        sourceObvListB[0].MyNum = -1;
        sourceObvListB[0].MyStringLower = string0.ToLower();
        destObvListB[1].MyStringUpper = string1.ToUpper();

        Assert.Multiple(() => {
            Assert.That(destObvListB[0].MyNum, Is.EqualTo("-1"));
            Assert.That(destObvListB[0].MyStringUpper, Is.EqualTo(string0.ToUpper()));
            Assert.That(sourceObvListB[1].MyStringLower, Is.EqualTo(string1.ToLower()));

            Assert.That(sourceEventCount0, Is.EqualTo(2));
            Assert.That(destEventCount0, Is.EqualTo(2));
            Assert.That(sourceEventCount1, Is.EqualTo(1));
            Assert.That(destEventCount1, Is.EqualTo(1));
        });
    }
    */
}
