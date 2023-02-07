using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using NUnit.Framework;

namespace Gstc.Utility.UnitTest.Event;

public class AssertEvent {
    //todo: Refactor and integrate into main AssertEvent Package. Change to a generic and test that GetInvocationList exists then call it.
    public static int NumberOfEvents_NotifyCollection(object parent, string eventName) {

        EventInfo eventInfo = parent.GetType().GetEvent(eventName);
        if (!eventInfo.IsMulticast) throw new NotSupportedException("The name does not correspond to a multicast delegate");

        Type targetType = parent.GetType();
        const BindingFlags bindingFlags = BindingFlags.NonPublic |
                        BindingFlags.Static | BindingFlags.Instance;
        FieldInfo? fieldInfo = targetType.GetField(eventName, bindingFlags);


        if (fieldInfo.GetValue(parent) is NotifyCollectionChangedEventHandler eventHandler) return eventHandler.GetInvocationList().Length;
        else return 0;
    }
    public static int NumberOfEvents_NotifyProperty(object parent, string eventName) {

        EventInfo eventInfo = parent.GetType().GetEvent(eventName);
        if (!eventInfo.IsMulticast) throw new NotSupportedException("The name does not correspond to a multicast delegate");

        Type targetType = parent.GetType();
        const BindingFlags bindingFlags = BindingFlags.NonPublic |
                        BindingFlags.Static | BindingFlags.Instance;
        FieldInfo? fieldInfo = targetType.GetField(eventName, bindingFlags);

        if (fieldInfo.GetValue(parent) is PropertyChangedEventHandler eventHandler) return eventHandler.GetInvocationList().Length;
        else return 0;
    }
}

public partial class AssertEvent<TEventArgs> {
    public void AssertAll(int expectedTimesCalled)
        => Assert.That(TestAll(expectedTimesCalled), ErrorMessages);
}
