namespace Gstc.Collections.ObservableLists.Binding;
public interface IObservableListBindProperty<TItemA, TItemB> : IObservableListBind<TItemA, TItemB> {
    /// <summary>
    /// Enables property binding on a ObservableListBindProperty object. When disabled, property events are removed from items and the 
    /// ObservableListBindProperty object behaves like a ObservableListBind object.
    /// </summary>
    bool IsPropertyBindingEnabled { get; set; }
}
