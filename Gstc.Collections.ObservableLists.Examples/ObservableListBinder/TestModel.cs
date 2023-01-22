using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {
    public class TestModel : INotifyPropertyChanged {

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
        public void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        private int _num1 = 5;
        private int _num2 = 10;

        public int Num1 { get => _num1; set { _num1 = value; OnPropertyChanged(nameof(Num1)); } }
        public int Num2 { get => _num2; set { _num2 = value; OnPropertyChanged(nameof(Num2)); } }
    }
}
