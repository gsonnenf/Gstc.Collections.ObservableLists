using Gstc.Collections.ObservableLists.Synchronizer;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListSynchronizerPropertyTest {

    //TODO: Find a test situation where a two way notify might be used. It seems that any user written mapping between objects would trigger INotifyProperty without needing this.
    [Test, Description("Test that changes in item properties of one list propagate to the other if INotifyPropertyChanged is implemented on the TItem")]
    public void TestMethod_PropertyNotify() {
        //Arrange
        ObservableList<ItemBSource> sourceObvListB = new();
        ObservableList<ItemBDest> destObvListB = new();

        ObservableListSynchronizerFunc<ItemBSource, ItemBDest> obvListSyncB = new(
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
}
