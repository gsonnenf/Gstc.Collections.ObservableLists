using System;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists.Examples.ObservableList {
    public class Customer {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public double PurchaseAmount { get; set; }
        public string Id { get; set; }

        private static Random RandomGenerator { get; } = new Random();

        private static int _idCounter = 0;

        private static string GenerateId() => "id_" + _idCounter++;

        public static Customer GenerateCustomer() {
            return new Customer() {
                FirstName = FirstNameList[RandomGenerator.Next(10)],
                LastName = LastNameList[RandomGenerator.Next(10)],
                BirthDate = DateTime.Now.AddDays(-1 * RandomGenerator.Next(30000) - 3000),
                PurchaseAmount = 1 + RandomGenerator.Next(10000) * 0.01,
                Id = GenerateId()
            };
        }

        public static List<Customer> GenerateCustomerList() {
            return new List<Customer>() {
                Customer.GenerateCustomer(),
                Customer.GenerateCustomer(),
                Customer.GenerateCustomer(),
                Customer.GenerateCustomer(),
                Customer.GenerateCustomer(),
            };
        }

        private readonly static List<string> FirstNameList = new List<string>() {
            "Emma",
            "Sophia",
            "Olivia",
            "Isabella",
            "Ava",
            "Mason",
            "Noah",
            "Lucas",
            "Jacob",
            "Jack"
        };

        private readonly static List<string> LastNameList = new List<string>() {
            "Smith",
            "Trujillo",
            "Jackson",
            "Lee",
            "Mercado",
            "Sonnenfeld",
            "Valdez",
            "Johnson",
            "Parker",
            "Warren"
        };
    }
}
