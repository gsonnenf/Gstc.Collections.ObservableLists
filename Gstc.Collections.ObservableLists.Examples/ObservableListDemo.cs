using System;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Examples.ObservableList;


namespace Gstc.Collections.ObservableLists.Examples {
    public class ObservableListDemo {
        public List<Customer> CustomerList { get; set; }
        public ObservableList<Customer> CustomerObservableListWrapper { get; set; }
        public ObservableList<Customer> CustomerObservableList { get; set; }

        public ObservableListDemo() {
            CustomerList = new List<Customer>();
            CustomerObservableList = new ObservableList<Customer>();
            CustomerObservableListWrapper = new ObservableList<Customer>();

            CustomerObservableListWrapper.CollectionChanged += (sender, args) => {
                Console.WriteLine("Collection Changed");
                foreach (Customer customer in args.NewItems)
                    Console.WriteLine("Customer Added: " + customer.FirstName + " " + customer.LastName);
            };

            CustomerObservableList.CollectionChanged += (sender, args) => {
                Console.WriteLine("Collection Changed");
                foreach (Customer customer in args.NewItems)
                    Console.WriteLine("Customer Added: " + customer.FirstName + " " + customer.LastName);
            };

            //Populates initial standard list.
            CustomerList.Add(Customer.GenerateCustomer());
            CustomerList.Add(Customer.GenerateCustomer());
            CustomerList.Add(Customer.GenerateCustomer());
            CustomerList.Add(Customer.GenerateCustomer());

            //Inserts standard list into the wrapper as its backing list. 
            CustomerObservableListWrapper.List = CustomerList;

            //Adds customers directly to second observable list.
            CustomerObservableList.Add(Customer.GenerateCustomer());
            CustomerObservableList.Add(Customer.GenerateCustomer());
            CustomerObservableList.Add(Customer.GenerateCustomer());

            //Adds list of customers from initial list into second observable list.
            CustomerObservableList.AddRange(CustomerList);

        }
    }
}
