using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
public class ObservableListBindingTestItem : ObservableListBind<ItemASource, ItemBDest> {
    public override ItemBDest ConvertItem(ItemASource item) => throw new System.NotImplementedException();
    public override ItemASource ConvertItem(ItemBDest item) => throw new System.NotImplementedException();
}
