using AutoFixture;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

/// <summary>
/// A Test item used in these tests
/// </summary>
public class TestItem {
    private static Fixture Fixture { get; } = new();
    public string Id { get; set; }
    public TestItem() => Id = Fixture.Create<string>();
}
