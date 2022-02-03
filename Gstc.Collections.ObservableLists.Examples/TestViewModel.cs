namespace Gstc.Collections.ObservableLists.Examples {
    public class TestViewModel : NotifyPropertySyncChanged {
        public TestModel TestModel;

        public TestViewModel() => TestModel = new TestModel();

        public TestViewModel(TestModel testModel) => TestModel = testModel;

        public string Num1 {
            get => TestModel.Num1.ToString();
            set {
                int num = 0;
                var isParsed = int.TryParse(value, out num);
                if (!isParsed) return;
                TestModel.Num1 = num;
                OnPropertyChanged(null);
            }
        }

        public int Num2 {
            get => TestModel.Num2 + 5;
            set {
                TestModel.Num2 = value - 5;
                OnPropertyChanged(null);
            }
        }
    }

}

