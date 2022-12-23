using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Gstc.Collections.ObservableLists.Interface;
using Gstc.Collections.ObservableLists.Multithread;

namespace Gstc.Collections.ObservableLists.Examples.ObservableList {
    /// <summary>
    /// Interaction logic for ObservableListControl.xaml
    /// </summary>
    public partial class ObservableListControl : UserControl {

        /// <summary>
        /// Dependency property for data binding to the ItemsSource of the list view to allow swapping of list types.
        /// </summary>
        public readonly static DependencyProperty BindingListProperty =
            DependencyProperty.Register(nameof(BindingList), typeof(IObservableList<Customer>),
                typeof(ObservableListControl), new PropertyMetadata(new ObservableList<Customer>()));

        /// <summary>
        /// Accessor for dependency property.
        /// </summary>
        public IObservableList<Customer> BindingList {
            get => (IObservableList<Customer>)GetValue(BindingListProperty);
            set => SetValue(BindingListProperty, value);
        }

        /// <summary>
        /// Demo for ObservableList{}
        /// </summary>
        private ObservableList<Customer> CustomerObservableList { get; set; } = new ObservableList<Customer>();

        /// <summary>
        /// Demo for ObservableIList{}
        /// </summary>
        private ObservableIList<Customer, List<Customer>> CustomerObservableIList { get; set; } = new ObservableIList<Customer, List<Customer>>();

        /// <summary>
        /// Demo for ObservableIListLocking{}
        /// </summary>
        private ObservableIListLocking<Customer, List<Customer>> CustomerObservableIListLocking { get; set; } = new ObservableIListLocking<Customer, List<Customer>>();


        /// <summary>
        /// Demonstrates the observable list upcast to the IList{} interface and still generating OnChange events.
        /// </summary>
        private IList<Customer> UpcastList => BindingList;

        public ObservableListControl() {
            InitializeComponent();
            BindingList = CustomerObservableList;
            CustomerObservableList.IsResetForAddRange = true; //Fixes WPF issue with add range.
            CustomerObservableList.List = Customer.GenerateCustomerList();
            CustomerObservableIList.List = Customer.GenerateCustomerList();
            CustomerObservableIListLocking.List = Customer.GenerateCustomerList();

            AddEvents(CustomerObservableList);
            AddEvents(CustomerObservableIList);
            AddEvents(CustomerObservableIListLocking);

        }

        #region events
        private void Button_Click_Add(object sender, RoutedEventArgs args)
            => UpcastList.Add(Customer.GenerateCustomer());

        private void Button_Click_AddRange(object sender, RoutedEventArgs args) {

            if (UpcastList is ObservableList<Customer> list)
                list.AddRange(Customer.GenerateCustomerList());

            else if (UpcastList is ObservableIList<Customer, List<Customer>> list2)
                foreach (var customer in Customer.GenerateCustomerList())
                    list2.Add(customer);

            else if (UpcastList is ObservableIListLocking<Customer, List<Customer>> list3)
                foreach (var customer in Customer.GenerateCustomerList())
                    list3.Add(customer);
        }

        private void Button_Click_New(object sender, RoutedEventArgs e) {

            if (UpcastList is ObservableList<Customer> list)
                list.List = Customer.GenerateCustomerList();

            else if (UpcastList is ObservableIList<Customer, List<Customer>> list2)
                list2.List = Customer.GenerateCustomerList();

            else if (UpcastList is ObservableIListLocking<Customer, List<Customer>> list3)
                list3.List = Customer.GenerateCustomerList();
        }

        private void Button_Click_Remove(object sender, RoutedEventArgs args) {
            var index = CustomerListView.SelectedIndex;
            if (index < 0 || index >= BindingList.Count) {
                AddToTextBox("Customer not selected.\n");
                return;
            }
            try { BindingList.RemoveAt(index); }
            catch (InvalidOperationException e) { AddToTextBox(e.Message); }
        }

        private void Button_Click_List(object sender, RoutedEventArgs e) => BindingList = CustomerObservableList;
        private void Button_Click_IList(object sender, RoutedEventArgs e) => BindingList = CustomerObservableIList;
        private void Button_Click_IListLocking(object sender, RoutedEventArgs e) => BindingList = CustomerObservableIListLocking;
        #endregion

        #region helpers
        private void AddToTextBox(string message) {
            EventTextBox.Text += message + "\n";
            EventTextBox.ScrollToEnd();
        }

        private void AddEvents(IObservableList<Customer> list) {
            list.CollectionChanged += (sender, args) => {

                var message = "Collection Changed - ";

                if (args.NewItems != null) {
                    var customer = args.NewItems[0] as Customer;
                    message += ("Customer Added: " + customer.FirstName + " " + customer.LastName);
                }
                else if (args.OldItems != null) {
                    var customer = args.OldItems[0] as Customer;
                    message += ("Customer Removed: " + customer.FirstName + " " + customer.LastName);
                }
                else message += "Reset";

                AddToTextBox(message + "\n");
            };

            list.CollectionChanging += (sender, args)
                => AddToTextBox("Attempting to connect to database...");

            list.Removing += (sender, args) => {
                if (((Customer)args.OldItems?[0])?.PurchaseAmount >= 50)
                    throw new InvalidOperationException("Purchases above $50 require approval for removal.\n");
            };
        }
        #endregion
    }
}
