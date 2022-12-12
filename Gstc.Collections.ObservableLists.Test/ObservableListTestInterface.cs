using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Gstc.Collections.ObservableLists.Interface;
using Gstc.Collections.ObservableLists.Multithread;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using Gstc.Collections.ObservableLists.Test.Tools;


namespace Gstc.Collections.ObservableLists.Test {

    [TestFixture]
    public class ObservableListTestInterface : CollectionTestBase<TestItem> {

        public static object[] StaticDataSource => new object[] {
            new ObservableList<TestItem>(),
            new ObservableIList<TestItem, List<TestItem>>(),
            new ObservableIListLocking<TestItem,List<TestItem>>()
        };

        [SetUp]
        public new void TestInit() => base.TestInit();

        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        [Description("Tests functionality when upcast to ICollection interface.")]
        public void CollectionInterfaceTest(ICollection collection) {

            //Staging
            if (collection is IObservableCollection<TestItem> observable) {
                observable.Add(new TestItem());
                observable.Add(new TestItem());
                observable.Add(new TestItem());
                MockEvent.AddNotifiersCollectionAndProperty(observable);
            } else throw new InvalidCastException("Collection does not support the IObservable Interface");

            //Count Test
            Assert.AreEqual(3, collection.Count);

            //SyncRoot Test
            try {
                Assert.IsNotNull(collection.SyncRoot);
            } catch (NotSupportedException e) {
                Console.WriteLine(e.Message); //Concurrent Collections will not support syncroot.
            }

            //isSynchronized Test
            Assert.IsNotNull(collection.IsSynchronized);

            //IEnumerator test
            IEnumerator enumerator = collection.GetEnumerator();
            Assert.IsNotNull(enumerator);

            //CopyTo(,) test
            Array array = new object[3];
            collection.CopyTo(array, 0);

            // Enumeration test
            foreach (var item in array) {
                enumerator.MoveNext();
                Assert.AreEqual(item, enumerator.Current);
                Assert.IsTrue(item != null);
            }
        }

        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        [Description("Tests functionality when upcast to ICollection<> interface.")]
        public void CollectionInterfaceGenericTest(ICollection<TestItem> collection) {
            //Staging
            if (collection is IObservableCollection<TestItem> observable) MockEvent.AddNotifiersCollectionAndProperty(observable);
            else throw new InvalidCastException("Collection does not support the IObservable Interface");

            //Add and count Test
            collection.Add(Item1);
            Assert.AreEqual(1, collection.Count);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //Remove Test
            collection.Remove(Item1);
            Assert.AreEqual(0, collection.Count);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //Clear Test
            collection.Add(Item1);
            collection.Add(Item2);
            collection.Add(Item3);
            Assert.AreEqual(3, collection.Count);
            MockEvent.AssertMockNotifiersCollection(6, 3);

            collection.Clear();
            Assert.AreEqual(0, collection.Count);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //Contains Test
            collection.Add(Item1);
            Assert.IsTrue(collection.Contains(Item1));
            Assert.IsFalse(collection.Contains(Item2));

            //Syncroot Test
            Assert.IsFalse(collection.IsReadOnly);

            //IEnumerator/ EnumeratorGeneric test
            collection.Clear();
            collection.Add(Item1);
            collection.Add(Item2);
            collection.Add(Item3);
            Assert.AreEqual(3, collection.Count);

            IEnumerator enumerator = collection.GetEnumerator();
            Assert.IsNotNull(enumerator);
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, Item1);

            IEnumerator<TestItem> enumeratorGeneric = collection.GetEnumerator();
            Assert.IsNotNull(enumeratorGeneric);
            enumeratorGeneric.MoveNext();
            Assert.AreEqual(enumeratorGeneric.Current, Item1);

            //CopyTo test
            var array = new TestItem[3];
            collection.CopyTo(array, 0);
            Assert.AreEqual(array[0], Item1);
            Assert.AreEqual(array[1], Item2);
            Assert.AreEqual(array[2], Item3);

        }

        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        [Description("Tests functionality when upcast to IList<> interface.")]
        public void ListInterfaceGenericTest(IList<TestItem> list) {

            //Staging
            if (list is IObservableCollection<TestItem> observable) MockEvent.AddNotifiersCollectionAndProperty(observable);
            else throw new InvalidCastException("Collection does not support the IObservable Interface");

            //Add Test
            list.Add(Item1);
            Assert.AreEqual(Item1, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //Index Test
            list[0] = Item2;
            Assert.AreEqual(Item2, list[0]);
            MockEvent.AssertMockNotifiersCollection(1, 1);

            //Index of test
            Assert.AreEqual(0, list.IndexOf(Item2));

            //Insert(,)
            list.Insert(0, Item3);
            Assert.AreEqual(Item3, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //RemoveAt()
            list.RemoveAt(0);
            Assert.AreEqual(Item2, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);
        }

        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        [Description("Tests functionality when upcast to IList interface.")]
        public void ListInterfaceTest(IList list) {
            
            //Staging
            if (list is IObservableCollection<TestItem> observable) MockEvent.AddNotifiersCollectionAndProperty(observable);
            else throw new InvalidCastException("Collection does not support the IObservable Interface");

            //Add test
            list.Add(Item1);
            Assert.AreEqual(Item1, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //Index Test
            list[0] = Item2;
            Assert.AreEqual(Item2, list[0]);
            MockEvent.AssertMockNotifiersCollection(1, 1);

            //Index of test
            Assert.AreEqual(0, list.IndexOf(Item2));

            //Insert(,)
            list.Insert(0, Item3);
            Assert.AreEqual(Item3, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //RemoveAt()
            list.RemoveAt(0);
            Assert.AreEqual(Item2, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);
        }


        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        [Description("Tests functionality when upcast to (IObservableList<> interface.")]
        public void ObservableListInterfaceTest(IObservableList<TestItem> list) {
            
            MockEvent.AddNotifiersCollectionAndProperty(list);

            //Index Test
            list.Add(Item1);
            Assert.AreEqual(Item1, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);
            Assert.AreEqual(1, list.Count);

            list[0] = Item2;
            Assert.AreEqual(Item2, list[0]);
            MockEvent.AssertMockNotifiersCollection(1, 1);
            Assert.AreEqual(1, list.Count);

            //Index of test
            Assert.AreEqual(0, list.IndexOf(Item2));

            //Insert(,)
            list.Insert(0, Item3);
            Assert.AreEqual(Item3, list[0]);
            Assert.AreEqual(2, list.Count);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //RemoveAt()
            list.RemoveAt(0);
            Assert.AreEqual(Item2, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);
            Assert.AreEqual(1, list.Count);

            //Clear(), For ambiguity problem.
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        [Description("Tests functionality when upcast to (IObservableCollection<> interface.")]
        public void ObservableCollectionInterfaceTest(IObservableCollection<TestItem> list) {
            MockEvent.AddNotifiersCollectionAndProperty(list);

            list.Add(Item1);
            MockEvent.AssertMockNotifiersCollection(2, 1);
            Assert.AreEqual(1, list.Count);

            list.Add(Item2);
            //MockEvent.AssertMockNotifiersCollection(2, 1);
            Assert.AreEqual(2, list.Count);

            //Remove
            list.Remove(Item1);
            //MockEvent.AssertMockNotifiersCollection(2, 1);
            Assert.AreEqual(1, list.Count);

            //Clear(), For ambiguity problem.
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }
    }
}
