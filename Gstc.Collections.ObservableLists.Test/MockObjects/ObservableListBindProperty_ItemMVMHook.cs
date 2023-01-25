using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
internal class ObservableListBindProperty_ItemMVMHook : ObservableListBindProperty<ItemModelHook, ItemViewModelHook> {

    public static ItemViewModelHook ConvertItemMToVM(ItemModelHook itemM) => new(itemM);
    public static ItemModelHook ConvertItemVMToM(ItemViewModelHook itemVM) => itemVM.ItemM;

    public ObservableListBindProperty_ItemMVMHook(
        IObservableList<ItemModelHook> obvListA,
        IObservableList<ItemViewModelHook> obvListB,
        PropertyBindType bindType,
        bool isBidirectional = true,
        bool isPropertyBindEnabled = true
        )
        : base(obvListA, obvListB, bindType, isBidirectional, isPropertyBindEnabled) {
    }

    public override ItemViewModelHook ConvertItem(ItemModelHook itemM) => ConvertItemMToVM(itemM);
    public override ItemModelHook ConvertItem(ItemViewModelHook itemVM) => ConvertItemVMToM(itemVM);
}
