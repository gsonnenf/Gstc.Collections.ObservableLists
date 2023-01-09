using System;
using System.ComponentModel;
using Gstc.Collections.ObservableLists.Binding;
using Gstc.Collections.ObservableLists.Test.Tools;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;
[TestFixture]
public class GithubExampleTest {
    [Test]
    public static void Start() { //Todo: checkresults to see if they still work after binding list change
        ObservableList<Model> sourceList = new();
        ObservableList<ViewModel> destList = new();

        // This class synchronization between the sourceList and the destList, in this case a model and a view model.
        // The convertSourceToDest/convertDestToSource define a mapping between the two lists.
        // An abstract class version also exists wherein the mapping is defined by implementing the method interface.
        ObservableListBind<Model, ViewModel> obvListSync =
                   new ObservableListBindFunc<Model, ViewModel>(
                       convertSourceToDest: (sourceItem) => new ViewModel(sourceItem),
                       convertDestToSource: (destItem) => destItem.SourceItem,
                       obvListA: sourceList,
                       obvListB: destList

                   );

        sourceList.CollectionChanged += (_, _) => Console.WriteLine("Source Collection Changed.");
        destList.CollectionChanged += (_, _) => Console.WriteLine("Dest Collection Changed.");

        //Adding items to source, syncing to destination
        sourceList.Add(new Model { MyNumber = 10, MyStringLower = "item number 0" });
        // Output: Dest Collection Changed.
        // Output: Source Collection Changed.
        Console.WriteLine(sourceList[0].MyStringLower);        //Output: item number 0
        Console.WriteLine(destList[0].MyStringUpper);          //Output: ITEM NUMBER 0
        Console.WriteLine(sourceList[0].MyNumber);             //Output: 10
        Console.WriteLine(destList[0].MyNumberString + "\n");   //Output: Your number is 10

        //Adding items to destination, syncing to source
        destList.Add(new ViewModel { MyStringUpper = "ITEM NUMBER 1" });
        // Output: Dest Collection Changed.
        // Output: Source Collection Changed.
        Console.WriteLine(sourceList[1].MyStringLower);        //Output: item number 1
        Console.WriteLine(destList[1].MyStringUpper);          //Output: ITEM NUMBER 1
        Console.WriteLine(sourceList[1].MyNumber);             //Output: 0
        Console.WriteLine(destList[1].MyNumberString + "\n");  //Output: Your number is 0

        //changing source, propagating event to destination
        sourceList[1].MyNumber = -1;

        Console.WriteLine(sourceList[1].MyNumber);             //Output: -1
        Console.WriteLine(destList[1].MyNumberString + "\n");  //Output: Your number is -1
    }

    //This class defines a mapping from a model to a view model. It also implements on property changed.
    public class Model : IPropertyChangedSyncHook {
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
    public class ViewModel : NotifyPropertySyncChanged {
        public Model SourceItem { get; set; }
        public ViewModel() => SourceItem = new Model();
        public ViewModel(Model sourceItem) => SourceItem = sourceItem;
        public string MyNumberString => "Your number is: " + SourceItem.MyNumber;
        public string MyStringUpper {
            get => SourceItem.MyStringLower.ToUpper();
            set => SourceItem.MyStringLower = value.ToLower();
        }

    }
}
