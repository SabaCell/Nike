using Nike.Framework.Domain;
using System;

namespace Nike.CustomerManagement.Tests.Units.TestDoubles
{
    public class ClockStub : IClock
    {
        private DateTime? _dateTime;

        public void Adjust(DateTime dateTime)
        {
            this._dateTime = dateTime;
        }

        /// <inheritdoc />
        public DateTime Now()
        {
            return _dateTime ?? DateTime.Now;
        }
    }
}