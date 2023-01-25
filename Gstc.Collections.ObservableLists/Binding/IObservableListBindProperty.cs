namespace Gstc.Collections.ObservableLists.Binding;
public interface IObservableListBindProperty<TItemA, TItemB> : IObservableListBind<TItemA, TItemB> {
    bool IsPropertyBindingEnabled { get; set; }
}
