using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Base.Notify {

    /// <summary>
    /// Provides functionality for generating collection changed events.
    /// </summary>
    public class NotifyCollectionComposition<TCollection> :
        NotifyPropertyComposition,
        INotifyCollectionChanged,
        INotifyListChanged
        where TCollection : INotifyCollectionChanged, INotifyPropertyChanged {

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

        public NotifyCollectionComposition(TCollection parent) : base(parent) { Parent = parent; }

        #region Methods
        public void OnCollectionChangedReset() {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Parent, eventArgs);
                Reset?.Invoke(Parent, eventArgs);
            }
        }

        public void OnCollectionChangedAdd(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Parent, eventArgs);
                Added?.Invoke(Parent, eventArgs);
            }
        }
        //TODO: "Range actions" in WPF is not supported. This is a WPF problem, not a GSTC problem. Perhaps a workaround could be good. 
        public void OnCollectionChangedAddMany(IList valueList, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, valueList, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Parent, eventArgs);
                Added?.Invoke(Parent, eventArgs);
            }
        }

        public void OnCollectionChangedRemove(object value, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Parent, eventArgs);
                Removed?.Invoke(Parent, eventArgs);
            }
        }

        public void OnCollectionChangedMove(object value, int index, int oldIndex) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Parent, eventArgs);
                Moved?.Invoke(Parent, eventArgs);
            }
        }

        public void OnCollectionChangedReplace(object oldValue, object newValue, int index) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index);
            using (BlockReentrancy()) {
                CollectionChanged?.Invoke(Parent, eventArgs);
                Replaced?.Invoke(Parent, eventArgs);
            }
        }
        #endregion
    }
}
