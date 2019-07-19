using Gstc.Collections.ObservableLists;

namespace Gstc.Collections.ObservableLists.Examples {
    //Implementation of abstract class Observable list Adapter.
    public class CustomerViewModelList : ObservableListAdapter<Customer, CustomerViewModel> {
        public override CustomerViewModel Convert(Customer item) => new CustomerViewModel(item);
        public override Customer Convert(CustomerViewModel item) => item.Customer;
    }
}
