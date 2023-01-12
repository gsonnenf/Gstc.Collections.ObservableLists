using System;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// The ObservableListSynchronizerFunc is a concrete implementation of ObservableListSynchronizer that
/// lets you provide the ConvertSourceToDestination(...) and ConvertDestinationToSource(...) by passing 
/// in a method call.
/// 
/// The ObservableListSynchronizer provides synchronization between two ObservableLists of different types 
/// {TSource} and {TDestination}. Add, Remove, clear, etc on one list is propagated to the other.
/// The user is required to provide a ConvertSourceToDestination(...) and ConvertDestinationToSource(...) 
/// that provide two way conversion between {TSource} and {TDestination}. Used in conjunction with the 
/// INotifyPropertySyncChanged interface, this class can also provide synchronization for notify events properties
/// within an item of {TSource} and {TDestination}. If a PropertyChanged event is triggered on an item in {TSource}
/// the class can trigger a PropertyChanged event in the corresponding {TDestination} item, and vice-versa.
/// This class can serve as a map between a model and viewmodel for user interfaces or headless data servers.
///
///
/// Author: Greg Sonnenfeld
/// Copyright 2019
///
/// 
/// </summary>
/// <typeparam name="TItemA">The source or model list type.</typeparam>
/// <typeparam name="TItemB">The destination or viewmodel list type.</typeparam>
public class ObservableListBindFunc<TItemA, TItemB> : ObservableListBind<TItemA, TItemB> {

    private readonly Func<TItemA, TItemB> _convertItemAToItemB;
    private readonly Func<TItemB, TItemA> _convertItemBTItemA;
    public override TItemB ConvertItem(TItemA item) => _convertItemAToItemB(item);
    public override TItemA ConvertItem(TItemB item) => _convertItemBTItemA(item);

    /// <summary>
    /// The ObservableListSynchronizerFunc is a concrete implementation of ObservableListSynchronizer that
    /// lets you provide the ConvertSourceToDestination(...) and ConvertDestinationToSource(...) by passing 
    /// in a method call.
    /// 
    /// The ObservableListSynchronizer provides synchronization between two ObservableLists of different types 
    /// {TSource} and {TDestination}. Add, Remove, clear, etc on one list is propagated to the other.
    /// The user is required to provide a ConvertSourceToDestination(...) and ConvertDestinationToSource(...) 
    /// that provide two way conversion between {TSource} and {TDestination}. Used in conjunction with the 
    /// INotifyPropertySyncChanged interface, this class can also provide synchronization for notify events properties
    /// within an item of {TSource} and {TDestination}. If a PropertyChanged event is triggered on an item in {TSource}
    /// the class can trigger a PropertyChanged event in the corresponding {TDestination} item, and vice-versa.
    /// This class can serve as a map between a model and viewmodel for user interfaces or headless data servers.
    /// 
    /// </summary>
    /// <param name="convertSourceToDest">Method for converting a {TSource} type to {TDestination} type.</param>
    /// <param name="convertDestToSource">Method for converting a {TDestination} type to {TSource} type.</param>
    /// <param name="propertyNotifySourceToDest">If true, triggers a PropertyChanged event on {TDestination} if one occurs on {TSource}. Requires INotifyPropertySyncChanged to be implemented on {TDestination}.</param>
    /// <param name="propertyNotifyDestToSource">If true, triggers a PropertyChanged event on {TSource} if one occurs on {TDestination}. Requires INotifyPropertySyncChanged to be implemented on {TSource}.</param>
    public ObservableListBindFunc(
        Func<TItemA, TItemB> convertSourceToDest,
        Func<TItemB, TItemA> convertDestToSource,
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA
        ) : base() {
        _convertItemAToItemB = convertSourceToDest;
        _convertItemBTItemA = convertDestToSource;

        IsBidirectional = isBidirectional;
        SourceList = sourceList;
    }

    /// <summary>
    /// The ObservableListSynchronizerFunc is a concrete implementation of ObservableListSynchronizer that
    /// lets you provide the ConvertSourceToDestination(...) and ConvertDestinationToSource(...) by passing 
    /// in a method call.
    /// 
    /// The ObservableListSynchronizer provides synchronization between two ObservableLists of different types 
    /// {TSource} and {TDestination}. Add, Remove, clear, etc on one list is propagated to the other.
    /// The user is required to provide a ConvertSourceToDestination(...) and ConvertDestinationToSource(...) 
    /// that provide two way conversion between {TSource} and {TDestination}. Used in conjunction with the 
    /// INotifyPropertySyncChanged interface, this class can also provide synchronization for notify events properties
    /// within an item of {TSource} and {TDestination}. If a PropertyChanged event is triggered on an item in {TSource}
    /// the class can trigger a PropertyChanged event in the corresponding {TDestination} item, and vice-versa.
    /// This class can serve as a map between a model and viewmodel for user interfaces or headless data servers.
    ///  
    /// </summary>
    /// <param name="convertSourceToDest">Method for converting a {TSource} type to {TDestination} type.</param>
    /// <param name="convertDestToSource">Method for converting a {TDestination} type to {TSource} type.</param>
    /// <param name="obvListA">The source ObservableList{TSource} to be synchronized.</param>
    /// <param name="obvListB">The destination ObservableList{TDestination} to be synchronized.</param>
    /// <param name="propertySyncListAToListB">If true, triggers a PropertyChanged event on {TDestination} if one occurs on {TSource}. Requires INotifyPropertySyncChanged to be implemented on {TDestination}.</param>
    /// <param name="propertyNotifyDestToSource">If true, triggers a PropertyChanged event on {TSource} if one occurs on {TDestination}. Requires INotifyPropertySyncChanged to be implemented on {TSource}.</param>

    public ObservableListBindFunc(
        Func<TItemA, TItemB> convertSourceToDest,
        Func<TItemB, TItemA> convertDestToSource,
        ObservableList<TItemA> obvListA,
        ObservableList<TItemB> obvListB,
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA
        ) {
        _convertItemAToItemB = convertSourceToDest;
        _convertItemBTItemA = convertDestToSource;

        SourceList = sourceList;
        IsBidirectional = isBidirectional;
        ReplaceListA(obvListA);
        ReplaceListB(obvListB);

    }
}
