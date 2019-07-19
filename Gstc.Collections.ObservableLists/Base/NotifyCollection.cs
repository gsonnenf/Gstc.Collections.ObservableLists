using System.Collections;
using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.Base {

    public abstract class NotifyCollection :
        NotifyProperty,
        INotifyCollectionChanged {

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

        #region Methods
        internal void OnCollectionChangedReset() {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(this, eventArgs);
                Reset?.Invoke(this, eventArgs);
            }
        }

        internal void OnCollectionChangedAdd(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(this, eventArgs);
                Added?.Invoke(this, eventArgs);
            }
        }

        internal void OnCollectionChangedAddMany(IList valueList, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, valueList, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(this, eventArgs);
                Added?.Invoke(this, eventArgs);
            }
        }

        internal void OnCollectionChangedRemove(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(this, eventArgs);
                Removed?.Invoke(this, eventArgs);
            }
        }

        internal void OnCollectionChangedMove(object value, int index, int oldIndex) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(this, eventArgs);
                Moved?.Invoke(this, eventArgs);
            }
        }

        internal void OnCollectionChangedReplace(object oldValue, object newValue, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(this, eventArgs);
                Replaced?.Invoke(this, eventArgs);
            }
        }
        #endregion
    }
}
