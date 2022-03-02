using System;

namespace Nike.Framework.Domain
{
    [Flags]
    public enum CommitTime
    {
        None = 0,
        BeforeCommit = 2,
        AfterCommit = 4,
    }
}