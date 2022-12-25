using NUnit.Framework;

namespace Gstc.Utility.UnitTest.Event;

public partial class AssertEvent<TEventArgs> {
    public void AssertAll(int expectedTimesCalled)
        => Assert.That(TestAll(expectedTimesCalled), ErrorMessages);
}
