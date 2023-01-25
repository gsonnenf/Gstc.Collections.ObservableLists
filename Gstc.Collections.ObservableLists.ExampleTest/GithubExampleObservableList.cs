using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Gstc.Collections.ObservableLists.ExampleTest.Fakes;
using Gstc.Collections.ObservableLists.Multithread;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;

[TestFixture]
public class GitHubExampleObservableList {

    [Test]
    public void ObservableListExample() {
        //Initialized wrapping a new default List<> as its internal list
        ObservableList<Customer> obvEmptyCustomerList = new();
        obvEmptyCustomerList.Add(Customer.GenerateCustomer());

        //Initialized wrapping an already existing list as its internal list
        List<Customer> customerList2 = Customer.GenerateCustomerList();
        ObservableList<Customer> obvCustomerList2 = new(customerList2);

        //Initialized, then assigned an already existing list as its internal list.
        List<Customer> customerList3 = Customer.GenerateCustomerList();
        ObservableList<Customer> obvCustomerList3 = new();
        obvCustomerList3.List = customerList3;

    }

    [Test]
    public void ObservableListBehaviorExample() {
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
    }

    [Test]
    public void ListTypeExample() {
        // By default the internal list is an List<T>:
        IObservableList<Customer> obvList1 = new ObservableList<Customer>();

        //If you need to use it with any class that implements IList, you can use the following implementation:
        IObservableList<Customer> obvList2 = new ObservableIList<Customer, DerivedIList<Customer>>();

        //If you need a thread safe observable list, you can use the following implementation:
        IObservableList<Customer> obvList3 = new ObservableIListLocking<Customer, List<Customer>>();
    }
    public class DerivedIList<TItem> : List<TItem> { }

    [Test]
    public void Multithread() {
        // This code would have race conditions.
        //IObservableList<Customer> obvList = new ObservableList<Customer>();
        //obvList.AllowReentrancy = true;

        //We use the Locking list instead
        IObservableList<Customer> obvList = new ObservableIListLocking<Customer, List<Customer>>();

        List<Task> taskList = new();
        Random rand = new();

        //This ensures many add operations are started before earlier ones finish. The locking prevents race conditions.
        obvList.Adding += (sender, args) => {
            int initialCount = obvList.Count;
            Thread.Sleep(rand.Next(10));
            int finalCount = obvList.Count;
            if (initialCount != finalCount) throw new TimeoutException("Race condition shound not be detected.");
        };

        //Generates a series of add operations on many threads.
        for (int index = 0; index < 100; index++) {
            Task task = Task.Run(() => obvList.Add(Customer.GenerateCustomer()));
            taskList.Add(task);
        }

        Task.WaitAll(taskList.ToArray());
        // All tasks are completed without race conditions encountered.
    }

}
