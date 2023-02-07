namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {
    public class ItemVM : NotifyPropertyChanged, IItemB {
        private static int _idCount;
        private static int GetNewId() => _idCount++;

        private ItemM TestModel { get; }

        public int Id { get; }
        public string ItemName => "ViewModel";

        public string Num1String {
            get => MapItem.ConvertNum1(TestModel.Num1);
            set {
                TestModel.Num1 = MapItem.ConvertNum1(value);
                OnPropertyChanged(nameof(Num1String));
            }
        }

        public int Num2 {
            get => MapItem.ConvertNum2(TestModel.Num2);
            set {
                TestModel.Num2 = MapItem.ConvertNum2(value);
                OnPropertyChanged(nameof(Num2));
            }
        }
        public ItemVM() {
            Id = GetNewId();
            TestModel = new ItemM();
        }

    }

}
