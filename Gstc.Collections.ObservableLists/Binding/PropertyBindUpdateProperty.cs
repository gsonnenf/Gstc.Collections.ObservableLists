using System;
using System.ComponentModel;
using System.Reflection;

namespace Gstc.Collections.ObservableLists.Binding;
public class PropertyBindUpdateProperty<TItemSource, TItemTarget>
    : AbstractPropertyBindManager<TItemSource, TItemTarget>
    where TItemSource : class, INotifyPropertyChanged
    where TItemTarget : class, INotifyPropertyChanged {

    public PropertyBindUpdateProperty(
        IObservableList<TItemSource> sourceList,
        IObservableList<TItemTarget> targetList,
        bool isBidirectional = true,
        bool isBindingEnabled = true)
        : base(sourceList, targetList, isBidirectional, isBindingEnabled) {
    }

    private Func<string, TItemSource, TItemTarget> _sourceToTargetProperty;
    private Func<string, TItemTarget, TItemSource> _targetToSourceProperty;

    private static MethodInfo RaiseMethodSource = typeof(TItemTarget).GetEvent("PropertyChanged").GetRaiseMethod();
    private static MethodInfo RaiseMethodTarget = typeof(TItemTarget).GetEvent("PropertyChanged").GetRaiseMethod();

    protected override void SourceItemChanged(TItemSource itemS, TItemTarget itemT) {
        if (!IsBindingEnabled) return;
        _ = RaiseMethodTarget.Invoke(itemT, new object[] { itemT, new PropertyChangedEventArgs(null) });

    }

    protected override void TargetItemChanged(TItemSource itemS, TItemTarget itemT) {
        if (!IsBindingEnabled) return;//todo: fix, update, test
        if (!IsBidirectional) return;
        _ = RaiseMethodSource.Invoke(itemS, new object[] { itemS, new PropertyChangedEventArgs(null) });
    }

}
