using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Examples.ObservableListSync {
    public class TestModel : INotifyPropertyChanged {

        private int _num1 = 5;
        private int _num2 = 10;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int Num1 { get => _num1; set { _num1 = value; OnPropertyChanged(null); } }
        public int Num2 { get => _num2; set { _num2 = value; OnPropertyChanged(null); } }
    }
}
