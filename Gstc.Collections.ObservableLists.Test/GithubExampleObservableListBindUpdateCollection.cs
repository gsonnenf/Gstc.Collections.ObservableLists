using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Gstc.Collections.ObservableLists.Binding;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;
[TestFixture]
public class GithubExampleObservableListBindUpdateCollection {

    [Test]
    public void Example() {
        ObservableList<PhoneViewModel> obvPhoneListVM = new(); // Empty list
        ObservableList<PhoneModel> obvPhoneListM = new() { // Our example list with initial data
            new() { PhoneNumber = 5551112222 },
            new() { PhoneNumber = 5553334444 },
        };
        IObservableListBind<PhoneModel, PhoneViewModel> obvListBind
            = new ObservableListBindPropertyUpdateCollection(obvPhoneListM, obvPhoneListVM);

        obvPhoneListM[0].PhoneNumber = 1112223333;
        Console.WriteLine(obvPhoneListM[0].PhoneNumber);
        Console.WriteLine(obvPhoneListVM[0].PhoneString);
        /// Output:
        /// 1112223333
        /// 111-222-3333

        obvPhoneListVM[0].PhoneString = "999-888-7777";
        Console.WriteLine(obvPhoneListM[0].PhoneNumber);
        Console.WriteLine(obvPhoneListVM[0].PhoneString);
        /// Output:
        /// 9998887777
        /// 999-888-7777
    }

    public class ObservableListBindPropertyUpdateCollection : ObservableListBindProperty<PhoneModel, PhoneViewModel> {
        public ObservableListBindPropertyUpdateCollection(
            IObservableList<PhoneModel> obvListA,
            IObservableList<PhoneViewModel> obvListB)
            : base(obvListA, obvListB, PropertyBindType.UpdateCollectionNotify, true, true) { }

        public override PhoneViewModel ConvertItem(PhoneModel item)
          => new() { PhoneString = item.PhoneNumber.ToString("###-###-####") };
        public override PhoneModel ConvertItem(PhoneViewModel item)
            => new() { PhoneNumber = long.Parse(Regex.Replace(item.PhoneString, "[^0-9]", "")) };
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
