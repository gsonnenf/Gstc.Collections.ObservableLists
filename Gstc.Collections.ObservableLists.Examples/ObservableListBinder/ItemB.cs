namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {
    public class ItemB : NotifyPropertyChanged, IItemB {
        private static int _idCount;
        private static int GetNewId() => _idCount++;

        public string _num1String;
        public int _num2Negative;

        public int Id { get; }
        public string ItemName { get; set; }
        public string Num1String {
            get => _num1String;
            set {
                _num1String = value;
                OnPropertyChanged(nameof(Num1String));
            }
        }
        public int Num2 {
            get => _num2Negative;
            set {
                _num2Negative = value;
                OnPropertyChanged(nameof(Num2));
            }
        }
        public ItemB(string itemName) {
            Id = GetNewId();
            ItemName = itemName;
        }
    }
}
