using System;
using System.Collections.Specialized;
using Gstc.Collections.ObservableLists.Interface;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test {

    [TestFixture]
    public class ReentrancyTest {

        public static object[] StaticDataSource => new object[] {
            //new ObservableSynchronizedList<string>(),
            new ObservableList<string>(),
        };

        [Test]
        [TestCaseSource(nameof(StaticDataSource))]
        public void ReentrancyFailTest(IObservableCollection<string> list) {

            //Demonstrates single reentrancy success.
            list.AllowReentrancy = true;
            list.CollectionChanged += (sender, args) => {
                if (list.Count > 10) return;
                if (args.Action == NotifyCollectionChangedAction.Add) list.Add("String Object");
            };

            list.Add("Start String");
            list.AllowReentrancy = false;
            //Demonstrates multiple reentrancy fails.
            list.Clear();
            list.CollectionChanged += (sender, args) => {
                if (list.Count > 10) return;
                if (args.Action == NotifyCollectionChangedAction.Add) list.Add("String Object");
            };

            Assert.Throws<InvalidOperationException>( ()=> list.Add("Start String") );
        }
    }
}
