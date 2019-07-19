using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Gstc.Collections.ObservableLists.Base;

namespace Gstc.Collections.ObservableLists {
    /// <summary>
    /// The Observable list adapter is a one-way synchronizer between a source observable list of type
    /// TInput and a destination observable list of TOutput. The method Convert(...) must be implemented
    /// as an adapter between the source element and destination element. 
    /// 
    /// This class is intended to serve as a one-way map between a collection of models and a collection of
    /// ViewModels though can be used for arbitrary purpose. It is recommened the destination element should
    /// contain a reference to the source element and propogate changes to the destination element via
    /// INotifyPropertyChanged or direct mapping to the source. 
    /// </summary>
    /// <typeparam name="TInput">Source element type (e.g. Model class)</typeparam>
    /// <typeparam name="TOutput">Destination element type (e.g. ViewModel class</typeparam>
    public abstract class ObservableListSync<TInput, TOutput> : ObservableList<TOutput> {

        private ObservableList<TInput> _sourceList;
        private bool _isPropertySync = false;
        private List<PropertySyncNotifier> propertySyncNotifierList = new List<PropertySyncNotifier>();

        /// <summary>
        /// Method for converting an item of type TInput to TOutput.
        /// </summary>
        /// <param name="item">The source TInput Item.</param>
        /// <returns>An output item type.</returns>
        public abstract TOutput Convert(TInput item);

        /// <summary>
        /// Method for converting an item of type TOuput back to its TInput type.
        /// </summary>
        /// <param name="item">The ouput item to be converted back to its original type.</param>
        /// <returns>The source item type.</returns>
        public abstract TInput ConvertBack(TOutput item);

        /// <summary>
        /// Initializes empty list. Must be filled.
        /// </summary>
        protected ObservableListSync() {
            CollectionChanged += DestinationCollectionChanged;
        }


        /// <summary>
        /// Initializes a ObservableListAdapter with a collection implementing IObservable collection.
        /// </summary>
        /// <param name="sourceCollection"></param>
        protected ObservableListSync(ObservableList<TInput> sourceCollection) {
            CollectionChanged += DestinationCollectionChanged;
            SourceObservableList = sourceCollection ?? throw new ArgumentNullException("BaseCollection can not be null");
        }

        /// <summary>
        /// A collection that implements IObservableCollection that the Adapter can watch.
        /// </summary>
        public ObservableList<TInput> SourceObservableList {
            get => _sourceList;
            set {
                if (_sourceList != null) _sourceList.CollectionChanged -= SourceCollectionChanged;
                _sourceList = value;
                if (_sourceList == null) return;

                CollectionChanged -= DestinationCollectionChanged;
                Clear();
                foreach (var sourceItem in _sourceList) {

                    var destItem = Convert(sourceItem);
                    Add(destItem);

                    var sourceNotify = sourceItem as INotifyPropertySyncChanged;
                    var destNotify = destItem as INotifyPropertySyncChanged;

                    _isPropertySync =
                        sourceItem is INotifyPropertyChanged && destItem is INotifyPropertyChanged &&
                        (sourceItem is INotifyPropertySyncChanged || destItem is INotifyPropertySyncChanged);

                    CreatePropertySync(sourceItem, destItem);
                }
                _sourceList.CollectionChanged += SourceCollectionChanged;
            }
        }

        private void CreatePropertySync(TInput sourceItem, TOutput destItem) {
            if (_isPropertySync) propertySyncNotifierList.Add(new PropertySyncNotifier(sourceItem as INotifyPropertyChanged, destItem as INotifyPropertyChanged));

        }



        //TODO: Add an optional dispatcher method to execute update code on a UI thread.
        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            CollectionChanged -= DestinationCollectionChanged;
            switch (args.Action) {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < args.NewItems.Count; i++) {
                        var sourceItem = (TInput)args.NewItems[i];
                        var destItem = Convert(sourceItem);
                        Insert(args.NewStartingIndex + i, destItem);
                        CreatePropertySync(sourceItem, destItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < args.OldItems.Count; i++) RemoveAt(args.OldStartingIndex + i);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < args.NewItems.Count; i++) {
                        var sourceItem = (TInput)args.NewItems[i];
                        var destItem = Convert(sourceItem);
                        this[args.OldStartingIndex + i] = destItem;
                        CreatePropertySync(sourceItem, destItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    for (var i = 0; i < args.OldItems.Count; i++) Move(args.OldStartingIndex + i, args.NewStartingIndex + i);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    foreach (var sourceItem in _sourceList) {
                        var destItem = Convert(sourceItem);
                        Add(destItem);
                        CreatePropertySync(sourceItem, destItem);
                    }
                    break;

                default:
                    CollectionChanged += DestinationCollectionChanged;
                    throw new ArgumentOutOfRangeException();
            }
            CollectionChanged += DestinationCollectionChanged;
        }

        private void DestinationCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            _sourceList.CollectionChanged -= SourceCollectionChanged;
            switch (args.Action) {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < args.NewItems.Count; i++) {
                        var destItem = (TOutput)args.NewItems[i];
                        var sourceItem = ConvertBack(destItem);
                        _sourceList.Insert(args.NewStartingIndex + i, sourceItem);
                        CreatePropertySync(sourceItem, destItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < args.OldItems.Count; i++) _sourceList.RemoveAt(args.OldStartingIndex + i);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < args.NewItems.Count; i++) {
                        var destItem = (TOutput)args.NewItems[i];
                        var sourceItem = ConvertBack(destItem);
                        _sourceList[args.OldStartingIndex + i] = sourceItem;
                        CreatePropertySync(sourceItem, destItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    for (var i = 0; i < args.OldItems.Count; i++) _sourceList.Move(args.OldStartingIndex + i, args.NewStartingIndex + i);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    _sourceList.Clear();
                    foreach (var destItem in this) {
                        var sourceItem = ConvertBack(destItem);
                        _sourceList.Add(sourceItem);
                        CreatePropertySync(sourceItem, destItem);
                    }
                    break;
                default:
                    _sourceList.CollectionChanged += SourceCollectionChanged;
                    throw new ArgumentOutOfRangeException();

            }
            _sourceList.CollectionChanged += SourceCollectionChanged;
        }
    }
}