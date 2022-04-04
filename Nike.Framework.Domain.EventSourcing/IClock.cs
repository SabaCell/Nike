using System;

namespace Nike.Framework.Domain.EventSourcing;

public interface IClock
{
    DateTime Now();
}