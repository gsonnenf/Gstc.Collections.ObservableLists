using System;
using Gstc.Collections.ObservableLists.Synchronizer;
using NotifyPropertySyncChanged = Gstc.Collections.ObservableLists.Examples.ObservableListSync.NotifyPropertySyncChanged;

namespace Gstc.Collections.ObservableLists.Examples.CodeExamples {
    public class GithubExample2 {
        public static void Start() {
            var sourceList = new ObservableList<Model>();
            var destList = new ObservableList<ViewModel>();

            //Synchronizes our lists
            ObservableListSynchronizer<Model, ViewModel> obvListSync =
                       new ObservableListSynchronizerFunc<Model, ViewModel>(
                           (sourceItem) => new ViewModel(sourceItem),
                           (destItem) => destItem.SourceItem,
                           sourceList,
                           destList
                       );

            //That's it for setup.

            //Example functionality
            sourceList.Add(new Model { MyNum = 10, MyStringLower = "x" });
            destList.Add(new ViewModel { MyNum = "1000", MyStringUpper = "A" });

            Console.WriteLine(sourceList.Count);
            Console.WriteLine(destList.Count);

            //OUTPUT
            // 2
            // 2

            sourceList[0].MyNum = -1;

            Console.WriteLine(sourceList[0].MyNum); //-1
            Console.WriteLine(destList[0].MyNum); // -1

            //OUTPUT
            // -1
            // -1

            destList[1].MyStringUpper = "TEST";

            Console.WriteLine(sourceList[1].MyStringLower);
            Console.WriteLine(destList[1].MyStringUpper);

            //OUTPUT
            // test
            // TEST

            sourceList[0].PropertyChanged += (sender, args) => Console.WriteLine("Source Event");
            destList[0].PropertyChanged += (sender, args) => Console.WriteLine("Dest Event");

            destList[0].MyNum = "100000";

            //OUTPUT
            //Dest Event
            //Source Event

        }
        public class Model : NotifyPropertySyncChanged {
            private int _myNum;
            private string _myStringLower;

            public int MyNum { get => _myNum; set { _myNum = value; OnPropertyChanged(null); } }
            public string MyStringLower { get => _myStringLower; set { _myStringLower = value; OnPropertyChanged(null); } }
        }

        public class ViewModel : NotifyPropertySyncChanged {

            public Model SourceItem { get; set; }
            public ViewModel() => SourceItem = new Model();

            public ViewModel(Model sourceItem) => SourceItem = sourceItem;

            public string MyNum { get => SourceItem.MyNum.ToString(); set => SourceItem.MyNum = int.Parse(value); }
            public string MyStringUpper { get => SourceItem.MyStringLower.ToUpper(); set => SourceItem.MyStringLower = value.ToLower(); }

        }

    }
}
