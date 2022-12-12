///
/// Author: Greg Sonnenfeld
/// Copyright 2019
///

using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Synchronizer;

/// <summary>
/// The ObservableListSyncronizer provides syncronization between two ObservableLists of different types 
/// {TSource} and {TDestination}. Add, Remove, clear, etc on one list is propogated to the other.
/// The user is required to provide a ConvertSourceToDestination(...) and ConvertDestinationToSource(...) 
/// that provide two way conversion between {TSource} and {TDestination}. Used in conjunction with the 
/// INotifyPropertySyncChanged interace, this class can also provide syncronization for notify events properties
/// within an item of {TSource} and {TDestination}. If a PropertyChanged event is triggered on an item in {TSource}
/// the class can trigger a PropertyChanged event in the corresponding {TDestination} item, and vice-versa.
/// This class can serve as a map between a model and viewmodel for user interfaces or headless data servers.
/// </summary>
/// <typeparam name="TSource">The source or model list type.</typeparam>
/// <typeparam name="TDestination">The destination or viewmodel list type.</typeparam>
public abstract class ObservableListSynchronizer<TSource, TDestination> {

    protected ObservableList<TSource> _sourceObservableList;
    protected ObservableList<TDestination> _destinationObservableList;

    /// <summary>
    /// Converts an item of type {TSource} to {TDestination}.
    /// </summary>
    /// <param name="item">The source {TSource} Item.</param>
    /// <returns>A {TDestination} item.</returns>
    public abstract TDestination ConvertSourceToDestination(TSource item);

    /// <summary>
    /// Converts an item of type {TDestination} to {TSource}.
    /// </summary>
    /// <param name="item">The source {TDestination} Item.</param>
    /// <returns>A {TSource} item.</returns>
    public abstract TSource ConvertDestinationToSource(TDestination item);

    #region Properties
    /// <summary>
    /// A source observable collection of type {TSource} that will be synchronized to a destination collection of type {TDestination}.
    /// Source Items are converted using your provided ConvertSourceToDestination(...) method and automatically added to the 
    /// destination collection.
    /// </summary>
    public ObservableList<TSource> SourceObservableList {
        get => _sourceObservableList;
        set => ReplaceSource_SyncToDestination(value);
    }

    /// <summary>
    /// A destination observable collection of type {TDestination} that will be synchronized to a source collection of type {TSource}.
    /// On assignment, the destination list is cleared, and items from the source list are converted anded added to the destination 
    /// using your provided ConvertSourceToDestination(...) method. After assignment changes to the destination list are propogated to
    /// the source list.
    /// 
    /// If you wish to propagate items from the destination list to the source list on assignment, use the ReplaceDestinationCopyToSource(...) method.
    /// </summary>
    public ObservableList<TDestination> DestinationObservableList {
        get => _destinationObservableList;
        set => ReplaceDestination_SyncFromSource(value);
    }

    /// <summary>
    /// If true, add, remove, and replace from source collection is propogated to the destination collection.  This is always true in the current version.
    /// </summary>
    public bool IsSyncSourceToDestCollection { get; set; } = true;
    //TODO: Fix events that use RemoveAt or other indexing methods with one-way sync, updating a non-synced list breaks the index map between lists. You may end up removing the wrong object.

    /// <summary>
    /// If true, add, remove, and replace from destination collection is propogated to the source collection.  This is always true in the current version.
    /// </summary>
    public bool IsSyncDestToSourceCollection { get; set; } = true;

    /// <summary>
    /// If true, an PropertyChanged event triggered on an source item will also trigger a PropertyChanged event on the coorelated desitination item.
    /// This is primarly used to ensure change notifications are triggered for destination items that are mapped to the source item instead of copied.
    /// True by default.
    /// </summary>
    public bool IsPropertyNotifySourceToDest { get; private set; }

    /// <summary>
    /// If true, an PropertyChanged event triggered on an desitination item will also trigger a PropertyChanged event on the coorelated source item.
    /// This is primarly used to ensure change notifications are triggered for source items that are mapped to the desitination item instead of copied.
    /// This is not very common. False by default.
    /// </summary>
    public bool IsPropertyNotifyDestToSource { get; private set; }

