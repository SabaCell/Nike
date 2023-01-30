using System;
using System.Runtime.Serialization;

namespace Nike.Framework.Domain.Exceptions
{
    [Serializable]
    public class DomainException : Exception
    {
        public DomainException()
        {
            AggregateRootType = GetType();
        }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Type AggregateRootType { get; }
    }
}