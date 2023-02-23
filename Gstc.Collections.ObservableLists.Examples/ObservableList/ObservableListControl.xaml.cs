using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        private ObservableList<Customer> ObservableListCustomer { get; set; } = new ObservableList<Customer>() { IsAddRangeResetEvent = true };

        private ObservableIList<Customer, List<Customer>> ObservableIListCustomer { get; set; } = new ObservableIList<Customer, List<Customer>>() { IsAddRangeResetEvent = true };

        private ObservableIListLocking<Customer, List<Customer>> ObservableIListLockingCustomer { get; set; } = new ObservableIListLocking<Customer, List<Customer>>() { IsAddRangeResetEvent = true };

        public Dictionary<string, IObservableList<Customer>> ComboBoxDictionary;

        public ObservableListControl() {
            ObservableListCustomer.IsAddRangeResetEvent = true; //Fixes WPF issue with add range.
            ObservableListCustomer.List = Customer.GenerateCustomerList();
            ObservableIListCustomer.List = Customer.GenerateCustomerList();
            ObservableIListLockingCustomer.List = Customer.GenerateCustomerList();

            InitializeComponent();

            ComboBoxDictionary = new Dictionary<string, IObservableList<Customer>>() {
                { "ObservableList<Customer>", ObservableListCustomer},
                { "ObservableIList<Customer,List<Customer>>",ObservableIListCustomer },
                { "ObservableIListLocking<Customer,List<Customer>>",ObservableIListLockingCustomer },
            };
            BindTypeComboBox.DisplayMemberPath = "Key";
            BindTypeComboBox.SelectedValuePath = "Value";
            BindTypeComboBox.ItemsSource = ComboBoxDictionary;
            BindTypeComboBox.SelectedIndex = 0;

            AddLoggingEvents(ObservableListCustomer);
            AddLoggingEvents(ObservableIListCustomer);
            AddLoggingEvents(ObservableIListLockingCustomer);

        }

        #region events
        private void ButtonClick_Add(object sender, RoutedEventArgs args) => BindingList.Add(Customer.GenerateCustomer());

        private void ButtonClick_AddRange(object sender, RoutedEventArgs args) => BindingList.AddRange(Customer.GenerateCustomerList());

        private void ButtonClick_New(object sender, RoutedEventArgs e) {
            //Accessing the internal list is not part of the IObservableList interface so an explict cast is necessary.
            var newList = Customer.GenerateCustomerList();
            if (BindingList is ObservableList<Customer> list) list.List = newList;
            else if (BindingList is ObservableIList<Customer, List<Customer>> list2) list2.List = newList;
            else if (BindingList is ObservableIListLocking<Customer, List<Customer>> list3) list3.List = newList;
        }

        private void ButtonClick_Remove(object sender, RoutedEventArgs args) {
            var index = CustomerListView.SelectedIndex;
            if (index < 0 || index >= BindingList.Count) {
                AddToTextBox("Customer not selected.\n");
                return;
            }
            try { BindingList.RemoveAt(index); }
            catch (NoPurchaseApprovalExpection e) { AddToTextBox(e.Message); }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
          BindingList = (IObservableList<Customer>)BindTypeComboBox.SelectedValue;
        #endregion

        #region helpers
        private void AddToTextBox(string message) {
            EventTextBox.Text += message + "\n";
            EventTextBox.ScrollToEnd();
        }

        private void AddLoggingEvents(IObservableList<Customer> list) {
            list.CollectionChanged += (sender, args) => {
                var message = "Collection Changed - ";
                if (args.NewItems != null) {
                    var customer = args.NewItems[0] as Customer;
                    message += ("Customer Added: " + customer.FirstName + " " + customer.LastName);
                } else if (args.OldItems != null) {
                    var customer = args.OldItems[0] as Customer;
                    message += ("Customer Removed: " + customer.FirstName + " " + customer.LastName);
                } else message += "Reset";
                AddToTextBox(message + "\n");
            };

            list.CollectionChanging += (sender, args)
                => AddToTextBox("Attempting to modify list...");

            list.Removing += (sender, args) => {
                if (((Customer)args.OldItems?[0])?.PurchaseAmount >= 50)
                    throw new NoPurchaseApprovalExpection("Purchases above $50 require approval for removal.\n");
            };
        }

        public class NoPurchaseApprovalExpection : Exception { public NoPurchaseApprovalExpection(string message) : base(message) { } }
        #endregion
    }
}
