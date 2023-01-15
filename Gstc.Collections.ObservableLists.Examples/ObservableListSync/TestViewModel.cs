﻿namespace Gstc.Collections.ObservableLists.Examples.ObservableListSync {
    public class TestViewModel : NotifyPropertySyncChanged {
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
