namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// Interface for binding between two observable lists of different types.
/// </summary>
/// <typeparam name="TItemA">The type of items in the first list.</typeparam>
/// <typeparam name="TItemB">The type of items in the second list.</typeparam>
public interface IObservableListBind<TItemA, TItemB> {
    /// <summary>
    /// Converts an item of type {TItemA} to type {TItemB}.
    /// </summary>
    /// <param name="item">The item to be converted.</param>
    /// <returns>The converted item of type {TItemB}.</returns>
    public TItemB ConvertItem(TItemA item);

    /// <summary>
    /// Converts an item of type {TItemB} to type {TItemA}.
    /// </summary>
    /// <param name="item">The item to be converted.</param>
    /// <returns>The converted item of type {TItemA}.</returns>
    public TItemA ConvertItem(TItemB item);

    /// <summary>
    /// Gets or sets a value indicating whether the conversion between {TItemA} and {TItemB} is bidirectional, or unidirectional 
    /// from the designated SourceList to the target list.
    /// </summary>
    public bool IsBidirectional { get; set; }

    /// <summary>
    /// An IObservableList{TItemA} that will be synchronized to the IObservableList{TItemB}.
    /// Items are converted using the provided ConvertItem(...) method.
    /// </summary>
    public IObservableList<TItemA> ObservableListA { get; set; }

    /// <summary>
    /// An IObservableList{TItemB} that will be synchronized to the IObservableList{TItemA}.
    /// Items are converted using the provided ConvertItem(...) method.
    /// </summary>
    public IObservableList<TItemB> ObservableListB { get; set; }

    /// <summary>
    /// Sets if ObservableListA or ObservableListB list will be the source or 'primary' list. The other list will be 
    /// designated as the target list. If a list is replaced the source list will be copied to the target list, and
    /// if bidirectional is false, changes are only allowed from the source list to the target list.
    /// </summary>
    public ListIdentifier SourceList { get; set; }

    /// <summary>
    /// Removes all lists, bindings and events.
    /// </summary>
    public void ReleaseAll();

}
