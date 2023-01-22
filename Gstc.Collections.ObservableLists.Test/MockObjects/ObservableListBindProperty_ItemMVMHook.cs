using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
internal class ObservableListBindProperty_ItemMVMHook : ObservableListBindProperty<ItemModelHook, ItemViewModelHook> {

    public ObservableListBindProperty_ItemMVMHook(
        IObservableList<ItemModelHook> obvListA,
        IObservableList<ItemViewModelHook> obvListB,
        bool isBidirectional = true,
        bool isPropertyBindEnabled = true,
        PropertyBindType bindType = PropertyBindType.UpdateCollectionNotify)
        : base(obvListA, obvListB, bindType, isBidirectional, isPropertyBindEnabled) {
    }

    public override ItemViewModelHook ConvertItem(ItemModelHook itemM) => new(itemM);
    public override ItemModelHook ConvertItem(ItemViewModelHook itemVM) => itemVM.ItemM;
}
