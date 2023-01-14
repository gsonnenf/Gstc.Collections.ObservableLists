namespace Gstc.Collections.ObservableLists.Binding;
public interface IObservableListBind<TItemA, TItemB> {
    /// <summary>
    /// Converts an item of type {TSource} to {TDestination}.
    /// </summary>
    /// <param name="item">The source {TSource} Item.</param>
    /// <returns>A {TDestination} item.</returns>
    public TItemB ConvertItem(TItemA item);

    /// <summary>
    /// Converts an item of type {TDestination} to {TSource}.
    /// </summary>
    /// <param name="item">The source {TDestination} Item.</param>
    /// <returns>A {TSource} item.</returns>
    public TItemA ConvertItem(TItemB item);

    /// <summary>
    /// todo: comments
    /// </summary>
    public bool IsBidirectional { get; set; }

    /// <summary>
    /// A source observable collection of type {TSource} that will be synchronized to a destination collection of type {TDestination}.
    /// Source Items are converted using your provided ConvertSourceToDestination(...) method and automatically added to the 
    /// destination collection.
    /// </summary>
    public IObservableList<TItemA> ObservableListA { get; set; }

    /// <summary>
    /// A destination observable collection of type {TDestination} that will be synchronized to a source collection of type {TSource}.
    /// On assignment, the destination list is cleared, and items from the source list are converted and added to the destination 
    /// using your provided ConvertSourceToDestination(...) method. After assignment changes to the destination list are propagated to
    /// the source list.
    /// 
    /// If you wish to propagate items from the destination list to the source list on assignment, use the ReplaceDestinationCopyToSource(...) method.
    /// </summary>
    public IObservableList<TItemB> ObservableListB { get; set; }

    /// <summary>
    /// todo: comments
    /// </summary>
    public ListIdentifier SourceList { get; set; }
}
