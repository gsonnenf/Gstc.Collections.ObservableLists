using System;
using System.Text.RegularExpressions;
using Gstc.Collections.ObservableLists.Binding;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;

[TestFixture]
public class GitHubExampleObservableListBind {

    [Test]
    public void Example() {
        ObservableList<PhoneViewModel> obvPhoneListVM = new(); // Empty list
        ObservableList<PhoneModel> obvPhoneListM = new() { // Our example list with initial data
            new() { PhoneNumber = 5551112222 },
            new() { PhoneNumber = 5553334444 },
        };

        //Creates a binding between the two lists so they will have same content in converted form.
        ObservableListBindPhone obvBindPhone = new(
            obvListPhoneModel: obvPhoneListM,
            obvListPhoneViewModel: obvPhoneListVM,
            isBidirectional: true
            );

        foreach (var item in obvPhoneListM) Console.WriteLine(item.PhoneNumber);
        foreach (var item in obvPhoneListVM) Console.WriteLine(item.PhoneString);
        /// Output:
        /// 5551112222
        /// 5553334444
        /// 555-111-2222
        /// 555-333-4444

        obvPhoneListVM.Clear();
        Console.WriteLine(obvPhoneListM.Count);
        Console.WriteLine(obvPhoneListVM.Count);
        /// Output:
        /// 0
        /// 0

        obvPhoneListM.Add(new() { PhoneNumber = 9876543210 });
        obvPhoneListVM.Add(new() { PhoneString = "123-456-7890" });
        foreach (var item in obvPhoneListM) Console.WriteLine(item.PhoneNumber);
        foreach (var item in obvPhoneListVM) Console.WriteLine(item.PhoneString);
        /// Output:
        /// 9876543210
        /// 1234567890
        /// 987-654-3210
        /// 123-456-7890

        /// ObservableListBindFunc can be used as alternate to inheriting an abstract class ObservableListBind by passing 
        /// conversion functions in the constructor.
        IObservableListBind<PhoneModel, PhoneViewModel> obvBindPhoneFunc
            = new ObservableListBindFunc<PhoneModel, PhoneViewModel>(
                convertItemAToItemB: (item) => new PhoneViewModel() { PhoneString = item.PhoneNumber.ToString("###-###-####") },
                convertItemBtoItemA: (item) => new PhoneModel() { PhoneNumber = long.Parse(Regex.Replace(item.PhoneString, "[^0-9]", "")) },
                obvListA: new() { new PhoneModel() { PhoneNumber = 1112223333 } },
                obvListB: new(),
                isBidirectional: false,
                sourceList: ListIdentifier.ListA
            );

        foreach (var item in obvBindPhoneFunc.ObservableListA) Console.WriteLine(item.PhoneNumber);
        foreach (var item in obvBindPhoneFunc.ObservableListB) Console.WriteLine(item.PhoneString);
        /// Output:
        /// 1112223333
        /// 111-222-3333
    }

    public class ObservableListBindPhone : ObservableListBind<PhoneModel, PhoneViewModel> {
        public ObservableListBindPhone(
            IObservableList<PhoneModel> obvListPhoneModel,
            IObservableList<PhoneViewModel> obvListPhoneViewModel,
            bool isBidirectional)
             : base(obvListPhoneModel, obvListPhoneViewModel, isBidirectional, ListIdentifier.ListA) { }

        public override PhoneViewModel ConvertItem(PhoneModel item)
            => new() { PhoneString = item.PhoneNumber.ToString("###-###-####") };
        public override PhoneModel ConvertItem(PhoneViewModel item) =>
            new() { PhoneNumber = long.Parse(Regex.Replace(item.PhoneString, "[^0-9]", "")) };
    }

    public class PhoneModel {
        public long PhoneNumber { get; set; }
    }

    public class PhoneViewModel {
        public string PhoneString { get; set; }
    }
}
