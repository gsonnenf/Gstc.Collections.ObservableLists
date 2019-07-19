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

        internal virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        internal void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        internal void OnPropertyChangedCountAndIndex() {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
        }

        internal void OnPropertyChangedIndex() {
            OnPropertyChanged(IndexerName);
        }

        #region Reentrancy
        private readonly SimpleMonitor _monitor = new SimpleMonitor();


        protected IDisposable BlockReentrancy() {
            _monitor.Enter();
            return _monitor;
        }

        //TODO: Add Monitor for Collection Changed and Dictionary Changed
        internal void CheckReentrancy() {
            if (!_monitor.Busy) return;
            //if ((CollectionChanged == null) || (CollectionChanged.GetInvocationList().Length <= 1)) return;
            //throw new InvalidOperationException("ObservableCollectionReentrancyNotAllowed");
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
