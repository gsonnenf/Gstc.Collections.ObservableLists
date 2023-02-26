using System.Collections.Generic;
using System.Linq;
using Gstc.Collections.ObservableDictionary.Test.Tools;
using Gstc.Collections.ObservableLists.ExampleTest.Fakes;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;

public class Benchmark {

    //[Test, Sequential]
    [Description("Tests iterator to see which is fastest, ForEach vs. For loop")]
    public void AddingBenchmarkIntTest(
            [Values(1000000, 100000, 10000, 100)] int iterations,
            [Values(10, 100, 1000, 10000)] int numOfItems) {

        List<int> listInitial;
        List<int> itemList;

        listInitial = Enumerable.Range(0, numOfItems).ToList();
        itemList = Enumerable.Range(0, numOfItems).ToList();
        using (ScopedStopwatch.Start("\nWarmup"))
            for (var i = 0; i < iterations; i++)
                foreach (var item in itemList) listInitial.Add(item);

        listInitial = Enumerable.Range(0, numOfItems).ToList();
        itemList = Enumerable.Range(0, numOfItems).ToList();
        using (ScopedStopwatch.Start("\nForEach"))
            for (var i = 0; i < iterations; i++)
                foreach (var item in itemList) listInitial.Add(item);

        listInitial = Enumerable.Range(0, numOfItems).ToList();
        itemList = Enumerable.Range(0, numOfItems).ToList();
        using (ScopedStopwatch.Start("\nFor loop"))
            for (var i = 0; i < iterations; i++)
                for (var j = 0; j < itemList.Count; j++) listInitial.Add(itemList[j]);

    }

    //[Test, Sequential]
    [Description("Foreach/ForLoop - Tests iterator speed for adding value types.")]
    public void AddingBenchmarkDoubleTest(
          [Values(1000000, 100000, 10000, 100)] int iterations,
          [Values(10, 100, 1000, 10000)] int numOfItems) {

        List<double> listInitial;
        List<double> itemList;

        listInitial = Enumerable.Range(0, numOfItems).Select(item => (double)item).ToList();
        itemList = Enumerable.Range(0, numOfItems).Select(item => (double)item).ToList();
        using (ScopedStopwatch.Start("\nWarmup"))
            for (var i = 0; i < iterations; i++)
                foreach (var item in itemList) listInitial.Add(item);

        listInitial = Enumerable.Range(0, numOfItems).Select(item => (double)item).ToList(); ;
        itemList = Enumerable.Range(0, numOfItems).Select(item => (double)item).ToList();
        using (ScopedStopwatch.Start("\nForEach"))
            for (var i = 0; i < iterations; i++)
                foreach (var item in itemList) listInitial.Add(item);

        listInitial = Enumerable.Range(0, numOfItems).Select(item => (double)item).ToList();
        itemList = Enumerable.Range(0, numOfItems).Select(item => (double)item).ToList();
        using (ScopedStopwatch.Start("\nFor loop"))
            for (var i = 0; i < iterations; i++)
                for (var j = 0; j < itemList.Count; j++) listInitial.Add(itemList[j]);

    }

    //[Test, Sequential]
    [Description("Foreach/ForLoop - Tests iterator speed for adding reference types.")]
    public void AddingBenchmarkCustomerTest(
            [Values(1000000, 100000, 10000, 100)] int iterations,
            [Values(10, 100, 1000, 10000)] int numOfItems) {
        List<Customer> listInitial;
        List<Customer> itemList;

        listInitial = Customer.GenerateCustomerList(numOfItems);
        itemList = Customer.GenerateCustomerList(numOfItems);
        using (ScopedStopwatch.Start("\nWarmup"))
            for (var i = 0; i < iterations; i++)
                foreach (var item in itemList) listInitial.Add(item);

        listInitial = Customer.GenerateCustomerList(numOfItems);
        itemList = Customer.GenerateCustomerList(numOfItems);
        using (ScopedStopwatch.Start("\nForEach"))
            for (var i = 0; i < iterations; i++)
                foreach (var item in itemList) listInitial.Add(item);

        listInitial = Customer.GenerateCustomerList(numOfItems);
        itemList = Customer.GenerateCustomerList(numOfItems);
        using (ScopedStopwatch.Start("\nFor loop"))
            for (var i = 0; i < iterations; i++)
                for (var j = 0; j < itemList.Count; j++) listInitial.Add(itemList[j]);
    }
}