    /// <summary>
    /// Returns true if source or destination items are configured to share PropertyChanged events.
    /// </summary>
    public bool IsPropertyNotify => IsPropertyNotifySourceToDest || IsPropertyNotifyDestToSource;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new ObservableListSynchronizer with an empty source list and an empty destination list.
    /// </summary>
    protected ObservableListSynchronizer(bool propertyNotifySourceToDest = true, bool propertyNotifyDestToSource = true) {
        IsPropertyNotifySourceToDest = propertyNotifySourceToDest;
        IsPropertyNotifySourceToDest = propertyNotifyDestToSource;

        //_sourceObservableList = new ObservableList<TSource>();
        //_destinationObservableList = new ObservableList<TDestination>();
        //_sourceObservableList.CollectionChanged += SourceCollectionChanged;
        //_destinationObservableList.CollectionChanged += DestinationCollectionChanged;
    }

    /// <summary>
    /// Creates a new ObservableListSynchronizer with the provided sourceCollection and destination collection.
    /// </summary>
    protected ObservableListSynchronizer(ObservableList<TSource> sourceCollection, ObservableList<TDestination> destCollection, bool propertyNotifySourceToDest = true, bool propertyNotifyDestToSource = false) {
        IsPropertyNotifySourceToDest = propertyNotifySourceToDest;
        IsPropertyNotifySourceToDest = propertyNotifyDestToSource;
        _destinationObservableList = destCollection;
        _destinationObservableList.CollectionChanged += DestinationCollectionChanged;

        ReplaceSource_SyncToDestination(sourceCollection);
    }

    #endregion

    #region Replace Source or destination
    /// <summary>
    /// Replaces the source collection, then clears and syncronizes the destination collection with the newly added source collection.
    /// </summary>
    /// <param name="sourceObvList"></param>
    public void ReplaceSource_SyncToDestination(ObservableList<TSource> sourceObvList) {

        if (_sourceObservableList != null) _sourceObservableList.CollectionChanged -= SourceCollectionChanged;
        _sourceObservableList = sourceObvList;
        if (_sourceObservableList == null) { _destinationObservableList?.Clear(); return; }

        if (_destinationObservableList == null) { _sourceObservableList.CollectionChanged += SourceCollectionChanged; return; }


        _destinationObservableList.CollectionChanged -= DestinationCollectionChanged;
        _destinationObservableList.Clear();

        foreach (var sourceItem in _sourceObservableList) {
            var destItem = ConvertSourceToDestination(sourceItem);
            _destinationObservableList.Add(destItem);
            if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
        }
        _sourceObservableList.CollectionChanged += SourceCollectionChanged;
        _destinationObservableList.CollectionChanged += DestinationCollectionChanged;
    }
    /// <summary>
    /// Replaces the source collection, then clears and syncronizes the newly added source collection with the destination collection.
    /// </summary>
    /// <param name="sourceObvList"></param>
    public void ReplaceSource_SyncFromDestination(ObservableList<TSource> sourceObvList) {

        if (_sourceObservableList != null) _sourceObservableList.CollectionChanged -= SourceCollectionChanged;
        _sourceObservableList = sourceObvList;
        if (_sourceObservableList == null) return;

        _sourceObservableList.Clear();

        if (_destinationObservableList == null) {
            _sourceObservableList.CollectionChanged += SourceCollectionChanged;
            return;
        }

        foreach (var destItem in _destinationObservableList) {
            var sourceItem = ConvertDestinationToSource(destItem);
            _sourceObservableList.Add(sourceItem);
            if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
        }
        _sourceObservableList.CollectionChanged += SourceCollectionChanged;
    }

    /// <summary>
    /// Replaces the destination collection, then clears and syncronizes the source collection with the newly added destination collection. 
    /// </summary>
    /// <param name="destObvList"></param>
    public void ReplaceDestination_SyncToSource(ObservableList<TDestination> destObvList) {
        if (_destinationObservableList != null) _destinationObservableList.CollectionChanged -= DestinationCollectionChanged;
        _destinationObservableList = destObvList;
        if (_destinationObservableList == null) { _sourceObservableList?.Clear(); return; }

        if (_sourceObservableList == null) { _destinationObservableList.CollectionChanged += DestinationCollectionChanged; return; }

        _sourceObservableList.CollectionChanged -= SourceCollectionChanged;
        _sourceObservableList.Clear();

        foreach (var destItem in _destinationObservableList) {
            var sourceItem = ConvertDestinationToSource(destItem);
            _sourceObservableList.Add(sourceItem);
            if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
        }

        _destinationObservableList.CollectionChanged += DestinationCollectionChanged;
        _sourceObservableList.CollectionChanged += SourceCollectionChanged;
    }

