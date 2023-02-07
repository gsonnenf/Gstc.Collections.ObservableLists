namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {
    public class ItemM : NotifyPropertyChanged, IItemA {
        private static int _idCount;
        private static int GetNewId() => _idCount++;

        private int _num1 = 5;
        private int _num2 = 10;

        public int Id { get; }
        public string ItemName => "Model";

        public int Num1 {
            get => _num1;
            set {
                _num1 = value;
                OnPropertyChanged(nameof(Num1));
            }
        }
        public int Num2 {
            get => _num2;
            set {
                _num2 = value;
                OnPropertyChanged(nameof(Num2));
            }
        }

        public ItemM() {
            Id = GetNewId();
        }
    }
}
