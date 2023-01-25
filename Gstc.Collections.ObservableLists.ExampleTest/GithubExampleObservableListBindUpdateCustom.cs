using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Gstc.Collections.ObservableLists.Binding;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;

[TestFixture]
public class GithubExampleObservableListBindUpdateCustom {

    [Test]
    public void Example() {
        ObservableList<PhoneViewModel> obvPhoneListVM = new(); // Empty list
        ObservableList<PhoneModel> obvPhoneListM = new() { // Our example list with initial data
            new() { PhoneNumber = 5551112222 },
            new() { PhoneNumber = 5553334444 },
        };
        IObservableListBind<PhoneModel, PhoneViewModel> obvListBind
            = new ObservableListBindPropertyUpdateCustom(obvPhoneListM, obvPhoneListVM);


        obvPhoneListM[0].PhoneNumber = 4445556666;
        Console.WriteLine(obvPhoneListM[0].PhoneNumber);
        Console.WriteLine(obvPhoneListVM[0].PhoneString);
        /// Output:
        /// 4445556666
        /// 444-555-6666

        obvPhoneListVM[0].PhoneString = "100-000-0000";
        Console.WriteLine(obvPhoneListM[0].PhoneNumber);
        Console.WriteLine(obvPhoneListVM[0].PhoneString);
        /// Output:
        /// 1000000000
        /// 100-000-0000
    }
    public class ObservableListBindPropertyUpdateCustom : ObservableListBindProperty<PhoneModel, PhoneViewModel> {

        public ObservableListBindPropertyUpdateCustom(
            IObservableList<PhoneModel> obvListA,
            IObservableList<PhoneViewModel> obvListB)
            : base(obvListA, obvListB, new CustomPropertyMapPhone(), true, true) {
        }

        public override PhoneViewModel ConvertItem(PhoneModel itemM)
          => new() { PhoneString = CustomPropertyMapPhone.ConvertPhoneNumber(itemM) };
        public override PhoneModel ConvertItem(PhoneViewModel itemVM)
            => new() { PhoneNumber = CustomPropertyMapPhone.ConvertPhoneString(itemVM) };

        public class CustomPropertyMapPhone : ICustomPropertyMap<PhoneModel, PhoneViewModel> {
            public static string ConvertPhoneNumber(PhoneModel phoneM) => phoneM.PhoneNumber.ToString("###-###-####");
            public static long ConvertPhoneString(PhoneViewModel phoneVM) => long.Parse(Regex.Replace(phoneVM.PhoneString, "[^0-9]", ""));

            public void PropertyChangedSourceToTarget(PropertyChangedEventArgs args, PhoneModel itemS, PhoneViewModel itemT) {
                if (args.PropertyName == "PhoneNumber") itemT.PhoneString = ConvertPhoneNumber(itemS);
            }
            public void PropertyChangedTargetToSource(PropertyChangedEventArgs args, PhoneViewModel itemT, PhoneModel itemS) {
                if (args.PropertyName == "PhoneString") itemS.PhoneNumber = ConvertPhoneString(itemT);
            }
        }
    }

    public class PhoneModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private long _phoneNumber;

        public long PhoneNumber {
            get => _phoneNumber;
            set {
                _phoneNumber = value;
                PropertyChanged?.Invoke(this, new(nameof(PhoneNumber)));
            }
        }
    }

    public class PhoneViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _phoneString;

        public string PhoneString {
            get => _phoneString;
            set {
                _phoneString = value;
                PropertyChanged?.Invoke(this, new(nameof(PhoneString)));
            }
        }
    }
}
