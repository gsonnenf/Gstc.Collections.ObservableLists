# Gstc.Collections.ObservableLists
An observable lists library with upcast compatibility, list wrapping, reentrancy protection, thread safety, and robust unit testing.

***Observable Lists:***
A set of lists/list wrappers that implement the observable pattern, invoking events when a list is modified.<br/>
`ObservableList<TItem>`, `ObservableIList<TItem,TList<TItem>>`, `ObservableIListLocking<TItem,TList<TItem>>`, `IObservableList<TItem>`<br>

***Observable List Bindings:*** A set of list bindings that synchronize the content of observable lists using a mapping between item types.<br/>
`ObservableListBind<TItemA,TItemB>`, `ObservableListBindFunc<TItemA,TItemB>`, `ObservableListBindProperty<TItemA,TItemB>`, `ObservableListBindPropertyFunc<TItemA,TItemB>`

## License
<p align="left">
  <img src="https://user-images.githubusercontent.com/686792/53543486-0e638800-3ae0-11e9-9566-6d2f18a28e61.jpg" height="350">
</p>

***New Version 2! (2023-02-07)<br>***
Gstc.Collections.ObservableLists <br>
Author - Greg Sonnenfeld, Copyright 2019 to 2023 <br>
License: LGPL 3.0 <br>
Nuget: https://www.nuget.org/packages/Gstc.Collections.ObservableLists <br>
  
## Observable List 
The `ObservableList<TItem>`, `ObservableIList<TItem,TList<TItem>>`, `ObservableIListLocking<TItem,TList<TItem>>`, provide `IList<T>` implementations that invoke events ( `OnCollectionChanged`, `OnCollectionChanging`, `Adding`, `Added`, `Moving`, `Moved`, `Removing`, `Removed`, `Replacing`, `Replaced`,`Resetting`, `Reset`) when the list is modified. They provide a robust alternative to the .Net `ObservableCollection<T>`.

#### Classes
`ObservableList<TItem>` is the default observable list that utilizes an internal `List<TItem>` and can also serve as a wrapper for a pre-existing `List<T>`. It provides list modification events, maintains event calls on upcast, and provides reenetrancy protection.

```csharp
var obvList = new ObservableList<Customer>();
obvList.Adding += (sender,args)=> Console.WriteLine("Attempting to add a new customer.");
```

`ObservableIList<TItem,TList<TItem>>` is similar to `ObservableList<TItem>`, but allows the user to specify the internal list type with the `TList<TItem>` generic parameter. 

```csharp
Collection<Customer> customers = SomeDbApi.GetCustomers();
var obvList = new ObservableIList<Customer, Collection<Customer>>(customers); 
obvList.Adding += (sender,args)=> Console.WriteLine("Adding new customer to database mapped collection.");
```

`ObservableIListLocking<TItem,TList<TItem>>` is similar to `ObservableIList<TItem,TList<TItem>>`, but implements a `ReaderWriterLockSlim` list access, `lock` for event access, and special reentrancy rules for asynchronous/multithread operation.<br>

```csharp
var obvList = new ObservableIList<Customer, Collection<Customer>>() {List = SomeExampleCollection;}
obvList.Adding += (sender,args)=> Console.WriteLine("Fetching customer from Web API...");
for (int index = 0; index < 1000; index++) Task task = Task.Run(() => obvList.Add( MyWebApi.getNewCustomer() ));
```

`IObservableList<Item>` is the interfae for these classes and includes `IList<T>`, `IList`, `ICollection<T>`, `INotifyCollectionChanging`, `INotifyCollectionChanged`, `INotifyListChangingEvents`, `INotifyListChangingEvents` <br>

## List Binding

### Observable List Bind
`ObservableListBind<TItemA,TItemB>` provides synchronization between two ObservableLists of different but related types `<TItemA>` and `<TItemB>`. List methods (Add, Remove, clear, etc) on one list is propogated to the other given a conversion method. `ObservableListBindFunc<TItemA,TItemB>` is an implementation that allows the conversion method to be passed in the constructor as an anonymous function.

