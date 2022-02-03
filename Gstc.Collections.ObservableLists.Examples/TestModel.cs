using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Examples {
    public class TestModel : INotifyPropertyChanged {

        private int num1 = 5;
        private int num2 = 10;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int Num1 { get => num1; set { num1 = value; OnPropertyChanged(null); } }
        public int Num2 { get => num2; set { num2 = value; OnPropertyChanged(null); } }
    }
}
