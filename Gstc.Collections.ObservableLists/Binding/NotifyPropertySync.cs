using System.Collections.Generic;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// This class provides change notifications between two corresponding objects when using the Observable List sync.
/// </summary>
public class NotifyPropertySync {
    public List<PropertyChangedEventArgs> LastArgs { get; set; } = new();
    public IPropertyChangedSyncHook ItemASync { get; set; }
    public IPropertyChangedSyncHook ItemBSync { get; set; }
    public INotifyPropertyChanged ItemANotify { get; set; }
    public INotifyPropertyChanged ItemBNotify { get; set; }
    public NotifyPropertySync(INotifyPropertyChanged sourceItem, INotifyPropertyChanged destItem, bool sourceToDest = true, bool destToSource = true) {
        ItemANotify = sourceItem;
        ItemBNotify = destItem;

        ItemASync = sourceItem as IPropertyChangedSyncHook;
        ItemBSync = destItem as IPropertyChangedSyncHook;

        if (sourceItem == null && destItem == null) throw new System.ArgumentException("One of the objects must implement INotifyPropertySyncChanged.");

        if (ItemBSync != null && sourceToDest) ItemANotify.PropertyChanged += DestTrigger;
        if (ItemASync != null && destToSource) ItemBNotify.PropertyChanged += SourceTrigger;
    }

    public void DestTrigger(object sender, PropertyChangedEventArgs args) {
        if (LastArgs.Contains(args)) { _ = LastArgs.Remove(args); return; } //Allows concurrent execution. //Todo: verify this
        LastArgs.Add(args);
        ItemBSync.OnPropertyChanged(sender, args);
    }

    public void SourceTrigger(object sender, PropertyChangedEventArgs args) {
        if (LastArgs.Contains(args)) { _ = LastArgs.Remove(args); return; } //Allows concurrent execution.
        LastArgs.Add(args);
        ItemASync.OnPropertyChanged(sender, args);
    }

}
