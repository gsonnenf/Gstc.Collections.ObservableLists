using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {
    public interface IItemA : INotifyPropertyChanged {
        string ItemName { get; }
        int Id { get; }
        int Num1 { get; set; }
        int Num2 { get; set; }
    }
}
