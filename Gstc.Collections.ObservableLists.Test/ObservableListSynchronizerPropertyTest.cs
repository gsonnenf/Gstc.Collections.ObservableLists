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
        var sourceObvListB = new ObservableList<ItemBSource>();
        var destObvListB = new ObservableList<ItemBDest>();
        var obvListSyncB = new ObservableListSynchronizerFunc<ItemBSource, ItemBDest>(
            (sourceItem) => new ItemBDest(sourceItem),
            (destItem) => destItem.ItemBSourceItem,
            sourceObvListB,
            destObvListB,
            true,
            true
        );

        sourceObvListB.Add(new ItemBSource { MyNum = 10, MyStringLower = "x" });
        destObvListB.Add(new ItemBDest { MyNum = "1000", MyStringUpper = "A" });
        var string0 = "First Synchronized String";
        var string1 = "Second Synchronized String";

        //Add event checks
        var sourceEventCount0 = 0;
        var destEventCount0 = 0;
        var sourceEventCount1 = 0;
        var destEventCount1 = 0;

        sourceObvListB[0].PropertyChanged += (sender, args) => sourceEventCount0++;
        destObvListB[0].PropertyChanged += (sender, args) => destEventCount0++;
        sourceObvListB[1].PropertyChanged += (sender, args) => sourceEventCount1++;
        destObvListB[1].PropertyChanged += (sender, args) => destEventCount1++;

        //Act
        sourceObvListB[0].MyNum = -1;
        sourceObvListB[0].MyStringLower = string0.ToLower();
        destObvListB[1].MyStringUpper = string1.ToUpper();

        //Assert
        Assert.That(destObvListB[0].MyNum, Is.EqualTo("-1"));
        Assert.That(destObvListB[0].MyStringUpper, Is.EqualTo(string0.ToUpper()));
        Assert.That(sourceObvListB[1].MyStringLower, Is.EqualTo(string1.ToLower()));

        Assert.That(sourceEventCount0, Is.EqualTo(2));
        Assert.That(destEventCount0, Is.EqualTo(2));
        Assert.That(sourceEventCount1, Is.EqualTo(1));
        Assert.That(destEventCount1, Is.EqualTo(1));
    }
}
