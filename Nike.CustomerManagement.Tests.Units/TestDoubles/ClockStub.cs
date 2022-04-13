using System;
using Nike.Framework.Domain;

namespace Nike.CustomerManagement.Tests.Units.TestDoubles;

public class ClockStub : IClock
{
    private DateTime? _dateTime;

    /// <inheritdoc />
    public DateTime Now()
    {
        return _dateTime ?? DateTime.Now;
    }

    public void Adjust(DateTime dateTime)
    {
        _dateTime = dateTime;
    }
}