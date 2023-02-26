using System.ComponentModel;
using Gstc.Collections.ObservableLists.Utils;

namespace Gstc.Collections.ObservableLists.Binding.PropertyBinder;

internal class PropertyBinderUpdateCustom<TItemSource, TItemTarget>
        : PropertyBinderAbstract<TItemSource, TItemTarget>
        where TItemSource : class, INotifyPropertyChanged
        where TItemTarget : class, INotifyPropertyChanged {

    private ICustomPropertyMap<TItemSource, TItemTarget> _customPropertyMap;

    private readonly SyncingFlagScope _syncing = new();

    public PropertyBinderUpdateCustom(
        IObservableList<TItemSource> sourceList,
        IObservableList<TItemTarget> targetList,
        ICustomPropertyMap<TItemSource, TItemTarget> customPropertyMap,
        bool isBidirectional = true,
        bool isBindingEnabled = true)
        : base(sourceList, targetList, isBidirectional, isBindingEnabled) {

        _customPropertyMap = customPropertyMap;
    }

    protected override void SourceItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args) {
        if (_syncing.InProgress || !IsBindingEnabled) return;
        using (_syncing.Begin()) _customPropertyMap.PropertyChangedSourceToTarget(args, itemS, itemT);
    }

    protected override void TargetItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args) {
        if (_syncing.InProgress || !IsBindingEnabled) return;
        if (!IsBidirectional) return;
        using (_syncing.Begin()) _customPropertyMap.PropertyChangedTargetToSource(args, itemT, itemS);
    }
}

