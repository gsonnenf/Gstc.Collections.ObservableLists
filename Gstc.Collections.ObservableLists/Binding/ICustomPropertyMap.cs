using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// A user defined mapping between TItemSource and TItemTarget that will transform and update a property on
/// one item when a property on the other item has triggered an NotifyPropertyChanged event. This is used
/// in <see cref="ObservableListBindProperty{TItemA, TItemB}"/> for its UpdateCustomNotify property binding type. 
/// </summary>
/// <typeparam name="TItemSource">The type of source to be mapped onto the target</typeparam>
/// <typeparam name="TItemTarget">The type of target to be mapped onto the source</typeparam>
public interface ICustomPropertyMap<TItemSource, TItemTarget> {
    void PropertyChangedSourceToTarget(PropertyChangedEventArgs args, TItemSource itemS, TItemTarget itemT);
    void PropertyChangedTargetToSource(PropertyChangedEventArgs args, TItemTarget itemT, TItemSource itemS);
}
