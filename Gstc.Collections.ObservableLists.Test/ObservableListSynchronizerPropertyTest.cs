using Moq;
using NUnit.Framework;
using Gstc.Collections.ObservableLists.Test.MockObjects;

namespace Gstc.Collections.ObservableLists.Test {

    /// <summary>
    /// Test the ObservableListSynchronizer for synchronizing properties within items.
    /// </summary>
    [TestFixture]
    public class ObservableListSynchronizerPropertyTest {

        public ObservableList<ItemBSource> SourceObvListB;
        public ObservableList<ItemBDest> DestObvListB;
        public ObservableListSynchronizer<ItemBSource, ItemBDest> ObvListSyncB;

        protected MockEventClass MockEvent { get; set; }
        protected AssertEventClass AssertEvent { get; set; }

        [SetUp]
        public void TestInit() {
            MockEvent = new MockEventClass();
            AssertEvent = MockEvent.Object;
        }

        //TODO: Find a test situation where a two way notify might be used. It seems that any user written mapping between objects would trigger INotifyProperty without needing this.
        [Test, NUnit.Framework.Description("Test notification between a Source object, and a pass through destination object")]
        public void TestMethod_PropertyNotify() {
            SourceObvListB = new ObservableList<ItemBSource>();
            DestObvListB = new ObservableList<ItemBDest>();

            ObvListSyncB = new ObservableListSynchronizerFunc<ItemBSource, ItemBDest>(
              (sourceItem) => new ItemBDest(sourceItem),
              (destItem) => destItem.ItemBSourceItem,
              SourceObvListB,
              DestObvListB,
              true,
              true
          );
           
            var item1 = new ItemBSource { MyNum = 10, MyStringLower = "x" };       
            var item2 = new ItemBDest { MyNum = "1000", MyStringUpper = "A" };
            
            SourceObvListB.Add(item1);
            DestObvListB.Add(item2);

            SourceObvListB[0].PropertyChanged += (sender, args) => AssertEvent.Call("source[0] event");
            DestObvListB[0].PropertyChanged += (sender, args) => AssertEvent.Call("dest[0] event");
            SourceObvListB[1].PropertyChanged += (sender, args) => AssertEvent.Call("source[1] event");
            DestObvListB[1].PropertyChanged += (sender, args) => AssertEvent.Call("dest[1] event");
            
            var string1 = "TEST STRING";
            var string2 = "TEST STRING AGAIN";
            SourceObvListB[0].MyNum = -1;
            DestObvListB[1].MyStringUpper = string1;
            DestObvListB[1].MyStringUpper = string2;

            Assert.AreEqual(string2, DestObvListB[1].MyStringUpper);
            Assert.AreEqual(string2.ToLower(), SourceObvListB[1].MyStringLower);

            MockEvent.Verify(m => m.Call("source[0] event"), Times.Exactly(1));
            MockEvent.Verify(m => m.Call("dest[0] event"), Times.Exactly(1));
            MockEvent.Verify(m => m.Call("source[1] event"), Times.Exactly(2));
            MockEvent.Verify(m => m.Call("dest[1] event"), Times.Exactly(2));
        }
    }
}
