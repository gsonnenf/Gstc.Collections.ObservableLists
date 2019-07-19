using System.Windows;
using System.Windows.Controls;

namespace Gstc.Collections.ObservableLists.Examples {
    /// <summary>
    /// Interaction logic for ObservableListSyncControl.xaml
    /// </summary>
    public partial class ObservableListSyncControl : UserControl {

        /// <summary>
        /// The syncronized observable list for viewmodel.
        /// </summary>       
        private ObservableListSync<TestModel, TestViewModel> ObvListSync =
           new ObservableListSyncFunc<TestModel, TestViewModel>(
               (sourceItem) => new TestViewModel(sourceItem),
               (destItem) => destItem.TestModel
           );

        /// <summary>
        /// The source list to insert into the syrnchonized observable list.
        /// </summary>
        public ObservableList<TestModel> SourceObvList {
            get => ObvListSync.SourceObservableList;
            set {
                ObvListSync.SourceObservableList = value;
                SourceGrid.ItemsSource = value;
                DestGrid.ItemsSource = ObvListSync;
            }
        }
  
        public ObservableListSyncControl() {
            InitializeComponent();
            //Initializes example data
            ObservableList<TestModel> obvList = new ObservableList<TestModel>() {
                new TestModel {Num1=1, Num2 = 2},
                new TestModel {Num1=10, Num2 = 20},
                new TestModel {Num1=100, Num2 = 200},
            };

            SourceObvList = obvList;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            SourceObvList[0].Num1 += 100;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            ObvListSync[0].Num2 += 40;
        }
   
    }
}
