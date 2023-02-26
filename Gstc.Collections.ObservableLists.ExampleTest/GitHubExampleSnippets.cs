using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Gstc.Collections.ObservableLists.Binding;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;
[TestFixture]
internal class GitHubExampleSnippets {
    [Test]
    public void UsageExample() {
        var obvListBind = new ObservableListBindFunc<int, string>(
            (itemA) => itemA.ToString(),
            (ItemB) => int.Parse(ItemB),
            new ObservableList<int>(),
            new ObservableList<string>()
       );
    }


    //TODO: Low Priority work in progress. Should be cleaned up.

    public void UsageExample2() {
        var obvListCustomerView = new ObservableList<CustomerView>();
        var obvListCustomer = new ObservableIList<Customer, Collection<Customer>>(Db.FetchAll());

        var obvListBindCustomer = new ObservableListBindCustomer(
            obvListA: obvListCustomer,
            obvListB: obvListCustomerView,
            isBidirectional: true,
            sourceList: ListIdentifier.ListA
        );
        obvListCustomerView.Adding += ValidateDataHook;
        obvListCustomer.Reset += (_, _) => Logger.Log("Customer list has been reset.");
        customerControl.ItemsSource = obvListCustomerView;
    }

    public CustomerControl customerControl;

    public class CustomerControl {
        public INotifyCollectionChanged ItemsSource;
    }

    public class Logger {
        public static void Log(string msg) => throw new NotImplementedException();
    }
    private void Log(string v) => throw new NotImplementedException();
    private void ValidateDataHook(object? sender, NotifyCollectionChangedEventArgs e) => throw new NotImplementedException();

    public class CustomerView { }
    public class Customer { }

    public class Db {
        public static Collection<Customer> FetchAll() => new();

    }

    public class ObservableListBindCustomer : ObservableListBind<Customer, CustomerView> {

        public ObservableListBindCustomer(
           IObservableList<Customer> obvListA,
           IObservableList<CustomerView> obvListB,
           bool isBidirectional,
           ListIdentifier sourceList)
            : base(obvListA, obvListB, isBidirectional, ListIdentifier.ListA) { }

        public override CustomerView ConvertItem(Customer item) => throw new System.NotImplementedException();
        public override Customer ConvertItem(CustomerView item) => throw new System.NotImplementedException();
    }




}
