using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding.PropertyBinder;
internal class PropertyBinderUpdateCollectionNotify<TItemSource, TItemTarget>
    : PropertyBinderAbstract<TItemSource, TItemTarget>
    where TItemSource : class, INotifyPropertyChanged
    where TItemTarget : class, INotifyPropertyChanged {

    public PropertyBinderUpdateCollectionNotify(
        IObservableList<TItemSource> sourceList,
        IObservableList<TItemTarget> targetList,
        bool isBidirectional = true,
        bool isBindingEnabled = true)
        : base(sourceList, targetList, isBidirectional, isBindingEnabled) {
    }

    protected override void SourceItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args) {
        if (!IsBindingEnabled) return;
        for (var indexS = 0; indexS < SourceList.Count; indexS++) //Will generate an event for every listing of this item in the list
            if (itemS == SourceList[indexS] && itemT == TargetList[indexS]) SourceList.RefreshIndex(indexS);
    }

    protected override void TargetItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args) {
        if (!IsBindingEnabled || !IsBidirectional) return;
        var indexT = TargetList.IndexOf(itemT); //Target list can not have repeat elements so we find first.
        if (itemS != SourceList[indexT]) throw DuplicateException();
        TargetList.RefreshIndex(indexT);
    }

}
