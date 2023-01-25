using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;

//Todo: fix this comment.
/// <summary>
/// The ObservableListBinding provides syncing between two ObservableLists of different types 
/// {TItemA} and {TItemB}. Add, Remove, clear, etc on one list is converted and propagated to the other.
/// <br/><br/>
/// The binding requires one list to be the source, which will provide data if either list is replaced. The binding 
/// can be operated in unidirectional or bidirectional mode. In unidirectional mode the source list can be modified
/// and the target list will throw an error if there is an attempt to modify it. In bidirectional, changes are propogated
/// in either direction.
/// <br/><br/>
/// The developer is required to provide a ConvertItem(...) that converts between each type. For single direction mode, 
/// the unused direction can safely be implemented as ConvertItem(...) => throw new NotSupportedException();
/// <br/><br/>
/// Author: Greg Sonnenfeld
/// Copyright 2019-2023
/// </summary>
/// <typeparam name="TItemA">The source or model list type.</typeparam>
/// <typeparam name="TItemB">The destination or viewmodel list type.</typeparam>
public abstract class ObservableListBind<TItemA, TItemB> : IObservableListBind<TItemA, TItemB> {
    public abstract TItemB ConvertItem(TItemA item);
    public abstract TItemA ConvertItem(TItemB item);

    /// <summary>
    /// A flag to indicate when changes between lists are being synchronized. This prevents recursive onchanged updates.
    /// </summary>
    private bool _isSynchronizationInProgress;
    private IObservableList<TItemA> _observableListA;
    private IObservableList<TItemB> _observableListB;

    #region Properties
    public bool IsBidirectional { get; set; }

    public IObservableList<TItemA> ObservableListA {
        get => _observableListA;
        set => ReplaceListA(value);
    }

    public IObservableList<TItemB> ObservableListB {
        get => _observableListB;
        set => ReplaceListB(value);
    }

    public ListIdentifier SourceList { get; set; }
    #endregion

    #region Ctor
    /// <summary>
    /// todo: Add description
    /// </summary>
    /// <param name="isBidirectional"></param>
    /// <param name="sourceList"></param>
    protected ObservableListBind(
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA
        ) {
        SourceList = sourceList;
        IsBidirectional = isBidirectional;
    }

    /// <summary>
    /// todo: Add description
    /// </summary>
    /// <param name="obvListA"></param>
    /// <param name="obvListB"></param>
    /// <param name="isBidirectional"></param>
    /// <param name="sourceList"></param>
    protected ObservableListBind(
        IObservableList<TItemA> obvListA,
        IObservableList<TItemB> obvListB,
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA
        ) {
        SourceList = sourceList;
        IsBidirectional = isBidirectional;
        ReplaceListA(obvListA);
        ReplaceListB(obvListB);
    }

    ~ObservableListBind() => ReleaseAll();

    public void ReleaseAll() {
        if (_observableListA != null) _observableListA.CollectionChanged -= ListAChanged;
        if (_observableListB != null) _observableListB.CollectionChanged -= ListBChanged;
        _observableListA = null;
        _observableListB = null;
    }
    #endregion

    #region Replace Source or destination
    protected void ReplaceListA(IObservableList<TItemA> observableListA) {
        if (_observableListA != null) _observableListA.CollectionChanged -= ListAChanged;
        _observableListA = observableListA;
        RebindLists();
        if (_observableListA != null) _observableListA.CollectionChanged += ListAChanged;
    }

    protected void ReplaceListB(IObservableList<TItemB> observableListB) {
        if (_observableListB != null) _observableListB.CollectionChanged -= ListBChanged;
        _observableListB = observableListB;
        RebindLists();
        if (_observableListB != null) _observableListB.CollectionChanged += ListBChanged;
    }

    private void RebindLists() {
        _isSynchronizationInProgress = true;

        if (SourceList == ListIdentifier.ListA) {
            _observableListB?.Clear();
            if (_observableListA != null && _observableListB != null) foreach (var itemA in ObservableListA) {
                    var itemB = ConvertItem(itemA);
                    _observableListB.Add(itemB);
                }
        }

        if (SourceList == ListIdentifier.ListB) {
            _observableListA?.Clear();
            if (_observableListA != null && _observableListB != null) foreach (var itemB in ObservableListB) {
                    var itemA = ConvertItem(itemB);
                    _observableListA.Add(itemA);
                }
        }
        _isSynchronizationInProgress = false;
    }
    #endregion

    #region Collection Mapping
    //todo: - Feature: Add an optional dispatcher method to execute update code on a UI thread.
    private void ListAChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (_isSynchronizationInProgress || _observableListB == null) return;
        if (IsBidirectional == false && !(SourceList == ListIdentifier.ListA)) throw new InvalidOperationException("The target list was modified but bidirectional is not set to false.");
        ListChanged(args, _observableListA, _observableListB, ConvertItem);
    }

    private void ListBChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (_isSynchronizationInProgress || _observableListA == null) return;
        if (IsBidirectional == false && !(SourceList == ListIdentifier.ListB)) throw new InvalidOperationException("The target list was modified but bidirectional is not set to false.");
        ListChanged(args, _observableListB, _observableListA, ConvertItem);
    }

    private void ListChanged<TItemSource, TItemTarget>(
            NotifyCollectionChangedEventArgs args,
            IObservableList<TItemSource> observableListSource,
            IObservableList<TItemTarget> observableListTarget,
            Func<TItemSource, TItemTarget> convertItem
        ) {
        _isSynchronizationInProgress = true;

        switch (args.Action) {
            case NotifyCollectionChangedAction.Add:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemSource = (TItemSource)args.NewItems[index];
                    var itemTarget = convertItem(itemSource);
                    observableListTarget.Insert(args.NewStartingIndex + index, itemTarget);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                for (var index = 0; index < args.OldItems.Count; index++)
                    observableListTarget.RemoveAt(args.OldStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemSource = (TItemSource)args.NewItems[index];
                    var itemTarget = convertItem(itemSource);
                    observableListTarget[args.OldStartingIndex + index] = itemTarget;
                }
                break;

            case NotifyCollectionChangedAction.Move:
                for (var index = 0; index < args.OldItems.Count; index++)
                    observableListTarget.Move(args.OldStartingIndex + index, args.NewStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Reset:
                observableListTarget.Clear();
                foreach (var itemSource in observableListSource) {
                    var itemTarget = convertItem(itemSource);
                    observableListTarget.Add(itemTarget);
                }
                break;

            default:
                _isSynchronizationInProgress = false;
                throw new InvalidEnumArgumentException(args.Action.ToString());
        }
        _isSynchronizationInProgress = false;
    }
    #endregion
}

