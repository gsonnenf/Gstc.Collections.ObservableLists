using System;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Base {

    /// <summary>
    /// Provides functionality for generating Property changed events on collections.
    /// When reentrance is enabled (it is not currently enabled) it will create a threadsafe list.
    /// </summary>
    public abstract class NotifyProperty : INotifyPropertyChanged {
        protected const string CountString = "Count";
        protected const string IndexerName = "Item[]";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        protected void OnPropertyChangedCountAndIndex() {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
        }

        protected void OnPropertyChangedIndex() {
            OnPropertyChanged(IndexerName);
        }

        #region Reentrancy
        private readonly SimpleMonitor _monitor = new SimpleMonitor();

        //TODO: Thread Safe access is not currently implemented. This will be implemented and tested in a future version.
        protected IDisposable BlockReentrancy() {
            //_monitor.Enter();
            //return _monitor;
            return new Disposable();
        }

        //TODO: Add Monitor for Collection Changed and Dictionary Changed
        protected void CheckReentrancy() {
            //if (!_monitor.Busy) return;
            //if ((CollectionChanged == null) || (CollectionChanged.GetInvocationList().Length <= 1)) return;
            //throw new InvalidOperationException("ObservableCollectionReentrancyNotAllowed");
        }

        private class Disposable : IDisposable {
            public void Dispose() { }
        }
        private class SimpleMonitor : IDisposable {
            int _busyCount;
            public void Enter() => ++_busyCount;
            public void Dispose() => --_busyCount;
            public bool Busy => _busyCount > 0;
        }
        #endregion
    }
}
