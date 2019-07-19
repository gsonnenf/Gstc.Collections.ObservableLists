
using System;
using System.Collections;
using System.Collections.Generic;
using AutoFixture;

using NUnit.Framework;


namespace Gstc.Collections.ObservableLists.Test {
    public class InterfaceTestCases : CollectionTestBase<string, TestItem> {

        /// <summary>
        /// Test the interface for ICollection to make sure these are implemented.
        /// The collection passed to this test MUST come populated with 3 items.
        /// </summary>
        /// <param name="collection"></param>
        public void CollectionTest(ICollection collection) {

            //Count Test
            Assert.AreEqual(3, collection.Count);

            //Syncroot Test
            Assert.IsNotNull(collection.SyncRoot);

            //isSyncronized Test
            Assert.AreEqual(collection.IsSynchronized, false);

            //IEnumerator test
            IEnumerator enumerator = collection.GetEnumerator();
            Assert.IsNotNull(enumerator);

            //CopyTo(,) test
            Array array = new object[3];
            collection.CopyTo(array, 0);

            foreach (var item in array) {
                enumerator.MoveNext();
                Assert.AreEqual(item, enumerator.Current);
                Assert.IsTrue(item != null);
            }
        }

        /// <summary>
        /// Tests methods specific to ICollection<>. 
        /// </summary>
        /// <param name="collection"></param>
        public void CollectionGenericTest(ICollection<TestItem> collection) {

            Assert.IsNotNull(collection as IObservableCollection);

            MockEvent.AddNotifiersCollectionAndProperty(collection as IObservableCollection);
            //Add and count
            collection.Add(Item1);
            Assert.AreEqual(1, collection.Count);
            MockEvent.AssertMockNotifiersCollection(2, 1);

            //Remove Test
            collection.Remove(Item1);
            Assert.AreEqual(0, collection.Count);
            MockEvent.AssertMockNotifiersCollection(2, 1);

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




        public void ListGenericTest(IList<TestItem> list) {

            Assert.IsNotNull(list as IObservableCollection);

            MockEvent.AddNotifiersCollectionAndProperty(list as IObservableCollection);

            //Index Test
            list.Add(Item1);
            Assert.AreEqual(Item1, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);

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

        public void ListTest(IList list) {
            Assert.IsNotNull(list as IObservableCollection);

            MockEvent.AddNotifiersCollectionAndProperty(list as IObservableCollection);

            //Index Test
            list.Add(Item1);
            Assert.AreEqual(Item1, list[0]);
            MockEvent.AssertMockNotifiersCollection(2, 1);

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

    }

        /// <summary>
        /// A Test item used in these tests
        /// </summary>
        public class TestItem {
            private static Fixture Fixture { get; } = new Fixture();

            public string Id { get; set; }

            public TestItem() {
                Id = Fixture.Create<string>();
            }
        }
    
}
