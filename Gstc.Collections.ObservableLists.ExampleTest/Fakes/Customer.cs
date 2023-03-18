using System;
using System.Collections.Generic;
using System.Linq;

namespace Gstc.Collections.ObservableLists.ExampleTest.Fakes {
    public class Customer {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public double PurchaseAmount { get; set; }
        public string Id { get; set; }

        private static Random RandomGenerator { get; } = new Random();

        private static int _idCounter = 0;

        private static string GenerateId() => "id_" + _idCounter++;

        public static Customer GenerateCustomer() => new Customer() {
            FirstName = FirstNameList[RandomGenerator.Next(10)],
            LastName = LastNameList[RandomGenerator.Next(10)],
            BirthDate = DateTime.Now.AddDays(-1 * RandomGenerator.Next(30000) - 3000),
            PurchaseAmount = 1 + RandomGenerator.Next(10000) * 0.01,
            Id = GenerateId()
        };

        public static List<Customer> GenerateCustomerList() => new List<Customer>() {
                GenerateCustomer(),
                GenerateCustomer(),
                GenerateCustomer(),
                GenerateCustomer(),
                GenerateCustomer(),
            };

        public static List<Customer> GenerateCustomerList(int numOfItems)
            => Enumerable.Range(0, numOfItems).Select(x => GenerateCustomer()).ToList();

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
