using System.Windows;
using System.Windows.Controls;
using Gstc.Collections.ObservableLists.Synchronizer;

namespace Gstc.Collections.ObservableLists.Examples.ObservableListSync {
    /// <summary>
    /// Interaction logic for ObservableListSyncControl.xaml
    /// </summary>
    public partial class ObservableListSyncControl : UserControl {

        /// <summary>
        /// The synchronized observable list for viewmodel.
        /// </summary>       
        private readonly ObservableListSynchronizer<TestModel, TestViewModel> _obvListSync =
           new ObservableListSynchronizerFunc<TestModel, TestViewModel>(
               (sourceItem) => new TestViewModel(sourceItem),
               (destItem) => destItem.TestModel
           );

        public ObservableList<TestModel> SourceObvList = new ObservableList<TestModel>();
        public ObservableList<TestViewModel> DestObvList = new ObservableList<TestViewModel>();

        public ObservableListSyncControl() {
            InitializeComponent();
            //Initializes example data

            _obvListSync.SourceObservableList = SourceObvList;
            _obvListSync.DestinationObservableList = DestObvList;

            SourceGrid.ItemsSource = SourceObvList;
            DestGrid.ItemsSource = DestObvList;

            SourceObvList.Add(new TestModel { Num1 = 1, Num2 = 2 });
            SourceObvList.Add(new TestModel { Num1 = 10, Num2 = 20 });
            SourceObvList.Add(new TestModel { Num1 = 100, Num2 = 200 });

            /*
            ObservableList<TestModel> obvList = new ObservableList<TestModel>() {
                new TestModel {Num1=1, Num2 = 2},
                new TestModel {Num1=10, Num2 = 20},
                new TestModel {Num1=100, Num2 = 200},
            };
            SourceObvList = obvList;
            */
        }

        private void Button_Click(object sender, RoutedEventArgs e) => SourceObvList[0].Num1 += 100;

        private void Button_Click_1(object sender, RoutedEventArgs e) => DestObvList[0].Num2 += 40;
    }
}
