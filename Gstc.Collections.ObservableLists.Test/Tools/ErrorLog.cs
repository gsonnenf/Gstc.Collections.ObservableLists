using System.Collections.Concurrent;
using System.Linq;

namespace Gstc.Utility.UnitTest.Event;
/// <summary>
/// The ErrorLog class is a thread safe collection where error messages are stored individually in the collection.
/// </summary>
public class ErrorLog : BlockingCollection<string> {
    public static ErrorLog operator +(ErrorLog errorLog, string message) {
        errorLog.Add(message);
        return errorLog;
    }

    /// <summary>
    /// Returns all queued error messages as a single string.
    /// </summary>
    /// <returns>All error messages as a single string.</returns>
    public string ErrorMessages() => this.Aggregate("", (current, next) => current + (next + "\n\n"));

    /// <summary>
    /// Appends the content of another error log to the end of the list.
    /// </summary>
    /// <param name="errorLog"></param>
    public void Add(ErrorLog errorLog) {
        foreach (var msg in errorLog) Add(msg);
    }

    public void Clear() { while (TryTake(out _)) { } }
    /// <summary>
    /// Determines if the test was successfully executed with no errors.
    /// </summary>
    /// <returns>Returns true if no errors, otherwise false.</returns>
    /// 
    public bool IsSuccess() => Count == 0;

    /// <returns>All error messages as a single string.</returns>
    public override string ToString() => ErrorMessages();

}
