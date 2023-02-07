using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Gstc.Collections.ObservableLists.Binding;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;

[TestFixture]
public class GitHubExampleObservableListBindUpdatePropertyNotify {


    [Test]
    public void Example1() {
        ObservableList<PhoneViewModel> obvPhoneListVM = new(); // Empty list
        ObservableList<PhoneModel> obvPhoneListM = new() { // Our example list with initial data
            new() { PhoneNumber = 1112223333 },
        };

        IObservableListBind<PhoneModel, PhoneViewModel> obvListBind
            = new ObservableListBindPropertyNotifyPhone(obvPhoneListM, obvPhoneListVM);

        obvPhoneListM[0].PropertyChanged += (sender, args) => Console.WriteLine("Model Property Changed event called to notify a refresh:" + args.PropertyName);
        obvPhoneListVM[0].PropertyChanged += (sender, args) => Console.WriteLine("ViewModel Property Changed event called to notify a refresh:" + args.PropertyName);

        obvPhoneListM[0].PhoneNumber = 9998887777;
        /// ViewModel Property Changed event called to notify a refresh:
        /// Model Property Changed event called to notify a refresh:PhoneNumber

        obvPhoneListVM[0].PhoneString = "123-456-7890";
        /// ViewModel Property Changed event called to notify a refresh:
        /// Model Property Changed event called to notify a refresh:PhoneNumber

    }

    public class ObservableListBindPropertyNotifyPhone : ObservableListBindProperty<PhoneModel, PhoneViewModel> {
        public ObservableListBindPropertyNotifyPhone(
            IObservableList<PhoneModel> obvListA,
            IObservableList<PhoneViewModel> obvListB) : base(obvListA, obvListB, PropertyBindType.UpdatePropertyNotify, false, true) {
        }
        public override PhoneViewModel ConvertItem(PhoneModel item) => new() { PhoneModel = item };
        public override PhoneModel ConvertItem(PhoneViewModel item) => item.PhoneModel;
    }

    public class PhoneModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private long _phoneNumber;
        public long PhoneNumber {
            get => _phoneNumber;
            set {
                _phoneNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
            }
        }
    }

    public class PhoneViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public PhoneModel PhoneModel { get; set; } = new();
        public string PhoneString {
            get => PhoneModel.PhoneNumber.ToString("###-###-####");
            set => PhoneModel.PhoneNumber = long.Parse(Regex.Replace(value, "[^0-9]", ""));
        }
    }
}
