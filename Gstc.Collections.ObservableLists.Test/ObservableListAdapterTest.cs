using System.Linq;
using AutoFixture;
using Gstc.Collections.ObservableLists;
using Gstc.Collections.ObservableLists.Base;
using Moq;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test {

    [TestFixture]
    public class ObservableListAdapterTest : CollectionTestBase<object, string> {
        private BaseObservableList<string> TestBaseList { get; set; }
        private ObservableListAdapterConcrete ListAdapter { get; set; }

        [SetUp]
        public new void TestInit() {
            base.TestInit();
            TestBaseList = new ObservableList<string>();
        }

        [Test, Description("")]
        public void TestMethod_Set2() {

            TestBaseList.Add(Item1);
            TestBaseList.Add(Item2);
            TestBaseList.Add(Item3);

            ListAdapter.SourceCollection = TestBaseList;

            Assert.That(ListAdapter.Count, Is.EqualTo(3));
            Assert.That(Item1, Is.EqualTo(ListAdapter.First().BaseString));
            Assert.That(Item2, Is.EqualTo(ListAdapter[1].BaseString));
            Assert.That(Item3, Is.EqualTo(ListAdapter[2].BaseString));
            Assert.That(Item1 + ListAdapter.First().AddedString, Is.EqualTo(ListAdapter.First().StringView));
        }

        [Test, Description("")]
        public void TestMethod_SetConstructorAndAdd() {
            ListAdapter = new ObservableListAdapterConcrete(TestBaseList);

            TestBaseList.PropertyChanged += (sender, args) => AssertEvent.Call("PropertyChanged");
            TestBaseList.CollectionChanged += (sender, args) => AssertEvent.Call("CollectionChanged");

            TestBaseList.Add(Item1);
            TestBaseList.Add(Item2);
            TestBaseList.Add(Item3);

            Assert.That(ListAdapter.Count, Is.EqualTo(3));
            Assert.That(Item1, Is.EqualTo(ListAdapter.First().BaseString));
            Assert.That(Item2, Is.EqualTo(ListAdapter[1].BaseString));
            Assert.That(Item3, Is.EqualTo(ListAdapter[2].BaseString));
            Assert.That(Item1 + ListAdapter.First().AddedString, Is.EqualTo(ListAdapter.First().StringView));

            MockEvent.Verify(m => m.Call("PropertyChanged"), Times.Exactly(6));
            MockEvent.Verify(m => m.Call("CollectionChanged"), Times.Exactly(3));
        }

        [Test, Description("")]
        public void TestMethod_Set4() {

            TestBaseList.Add(Item1);
            TestBaseList.Add(Item2);
            TestBaseList.Add(Item3);

            ListAdapter = new ObservableListAdapterConcrete(TestBaseList);

            Assert.That(ListAdapter.Count, Is.EqualTo(3));
            Assert.That(Item1, Is.EqualTo(ListAdapter.First().BaseString));
            Assert.That(Item2, Is.EqualTo(ListAdapter[1].BaseString));
            Assert.That(Item3, Is.EqualTo(ListAdapter[2].BaseString));
            Assert.That(Item1 + ListAdapter.First().AddedString, Is.EqualTo(ListAdapter.First().StringView));
        }

        [Test, Description("")]
        public void TestMethod_SetAndAdd() {
            ListAdapter.SourceCollection = TestBaseList;

            TestBaseList.Add(Item1);
            TestBaseList.Add(Item2);
            TestBaseList.Add(Item3);

            Assert.That(ListAdapter.Count, Is.EqualTo(3));
            Assert.That(Item1, Is.EqualTo(ListAdapter.First().BaseString));
            Assert.That(Item2, Is.EqualTo(ListAdapter[1].BaseString));
            Assert.That(Item3, Is.EqualTo(ListAdapter[2].BaseString));
            Assert.That(Item1 + ListAdapter.First().AddedString, Is.EqualTo(ListAdapter.First().StringView));
        }

        [Test, Description("")]
        public void TestMethod_ReplaceList() {
            var testListA = new ObservableList<string>();

            testListA.Add(Item1);
            testListA.Add(Item2);
            testListA.Add(Item3);

            var testListB = new ObservableList<string>();
            var item1B = Fixture.Create<string>();
            var item2B = Fixture.Create<string>();
            testListB.Add(item1B);
            testListB.Add(item2B);

            ListAdapter.SourceCollection = testListA;

            Assert.That(ListAdapter.Count, Is.EqualTo(3));
            Assert.That(Item1 == ListAdapter[0].BaseString);
            Assert.That(Item2 == ListAdapter[1].BaseString);
            Assert.That(Item3 == ListAdapter[2].BaseString);


            ListAdapter.SourceCollection = testListB;

            Assert.That(item1B == ListAdapter[0].BaseString);
            Assert.That(item2B == ListAdapter[1].BaseString);

            Assert.That(ListAdapter.Count, Is.EqualTo(2));
        }


        [Test, Description("")]
        public void TestMethod_Remove() {

            TestBaseList.Add(Item1);
            TestBaseList.Add(Item2);
            TestBaseList.Add(Item3);

            ListAdapter = new ObservableListAdapterConcrete(TestBaseList);

            Assert.That(ListAdapter.Count == 3);

            TestBaseList.RemoveAt(1);

            Assert.That(ListAdapter.Count == 2);
            Assert.That(ListAdapter[0].BaseString == Item1);
            Assert.That(ListAdapter[1].BaseString == Item3);
        }

        [Test, Description("")]
        public void TestMethod_Move() {

            TestBaseList.Add(Item1);
            TestBaseList.Add(Item2);
            TestBaseList.Add(Item3);

            ListAdapter = new ObservableListAdapterConcrete(TestBaseList);

            Assert.That(ListAdapter.Count == 3);

            TestBaseList.Move(1, 0);

            Assert.That(ListAdapter.Count == 3);
            Assert.That(ListAdapter[0].BaseString == Item2);
            Assert.That(ListAdapter[1].BaseString == Item1);
            Assert.That(ListAdapter[2].BaseString == Item3);
        }


        [Test, Description("")]
        public void TestMethod_Replace() {

            TestBaseList.Add(Item1);
            TestBaseList.Add(Item2);
            TestBaseList.Add(Item3);

            ListAdapter = new ObservableListAdapterConcrete(TestBaseList);

            Assert.That(ListAdapter.Count == 3);

            var item4 = Fixture.Create<string>();
            TestBaseList[1] = item4;

            Assert.That(ListAdapter.Count == 3);
            Assert.That(ListAdapter[0].BaseString == Item1);
            Assert.That(ListAdapter[1].BaseString == item4);
            Assert.That(ListAdapter[2].BaseString == Item3);
        }

        [Test, Description("")]
        public void TestMethod_Clear() {

            TestBaseList.Add(Item1);
            TestBaseList.Add(Item2);
            TestBaseList.Add(Item3);

            ListAdapter = new ObservableListAdapterConcrete(TestBaseList);

            Assert.That(ListAdapter.Count == 3);

            TestBaseList.Clear();

            Assert.That(ListAdapter.Count == 0);
        }


        #region Test Class Definitions
        /// <summary>
        /// Items used in test.
        /// </summary>
        public class TestItemClass {

            //public static string AddedString = ":ViewModel";
            public static Fixture Fixture = new Fixture();

            public TestItemClass(string myString) {
                BaseString = myString;
                AddedString = Fixture.Create<string>();
                StringView = myString + AddedString;
            }
            public string StringView { get; set; }
            public string BaseString { get; set; }
            public string AddedString { get; set; }

        }

        public class ObservableListAdapterConcrete : ObservableListAdapter<string, TestItemClass> {

            public ObservableListAdapterConcrete() : base() { }

            public ObservableListAdapterConcrete(IObservableCollection<string> sourceCollection) : base(sourceCollection) { }

            public override string Convert(TestItemClass itemClass) => itemClass.StringView;

            public override TestItemClass Convert(string item) => new TestItemClass(item);
        }
        #endregion
    }
}
