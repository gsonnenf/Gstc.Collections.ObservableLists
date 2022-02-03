using Gstc.Collections.ObservableLists.Notify;

namespace Gstc.Collections.ObservableLists.Test.MockObjects {
    public class ItemBSource : NotifyPropertySyncChanged {
        public static ObservableList<ItemBSource> GetSampleSourceBList() {
            return new ObservableList<ItemBSource> {
                new ItemBSource { MyNum = 10, MyStringLower = "x" },
                new ItemBSource { MyNum = 15, MyStringLower = "y" },
                new ItemBSource { MyNum = 20, MyStringLower = "z" },
            };
        }

        private int _myNum;
        private string _myStringLower;

        public int MyNum {
            get => _myNum;
            set { _myNum = value; OnPropertyChanged(null); }
        }

        public string MyStringLower {
            get => _myStringLower;
            set { _myStringLower = value; OnPropertyChanged(null); }
        }

        public override bool Equals(object obj) {
            var temp = obj as ItemBSource;
            if (temp == null) return false;
            if (temp.MyNum == MyNum && temp.MyStringLower == MyStringLower) return true;
            return false;
        }
    }
}
