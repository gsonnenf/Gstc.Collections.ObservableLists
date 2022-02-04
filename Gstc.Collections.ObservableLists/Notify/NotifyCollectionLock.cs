using Gstc.Collections.ObservableLists.ComponentModel;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;

namespace Gstc.Collections.ObservableLists.Notify {

    /// <summary>
    /// Provides functionality for generating collection changed events.
    /// </summary>
    public class NotifyCollectionLock :
        NotifyProperty,
        INotifyCollectionLock,
        INotifyCollectionChangedExtended {

        #region Events
        /// <summary>
        /// Triggers events on any change of collection. 
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Triggers events when an item or items are added. 
        /// </summary>
        public event NotifyCollectionChangedEventHandler Added;
        /// <summary>
        /// Triggers events when an item or items are removed. 
        /// </summary>
        public event NotifyCollectionChangedEventHandler Removed;
        /// <summary>
        /// Triggers events when an item has changed position. 
        /// </summary>
        public event NotifyCollectionChangedEventHandler Moved;
        /// <summary>
        /// Triggers events when an item has been replaced. 
        /// </summary>
        public event NotifyCollectionChangedEventHandler Replaced;
        /// <summary>
        /// Triggers events when an the list has changed substantially such as a Clear(). 
        /// </summary>
        public event NotifyCollectionChangedEventHandler Reset;
        #endregion

        #region Constructors
        public NotifyCollectionLock() { }
        public NotifyCollectionLock(object sender) : base(sender) { Sender = sender; }
        #endregion

        #region Methods
        public void OnCollectionChangedReset() {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanged?.Invoke(Sender, eventArgs);
            Reset?.Invoke(Sender, eventArgs);
        }

        public void OnCollectionChangedAdd(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index);
            CollectionChanged?.Invoke(Sender, eventArgs);
            Added?.Invoke(Sender, eventArgs);
        }
        //TODO: "Range actions" in WPF is not supported. This is a WPF problem, not a GSTC problem. Perhaps a workaround could be good. 
        public void OnCollectionChangedAddMany(IList valueList, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, valueList, index);
            CollectionChanged?.Invoke(Sender, eventArgs);
            Added?.Invoke(Sender, eventArgs);
        }

        public void OnCollectionChangedRemove(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index);
            CollectionChanged?.Invoke(Sender, eventArgs);
            Removed?.Invoke(Sender, eventArgs);
        }

        public void OnCollectionChangedMove(object value, int index, int oldIndex) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex);
            CollectionChanged?.Invoke(Sender, eventArgs);
            Moved?.Invoke(Sender, eventArgs);
        }

        public void OnCollectionChangedReplace(object oldValue, object newValue, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index);
            CollectionChanged?.Invoke(Sender, eventArgs);
            Replaced?.Invoke(Sender, eventArgs);
        }
        #endregion

        #region .NET Monitor
        public void CheckReentrancy() => throw new NotSupportedException("Please use Lock() instead of Check Reentrancy()");
        public IDisposable Lock() => LockMonitor.GetLock();
        public SimpleMonitor LockMonitor = new();

        public class SimpleMonitor : IDisposable {
            private Mutex _mutex = new();
            public SimpleMonitor GetLock() {
                _mutex.WaitOne();
                return this;
            }
            public void Dispose() => _mutex.ReleaseMutex();
        }
        #endregion
    }
}
