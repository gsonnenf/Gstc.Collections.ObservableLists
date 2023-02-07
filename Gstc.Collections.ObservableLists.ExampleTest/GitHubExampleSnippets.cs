using Gstc.Collections.ObservableLists.Binding;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.ExampleTest;
[TestFixture]
internal class GitHubExampleSnippets {
    [Test]
    public void Example() {
        var obvListBind = new ObservableListBindFunc<int, string>(
            (itemA) => itemA.ToString(),
            (ItemB) => int.Parse(ItemB),
            new ObservableList<int>(),
            new ObservableList<string>()
       );

    }
}