    /// <summary>
    /// Replaces the destination collection, then clears and syncronizes the newly added destination collection with the source collection. 
    /// </summary>
    /// <param name="destObvList"></param>
    public void ReplaceDestination_SyncFromSource(ObservableList<TDestination> destObvList) {
        if (_destinationObservableList != null) _destinationObservableList.CollectionChanged -= DestinationCollectionChanged;
        _destinationObservableList = destObvList;
        if (_destinationObservableList == null) return;

        _destinationObservableList.Clear();

        if (_sourceObservableList == null) {
            _destinationObservableList.CollectionChanged += DestinationCollectionChanged;
            return;
        }

        foreach (var sourceItem in _sourceObservableList) {
            var destItem = ConvertSourceToDestination(sourceItem);
            _destinationObservableList.Add(destItem);
            if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
        }
        _destinationObservableList.CollectionChanged += DestinationCollectionChanged;
    }

    #endregion

    private void CreatePropertySync(TSource sourceItem, TDestination destItem) {

        //TODO: On the removal of an item, or the reset of a list, it might be useful to remove the sync from the removed objects.
        if (!(sourceItem is INotifyPropertyChanged && destItem is INotifyPropertyChanged)) return;
        if (!(sourceItem is IPropertyChangedSyncHook || destItem is IPropertyChangedSyncHook)) return;

        var propertySyncNotifer = new NotifyPropertySync(
            sourceItem as INotifyPropertyChanged,
            destItem as INotifyPropertyChanged,
            IsPropertyNotifySourceToDest,
            IsPropertyNotifyDestToSource);
        //propertySyncNotifierList.Add(propertySyncNotifer);
    }

    //TODO: Add an optional dispatcher method to execute update code on a UI thread.
    private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (!IsSyncSourceToDestCollection) return;
        if (_destinationObservableList == null) return;
        _destinationObservableList.CollectionChanged -= DestinationCollectionChanged;
        switch (args.Action) {
            case NotifyCollectionChangedAction.Add:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var sourceItem = (TSource)args.NewItems[index];
                    var destItem = ConvertSourceToDestination(sourceItem);
                    _destinationObservableList.Insert(args.NewStartingIndex + index, destItem);
                    if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                for (var index = 0; index < args.OldItems.Count; index++)
                    _destinationObservableList.RemoveAt(args.OldStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var sourceItem = (TSource)args.NewItems[index];
                    var destItem = ConvertSourceToDestination(sourceItem);
                    _destinationObservableList[args.OldStartingIndex + index] = destItem;
                    if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
                }
                break;

            case NotifyCollectionChangedAction.Move:
                for (var index = 0; index < args.OldItems.Count; index++)
                    _destinationObservableList.Move(args.OldStartingIndex + index, args.NewStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Reset:
                _destinationObservableList.Clear();
                foreach (var sourceItem in _sourceObservableList) {
                    var destItem = ConvertSourceToDestination(sourceItem);
                    _destinationObservableList.Add(destItem);
                    if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
                }
                break;

            default:
                _destinationObservableList.CollectionChanged += DestinationCollectionChanged;
                throw new ArgumentOutOfRangeException();
        }
        _destinationObservableList.CollectionChanged += DestinationCollectionChanged;
    }

    private void DestinationCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (!IsSyncDestToSourceCollection) return;
        if (_sourceObservableList == null) return;
        _sourceObservableList.CollectionChanged -= SourceCollectionChanged;
        switch (args.Action) {
            case NotifyCollectionChangedAction.Add:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var destItem = (TDestination)args.NewItems[index];
                    var sourceItem = ConvertDestinationToSource(destItem);
                    _sourceObservableList.Insert(args.NewStartingIndex + index, sourceItem);
                    if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                for (var index = 0; index < args.OldItems.Count; index++)
                    _sourceObservableList.RemoveAt(args.OldStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var destItem = (TDestination)args.NewItems[index];
                    var sourceItem = ConvertDestinationToSource(destItem);
                    _sourceObservableList[args.OldStartingIndex + index] = sourceItem;
                    if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
                }
                break;

            case NotifyCollectionChangedAction.Move:
                for (var index = 0; index < args.OldItems.Count; index++)
                    _sourceObservableList.Move(args.OldStartingIndex + index, args.NewStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Reset:
                _sourceObservableList.Clear();
                foreach (var destItem in _destinationObservableList) {
                    var sourceItem = ConvertDestinationToSource(destItem);
                    _sourceObservableList?.Add(sourceItem);
                    if (IsPropertyNotify) CreatePropertySync(sourceItem, destItem);
                }
                break;
            default:
                _sourceObservableList.CollectionChanged += SourceCollectionChanged;
                throw new ArgumentOutOfRangeException();

        }
        _sourceObservableList.CollectionChanged += SourceCollectionChanged;
    }

}
