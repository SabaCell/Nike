using System;

namespace Nike.Framework.Domain;

public class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTime Now()
    {
        return DateTime.Now;
    }
}