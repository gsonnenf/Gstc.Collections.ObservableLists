using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gstc.Utility.UnitTest.Event;

/// <summary>
/// The AssertNotifyProperty class provides a test tool for determining the number of times NotifyPropertyChanged was
/// called in total and for each individual property. It also provides an interface for binding callbacks to properties
/// and reports if callbacks were invoked.
/// </summary>
//TODO - Feature: Add a AddCallback feature for the more general OnPropertyChanged event.
public class AssertNotifyProperty : IDisposable {

    #region Fields and Properties
    public ErrorLog ErrorLog { get; protected set; } = new();
    public string ErrorMessages => ErrorLog.ErrorMessages();
    public INotifyPropertyChanged Observable { get; protected set; }
    public ConcurrentDictionary<string, CountAndCallbackList> PropertyDictionary { get; protected set; } = new();
    public int TimesCalled { get; protected set; } = 0;

    /// <summary>
    /// Specifies if count and callback invoked flags reset on assert. Default is true.
    /// </summary>
    public bool IsResetCountOnAssert { get; set; } = true;
    #endregion

    #region ctor
    public AssertNotifyProperty(INotifyPropertyChanged observable) {
        Observable = observable;
        observable.PropertyChanged += PropertyChangedHandler;
    }
    #endregion

    public void Dispose() {
        Observable.PropertyChanged -= PropertyChangedHandler;
        PropertyDictionary.Clear();
        PropertyDictionary = null;
        Observable = null;
    }

