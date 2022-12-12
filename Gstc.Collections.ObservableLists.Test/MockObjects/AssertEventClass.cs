namespace Gstc.Collections.ObservableLists.Test.MockObjects;

/// <summary>
/// Simple class for testing events using Moq. Moq proxies the Call method and counts number of times called.
/// </summary>
public class AssertEventClass {
    public virtual void Call(string obj) { }
}
