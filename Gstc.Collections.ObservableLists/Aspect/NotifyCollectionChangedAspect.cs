/*
using Gstc.Collections.ObservableLists.ComponentModel;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.Notify {

    /// <summary>
    /// Provides functionality for generating collection changed events.
    /// </summary>
    public class NotifyCollectionChangedAspect :
        NotifyPropertyChangedExtended,
        IObservableListAspect,
        INotifyListChangedEvents {

        #region Events Changing
        public event NotifyCollectionChangedEventHandler CollectionChanging;

        public event NotifyCollectionChangedEventHandler Adding;

        public event NotifyCollectionChangedEventHandler Removing;

        public event NotifyCollectionChangedEventHandler Moving;

        public event NotifyCollectionChangedEventHandler Replacing;

        public event NotifyCollectionChangedEventHandler Reseting;
        #endregion

        #region Events Changed
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        public event NotifyCollectionChangedEventHandler Added;
       
        public event NotifyCollectionChangedEventHandler Removed;
       
        public event NotifyCollectionChangedEventHandler Moved;
       
        public event NotifyCollectionChangedEventHandler Replaced;
        
        public event NotifyCollectionChangedEventHandler Reset;
        #endregion


        #region Constructor
        public NotifyCollectionChangedAspect() { }
        public NotifyCollectionChangedAspect(object sender) : base(sender) { Sender = sender; }
        #endregion

        #region Methods Changed
        public void OnListReset() {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Sender, eventArgs);
                Reset?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListAdd(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Sender, eventArgs);
                Added?.Invoke(Sender, eventArgs);
            }
        }
        //TODO: "Range actions" in WPF is not supported. This is a WPF problem, not a GSTC problem. Perhaps a workaround could be good. 
        public void OnListRangeAdd(IList valueList, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, valueList, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Sender, eventArgs);
                Added?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListRemove(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Sender, eventArgs);
                Removed?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListMove(object value, int index, int oldIndex) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Sender, eventArgs);
                Moved?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListReplace(object oldValue, object newValue, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Sender, eventArgs);
                Replaced?.Invoke(Sender, eventArgs);
            }
        }
        #endregion

        #region Method Changing
        public void OnListReseting() {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            using (BlockReentrancy()) {
                CollectionChanging?.Invoke(Sender, eventArgs);
                Reseting?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListAdding(object value, int index) { //TODO: Index will not always be known in advanced
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index);
            using (BlockReentrancy()) {
                CollectionChanging?.Invoke(Sender, eventArgs);
                Adding?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListRangeAdding(IList valueList, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, valueList, index);
            using (BlockReentrancy()) {
                CollectionChanging?.Invoke(Sender, eventArgs);
                Adding?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListRemoving(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index);
            using (BlockReentrancy()) {
                CollectionChanging?.Invoke(Sender, eventArgs);
                Removing?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListMoving(object value, int index, int oldIndex) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex);
            using (BlockReentrancy()) {
                CollectionChanging?.Invoke(Sender, eventArgs);
                Moving?.Invoke(Sender, eventArgs);
            }
        }

        public void OnListReplacing(object oldValue, object newValue, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index);
            using (BlockReentrancy()) {
                CollectionChanging?.Invoke(Sender, eventArgs);
                Replacing?.Invoke(Sender, eventArgs);
            }
        }
        #endregion

        #region Methods Read
        public void OnListRead() {}

        public void OnIndexRead() {}
        #endregion

        #region .NET Monitor
        public SimpleMonitor ReentrancyMonitor => _monitor ??= new SimpleMonitor(this);
        private SimpleMonitor _monitor; // Lazily allocated only when a subclass calls BlockReentrancy() or during serialization. 
        private int _blockReentrancyCount;

        public void CheckReentrancy() {
            if (_blockReentrancyCount <= 0) return;
            if (CollectionChanged?.GetInvocationList().Length > 1)
                throw new InvalidOperationException("ObservableCollectionReentrancyNotAllowed");
        }
        protected IDisposable BlockReentrancy() {
            _blockReentrancyCount++;
            return ReentrancyMonitor;
        }

        /// <summary>
        /// Simple monitor allows use of using block for tracking ReentrancyCount.
        /// </summary>
        public class SimpleMonitor : IDisposable {
            private readonly NotifyCollectionChangedAspect _notify;
            public SimpleMonitor(NotifyCollectionChangedAspect notify) => _notify = notify;
            public void Dispose() => _notify._blockReentrancyCount--;
        }
        #endregion
    }
}
*/