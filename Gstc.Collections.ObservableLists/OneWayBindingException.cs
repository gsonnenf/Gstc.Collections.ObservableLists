using System;

namespace Gstc.Collections.ObservableLists;
/// <summary>
/// In a collection binding class, this exception is thrown if an attempt is made to modify a target collection but
/// isBidirectional is set to false.
/// </summary>
public class OneWayBindingException : NotSupportedException {
    /// <summary>
    /// Creates a OneWayBindingException with a specific message.
    /// </summary>
    /// <returns></returns>
    public static OneWayBindingException Create() => new("The target collection was modified but IsBidirectional is not set to true.");
    public OneWayBindingException(string message) : base(message) { }
}
