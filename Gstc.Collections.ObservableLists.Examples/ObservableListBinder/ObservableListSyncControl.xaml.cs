using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {

    //todo: Add a button to select bidirectionalty
    //todo: add an exception popup for exception when unidirectionality is set
    public partial class ObservableListBinderControl : UserControl {

        private Random _rand = new Random();

        private ObservableListBind<IItemA, IItemB> _obvListBind_Normal;
        private ObservableListBindProperty<IItemA, IItemB> _obvListBind_ReplaceCollection;
        private ObservableListBindProperty<IItemA, IItemB> _obvListBind_NotifyProperty;
        private ObservableListBindProperty<IItemA, IItemB> _obvListBind_CustomMap;
        private IObservableListBind<IItemA, IItemB> _currentListBind;

        public IObservableList<IItemA> ListA => CurrentListBind.ObservableListA;
        public IObservableList<IItemB> ListB => CurrentListBind.ObservableListB;

        public IObservableListBind<IItemA, IItemB> CurrentListBind {
            get => _currentListBind;
            set {
                _currentListBind = value;
                GridA.ItemsSource = ListA;
                GridB.ItemsSource = ListB;
            }
        }

        public Dictionary<string, IObservableListBind<IItemA, IItemB>> ComboBoxDictionary;

        public ObservableListBinderControl() {
            InitializeComponent();

            InitObservableListBind();
            InitListBindNotifyCollection();
            InitListBindNotifyProperty();
            InitListBindCustomMap();

            ComboBoxDictionary = new Dictionary<string, IObservableListBind<IItemA, IItemB>>() {
                { "ObservableListBind", _obvListBind_Normal},
                { "ObservableListBindProperty_NotifyCollection",_obvListBind_ReplaceCollection },
                { "ObservableListBindProperty_NotifyProperty",_obvListBind_NotifyProperty },
                { "ObservableListBindProperty_CustomMap", _obvListBind_CustomMap },
            };
            BindTypeComboBox.DisplayMemberPath = "Key";
            BindTypeComboBox.SelectedValuePath = "Value";
            BindTypeComboBox.ItemsSource = ComboBoxDictionary;
            BindTypeComboBox.SelectedIndex = 1;

            CurrentListBind = (IObservableListBind<IItemA, IItemB>)BindTypeComboBox.SelectedValue;
        }

        #region Init
        private void InitObservableListBind() {
            _obvListBind_Normal = new ObservableListBindFunc<IItemA, IItemB>(
                convertItemAToB: (itemA) => new ItemB("Bind_ItemB") {
                    Num1String = MapItem.ConvertNum1(itemA.Num1),
                    Num2 = MapItem.ConvertNum2(itemA.Num2)
                },
                convertItemBToA: (itemB) => new ItemA("Bind_ItemA") {
                    Num1 = MapItem.ConvertNum1(itemB.Num1String),
                    Num2 = MapItem.ConvertNum2(itemB.Num2)
                },
                new ObservableList<IItemA>(),
                new ObservableList<IItemB>()
                );

            _obvListBind_Normal.ObservableListA.AddRange(new[] {
                    new ItemA("Bind_ItemA") { Num1 = 1, Num2 = 2 },
                    new ItemA("Bind_ItemA") { Num1 = 10, Num2 = 20 },
                    new ItemA("Bind_ItemA") { Num1 = 100, Num2 = 200 }
            });
        }

        private void InitListBindNotifyCollection() {
            _obvListBind_ReplaceCollection = new ObservableListBindPropertyFunc<IItemA, IItemB>(
                convertItemAToB: (itemA) => new ItemB("NotifyCollection_ItemB") {
                    Num1String = MapItem.ConvertNum1(itemA.Num1),
                    Num2 = MapItem.ConvertNum2(itemA.Num2)
                },
                convertItemBToA: (itemB) => new ItemA("NotifyCollection_ItemA") {
                    Num1 = MapItem.ConvertNum1(itemB.Num1String),
                    Num2 = MapItem.ConvertNum2(itemB.Num2)
                },
                bindType: PropertyBindType.UpdateCollectionNotify,
                new ObservableList<IItemA>(),
                new ObservableList<IItemB>()
           );

            _obvListBind_ReplaceCollection.ObservableListA.AddRange(new[] {
                    new ItemA("NotifyCollection_ItemA") { Num1 = 1, Num2 = 2 },
                    new ItemA("NotifyCollection_ItemA") { Num1 = 10, Num2 = 20 },
                    new ItemA("NotifyCollection_ItemA") { Num1 = 100, Num2 = 200 }
            });
        }

        private void InitListBindCustomMap() {
            _obvListBind_CustomMap = new ObservableListBindPropertyFunc<IItemA, IItemB>(
                convertItemAToB: (itemA) => new ItemB("CustomMap_ItemB") {
                    Num1String = MapItem.ConvertNum1(itemA.Num1),
                    Num2 = MapItem.ConvertNum2(itemA.Num2)
                },
                convertItemBToA: (itemB) => new ItemA("CustomMap_ItemA") {
                    Num1 = MapItem.ConvertNum1(itemB.Num1String),
                    Num2 = MapItem.ConvertNum2(itemB.Num2)
                },
                customPropertyMap: new MapItem(),
                new ObservableList<IItemA>(),
                new ObservableList<IItemB>()
            );

            _obvListBind_CustomMap.ObservableListA.AddRange(new[] {
                    new ItemA("CustomMap_ItemA") { Num1 = 1, Num2 = 2 },
                    new ItemA("CustomMap_ItemA") { Num1 = 10, Num2 = 20 },
                    new ItemA("CustomMap_ItemA") { Num1 = 100, Num2 = 200 }
            });
        }

        private void InitListBindNotifyProperty() {
            _obvListBind_NotifyProperty = new ObservableListBindPropertyFunc<IItemA, IItemB>(
                convertItemAToB: (itemM) => new ItemVM() {
                    Num1String = MapItem.ConvertNum1(itemM.Num1),
                    Num2 = MapItem.ConvertNum2(itemM.Num2)
                },
                convertItemBToA: (itemVM) => new ItemM() {
                    Num1 = MapItem.ConvertNum1(itemVM.Num1String),
                    Num2 = MapItem.ConvertNum2(itemVM.Num2)
                },
                bindType: PropertyBindType.UpdateCollectionNotify,
                new ObservableList<IItemA>(),
                new ObservableList<IItemB>()
           );
            _obvListBind_NotifyProperty.ObservableListA.AddRange(new[] {
                    new ItemM { Num1 = 1, Num2 = 2 },
                    new ItemM { Num1 = 10, Num2 = 20 },
                    new ItemM { Num1 = 100, Num2 = 200 }
            });
        }
        #endregion

        #region Events

        #region Events List A
        private void ButtonClick_AddListA(object sender, RoutedEventArgs e) {
            if (CurrentListBind == _obvListBind_NotifyProperty) ListA.Add(new ItemM() { Num1 = _rand.Next(1000), Num2 = _rand.Next(1000) });
            else ListA.Add(new ItemA("Added ItemA") { Num1 = _rand.Next(1000), Num2 = _rand.Next(1000) });
        }

        private void ButtonClick_RemoveListA(object sender, RoutedEventArgs e) => ListA.Remove((IItemA)GridA.SelectedItem);

        private void ButtonClick_MoveUpListA(object sender, RoutedEventArgs e) => Move(ListA, GridA, -1);

        private void ButtonClick_MoveDownListA(object sender, RoutedEventArgs e) => Move(ListA, GridA, 1);

        private void ButtonClick_ClearAllListA(object sender, RoutedEventArgs e) => ListA.Clear();
        #endregion

        #region Events List B

        private void ButtonClick_AddListB(object sender, RoutedEventArgs e) {
            if (CurrentListBind == _obvListBind_NotifyProperty) ListB.Add(new ItemVM() { Num1String = _rand.Next(1000).ToString(), Num2 = _rand.Next(1000) });
            else ListB.Add(new ItemB("Added ItemB") { Num1String = _rand.Next(1000).ToString(), Num2 = _rand.Next(1000) });
        }

        private void ButtonClick_RemoveListB(object sender, RoutedEventArgs e) => ListB.Remove((IItemB)GridB.SelectedItem);

        private void ButtonClick_MoveUpListB(object sender, RoutedEventArgs e) => Move(ListB, GridB, -1);

        private void ButtonClick_MoveDownListB(object sender, RoutedEventArgs e) => Move(ListB, GridB, 1);

        private void ButtonClick_ClearAllListB(object sender, RoutedEventArgs e) => ListB.Clear();
        #endregion

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            CurrentListBind = (IObservableListBind<IItemA, IItemB>)BindTypeComboBox.SelectedValue;

        private void Move<TItem>(IObservableList<TItem> list, DataGrid grid, int moveDirection) {
            var item = (TItem)grid.SelectedItem;
            var index = list.IndexOf(item);
            var newIndex = index + moveDirection;
            if (index == -1 || newIndex < 0 || newIndex >= list.Count) {
                _ = MessageBox.Show("Index out of range.");
                return;
            }
            list.Move(index, newIndex);
            grid.SelectedIndex = newIndex;
        }
        #endregion
    }
}
