/*
using Gstc.Collections.ObservableLists.ComponentModel;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Threading;

namespace Gstc.Collections.ObservableLists.Notify {

    /// <summary>
    /// Provides functionality for generating collection changed events.
    /// </summary>
    public class NotifyCollectionChangedLockingAspect :
        NotifyPropertyChangedExtended,
        IObservableListAspect,
        INotifyListChangedEvents {

        #region Events Changing
        public event NotifyCollectionChangedEventHandler CollectionChanging;

        public event NotifyCollectionChangedEventHandler Adding;

        public event NotifyCollectionChangedEventHandler Removing;

        public event NotifyCollectionChangedEventHandler Moving;

        public event NotifyCollectionChangedEventHandler Replacing;

        public event NotifyCollectionChangedEventHandler Resetting;
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
        public NotifyCollectionChangedLockingAspect() { }
        public NotifyCollectionChangedLockingAspect(object sender) : base(sender) { Sender = sender; }
        #endregion

        #region Method Changing
        public void OnListReseting() {
            LockMonitor.GetLock();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanging?.Invoke(Sender, eventArgs);
            Resetting?.Invoke(Sender, eventArgs);
           
        }

        public void OnListAdding(object value, int index) { //TODO: Index will not always be known in advanced
            LockMonitor.GetLock();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index);

                CollectionChanging?.Invoke(Sender, eventArgs);
                Adding?.Invoke(Sender, eventArgs);
        }

        public void OnListRangeAdding(IList valueList, int index) {
            LockMonitor.GetLock();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, valueList, index);
         
                CollectionChanging?.Invoke(Sender, eventArgs);
                Adding?.Invoke(Sender, eventArgs);

        }

        public void OnListRemoving(object value, int index) {
            LockMonitor.GetLock();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index);

                CollectionChanging?.Invoke(Sender, eventArgs);
                Removing?.Invoke(Sender, eventArgs);

        }

        public void OnListMoving(object value, int index, int oldIndex) {
            LockMonitor.GetLock();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex);

                CollectionChanging?.Invoke(Sender, eventArgs);
                Moving?.Invoke(Sender, eventArgs);

        }

        public void OnListReplacing(object oldValue, object newValue, int index) {
            LockMonitor.GetLock();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index);

                CollectionChanging?.Invoke(Sender, eventArgs);
                Replacing?.Invoke(Sender, eventArgs);

        }
        #endregion

        #region Methods Changed
        public void OnListReset() {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanged?.Invoke(Sender, eventArgs);
            Reset?.Invoke(Sender, eventArgs);
        }

        public void OnListAdd(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index);

                CollectionChanged?.Invoke(Sender, eventArgs);
                Added?.Invoke(Sender, eventArgs);
            
        }
        //TODO: "Range actions" in WPF is not supported. This is a WPF problem, not a GSTC problem. Perhaps a workaround could be good. 
        public void OnListRangeAdd(IList valueList, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, valueList, index);

                CollectionChanged?.Invoke(Sender, eventArgs);
                Added?.Invoke(Sender, eventArgs);
  
        }

        public void OnListRemove(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index);
 
                CollectionChanged?.Invoke(Sender, eventArgs);
                Removed?.Invoke(Sender, eventArgs);
            
        }

        public void OnListMove(object value, int index, int oldIndex) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex);

                CollectionChanged?.Invoke(Sender, eventArgs);
                Moved?.Invoke(Sender, eventArgs);
            
        }

        public void OnListReplace(object oldValue, object newValue, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index);

                CollectionChanged?.Invoke(Sender, eventArgs);
                Replaced?.Invoke(Sender, eventArgs);
        }
        #endregion


        #region Methods Read
        public void OnListRead() { }

        public void OnIndexRead() { }

        public Exception Catch(string message, Exception e) => e;
        public Exception Catch(string message) {
            throw new NotImplementedException();
        }

        public void Finally() => LockMonitor.ReleaseLock();
        #endregion

        #region Lock


        public IDisposable Lock() => LockMonitor.GetLock();

        public SimpleMonitor LockMonitor = new();

        /// <summary>
        /// Allows multiple threads to wait for the observable, but prevents reentrancy from a single thread to prevent deadlock.
        /// Deadlock protection does not work if onchange event starts a new thread to access the list.
        /// </summary>
        public class SimpleMonitor : IDisposable {
            private Mutex _mutex = new();
            private ConcurrentDictionary<int, int> ReentrancyDictionary = new ConcurrentDictionary<int, int>();
            public SimpleMonitor GetLock() {

                var threadId = Thread.CurrentThread.ManagedThreadId;
                if (!ReentrancyDictionary.TryAdd(threadId, threadId)) throw new InvalidOperationException("Reentrancy not allowed to avoid deadlock.");
                _mutex.WaitOne();
                return this;
            }
            public void Dispose() => ReleaseLock();
            public void ReleaseLock() {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                ReentrancyDictionary.TryRemove(threadId, out _);
                _mutex.ReleaseMutex();
            }
        }
        #endregion
    }
}
*/