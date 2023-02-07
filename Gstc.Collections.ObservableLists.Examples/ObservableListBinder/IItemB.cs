using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {
    public interface IItemB : INotifyPropertyChanged {
        string ItemName { get; }
        int Id { get; }
        string Num1String { get; set; }
        int Num2 { get; set; }
    }
}
