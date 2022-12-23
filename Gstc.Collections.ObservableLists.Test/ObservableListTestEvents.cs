using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Gstc.Collections.ObservableLists.Interface;
using Gstc.Collections.ObservableLists.Multithread;
using Gstc.Collections.ObservableLists.Test.MockObjects;
using Gstc.Collections.ObservableLists.Test.Tools;
using Gstc.Utility.UnitTest.Event;
using NUnit.Framework;
// ReSharper disable InconsistentNaming

namespace Gstc.Collections.ObservableLists.Test;

[TestFixture]
public class ObservableListTestEvents : CollectionTestBase<TestItem> {


    public readonly static TestItem StaticTestItem = new();
    public static List<EventTestSet> StaticOperationListDataSource => new() {
        new EventTestSet {
            Name = "Add",
            EventOrderList = EventTestSet.EventOrderList_Add,
            ArrangeAction = (_) => { },
            ActAction = (obvList) => obvList.Add(StaticTestItem),
            IsCountChanged = true
        },
        //new EventTestSet { //Todo: after addRange is added to interface
        //    Name = "AddRange",
        //    EventOrderList = EventTestSet.EventOrderList_AddRange, 
        //    ArrangeAction = (_) => { },
        //    ActAction = (obvList) => {obvList.AddRange(StaticTestItem, StaticTestItem, StaticTestItem) },
        //    IsCountChanged = true
        //},
        new EventTestSet {
            Name = "Clear",
            EventOrderList = EventTestSet.EventOrderList_Clear,
            ArrangeAction = (_) => { },
            ActAction = (obvList) => obvList.Clear(),
            IsCountChanged = true
        },
        new EventTestSet {
            Name = "Index",
            EventOrderList = EventTestSet.EventOrderList_Index,
            ArrangeAction = (obvList) => obvList.Add(StaticTestItem),
            ActAction = (obvList) => obvList[0] = StaticTestItem,
            IsCountChanged = false
        },
        new EventTestSet {
            Name = "Insert",
            EventOrderList = EventTestSet.EventOrderList_Insert,
            ArrangeAction = (_) => { },
            ActAction = (obvList) => obvList.Insert(0, StaticTestItem),
            IsCountChanged = true
        },
        //new EventTestSet {
        //    Name = "Move",
        //    EventOrderList = EventTestSet.EventOrderList_Move, 
        //    ArrangeAction = (obvList) => obvList.AddRange(StaticTestItem,StaticTestItem) ,
        //    ActAction = (obvList) => obvList.Move(1,0),
        //    IsCountChanged = false
        //}, //Todo after move is added to interface
        new EventTestSet {
            Name = "Remove",
            EventOrderList = EventTestSet.EventOrderList_Remove,
            ArrangeAction = (obvList) => obvList.Add(StaticTestItem),
            ActAction = (obvList) => obvList.Remove(StaticTestItem),
            IsCountChanged = true
        },
        new EventTestSet {
            Name = "RemoveAt",
            EventOrderList = EventTestSet.EventOrderList_RemoveAt,
            ArrangeAction = (obvList) => obvList.Add(StaticTestItem),
            ActAction = (obvList) => obvList.RemoveAt(0),
            IsCountChanged = true
        }
    };

    public static object[] ObservableListDataSource = {
        () => new ObservableList<TestItem>(),
        () => new ObservableIList<TestItem, List<TestItem>>(),
        () => new ObservableIListLocking<TestItem,List<TestItem>>()
    };

    public readonly AssertEventArgs<TestItem> TestArgs = new();

    /// <summary>
    /// The cardinality of these tests are high, so loops and value sources are used to auto-generate tests.
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

        var obvList = obvListGenerator();
        Console.WriteLine("Name: " + testSet.Name);
        Console.WriteLine("List: " + obvList.GetType());

        testSet.ArrangeAction(obvList);

        var testEventList = new List<object>();
        var callOrder = 0;
        var index = 0;

        foreach (var eventName in testSet.EventOrderList) {
            var staticIndex = index++;
            if (eventName != nameof(IObservableList<TestItem>.PropertyChanged)) {
                var testEvent = new AssertEvent<NotifyCollectionChangedEventArgs>(obvList, eventName);
                testEventList.Add(testEvent);
                testEvent.AddCallback((_, _) => Console.WriteLine("Expected: " + staticIndex + ": Call: " + callOrder + " : " + eventName));
                testEvent.AddCallback((_, _) => callOrder = (callOrder == staticIndex) ? callOrder + 1
                    : throw new Exception(testSet.Name + ": Call order of " + eventName + " was not correct. " + staticIndex + " was expected, but " + callOrder + " was received."));
            }
            else {
                var testEvent = new AssertEvent<PropertyChangedEventArgs>(obvList, eventName);
                testEventList.Add(testEvent);
                testEvent.AddCallback((_, args) => Console.WriteLine("Expected: " + staticIndex + ": Call: " + callOrder + " : " + eventName + " : " + args.PropertyName));
                testEvent.AddCallback((_, args) => {
                    if (args.PropertyName == "Count") Assert.True(testSet.IsCountChanged, "OnPropertyChanged: Count is not suppose to be called for method: " + testSet.Name);

                    if (args.PropertyName == "Item[]") callOrder = (callOrder == staticIndex) ? callOrder + 1
                    : throw new Exception(testSet.Name + ": Call order of " + eventName + " was not correct. " + staticIndex + " was expected, but " + callOrder + " was received.");
                });
            }
        }

        testSet.ActAction(obvList);

        foreach (var item in testEventList) {
            if (item is AssertEvent<PropertyChangedEventArgs> testEventProperty) testEventProperty.AssertAll((testSet.IsCountChanged) ? 2 : 1);
            else if (item is AssertEvent<CollectionChangeEventArgs> testEventCollection) testEventCollection.AssertAll(1);
        }
    }

    public class EventTestSet {
        public string Name;
        public List<string> EventOrderList;
        public Action<IObservableList<TestItem>> ArrangeAction;
        public Action<IObservableList<TestItem>> ActAction;
        public bool IsCountChanged;

        #region Orders of Events for differnet list methods
        
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
