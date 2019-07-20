using Gstc.Collections.ObservableLists.Base;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Test {
    [TestFixture]
    public class ObservableListSyncronizerPropertyTest {

        public ObservableList<SourceItemB> SourceObvListB;
        public ObservableList<DestItemB> DestObvListB;
        public ObservableListSynchronizer<SourceItemB, DestItemB> ObvListSyncB;

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
            SourceObvListB = new ObservableList<SourceItemB>();
            DestObvListB = new ObservableList<DestItemB>();

            ObvListSyncB = new ObservableListSynchronizerFunc<SourceItemB, DestItemB>(
              (sourceItem) => new DestItemB(sourceItem),
              (destItem) => destItem.SourceItem,
              SourceObvListB,
              DestObvListB,
              true,
              true
          );
           
            var item1 = new SourceItemB { MyNum = 10, MyStringLower = "x" };       
            var item2 = new DestItemB { MyNum = "1000", MyStringUpper = "A" };
         

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

        #region Test Helpers

        private ObservableList<SourceItemB> getSampleSourceList() {
            return new ObservableList<SourceItemB> {
                new SourceItemB { MyNum = 10, MyStringLower = "x" },
                new SourceItemB { MyNum = 15, MyStringLower = "y" },
                new SourceItemB { MyNum = 20, MyStringLower = "z" },
                };
        }

        private ObservableList<DestItemB> getSampleDestList() {
            return new ObservableList<DestItemB> {
                new DestItemB { MyNum = "1000", MyStringUpper = "A" },
                new DestItemB { MyNum = "1500", MyStringUpper = "B" },
                new DestItemB { MyNum = "2000", MyStringUpper = "C" },
                new DestItemB { MyNum = "3000", MyStringUpper = "D" },
                };
        }
        public class SourceItemB : NotifyPropertySyncChanged {
            private int myNum;
            private string myStringLower;

            public int MyNum { get => myNum; set { myNum = value; OnPropertyChanged(null); } }
            public string MyStringLower { get => myStringLower; set { myStringLower = value; OnPropertyChanged(null); } }

        public override bool Equals(object obj) {
                var temp = obj as SourceItemB;
                if (temp == null) return false;
                if (temp.MyNum == MyNum && temp.MyStringLower == MyStringLower) return true;
                return false;
            }
        }

        public class DestItemB : NotifyPropertySyncChanged {

            public SourceItemB SourceItem { get; set; }

            public DestItemB() => SourceItem = new SourceItemB();

            public DestItemB(SourceItemB sourceItem) => SourceItem = sourceItem;

            public string MyNum {
                get => SourceItem.MyNum.ToString();
                set => SourceItem.MyNum = int.Parse(value);
            }
            public string MyStringUpper {
                get => SourceItem.MyStringLower.ToUpper();
                set => SourceItem.MyStringLower = value.ToLower();
            }
            public override bool Equals(object obj) {
                var temp = obj as DestItemB;
                if (temp == null) return false;
                if (temp.MyNum == MyNum && temp.MyStringUpper == MyStringUpper) return true;
                return false;
            }
        }
        #endregion

        #region Moq Event
        /// <summary>
        /// Class for testing mock events for observable collections. Uses Moq framework to verify event are called.
        /// </summary>
        public class MockEventClass : Mock<AssertEventClass> {

            private int _timesPropertyCalled = 0;
            private int _timesCollectionCalled = 0;
            private int _timesDictionaryCalled = 0;

            public void AddNotifiersCollectionAndProperty(IObservableCollection obvList) {
                //Sets up event testers
                obvList.PropertyChanged += OnPropertyChanged;
                obvList.CollectionChanged += OnCollectionChanged;
            }


            public void AssertMockNotifiersCollection(int timesPropertyCalled, int timesCollectionCalled) {
                _timesPropertyCalled += timesPropertyCalled;
                _timesCollectionCalled += timesCollectionCalled;
                Verify(m => m.Call("PropertyChanged"), Times.Exactly(_timesPropertyCalled));
                Verify(m => m.Call("CollectionChanged"), Times.Exactly(_timesCollectionCalled));
            }

            public void AssertMockNotifiersDictionary(int timesDictionaryCalled) {
                _timesDictionaryCalled += timesDictionaryCalled;
                Verify(m => m.Call("DictionaryChanged"), Times.Exactly(_timesDictionaryCalled));
            }

            public void RemoveCollectionAndPropertyNotifiers(IObservableCollection obvList) {
                obvList.PropertyChanged -= OnPropertyChanged;
                obvList.CollectionChanged -= OnCollectionChanged;
            }



            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                => Object.Call("CollectionChanged");

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
                => Object.Call("PropertyChanged");

        }
        /// <summary>
        /// Simple class for testing events using Moq. Moq proxies the Call method and counts number of times called.
        /// </summary>
        public class AssertEventClass {
            public virtual void Call(string obj) { }
        }

        #endregion
    }
}
