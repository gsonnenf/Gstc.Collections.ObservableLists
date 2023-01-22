using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {
    public class TestViewModel : INotifyPropertyChanged {

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
        public void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        public readonly TestModel TestModel;

        public TestViewModel() => TestModel = new TestModel();

        public TestViewModel(TestModel testModel) => TestModel = testModel;

        public string Num1 {
            get => TestModel.Num1.ToString();
            set {
                var isParsed = int.TryParse(value, out var num);
                if (!isParsed) return;
                TestModel.Num1 = num;
                OnPropertyChanged();
            }
        }

        public int Num2 {
            get => TestModel.Num2 * -1;
            set {
                TestModel.Num2 = value * -1;
                OnPropertyChanged();
            }
        }
    }

}
