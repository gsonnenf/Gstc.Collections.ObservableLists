using System.Collections.Specialized;
using System.ComponentModel;
using AutoFixture;
using Gstc.Collections.ObservableLists.Interface;
using Gstc.Utility.UnitTest.Event;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test.Tools;

public class CollectionTestBase<TItem> {

    #region Test Assets
    protected static Fixture Fixture { get; } = new();
    protected AssertEvent<NotifyCollectionChangedEventArgs> CollectionTest { get; set; }
    protected AssertNotifyProperty PropertyTest { get; private set; }

    protected AssertEventArgs<TItem> AssertArgs { get; }= new();

    protected TItem DefaultTestItem { get; set; }
    protected TItem UpdateTestItem { get; set; }

    protected TItem Item1 { get; set; }
    protected TItem Item2 { get; set; }
    protected TItem Item3 { get; set; }

    #endregion

    #region ctor

    protected void TestInit() {

        //Generalize mock data
        DefaultTestItem = Fixture.Create<TItem>();
        UpdateTestItem = Fixture.Create<TItem>();

        Item1 = Fixture.Create<TItem>();
        Item2 = Fixture.Create<TItem>();
        Item3 = Fixture.Create<TItem>();
    }
    #endregion

    #region Event tools

    /// <summary>
    /// Initializes watchers on the CollectionChanged and PropertyChanged events of the observable object.
    /// The number of times called is tracked. Callbacks can also be assigned to the events which can assert
    /// on the NotifyCollectionChangedEvent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obvList"></param>
    /// <param name="eventHandler"></param>
    protected void InitPropertyCollectionTest<T>(T obvList, NotifyCollectionChangedEventHandler eventHandler = null) where T : INotifyPropertyChanged, INotifyCollectionChanged {
        InitPropertyTest_Collection(obvList);
        InitCollectionTest(obvList, eventHandler);
    }

    protected void InitCollectionTest(INotifyCollectionChanged obvList, NotifyCollectionChangedEventHandler eventHandler) {
        CollectionTest = new AssertEvent<NotifyCollectionChangedEventArgs>(obvList, nameof(obvList.CollectionChanged));
        if (eventHandler != null) CollectionTest.AddCallback(
            description: "Provides a callback to test collection changed events.",
            callback: eventHandler.Invoke
        );
    }

    protected void InitPropertyTest_Collection(INotifyPropertyChanged obvList) => PropertyTest = new AssertNotifyProperty(obvList);


    /// <summary>
    /// Asserts that the callbacks have been triggered/invoked, all counters and callback triggers are reset.
    /// </summary>
    /// <param name="timesItemCalled"></param>
    /// <param name="timesCountCalled"></param>
    /// <param name="timesCollectionChangedCalled"></param>
    protected void AssertPropertyCollectionTest(int timesItemCalled = 1, int timesCountCalled = 1, int timesCollectionChangedCalled = 1) {
        AssertPropertyTestForCollection(timesItemCalled, timesCountCalled);
        AssertCollectionTest(timesCollectionChangedCalled);
    }

    protected void AssertPropertyTestForCollection(int timesItemCalled, int timesCountCalled) {
        Assert.True(PropertyTest.TestPropertyCalled(timesItemCalled, "Item[]"), PropertyTest.ErrorMessages);
        Assert.True(PropertyTest.TestPropertyCalled(timesCountCalled, "Count"), PropertyTest.ErrorMessages);
        Assert.True(PropertyTest.TestOnChangedAll(timesItemCalled + timesCountCalled), PropertyTest.ErrorMessages);
    }

    protected void AssertCollectionTest(int expectedTimesCalled)
        => Assert.True(CollectionTest.TestAll(expectedTimesCalled), CollectionTest.ErrorMessages);
    #endregion


    #region Event Utilities      
    public void AddNotifiers<T>(IObservableList<T> obvList) {
        //Sets up event testers
        //obvList.Adding += (sender, args) => AssertEvent.Call(nameof(obvList.Resetting));
        //obvList.Moving += (sender, args) => AssertEvent.Call(nameof(obvList.Resetting));
        //obvList.Removing += (sender, args) => AssertEvent.Call(nameof(obvList.Resetting));
        //obvList.Replacing += (sender, args) => AssertEvent.Call(nameof(obvList.Resetting));
        //obvList.Resetting += (sender, args) => AssertEvent.Call(nameof(obvList.Resetting));

    }



    #endregion


}
