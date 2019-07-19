using System.Windows.Controls;
using Gstc.Collections.ObservableLists;

namespace Gstc.Collections.ObservableLists.Examples {
    /// <summary>
    /// Interaction logic for ObservableListAdapterControl.xaml
    /// </summary>
    public partial class ObservableListAdapterControl : UserControl {

        //List of Customer models
        public ObservableList<Customer> CustomerList {
            get => (ObservableList<Customer>) CustomerViewModelList.SourceCollection;
            set => CustomerViewModelList.SourceCollection = value;
        }

        //List of Customer View Models: Method one - using implementation of abstract class. 
        public CustomerViewModelList CustomerViewModelList { get; set; } = new CustomerViewModelList();

        //List of Customer View Models: Method two - using anonymous functions to map Model to ViewModel.
        //public ObservableListAdapter<Customer, CustomerViewModel> CustomerViewModelList { get; set;}  
        //    = new ObservableListAdapterFunc<Customer, CustomerViewModel>( (customer) => new CustomerViewModel(customer), (customerVm) => customerVm.Customer );

        public ObservableListAdapterControl() {
            InitializeComponent();
            DataContext = this;
            CustomerList = CreateList();
        }

        private void Button_Click_Add(object sender, System.Windows.RoutedEventArgs e) {
            CustomerList.Add(Customer.GenerateCustomer());
        }

        private void Button_Click_Remove(object sender, System.Windows.RoutedEventArgs e) {
            if (CustomerList.Count > 0) CustomerList.RemoveAt(0);
        }

        private void Button_Click_New(object sender, System.Windows.RoutedEventArgs e) {
            CustomerList = CreateList();
        }

        private static ObservableList<Customer> CreateList() => new ObservableList<Customer>() {
            Customer.GenerateCustomer(),
            Customer.GenerateCustomer(),
            Customer.GenerateCustomer(),
            Customer.GenerateCustomer()
        };
    }
}
