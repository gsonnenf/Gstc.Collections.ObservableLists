using Gstc.Collections.ObservableLists;
using NUnit.Framework;
using static Gstc.Collections.ObservableLists.Test.InterfaceTestCases;

namespace Gstc.Collections.ObservableLists.Test {

    [TestFixture]
    public class ObservableListTestInterface {

        public ObservableList<TestItem> Observable;

        private InterfaceTestCases _testCases = new InterfaceTestCases();

        [SetUp]
        public void TestInit() {
            _testCases.TestInit();
            Observable = new ObservableList<TestItem>();
        }

        [Test]
        public void CollectionInterface() {
            //Test needs 3 test items.
            Observable.Add(new TestItem());
            Observable.Add(new TestItem());
            Observable.Add(new TestItem());
            _testCases.CollectionTest(Observable);
        }

        [Test]
        public void CollectionGenericInterface() => _testCases.CollectionGenericTest(Observable);

        //[Test]
        //public void CollectionKvp() => _testCases.CollectionKeyValuePairTest(Observable);

        //[Test]
        //public void Dictionary() => _testCases.DictionaryTest(Observable);

        //[Test]
        //public void DictionaryGeneric() => _testCases.DictionaryGenericTest(Observable);

        [Test]
        public void ListGenericInterface() => _testCases.ListGenericTest(Observable);

        [Test]
        public void ListInterface() => _testCases.ListTest(Observable);


    }
}
