using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;
public interface ICustomPropertyMap<TItemSource, TItemTarget> {
    void PropertyChangedSourceToTarget(PropertyChangedEventArgs args, TItemSource itemS, TItemTarget itemT);
    void PropertyChangedTargetToSource(PropertyChangedEventArgs args, TItemTarget itemT, TItemSource itemS);
}
