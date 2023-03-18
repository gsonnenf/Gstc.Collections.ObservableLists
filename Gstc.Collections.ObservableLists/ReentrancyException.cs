using System;
using System.Runtime.Serialization;

namespace Gstc.Collections.ObservableDictionary.ObservableList
{
    public class ReentrancyException : InvalidOperationException
    {
        public ReentrancyException() { }

        public ReentrancyException(string message) : base(message) { }

        public ReentrancyException(string message, Exception innerException) : base(message, innerException) { }

        protected ReentrancyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
