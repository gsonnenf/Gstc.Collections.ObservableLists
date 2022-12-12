using System.Collections.Specialized;
using System.ComponentModel;
using Moq;

using Gstc.Collections.ObservableLists.Interface;



namespace Gstc.Collections.ObservableLists.Test.MockObjects {
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

        public void AssertMockNotifiersCollection(int expectedTimesPropertyCalled, int expectedTimesCollectionCalled) {
            _timesPropertyCalled += expectedTimesPropertyCalled;
            _timesCollectionCalled += expectedTimesCollectionCalled;
            Verify(m => m.Call("PropertyChanged"), Times.Exactly(_timesPropertyCalled));
            Verify(m => m.Call("CollectionChanged"), Times.Exactly(_timesCollectionCalled));
        }

        public void AssertMockNotifiersDictionary(int expectedTimesPropertyCalled, int expectedTimesDictionaryCalled) {
            _timesPropertyCalled += expectedTimesPropertyCalled;
            _timesDictionaryCalled += expectedTimesDictionaryCalled;
            Verify(m => m.Call("DictionaryChanged"), Times.Exactly(_timesDictionaryCalled));
            Verify(m => m.Call("PropertyChanged"), Times.Exactly(_timesPropertyCalled));
        }

        public void RemoveCollectionAndPropertyNotifiers(IObservableCollection obvList) {
            obvList.PropertyChanged -= OnPropertyChanged;
            obvList.CollectionChanged -= OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            Object.Call("CollectionChanged");

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => Object.Call("PropertyChanged");
    }
}

