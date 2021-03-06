﻿using Gstc.Collections.ObservableLists;
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
            //Test needs 3 test items. ICollection does not provide Add.
            Observable.Add(new TestItem());
            Observable.Add(new TestItem());
            Observable.Add(new TestItem());
            _testCases.CollectionTest(Observable);
        }

        [Test]
        public void CollectionGenericInterface() => _testCases.CollectionGenericTest(Observable);

        [Test]
        public void ListInteerfaceGeneric() => _testCases.ListGenericTest(Observable);

        [Test]
        public void ListInterface() => _testCases.ListTest(Observable);


    }
}
