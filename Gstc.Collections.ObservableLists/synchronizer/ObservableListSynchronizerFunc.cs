using System;

namespace Gstc.Collections.ObservableLists.Synchronizer;

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
/// <typeparam name="TSource">The source or model list type.</typeparam>
/// <typeparam name="TDestination">The destination or viewmodel list type.</typeparam>
public class ObservableListSynchronizerFunc<TSource, TDestination> : ObservableListSynchronizer<TSource, TDestination> {

    private readonly Func<TSource, TDestination> _convertSourceToDest;
    private readonly Func<TDestination, TSource> _convertDestToSource;

    public override TDestination ConvertSourceToDestination(TSource item) => _convertSourceToDest(item);

    public override TSource ConvertDestinationToSource(TDestination item) => _convertDestToSource(item);

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
    public ObservableListSynchronizerFunc(
        Func<TSource, TDestination> convertSourceToDest,
        Func<TDestination, TSource> convertDestToSource,
        bool propertyNotifySourceToDest = true,
        bool propertyNotifyDestToSource = true) : base(propertyNotifySourceToDest, propertyNotifyDestToSource) {
        _convertSourceToDest = convertSourceToDest;
        _convertDestToSource = convertDestToSource;
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
    /// <param name="sourceObvList">The source ObservableList{TSource} to be synchronized.</param>
    /// <param name="destObvList">The destination ObservableList{TDestination} to be synchronized.</param>
    /// <param name="propertyNotifySourceToDest">If true, triggers a PropertyChanged event on {TDestination} if one occurs on {TSource}. Requires INotifyPropertySyncChanged to be implemented on {TDestination}.</param>
    /// <param name="propertyNotifyDestToSource">If true, triggers a PropertyChanged event on {TSource} if one occurs on {TDestination}. Requires INotifyPropertySyncChanged to be implemented on {TSource}.</param>

    public ObservableListSynchronizerFunc(
        Func<TSource, TDestination> convertSourceToDest,
        Func<TDestination, TSource> convertDestToSource,
        ObservableList<TSource> sourceObvList,
        ObservableList<TDestination> destObvList,
        bool propertyNotifySourceToDest = true,
        bool propertyNotifyDestToSource = true) : base(propertyNotifySourceToDest, propertyNotifyDestToSource) {
        _convertSourceToDest = convertSourceToDest;
        _convertDestToSource = convertDestToSource;
        SourceObservableList = sourceObvList;
        DestinationObservableList = destObvList;
    }
}
