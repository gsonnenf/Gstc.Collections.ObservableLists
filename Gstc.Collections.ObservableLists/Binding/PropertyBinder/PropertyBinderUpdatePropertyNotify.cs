using System;
using System.ComponentModel;
using System.Reflection;
using Gstc.Collections.ObservableLists.Utils;

namespace Gstc.Collections.ObservableLists.Binding.PropertyBinder;
internal class PropertyBinderUpdatePropertyNotify<TItemSource, TItemTarget>
    : PropertyBinderAbstract<TItemSource, TItemTarget>
    where TItemSource : class, INotifyPropertyChanged
    where TItemTarget : class, INotifyPropertyChanged {

    private readonly static FieldInfo _fieldInfoSource = typeof(TItemSource).GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
    private readonly static FieldInfo _fieldInfoTarget = typeof(TItemTarget).GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);

    private MulticastDelegate _multicastDelegateSource;
    private MulticastDelegate _multicastDelegateTarget;

    private readonly SyncingFlagScope _syncing = new();

    private static bool IsPropertyChangedHookSource { get; } = typeof(INotifyPropertyChangedHook).IsAssignableFrom(typeof(TItemSource));
    private static bool IsPropertyChangedHookTarget { get; } = typeof(INotifyPropertyChangedHook).IsAssignableFrom(typeof(TItemTarget));

    public PropertyBinderUpdatePropertyNotify(
        IObservableList<TItemSource> sourceList,
        IObservableList<TItemTarget> targetList,
        bool isBidirectional = true,
        bool isBindingEnabled = true)
        : base(sourceList, targetList, isBidirectional, isBindingEnabled) { }

    private void OnPropertyChangedReflection(TItemSource itemS) {
        _multicastDelegateSource = (MulticastDelegate)_fieldInfoSource.GetValue(itemS);
        foreach (var handler in _multicastDelegateSource.GetInvocationList())
            _ = handler.Method.Invoke(handler.Target, new object[] { itemS, new PropertyChangedEventArgs(string.Empty) });
    }

    private void OnPropertyChangedReflection(TItemTarget itemT) {
        _multicastDelegateTarget = (MulticastDelegate)_fieldInfoTarget.GetValue(itemT);
        foreach (var handler in _multicastDelegateTarget.GetInvocationList())
            _ = handler.Method.Invoke(handler.Target, new object[] { itemT, new PropertyChangedEventArgs(string.Empty) });
    }

    protected override void SourceItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args) {
        if (_syncing.InProgress || !IsBindingEnabled) return;
        using (_syncing.Begin()) {
            if (IsPropertyChangedHookTarget) ((INotifyPropertyChangedHook)itemT).OnPropertyChanged(itemT, new PropertyChangedEventArgs(string.Empty));
            else OnPropertyChangedReflection(itemT);
        }
    }

    protected override void TargetItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args) {
        if (_syncing.InProgress || !IsBindingEnabled) return;
        if (!IsBidirectional) return;
        using (_syncing.Begin()) {
            if (IsPropertyChangedHookSource) ((INotifyPropertyChangedHook)itemS).OnPropertyChanged(itemS, new PropertyChangedEventArgs(string.Empty));
            else OnPropertyChangedReflection(itemS);
        }
    }
}