The `<TItemA>` and `<TItemB>` are classes that map to each other in an injective way, usually containing ommissions or data transformation. The user provides a `ConvertItem(...)` method that provide a two way conversion between a `<TItemA>` and `<TItemB>` object. This is most often used when one needs to transform model data for display or a public API.

```csharp
var obvListBind = new ObservableListBindFunc<int, string>(
            (itemA) => itemA.ToString(),
            (ItemB) => int.Parse(ItemB),
            new ObservableList<int>(),
            new ObservableList<string>()
       );
```
### Observable List Bind Property
`ObservableListBindProperty<TItemA,TItemB>` provides the functionality of `ObservableListBind<TItemA,TItemB>` and also provides synchronization between the properties of list item that implement `INotifyPropertyChanged`. 

```csharp
//See Gstc.Collections.ObservableLists.ExampleTest for example usage.
```

#### The class provides several different methods for synchronizing item properties including:

`UpdateCollectionNotify` - When a PropertyChanged event is raised, the corresponding item on the alternate list will be replaced by a new item created using the ConvertItem(...) method.
 
`UpdatePropertyNotify` - When a PropertyChanged event is raised, the corresponding item on the alternate list will have its PropertyChanged event triggered. The user is expected to provide any property synchronization. This is useful when the ItemB is a a wrapper for ItemA, and a PropertyChanged event is needed to trigger callbacks.
   
`UpdateCustomNotify` - When a PropertyChanged event is raised, the user provided `ICustomPropertyMap` is invoked to update item property on the alternate list.


## How do I get started?

The `ObservableList<T>` should work somewhat similar to the standard .NET `ObservableCollection<T>`. 

