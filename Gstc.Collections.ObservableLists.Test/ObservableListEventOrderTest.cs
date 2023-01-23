// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Gstc.Collections.ObservableLists.Multithread;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using Gstc.Collections.ObservableLists.Test.Tools;
using Gstc.Utility.UnitTest.Event;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListEventOrderTest : CollectionTestBase<TestItem> {

    public readonly static object[] ObservableListDataSource = {
        () => new ObservableList<TestItem>(), //A generator is needed. The static data souce is only called once per ValueSource method.
        () => new ObservableIList<TestItem, List<TestItem>>(),
        () => new ObservableIListLocking<TestItem,List<TestItem>>()
    };

    public static List<EventTestSet> StaticOperationListDataSource => EventTestSet.GetEventTestSets();

    /// <summary>
    /// The combinations of these tests are high, so loops and value sources are used to auto-generate tests.
    /// This test runs over different methods called by lists, the different events in the observable lists,
    /// and over the included variations of observable list.
    /// </summary>
    /// <param name="obvListGenerator"></param>
    /// <param name="testSet"></param>
    /// <exception cref="Exception"></exception>
    [Test, NUnit.Framework.Description("Tests that all events are called in the correct order.")]
    public void TestCallOrderOfEvents(
        [ValueSource(nameof(ObservableListDataSource))] Func<IObservableList<TestItem>> obvListGenerator,
        [ValueSource(nameof(StaticOperationListDataSource))] EventTestSet testSet) {

        IObservableList<TestItem> obvList = obvListGenerator();
        Console.WriteLine("Name: " + testSet.Name);
        Console.WriteLine("List: " + obvList.GetType());

        testSet.ArrangeAction(obvList);

        List<object> testEventList = new();
        int callOrder = 0;
        int index = 0;

        foreach (string eventName in testSet.EventOrderList) {
            int staticIndex = index++;
            if (eventName != nameof(IObservableList<TestItem>.PropertyChanged)) {
                AssertEvent<NotifyCollectionChangedEventArgs> testEvent = new(obvList, eventName);
                testEventList.Add(testEvent);
                testEvent.AddCallback((_, _) => Console.WriteLine("Expected: " + staticIndex + ": Call: " + callOrder + " : " + eventName));
                testEvent.AddCallback((_, _) => callOrder = (callOrder == staticIndex) ? callOrder + 1
                    : throw new Exception(testSet.Name + ": Call order of " + eventName + " was not correct. " + staticIndex + " was expected, but " + callOrder + " was received."));
            } else {
                AssertEvent<PropertyChangedEventArgs> testEvent = new(obvList, eventName);
                testEventList.Add(testEvent);
                testEvent.AddCallback((_, args) => Console.WriteLine("Expected: " + staticIndex + ": Call: " + callOrder + " : " + eventName + " : " + args.PropertyName));
                testEvent.AddCallback((_, args) => {
                    if (args.PropertyName == "Count") Assert.That(testSet.IsCountChanged, "OnPropertyChanged: Count is not suppose to be called for method: " + testSet.Name);
                    if (args.PropertyName == "Item[]") callOrder = (callOrder == staticIndex) ? callOrder + 1
                    : throw new Exception(testSet.Name + ": Call order of " + eventName + " was not correct. " + staticIndex + " was expected, but " + callOrder + " was received.");
                });
            }
        }

        testSet.ActAction(obvList);

        foreach (object item in testEventList) {
            if (item is AssertEvent<PropertyChangedEventArgs> testEventProperty) testEventProperty.AssertAll(testSet.IsCountChanged ? 2 : 1);
            else if (item is AssertEvent<CollectionChangeEventArgs> testEventCollection) testEventCollection.AssertAll(1);
        }
    }

    public class EventTestSet {
        public string Name;
        public List<string> EventOrderList;
        public Action<IObservableList<TestItem>> ArrangeAction;
        public Action<IObservableList<TestItem>> ActAction;
        public bool IsCountChanged;

        #region Predefined tests for each list operation.
        public static TestItem StaticTestItem { get; } = new();

        public static List<EventTestSet> GetEventTestSets() => new() {
        new EventTestSet {
            Name = "Add",
            EventOrderList = EventOrderList_Add,
            ArrangeAction = (_) => { },
            ActAction = (obvList) => obvList.Add(StaticTestItem),
            IsCountChanged = true
        },
        new EventTestSet {
            Name = "AddRange",
            EventOrderList = EventOrderList_AddRange,
            ArrangeAction = (_) => { },
            ActAction = (obvList) => obvList.AddRange(new[] {StaticTestItem, StaticTestItem}),
            IsCountChanged = true
        },
        new EventTestSet {
            Name = "Clear",
            EventOrderList = EventOrderList_Clear,
            ArrangeAction = (_) => { },
            ActAction = (obvList) => obvList.Clear(),
            IsCountChanged = true
        },
        new EventTestSet {
            Name = "Index",
            EventOrderList = EventOrderList_Index,
            ArrangeAction = (obvList) => obvList.Add(StaticTestItem),
            ActAction = (obvList) => obvList[0] = StaticTestItem,
            IsCountChanged = false
        },
        new EventTestSet {
            Name = "Insert",
            EventOrderList = EventOrderList_Insert,
            ArrangeAction = (_) => { },
            ActAction = (obvList) => obvList.Insert(0, StaticTestItem),
            IsCountChanged = true
        },
        new EventTestSet {
            Name = "Move",
            EventOrderList = EventOrderList_Move,
            ArrangeAction = (obvList) => obvList.AddRange(new [] {StaticTestItem,StaticTestItem}) ,
            ActAction = (obvList) => obvList.Move(1,0),
            IsCountChanged = false
        },
        //Todo: Add RefreshIndex and RefreshAll

        new EventTestSet {
            Name = "RefreshIndex",
            EventOrderList = EventOrderList_Index,
            ArrangeAction = (obvList) => obvList.Add(StaticTestItem),
            ActAction = (obvList) => obvList.RefreshIndex(0),
            IsCountChanged = false
        },

        new EventTestSet {
            Name = "RefreshAll",
            EventOrderList = EventOrderList_Clear,
            ArrangeAction = (_) => { },
            ActAction = (obvList) => obvList.RefreshAll(),
            IsCountChanged = true
        },

        new EventTestSet {
            Name = "Remove",
            EventOrderList = EventOrderList_Remove,
            ArrangeAction = (obvList) => obvList.Add(StaticTestItem),
            ActAction = (obvList) => obvList.Remove(StaticTestItem),
            IsCountChanged = true
        },
        new EventTestSet {
            Name = "RemoveAt",
            EventOrderList = EventOrderList_RemoveAt,
            ArrangeAction = (obvList) => obvList.Add(StaticTestItem),
            ActAction = (obvList) => obvList.RemoveAt(0),
            IsCountChanged = true
        }
    };
        #endregion

        #region Orders of Events for different list methods

        //TODO: find good way to test obvList.List
        public static List<string> EventOrderList_Add => new() {
        nameof(IObservableList<TestItem>.CollectionChanging),
        nameof(IObservableList<TestItem>.Adding),
        nameof(IObservableList<TestItem>.PropertyChanged),
        nameof(IObservableList<TestItem>.CollectionChanged),
        nameof(IObservableList<TestItem>.Added),
    };

        public static List<string> EventOrderList_AddRange => new() {
        nameof(IObservableList<TestItem>.CollectionChanging),
        nameof(IObservableList<TestItem>.Adding),
        nameof(IObservableList<TestItem>.PropertyChanged),
        nameof(IObservableList<TestItem>.CollectionChanged),
        nameof(IObservableList<TestItem>.Added),
    };

        public static List<string> EventOrderList_Clear => new() {
        nameof(IObservableList<TestItem>.CollectionChanging),
        nameof(IObservableList<TestItem>.Resetting),
        nameof(IObservableList<TestItem>.PropertyChanged),
        nameof(IObservableList<TestItem>.CollectionChanged),
        nameof(IObservableList<TestItem>.Reset),
    };

        public static List<string> EventOrderList_Index => new() {
        nameof(IObservableList<TestItem>.CollectionChanging),
        nameof(IObservableList<TestItem>.Replacing),
        nameof(IObservableList<TestItem>.PropertyChanged),
        nameof(IObservableList<TestItem>.CollectionChanged),
        nameof(IObservableList<TestItem>.Replaced),
    };

        public static List<string> EventOrderList_Insert => new() {
        nameof(IObservableList<TestItem>.CollectionChanging),
        nameof(IObservableList<TestItem>.Adding),
        nameof(IObservableList<TestItem>.PropertyChanged),
        nameof(IObservableList<TestItem>.CollectionChanged),
        nameof(IObservableList<TestItem>.Added),
    };

        public static List<string> EventOrderList_Move => new() {
        nameof(IObservableList<TestItem>.CollectionChanging),
        nameof(IObservableList<TestItem>.Moving),
        nameof(IObservableList<TestItem>.PropertyChanged),
        nameof(IObservableList<TestItem>.CollectionChanged),
        nameof(IObservableList<TestItem>.Moved),
    };

        public static List<string> EventOrderList_Remove => new() {
        nameof(IObservableList<TestItem>.CollectionChanging),
        nameof(IObservableList<TestItem>.Removing),
        nameof(IObservableList<TestItem>.PropertyChanged),
        nameof(IObservableList<TestItem>.CollectionChanged),
        nameof(IObservableList<TestItem>.Removed),
    };

        public static List<string> EventOrderList_RemoveAt => new() {
        nameof(IObservableList<TestItem>.CollectionChanging),
        nameof(IObservableList<TestItem>.Removing),
        nameof(IObservableList<TestItem>.PropertyChanged),
        nameof(IObservableList<TestItem>.CollectionChanged),
        nameof(IObservableList<TestItem>.Removed),
    };
        #endregion
    }

}
