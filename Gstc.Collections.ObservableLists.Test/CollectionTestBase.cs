using System;
using System.Collections.Specialized;
using System.ComponentModel;
using AutoFixture;
using Gstc.Collections.ObservableLists;
using Moq;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test {
    public class CollectionTestBase<TKey, TItem> {

        #region Default Test Tools
        protected static Fixture Fixture { get; } = new Fixture();
        protected MockEventClass MockEvent { get; set; }
        protected AssertEventClass AssertEvent { get; set; }
        #endregion

        #region Default test items

        protected TItem DefaultTestItem { get; set; }
        protected TItem UpdateTestItem { get; set; }

        protected TItem Item1 { get; set; }
        protected TItem Item2 { get; set; }
        protected TItem Item3 { get; set; }


        protected TKey Key1 { get; set; }
        protected TKey Key2 { get; set; }
        protected TKey Key3 { get; set; }

        protected TKey DefaultKey { get; set; }
        protected TKey UpdateKey { get; set; }
        protected TItem DefaultValue { get; set; }
        protected TItem UpdateValue { get; set; }



        #endregion

        public void TestInit() {
            MockEvent = new MockEventClass();
            AssertEvent = MockEvent.Object;

            //Generalize mock data
            DefaultTestItem = Fixture.Create<TItem>();
            UpdateTestItem = Fixture.Create<TItem>();

            Item1 = Fixture.Create<TItem>();
            Item2 = Fixture.Create<TItem>();
            Item3 = Fixture.Create<TItem>();

            Key1 = Fixture.Create<TKey>();
            Key2 = Fixture.Create<TKey>();
            Key3 = Fixture.Create<TKey>();

            DefaultKey = Fixture.Create<TKey>();
            UpdateKey = Fixture.Create<TKey>();
            DefaultValue = Fixture.Create<TItem>();
            UpdateValue = Fixture.Create<TItem>();
        }

        #region Collection Event Args Tests
        protected void AssertCollectionEventReset(object sender, NotifyCollectionChangedEventArgs args) {
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
            Assert.That(args.OldItems, Is.Null);
            Assert.That(args.NewItems, Is.Null);
            Assert.That(args.OldStartingIndex == -1);
            Assert.That(args.NewStartingIndex == -1);
        }

        protected NotifyCollectionChangedEventHandler GenerateAssertCollectionEventAddOne(int index, TItem item) {
            return (sender, args) => {
                Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(args.OldStartingIndex == -1);
                Assert.That(args.NewStartingIndex == index);
                Assert.That(args.OldItems, Is.Null);
                Assert.That(args.NewItems[0], Is.EqualTo(item));
            };
        }

        protected NotifyCollectionChangedEventHandler GenerateAssertCollectionEventAddThree(int index, TItem item1, TItem item2, TItem item3) {
            return (sender, args) => {
                Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(args.OldStartingIndex, Is.EqualTo(-1));
                Assert.That(args.NewStartingIndex, Is.EqualTo(index));
                Assert.That(args.OldItems, Is.Null);
                Assert.That(args.NewItems[0], Is.EqualTo(item1));
                Assert.That(args.NewItems[1], Is.EqualTo(item2));
                Assert.That(args.NewItems[2], Is.EqualTo(item3));
            };
        }

        protected NotifyCollectionChangedEventHandler GenerateAssertCollectionEventRemoveOne(int index, TItem item) {
            return (sender, args) => {
                Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(args.OldStartingIndex == index);
                Assert.That(args.NewStartingIndex == -1);
                Assert.That(args.OldItems[0], Is.EqualTo(item));
                Assert.That(args.NewItems, Is.Null);
            };
        }
        #endregion


        protected void Log(object message) => Console.WriteLine(message);

        #region Event Call Test


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
