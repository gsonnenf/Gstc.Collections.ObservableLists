using System.Collections.Concurrent;
using System.Linq;

namespace Gstc.UnitTest.Events {
    public class ErrorLog : BlockingCollection<string> {
        public static ErrorLog operator +(ErrorLog errorLog, string message) {
            errorLog.Add(message);
            return errorLog;
        }
        public string ErrorMessages() => ToString();

        public void Add(ErrorLog errorLog) {
            foreach (var msg in errorLog) Add(msg);
        }
        public bool IsSuccess() => Count == 0;

        public override string ToString() => this.Aggregate("", (current, next) => current + (next + "\n\n"));

    }
}
