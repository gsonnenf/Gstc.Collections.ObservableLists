using Gstc.Collections.ObservableDictionary.ComponentModel;
using Gstc.Collections.ObservableDictionary.Interface;
using Gstc.Collections.ObservableLists.Interface;
using Moq;
using System.Collections.Specialized;
using System.ComponentModel;

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

        public void AddNotifiersDictionary(INotifyDictionaryChanged obvDict) {
            //Sets up event testers
            obvDict.PropertyChanged += OnPropertyChanged;
            obvDict.DictionaryChanged += OnDictionaryChanged;
        }

        public void AssertMockNotifiersCollection(int timesPropertyCalled, int timesCollectionCalled) {
            _timesPropertyCalled += timesPropertyCalled;
            _timesCollectionCalled += timesCollectionCalled;
            Verify(m => m.Call("PropertyChanged"), Times.Exactly(_timesPropertyCalled));
            Verify(m => m.Call("CollectionChanged"), Times.Exactly(_timesCollectionCalled));
        }

        public void AssertMockNotifiersDictionary(int timesPropertyCalled, int timesDictionaryCalled) {
            _timesPropertyCalled += timesPropertyCalled;
            _timesDictionaryCalled += timesDictionaryCalled;
            Verify(m => m.Call("DictionaryChanged"), Times.Exactly(_timesDictionaryCalled));
            Verify(m => m.Call("PropertyChanged"), Times.Exactly(_timesPropertyCalled));
        }

        public void RemoveCollectionAndPropertyNotifiers(IObservableCollection obvList) {
            obvList.PropertyChanged -= OnPropertyChanged;
            obvList.CollectionChanged -= OnCollectionChanged;
        }

        public void RemoveDictionaryNotifiers(IObservableDictionary<string, TestItem> obvDict) {
            obvDict.PropertyChanged -= OnPropertyChanged;
            obvDict.DictionaryChanged -= OnDictionaryChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => Object.Call("CollectionChanged");

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => Object.Call("PropertyChanged");

        private void OnDictionaryChanged(object sender, NotifyDictionaryChangedEventArgs e) => Object.Call("DictionaryChanged");
    }
}
