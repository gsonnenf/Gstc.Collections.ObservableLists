using System;

namespace Gstc.Collections.ObservableLists.Examples {
    public class CustomerViewModel {

        public CustomerViewModel(Customer customer) {  Customer = customer; }

        public Customer Customer { get; }

        public string Name => Customer.FirstName + " " + Customer.LastName;
        public int Age => (int)((DateTime.Now - Customer.BirthDate).TotalDays/365);
        public string Amount => Customer.PurchaseAmount + " Dollars";
    }
}
