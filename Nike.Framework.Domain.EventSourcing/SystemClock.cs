using System;

namespace Nike.Framework.Domain.EventSourcing
{
    public class SystemClock : IClock
    {
        /// <inheritdoc />
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}