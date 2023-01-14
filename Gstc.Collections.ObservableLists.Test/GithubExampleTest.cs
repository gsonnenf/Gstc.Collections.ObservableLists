using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Gstc.Collections.ObservableLists.Binding;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;
[TestFixture]
public class GithubExampleTest {
    [Test]
    public static void Start() { //Todo: checkresults to see if they still work after binding list change
        ObservableList<Model> sourceList = new() {
            new Model() { MyNumber = 0, MyStringLower = "item number 0" },
            new Model() { MyNumber = 1, MyStringLower = "item number 1" }
        };
        ObservableList<ViewModel> targetList = new();

        // This class binds between the sourceList and the destList, in this case a model and a view model.
        // The convertSourceToDest/convertDestToSource define a mapping between the two lists.
        // An abstract class version also exists wherein the mapping is defined by implementing the method interface.
        ObservableListBind<Model, ViewModel> obvListSync =
                   new ObservableListBindFunc<Model, ViewModel>(
                       convertItemAToItemB: (sourceItem) => new ViewModel(sourceItem),
                       convertItemBtoItemA: (destItem) => destItem.SourceItem,
                       obvListA: sourceList,
                       obvListB: targetList,
                       isBidirectional: true, //allows adding to source and to target
                       sourceList: ListIdentifier.ListA //ListA is source by default, but added here for clarity.
                   );

        //Lists are synced on initialization.
        Console.WriteLine(sourceList[0].MyStringLower);           //Output: item number 0
        Console.WriteLine(targetList[0].MyStringUpper);           //Output: ITEM NUMBER 0
        Console.WriteLine(sourceList[0].MyNumber);                //Output: 0
        Console.WriteLine(targetList[0].MyNumberString + "\n");   //Output: Your number is 0

        //adding notifiers so we can see changes.
        sourceList.CollectionChanged += (sender, args) => Console.WriteLine("Source Collection Changed.");
        targetList.CollectionChanged += (sender, args) => Console.WriteLine("Target Collection Changed.");

        ///adding to source
        sourceList.Add(new Model { MyNumber = 2, MyStringLower = "item number 2" });
        // Output: Target Collection Changed.
        // Output: Source Collection Changed.
        Console.WriteLine(sourceList[2].MyStringLower);          //Output: item number 2
        Console.WriteLine(targetList[2].MyStringUpper);          //Output: ITEM NUMBER 2
        Console.WriteLine(sourceList[2].MyNumber);               //Output: 2
        Console.WriteLine(targetList[2].MyNumberString + "\n");  //Output: Your number is 2

        ///Adding to target
        targetList.Add(new ViewModel { MyNumberString = "Your number is 3", MyStringUpper = "ITEM NUMBER 3" });
        // Output: Source Collection Changed.
        // Output: Target Collection Changed.
        Console.WriteLine(sourceList[3].MyStringLower);          //Output: item number 3
        Console.WriteLine(targetList[3].MyStringUpper);          //Output: ITEM NUMBER 3
        Console.WriteLine(sourceList[3].MyNumber);               //Output: 3
        Console.WriteLine(targetList[3].MyNumberString + "\n");  //Output: Your number is 3

        ///changing source, propagating event to destination
        sourceList[1].MyNumber = -1;
        Console.WriteLine(sourceList[1].MyNumber);               //Output: -1
        Console.WriteLine(targetList[1].MyNumberString + "\n");  //Output: Your number is -1
    }

    //This class defines a mapping from a model to a view model. It also implements on property changed.
    public class Model : INotifyPropertyChanged {
        private int _myNum;
        private string _myStringLower;

        #region events
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        public int MyNumber {
            get => _myNum;
            set {
                _myNum = value;
                OnPropertyChanged(nameof(MyNumber));
            }
        }
        public string MyStringLower {
            get => _myStringLower;
            set {
                _myStringLower = value;
                OnPropertyChanged(nameof(MyStringLower));
            }
        }
    }

    /// <summary>
    /// This class defines a mapping from a viewmodel back to a model. It does not use onProperty changed as it writes 
    /// directly back to the model.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged; //todo: add notifiers
        public Model SourceItem { get; set; }
        public ViewModel() => SourceItem = new Model();
        public ViewModel(Model sourceItem) => SourceItem = sourceItem;
        public string MyNumberString {
            get => "Your number is " + SourceItem.MyNumber;
            set => SourceItem.MyNumber = int.Parse(Regex.Replace(value, "[^0-9]", ""));
        }
        public string MyStringUpper {
            get => SourceItem.MyStringLower.ToUpper();
            set => SourceItem.MyStringLower = value.ToLower();
        }
    }
}
