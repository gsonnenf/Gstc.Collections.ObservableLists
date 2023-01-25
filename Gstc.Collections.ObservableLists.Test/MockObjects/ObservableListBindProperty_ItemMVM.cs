using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
internal class ObservableListBindProperty_ItemMVM : ObservableListBindProperty<ItemModel, ItemViewModel> {

    public static ItemViewModel ConvertItemMToVM(ItemModel itemM) => new(itemM);
    public static ItemModel ConvertItemVMToM(ItemViewModel itemVM) => itemVM.ItemM;

    public ObservableListBindProperty_ItemMVM(
        IObservableList<ItemModel> obvListA,
        IObservableList<ItemViewModel> obvListB,
        PropertyBindType bindType,
        bool isBidirectional = true,
        bool isPropertyBindEnabled = true)
        : base(obvListA, obvListB, bindType, isBidirectional, isPropertyBindEnabled) { }

    public override ItemViewModel ConvertItem(ItemModel itemM) => ConvertItemMToVM(itemM);
    public override ItemModel ConvertItem(ItemViewModel itemVM) => ConvertItemVMToM(itemVM);
}
