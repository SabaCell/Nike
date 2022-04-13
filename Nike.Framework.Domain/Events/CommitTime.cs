using System;

namespace Nike.Framework.Domain.Events;

[Flags]
public enum CommitTime
{
    None = 0,
    BeforeCommit = 2,
    AfterCommit = 4
}