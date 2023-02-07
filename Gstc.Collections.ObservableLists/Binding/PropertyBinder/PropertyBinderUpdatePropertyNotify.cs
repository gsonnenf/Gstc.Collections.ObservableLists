using System;
using System.ComponentModel;
using System.Reflection;

namespace Gstc.Collections.ObservableLists.Binding.PropertyBinder;
internal class PropertyBinderUpdatePropertyNotify<TItemSource, TItemTarget>
    : AbstractPropertyBinder<TItemSource, TItemTarget>
    where TItemSource : class, INotifyPropertyChanged
    where TItemTarget : class, INotifyPropertyChanged {

    private readonly static FieldInfo _fieldInfoSource = typeof(TItemSource).GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
    private readonly static FieldInfo _fieldInfoTarget = typeof(TItemTarget).GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);

    private MulticastDelegate _multicastDelegateSource;
    private MulticastDelegate _multicastDelegateTarget;

    private bool _isSynchronizationInProgress;

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
        if (_isSynchronizationInProgress || !IsBindingEnabled) return;
        _isSynchronizationInProgress = true;
        if (IsPropertyChangedHookTarget) ((INotifyPropertyChangedHook)itemT).OnPropertyChanged(itemT, new PropertyChangedEventArgs(string.Empty));
        else OnPropertyChangedReflection(itemT);
        _isSynchronizationInProgress = false;
    }

    protected override void TargetItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args) {
        if (_isSynchronizationInProgress || !IsBindingEnabled) return;
        if (!IsBidirectional) return;
        _isSynchronizationInProgress = true;
        if (IsPropertyChangedHookSource) ((INotifyPropertyChangedHook)itemS).OnPropertyChanged(itemS, new PropertyChangedEventArgs(string.Empty));
        else OnPropertyChangedReflection(itemS);
        _isSynchronizationInProgress = false;
    }
}
