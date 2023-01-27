using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// The <see cref="ObservableListBind{TItemA, TItemB}"/> provides data binding between a pair of <see cref="IObservableList{TItemA}"/>, 
/// <see cref="ObservableListA{TItemA}"/> and <see cref="ObservableListB{TItemB}"/>
/// A user defined map between <see cref="{TItemA}"/> and <see cref="{TItemB}"/> is defined in the ConvertItem(..) methods. 
/// <br/><br/>
/// The binding can be set to unidirectional mode or bidirectional mode, with the SourceList property specifying the source list.
/// In bidirectional mode, changes are propogated in both directions, In unidirectional mode only the source list can be modified and
/// the target list will throw an exception if externally modified. When replacing either list, the source list items are convertd
/// onto the target list.
/// <br/><br/>
/// Author: Greg Sonnenfeld
/// Copyright 2019 to 2023
/// </summary>
/// <typeparam name="TItemA">The item type of <see cref="ObservableListA{TItemA}"/> to be bound on onto <see cref="ObservableListB{TItemB}"/></typeparam>
/// <typeparam name="TItemB">The item type of <see cref="ObservableListB{TItemB}"/> to be bound on onto <see cref="ObservableListA{TItemA}"/></typeparam>
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
    /// Constructor initializes ObservableListBindFunc{TItemA,TItemB} with a user provided ObservableListA and ObservableListB.
    /// <br/><br/>
    /// <inheritdoc cref="ObservableListBind{TItemA, TItemB}"/> 
    /// </summary>
    /// <param name="observableListA">User provided ObservableListA</param>
    /// <param name="observableListB">User provided ObservableListB</param>
    /// <param name="isBidirectional">Specifies if changes are propagated in both directions or only from source list to target list</param>
    /// <param name="sourceList">1Specifies if ObservableListA or ObservableListB will be the source list.</param>
    protected ObservableListBind(
        IObservableList<TItemA> observableListA,
        IObservableList<TItemB> observableListB,
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA
        ) {
        SourceList = sourceList;
        IsBidirectional = isBidirectional;
        ReplaceListA(observableListA);
        ReplaceListB(observableListB);
    }

    /// <summary>
    /// Constructor initializes ObservableListBind{TItemA,TItemB} with ObservableListA and ObservableListB null. 
    /// These can be assigned using the ObservableListA and ObservableListB properties.
    /// <br/><br/>
    /// <inheritdoc cref="ObservableListBind{TItemA, TItemB}"/> 
    /// </summary>
    /// <param name="isBidirectional"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(bool, ListIdentifier)" path="/param[@name='isBidirectional']"/> </param>
    /// <param name="sourceList"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(bool, ListIdentifier)" path="/param[@name='sourceList']"/> </param>
    protected ObservableListBind(
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA
        ) {
        SourceList = sourceList;
        IsBidirectional = isBidirectional;
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

