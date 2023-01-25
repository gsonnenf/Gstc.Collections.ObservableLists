using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// A user defined mapping between TItemSource and TItemTarget that will transform and update a property on
/// one item when a property on the other item has triggered an NotifyPropertyChanged event. This is used
/// in the ObservableBindProperty<,> class for its UpdateCustomNotify property binding type. 
/// </summary>
/// <typeparam name="TItemSource"></typeparam>
/// <typeparam name="TItemTarget"></typeparam>
public interface ICustomPropertyMap<TItemSource, TItemTarget> {
    void PropertyChangedSourceToTarget(PropertyChangedEventArgs args, TItemSource itemS, TItemTarget itemT);
    void PropertyChangedTargetToSource(PropertyChangedEventArgs args, TItemTarget itemT, TItemSource itemS);
}
