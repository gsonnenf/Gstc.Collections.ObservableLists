﻿using AutoFixture;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using NUnit.Framework;
using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.Test.Tools {
    public class CollectionTestBase<TItem> {

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

    }
}
