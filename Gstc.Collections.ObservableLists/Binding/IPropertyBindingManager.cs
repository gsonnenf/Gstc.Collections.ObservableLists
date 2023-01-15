using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;
public interface IPropertyBindManager<TItemSource, TItemTarget>
    where TItemSource : class, INotifyPropertyChanged
    where TItemTarget : class {
    bool IsBidirectional { get; set; }
    bool IsBindingEnabled { get; set; }
    IObservableList<TItemSource> SourceList { get; set; }
    IObservableList<TItemTarget> TargetList { get; set; }

    void Bind(TItemSource itemS, TItemTarget itemT);
    void BindAll();
    void Unbind(TItemSource itemS, TItemTarget itemT);
    void UnbindAll();
}
