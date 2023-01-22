using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding.PropertyBinder;

internal class PropertyBinderUpdateCustom<TItemSource, TItemTarget>
        : AbstractPropertyBinder<TItemSource, TItemTarget>
        where TItemSource : class, INotifyPropertyChanged
        where TItemTarget : class, INotifyPropertyChanged {

    private ICustomPropertyMap<TItemSource, TItemTarget> _customPropertyMap;

    private bool _isSynchronizationInProgress;

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
        if (_isSynchronizationInProgress || !IsBindingEnabled) return;
        _isSynchronizationInProgress = true;
        _customPropertyMap.PropertyChangedSourceToTarget(args, itemS, itemT);
        _isSynchronizationInProgress = false;
    }

    protected override void TargetItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args) {
        if (_isSynchronizationInProgress || !IsBindingEnabled) return;
        if (!IsBidirectional) return;
        _isSynchronizationInProgress = true;
        _customPropertyMap.PropertyChangedTargetToSource(args, itemT, itemS);
        _isSynchronizationInProgress = false;
    }
}

