using System;
using System.Diagnostics;

namespace Gstc.Collections.ObservableDictionary.Test.Tools;
public class ScopedStopwatch : IDisposable {
    public static ScopedStopwatch Start(string testDescription) => new ScopedStopwatch(testDescription);
    public TimeSpan Elapsed => Stopwatch.Elapsed;

    public Stopwatch Stopwatch = new Stopwatch();

    public ScopedStopwatch(string testDescription) {
        LogDescription(testDescription);
        Stopwatch.Start();
    }

    public void Dispose() {
        Stopwatch.Stop();
        LogResult();
    }

    public virtual void LogDescription(string message) => Console.WriteLine(message);
    public virtual void LogResult() => Console.WriteLine("Time Elapsed: " + Stopwatch.Elapsed);
}
