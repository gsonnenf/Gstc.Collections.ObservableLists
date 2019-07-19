using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Gstc.Collections.ObservableLists {


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public abstract class ObservableListSynchronizer<TSource, TDestination> {


        private ObservableList<TSource> _sourceObservableList;
        private ObservableList<TDestination> _destinationObservableList;


        /// <summary>
        /// A method for converting an item of type {TSource} to {TDestination}.
        /// </summary>
        /// <param name="item">The source {TSource} Item.</param>
        /// <returns>A {TDestination} item.</returns>
        public abstract TDestination ConvertSourceToDestination(TSource item);

        /// <summary>
        /// A method for converting an item of type {TDestination} to {TSource}.
        /// </summary>
        /// <param name="item">The source {TDestination} Item.</param>
        /// <returns>A {TSource} item.</returns>
        public abstract TSource ConvertDestinationToSource(TDestination item);


        /// <summary>
        /// A source observable collection of type {TSource} that will be synchronized to a destination collection of type {TDestination}.
        /// Items are converted using your provided ConvertSourceToDestination(...) method.
        /// 
        /// </summary>
        public ObservableList<TSource> SourceObservableList {
            get { return _sourceObservableList; }
            set {
                _sourceObservableList = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableList<TDestination> DestinationObservableList {
            get { return _destinationObservableList; }
            set {
                _destinationObservableList = value;
            }
        }

        /// <summary>
        /// If true, add, remove, and replace from source collection is propogated to the destination collection. True by default.
        /// </summary>
        public bool SourceToDestinationCollectionSync { get; set; } = true;
        /// <summary>
        /// If true, add, remove, and replace from destination collection is propogated to the source collection. True by default.
        /// </summary>
        public bool DestinationToSourceCollectionSync { get; set; } = true;


        /// <summary>
        /// If true, an PropertyChanged event triggered on an source item will also trigger a PropertyChanged event on the coorelated desitination item.
        /// This is primarly used to ensure change notifications are triggered for destination items that are mapped to the source item instead of copied.
        /// True by default.
        /// </summary>
        public bool SourceToDestinationPropertyNotify { get; set; } = true;

        /// <summary>
        /// If true, an PropertyChanged event triggered on an desitination item will also trigger a PropertyChanged event on the coorelated source item.
        /// This is primarly used to ensure change notifications are triggered for source items that are mapped to the desitination item instead of copied.
        /// This is not very common. False by default.
        /// </summary>
        public bool DestinationToSourcePropertyNotify { get; set; } = false;



        protected ObservableListSynchronizer() {
            _sourceObservableList = new ObservableList<TSource>();
            _destinationObservableList = new ObservableList<TDestination>();

            _sourceObservableList.CollectionChanged += SourceCollectionChanged;
            _destinationObservableList.CollectionChanged += DestinationCollectionChanged;
        }

      
        protected ObservableListSynchronizer(ObservableList<TSource> sourceCollection, ObservableList<TDestination> destCollection) {
            _sourceObservableList = new ObservableList<TSource>();
            _destinationObservableList = new ObservableList<TDestination>();
            //TODO: Syncronize
            _sourceObservableList.CollectionChanged += SourceCollectionChanged;
            _destinationObservableList.CollectionChanged += DestinationCollectionChanged;            
        }



        public void ReplaceSourceCopyToDest(ObservableList<TSource> list) {
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

        public void ReplaceSourceRetrieveFromDest(ObservableList<TSource> list) {

        }

        public void ReplaceDestCopyToSource(ObservableList<TDestination> list) {

        }

        public void ReplaceDestRetrieveFromSource(ObservableList<TDestination> list) {

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
