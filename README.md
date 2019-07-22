# Gstc.Collections.ObservableLists
<p align="center">
  <img src="https://user-images.githubusercontent.com/686792/53543486-0e638800-3ae0-11e9-9566-6d2f18a28e61.jpg" height="350">
</p>


Gstc.Collections.ObservableLists <br>
Author - Greg Sonnenfeld, Copyright 2019 <br>
License: LGPL 3.0 <br>
Nuget: https://www.nuget.org/packages/Gstc.Collections.ObservableLists <br>

## What is it?
This library implements an `ObservableList<T>`, which generates `INotifyCollectionChanged` events, and an 
`ObservableListSynchronizer<TSource,TDestination>`, which keeps two related observable lists synchronized. The code has very strong
unit testing and provides example usage.

### `ObservableList<T>`
The `ObservableList<T>` implmenents `IList`, `IList<T>`, `ICollection`, `ICollection<T>`, `INotifyCollectionChanged`, `INotifyPropertyChanged` and will still generate collection changed events when downcast to its interfaces. The base functionality of the `ObservableList<T>` is backed by a standard `List<T>`. The ObservableList can thus serve as a wrapper for a pre-existing `List<T>`. 

The standard .net ObservableCollection is a bit limiting to work with. I believe it is sealed and does not implement many desirable
interfaces. It also cannot be used as a collection wrapper.

### `ObservableListSynchronizer<TSource,TDestination>`
The `ObservableListSynchronizer<TSource,TDestination>` provides synchronization between two ObservableLists of different but related 
types `<TSource>` and `<TDestination>`. List methods (Add, Remove, clear, etc) on one list is propogated to the other.

The `<TSource>` and `<TDestination>` are classes that map to each other in a one to one fashion, but may have differing field or include a data transformation. The user is required to provide a `ConvertSourceToDestination(...)` and `ConvertDestinationToSource(...)` that provide a two way conversion between a `<TSource>` and `<TDestination>` object. This is most often used when one needs to transform model data for display or a public API. A good example is mapping between a list of models and viewmodels. 

Used in conjunction with objects that implement an `INotifyPropertySyncChanged` interace, this class can also provide synchronization 
of `PropertyChanged` notify events in `<TSource>` and `<TDestination>` objects. If a `PropertyChanged` event is triggered on an item in 
`<TSource>`, an option exists to trigger a `PropertyChanged` event in the corresponding `<TDestination>` item, and vice-versa.

This package is a polished subset of another library (which includes observable dictionaries) still under development. The Extended Observable Collection. 

https://github.com/gsonnenf/ExtendedObservableCollection

## How do I get started?

The `ObservableList<T>` should work somewhat similar to the standard .NET `ObservableCollection<T>`. First, add the nuget package 
[ https://www.nuget.org/packages/Gstc.Collections.ObservableLists ] or checkout the code and include it in you project. 

The following example shows usage of an `ObservableList<T>`:

### `ObservableList<T>` Example
```csharp
var myObvList = new ObservableList<string>();
myObvList.CollectionChanged += (sender, args) => Console.Writeline("Collection has changed!");
myObvList.Added += (sender, args) => Console.Writeline("First item in NewItems is: " + args.NewItems[0]);
myObvList.Add("I am the first item.");

//works with downcasting
IList myIList = myObvList as IList;
myIList.Add("I am a second item added to a downcast IList.");

//Output:
// Collection has changed!
// First item in NewItems is: I am the first item.
// First item in NewItems is: I am a second item added to a downcast IList.
```

It can also be used with existing lists:

```csharp
var myList = new List<string>() { "one","two","three" };

//Wrapping a list
var myObvList = new ObservableList<string>();
myObvList.CollectionChanged += (sender, args) => Console.Writeline("Collection has changed!");
myObvList.Reset += (sender, args) => Console.Writeline("Collection has been reset!");
myObvList.List = myList;

//Events after wrapping a list
myObvList.Added += ()=> Console.Writeline("Item added: ");

myObvList.Add("I will trigger an event!");
myList.Add("I will not trigger an event. It may be better to copy me into an observable list if this will happen.");

//Output:
// Collection has changed!
// Collection has been reset!
// Item added: I will trigger an event!

``` 

### `ObservableListSynchronizer< TSource,TDestination >` Example

The following show how easy it is to setup a synchronization using ObservableListSynchronizer. It demonstrates a sync between
a Model and ViewModel list, where the ViewModel performs a transform on the data provided by the Model. For a different implementation
you could also copy data back and forth between Model and ViewModel.

```csharp
public class GithubExample2 {
        public static void Start() {
            ObservableList<Model> sourceList = new ObservableList<Model>();
            ObservableList<ViewModel> destList = new ObservableList<ViewModel>();

            //Synchronizes our lists
            ObservableListSynchronizer<Model, ViewModel> ObvListSync =
                       new ObservableListSynchronizerFunc<Model, ViewModel>(
                           (sourceItem) => new ViewModel(sourceItem),
                           (destItem) => destItem.SourceItem,
                           sourceList,
                           destList
                       );

            //Thats it for setup.

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

            sourceList[0].PropertyChanged += (sender,args) => Console.WriteLine("Source Event");
            destList[0].PropertyChanged += (sender,args) => Console.WriteLine("Dest Event");

            destList[0].MyNum = "100000";

            //OUTPUT
            //Dest Event
            //Source Event

    }
    
 //You don't need NotifyPropertySyncChanged to sync collections, they are used to sync item properties. 
 //A basic POCO would work for collections.
 
        public class Model : NotifyPropertySyncChanged {   
            private int myNum;
            private string myStringLower;

            public int MyNum { get => myNum; set { myNum = value; OnPropertyChanged(null); } }
            public string MyStringLower { get => myStringLower; set { myStringLower = value; OnPropertyChanged(null); } }
        }

        public class ViewModel : NotifyPropertySyncChanged {

            public Model SourceItem { get; set; }
            public ViewModel() => SourceItem = new Model();
            public ViewModel(Model sourceItem) => SourceItem = sourceItem;

            public string MyNum { get => SourceItem.MyNum.ToString(); set => SourceItem.MyNum = int.Parse(value); }
            public string MyStringUpper { get => SourceItem.MyStringLower.ToUpper(); set => SourceItem.MyStringLower = value.ToLower(); }

        }

    }
``` 
