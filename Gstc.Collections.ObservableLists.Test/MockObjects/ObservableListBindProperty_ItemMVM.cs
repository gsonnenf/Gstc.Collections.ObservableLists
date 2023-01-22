using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
internal class ObservableListBindProperty_ItemMVM : ObservableListBindProperty<ItemModel, ItemViewModel> {

    public ObservableListBindProperty_ItemMVM(
        IObservableList<ItemModel> obvListA,
        IObservableList<ItemViewModel> obvListB,
        bool isBidirectional = true,
        bool isPropertyBindEnabled = true,
        PropertyBindType bindType = PropertyBindType.UpdateCollectionNotify)
        : base(obvListA, obvListB, bindType, isBidirectional, isPropertyBindEnabled) {
    }

    public override ItemViewModel ConvertItem(ItemModel itemM) => new(itemM);
    public override ItemModel ConvertItem(ItemViewModel itemVM) => itemVM.ItemM;
}
