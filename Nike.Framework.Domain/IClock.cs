using System;

namespace Nike.Framework.Domain
{
    public interface IClock
    {
        DateTime Now();
    }
}