    #region Arrange
    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="invokeOrder"></param>
    /// <param name="description"></param>
    /// <param name="callback"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddCallback(string propertyName, int invokeOrder, string description, Action callback) {
        if (callback == null) throw new ArgumentNullException(nameof(callback), "A callback must be provided. If you only wish to assert call count, addCallback is not necessary.");
        var pair = PropertyDictionary.GetOrAdd(propertyName, new CountAndCallbackList(propertyName));
        pair.CallbackList.Add(new AssertCallback(propertyName, invokeOrder, description, callback));
    }
    //Overloads
    public void AddCallback(string propertyName, Action callback)
        => AddCallback(propertyName, -1, null, callback);
    public void AddCallback(string propertyName, int invokeOrder, Action callback)
        => AddCallback(propertyName, invokeOrder, null, callback);
    public void AddCallback(string propertyName, string description, Action callback)
        => AddCallback(propertyName, -1, description, callback);
    //TODO - Feature: Callbacks should probably have the same signature as the OnPropertyChanged event. Update this in a new version.

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void PropertyChangedHandler(object sender, PropertyChangedEventArgs e) {
        TimesCalled++;
        PropertyDictionary.AddOrUpdate(
            key: e.PropertyName,
            addValue: new CountAndCallbackList(e.PropertyName, 1),

            updateValueFactory: (_, oldValue) => {
                oldValue.CallbackList.ForEach(item => item.Invoke(oldValue.TimesCalled));
                oldValue.TimesCalled += 1;
                return oldValue;
            });
    }
    #endregion

    #region Assert
    /// <summary>
    /// Tests the onChanged event was triggered the correct number of times for a specific property. Resets time called after test.
    /// </summary>
    /// <param name="expectedTimesCalled"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public bool TestPropertyCalled(int expectedTimesCalled, string propertyName) {
        var timesCalled = TimesPropertyCalled(propertyName);
        if (timesCalled != expectedTimesCalled) ErrorLog += "PropertyChanged:" + propertyName +
                                                            " was called " + timesCalled +
                                                            " times, but " + expectedTimesCalled + " calls were expected.";
        if (IsResetCountOnAssert) ResetTimesPropertyCalled(propertyName);
        return ErrorLog.IsSuccess();
    }

    /// <summary>
    /// Tests that all callbacks assigned to a property were invoked.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public bool TestCallbacksInvoked(string propertyName) {
        if (!PropertyDictionary.TryGetValue(propertyName, out var countAndCallbackList)) {
            ErrorLog += "Callback for Property: " + propertyName + " does not exist.\n ";
            return ErrorLog.IsSuccess();
        }
        foreach (var item in countAndCallbackList.CallbackList) {
            if (item.IsInvoked == false) ErrorLog += "A callback was expected to be invoked but was not invoked." +
                                                     "\nProperty Name: " + propertyName +
                                                     "\nCallback Description:" + item.Description +
                                                     "\nCallback IsInvoked: " + item.IsInvoked +
                                                     "\n" + item.CallInfo() + "\n";
            if (IsResetCountOnAssert) item.ResetIsInvoked();
        }
        return ErrorLog.IsSuccess();
    }

    /// <summary>
    /// Tests the onChanged event was triggered the correct number of times for a specific property
    /// and that all callbacks assigned to a property were invoked.
    /// </summary>
    /// <param name="expectedTimesCalled"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public bool TestPropertyAll(int expectedTimesCalled, string propertyName) {
        TestCallbacksInvoked(propertyName);
        TestPropertyCalled(expectedTimesCalled, propertyName);
        return ErrorLog.IsSuccess();
    }

    /// <summary>
    /// Tests the onChanged event was triggered the correct number of times. Resets TimesCalled after test.
    /// </summary>
    /// <param name="expectedTimesCalled"></param>
    /// <returns></returns>
    public bool TestOnChangedTimesCalled(int expectedTimesCalled) {

        if (TimesCalled != expectedTimesCalled) ErrorLog += "PropertyChanged event was called " + TimesCalled +
                                                            " times, but " + expectedTimesCalled + " calls were expected.";
        if (IsResetCountOnAssert) TimesCalled = 0;
        return ErrorLog.IsSuccess();
    }

    /// <summary>
    /// Tests that all assigned callbacks were invoked.
    /// </summary>
    /// <returns></returns>
    public bool TestAllCallbacksInvoked() {
        foreach (var kvp in PropertyDictionary) {
            foreach (var item in kvp.Value.CallbackList) {
                if (item.IsInvoked == false) ErrorLog += "A callback was expected to be invoked but was not invoked." +
                                                         "\nCallback Description:" + item.Description +
                                                         "\nCallback IsInvoked: " + item.IsInvoked +
                                                         "\n" + item.CallInfo() + "\n";
                if (IsResetCountOnAssert) item.ResetIsInvoked();
            }
        }
        return ErrorLog.IsSuccess();
    }

    public bool TestOnChangedAll(int expectedTimesCalled) {
        TestOnChangedTimesCalled(expectedTimesCalled);
        TestAllCallbacksInvoked();
        return ErrorLog.IsSuccess();
    }
    #endregion

    #region Utility
    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public int TimesPropertyCalled(string propertyName)
        => PropertyDictionary.TryGetValue(propertyName, out var itemSet) ? itemSet.TimesCalled : 0;

    protected void ResetTimesPropertyCalled(string propertyName) {
        if (PropertyDictionary.TryGetValue(propertyName, out var itemSet)) itemSet.TimesCalled = 0;
    }

    public class CountAndCallbackList {
        public string PropertyName { get; set; }
        public int TimesCalled { get; set; }
        public List<AssertCallback> CallbackList { get; set; } = new();
        public CountAndCallbackList(string propertyName, int timesCalled = 0) {
            PropertyName = propertyName;
            TimesCalled = timesCalled;
        }
    }
    #endregion

    /// <summary>
    /// Contains status of a callback that are used to run asserts on event arguments. 
    /// </summary>
    public class AssertCallback {
        public readonly Action Callback;
        public readonly int InvokeOrder;
        public readonly string Description;
        public readonly string PropertyName;
        public bool IsInvoked = false;

        public AssertCallback(string propertyName, int invokeOrder, string description, Action callback) {
            PropertyName = propertyName;
            Callback = callback;
            InvokeOrder = invokeOrder;
            Description = Description = description ?? "No description provided.";
        }

        public string CallInfo() =>
            "Callback Order: " +
            ((InvokeOrder != -1) ? "Callback on invocation number " + InvokeOrder : "Every invocation") +
            "\nCallback:" + Callback;

        public void Invoke(int callOrder) {
            if (InvokeOrder != -1 && InvokeOrder != callOrder) return;
            Callback?.Invoke();
            IsInvoked = true;
        }

        public void ResetIsInvoked() => IsInvoked = false;
    }

}
