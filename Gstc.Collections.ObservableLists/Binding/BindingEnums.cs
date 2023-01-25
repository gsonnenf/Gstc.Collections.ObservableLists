namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// Identifies between ListA and ListB in the ObservableListBind class. This is primarily used to
/// set which list is the source list, the other becoming the target list.
/// </summary>
public enum ListIdentifier {
    ListA,
    ListB
}

/// <summary>
/// Used to set the property bind type in a ObservableListBindProperty class. 
/// <br/><br/>UpdateCollectionNotify: When the PropertyChanged event is raised on a list item, the corresponding item on the alternate list will be replaced by a new updated item created using the CreateItem method.
/// <br/><br/>UpdatePropertyNotify:  When the PropertyChanged event is raised on a list item, the corresponding item on the alternate list will have its PropertyChanged event triggered. This bind type does not modify the property and just raises a signal.
/// <br/><br/>UpdateCustomNotify: When the PropertyChanged event is raised on a list item, the corresponding item on the alternate list can be changed using user implemented methods implemented in a user provided ICustomPropertyMap.
/// </summary>
public enum PropertyBindType {
    UpdateCollectionNotify,
    UpdateCustomNotify,
    UpdatePropertyNotify
}