First, add the nuget package [ https://www.nuget.org/packages/Gstc.Collections.ObservableLists ] or checkout the code and include it in you project.  

Next utilize code from the following examples or look at the github examples in the ***Gstc.Collections.ObservableLists.ExampleTest*** namespace!

The following example shows usage of an `ObservableList<T>` :

### `ObservableList<T>` Example
```csharp
 ObservableList<Customer> obvCustomerList = new ObservableList<Customer>();

        //An event added for collection changing
        obvCustomerList.CollectionChanged += (sender, args) => {
            if (args.Action == NotifyCollectionChangedAction.Reset)
                foreach (Customer customer in (ObservableList<Customer>)sender)
                    Console.WriteLine("Initial Customers: " + customer.FirstName + " " + customer.LastName);
        };

        //An existing list is assigned to be the internal list
        List<Customer> customerList = Customer.GenerateCustomerList();
        obvCustomerList.List = customerList;

        //The ObservableList has functionality of a normal IList<>/Enumerable<>/etc.
        foreach (Customer item in obvCustomerList) Console.WriteLine("ObservableList has customer:" + item.FirstName);

        // IObservableList has hooks specific to actions (add, remove, reset, replace, move) as well as OnCollectionChanged.
        obvCustomerList.Adding += (sender, args) => {
            foreach (Customer customer in args.NewItems)
                Console.WriteLine("Going to add Customer: " + customer.FirstName + " " + customer.LastName);
        };

        obvCustomerList.Added += (sender, args) => {
            foreach (Customer customer in args.NewItems)
                Console.WriteLine("Customer was Added: " + customer.FirstName + " " + customer.LastName);
        };

        obvCustomerList.Add(Customer.GenerateCustomer());

        // IObservableList<> can be used in external methods that implement IList<> and IList
        void AddCustomerToList(IList<Customer> list) => list.Add(Customer.GenerateCustomer());
        void AddCustomerToList2(IList list) => list.Add(Customer.GenerateCustomer());

        AddCustomerToList(obvCustomerList);
        AddCustomerToList2(obvCustomerList);
```

### `ObservableListBind<TItemA,TItemB>` Example

The following demonstrates how the ObservableListBind works. For examples of `ObservableListBindProperty<TItemA,TItemB>` see the ***Gstc.Collections.ObservableLists.ExampleTest*** namespace.

```csharp
   public void ObservableListBindExample() {
        ObservableList<PhoneViewModel> obvPhoneListVM = new(); // Empty list
        ObservableList<PhoneModel> obvPhoneListM = new() { // Our example list with initial data
            new() { PhoneNumber = 5551112222 },
            new() { PhoneNumber = 5553334444 },
        };

        //Creates a binding between the two lists so they will have same content in converted form.
        ObservableListBindPhone obvBindPhone = new(
            obvListPhoneModel: obvPhoneListM,
            obvListPhoneViewModel: obvPhoneListVM,
            isBidirectional: true
            );

        foreach (var item in obvPhoneListM) Console.WriteLine(item.PhoneNumber);
        foreach (var item in obvPhoneListVM) Console.WriteLine(item.PhoneString);
        /// Output:
        /// 5551112222
        /// 5553334444
        /// 555-111-2222
        /// 555-333-4444

        obvPhoneListVM.Clear();
        Console.WriteLine(obvPhoneListM.Count);
        Console.WriteLine(obvPhoneListVM.Count);
        /// Output:
        /// 0
        /// 0

        obvPhoneListM.Add(new() { PhoneNumber = 9876543210 });
        obvPhoneListVM.Add(new() { PhoneString = "123-456-7890" });
        foreach (var item in obvPhoneListM) Console.WriteLine(item.PhoneNumber);
        foreach (var item in obvPhoneListVM) Console.WriteLine(item.PhoneString);
        /// Output:
        /// 9876543210
        /// 1234567890
        /// 987-654-3210
        /// 123-456-7890

        /// ObservableListBindFunc can be used as alternate to inheriting an abstract class ObservableListBind by passing 
        /// conversion functions in the constructor.
        IObservableListBind<PhoneModel, PhoneViewModel> obvBindPhoneFunc
            = new ObservableListBindFunc<PhoneModel, PhoneViewModel>(
                convertItemAToB: (item) => new PhoneViewModel() { PhoneString = item.PhoneNumber.ToString("###-###-####") },
                convertItemBToA: (item) => new PhoneModel() { PhoneNumber = long.Parse(Regex.Replace(item.PhoneString, "[^0-9]", "")) },
                observableListA: new ObservableList<PhoneModel>() { new PhoneModel() { PhoneNumber = 1112223333 } },
                observableListB: new ObservableList<PhoneViewModel>(),
                isBidirectional: false,
                sourceList: ListIdentifier.ListA
            );

        foreach (var item in obvBindPhoneFunc.ObservableListA) Console.WriteLine(item.PhoneNumber);
        foreach (var item in obvBindPhoneFunc.ObservableListB) Console.WriteLine(item.PhoneString);
        /// Output:
        /// 1112223333
        /// 111-222-3333
    }

    //This is the implmentation of the abstract class with a constructor and the convertItem(...) implemented.
    public class ObservableListBindPhone : ObservableListBind<PhoneModel, PhoneViewModel> {
        public ObservableListBindPhone(
            IObservableList<PhoneModel> obvListPhoneModel,
            IObservableList<PhoneViewModel> obvListPhoneViewModel,
            bool isBidirectional)
             : base(obvListPhoneModel, obvListPhoneViewModel, isBidirectional, ListIdentifier.ListA) { }

        public override PhoneViewModel ConvertItem(PhoneModel item) => new() { PhoneString = item.PhoneNumber.ToString("###-###-####") };
        public override PhoneModel ConvertItem(PhoneViewModel item) => new() { PhoneNumber = long.Parse(Regex.Replace(item.PhoneString, "[^0-9]", "")) };
    }

    public class PhoneModel {
        public long PhoneNumber { get; set; }
    }

    public class PhoneViewModel {
        public string PhoneString { get; set; }
    }
``` 
