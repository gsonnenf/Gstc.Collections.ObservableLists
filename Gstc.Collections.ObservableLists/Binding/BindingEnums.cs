namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// Identifies between ListA and ListB in the ObservableListBind class. This is primarily used to
/// set the source list in ObservableListBind, the other list becoming the target list.
/// </summary>
public enum ListIdentifier {
    ListA,
    ListB
}

/// <summary>
/// Used to set the property bind type in a ObservableListBindProperty class and its internal PropertyBinder class.
/// <br/><br/>UpdateCollectionNotify - <inheritdoc cref="PropertyBindType.UpdateCollectionNotify"/>
/// <br/><br/>UpdatePropertyNotify - <inheritdoc cref="PropertyBindType.UpdatePropertyNotify"/>
/// <br/><br/>UpdateCustomNotify - <inheritdoc cref="PropertyBindType.UpdateCustomNotify"/>
/// </summary>
public enum PropertyBindType {
    /// <summary>
    ///  When the PropertyChanged event is raised on a list item, the corresponding item on the alternate list will be replaced by a newly created updated item created using the ConvertItem(...) method.
    /// </summary>
    UpdateCollectionNotify,
    /// <summary>
    /// When the PropertyChanged event is raised on a list item, the corresponding item on the alternate list will have its PropertyChanged event triggered. This bind type does not modify the property and just raises a signal.
    /// </summary>
    UpdatePropertyNotify,
    /// <summary>
    /// When the PropertyChanged event is raised on a list item, the corresponding item on the alternate list can be changed using user implemented methods implemented in a user provided ICustomPropertyMap.
    /// </summary>
    UpdateCustomNotify
}
