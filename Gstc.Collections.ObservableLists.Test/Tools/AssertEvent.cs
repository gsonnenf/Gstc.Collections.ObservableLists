using System;
using System.Collections.Generic;
using System.Reflection;


namespace Gstc.Utility.UnitTest.Event;
/// <summary>
/// The AssertNotifyProperty class provides a test tool for determining the number of times an event was
/// called. It also provides an interface for binding callbacks to events and reports if callbacks were invoked.
/// </summary>
/// <typeparam name="TEventArgs">The type of event under test.</typeparam>
public class AssertEvent<TEventArgs> : IDisposable
    where TEventArgs : EventArgs {

    private object _parent;

    private readonly EventInfo _eventInfo;
    private readonly Delegate _delegate;

    #region Fields and Properties
    public ErrorLog ErrorLog { get; protected set; } = new();
    public string ErrorMessages => ErrorLog.ErrorMessages();
    public int TimesCalled { get; protected set; }
    public List<AssertCallback> AssertCallbackList { get; protected set; } = new();

    /// <summary>
    /// Specifies if count and callback invoked flags reset on assert. Default is true.
    /// </summary>
    public bool IsResetCountOnAssert { get; set; } = true;
    #endregion

    #region ctor
    public AssertEvent(object parent, string eventName) {
        _parent = parent;

        //Uses reflection to get event handler type, and a reflection reference to our method.
        var methodInfo = typeof(AssertEvent<TEventArgs>).GetMethod(nameof(EventHandler), BindingFlags.NonPublic | BindingFlags.Instance);
        var eventInfo = parent.GetType().GetEvent(eventName);

        //Casts our method group EventHandler to the proper EventHandler type
        _eventInfo = eventInfo ?? throw new NullReferenceException("Event was not found in the parent object.");
        _delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo!);

        eventInfo.AddEventHandler(parent, _delegate);
    }

    public void Dispose() {
        _eventInfo.RemoveEventHandler(_parent, _delegate);
        AssertCallbackList.Clear();
        AssertCallbackList = null;
        _parent = null;
    }
    #endregion

    #region Arrange
    /// <summary>
    /// Registers a callback to be executed when an event is called.
    /// </summary>
    /// <param name="invokeCallOrder">Callback will invoke once after n number of calls. Call order starts at 0 and -1 will invoke for all calls.</param>
    /// <param name="description">An identifier or description of the failed callback.</param>
    /// <param name="callback">A user provided callback that performs asserts on the {TEventArgs}. </param>
    public void AddCallback(int invokeCallOrder, string description, EventHandler<TEventArgs> callback)
        => AssertCallbackList.Add(new AssertCallback(invokeCallOrder, callback, description));
    //Overloads
    public void AddCallback(EventHandler<TEventArgs> callback)
        => AddCallback(-1, null, callback);
    public void AddCallback(string description, EventHandler<TEventArgs> callback)
        => AddCallback(-1, description, callback);
    public void AddCallback(int invokeCallOrder, EventHandler<TEventArgs> callback)
        => AddCallback(invokeCallOrder, null, callback);

    /// <summary>
    /// The handler to be passed to the event under test. This method is not meant to be manually invoked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected void EventHandler(object sender, TEventArgs args) {
        foreach (var item in AssertCallbackList) item.Invoke(TimesCalled, sender, args);
        TimesCalled++;
    }
    #endregion

    #region Asserts
    /// <summary>
    /// Tests the the number of times the event was called against the expected number of calls.
    /// Resets the number of calls on call.
    /// </summary>
    /// <param name="expectedTimesCalled">Expected number of times called.</param>
    /// <returns>True if number of calls is equal to expected number of calls.</returns>
    public bool TestTimesCalled(int expectedTimesCalled) {
        if (expectedTimesCalled != TimesCalled) ErrorLog += "Event was called " + TimesCalled +
                                                            " times, but " + expectedTimesCalled + " calls were expected.";
        if (IsResetCountOnAssert) TimesCalled = 0;
        return ErrorLog.IsSuccess();
    }

    /// <summary>
    /// Tests that all assigned callbacks were invoked.
    /// </summary>
    /// <returns>True if all callbacks invoked</returns>
    public bool TestAllCallbacksInvoked() {
        for (var index = 0; index < AssertCallbackList.Count; index++) {
            var item = AssertCallbackList[index];
            if (!item.IsInvoked) ErrorLog += "A callback was expected to be invoked but was not invoked." +
                                             "\nCallback Description:" + item.Description +
                                             "\nCallback Number:" + index +
                                             "\nCallback IsInvoked: " + item.IsInvoked +
                                             item.CallInfo() + "\n";
            if (IsResetCountOnAssert) item.ResetIsInvoked();
        }
        return ErrorLog.IsSuccess();
    }

    /// <summary>
    /// Tests for the the expected number of calls and that all callbacks were invoked.
    /// </summary>
    /// <param name="expectedTimesCalled"></param>
    /// <returns></returns>
    public bool TestAll(int expectedTimesCalled) {
        TestTimesCalled(expectedTimesCalled);
        TestAllCallbacksInvoked();
        return ErrorLog.IsSuccess();
    }
    #endregion

    /// <summary>
    /// Contains status of a callback that are used to run asserts on event arguments. 
    /// </summary>
    public class AssertCallback {
        public EventHandler<TEventArgs> Callback;
        public readonly int InvokeOrder;
        public readonly string Description;
        public bool IsInvoked = false;

        public AssertCallback(int invokeOrder, EventHandler<TEventArgs> callback, string description) {
            Callback = callback;
            InvokeOrder = invokeOrder;
            Description = description ?? "No description provided.";
        }

        public string CallInfo() =>
            "\nCallback Order: " +
            ((InvokeOrder != -1) ? "Callback on invocation number " + InvokeOrder : "Every invocation") +
            "\nCallback:" + Callback;

        public void Invoke(int callOrder, object sender, TEventArgs args) {
            if (InvokeOrder != -1 && InvokeOrder != callOrder) return;
            Callback?.Invoke(sender, args);
            IsInvoked = true;
        }

        public void ResetIsInvoked() => IsInvoked = false;
    }

};

