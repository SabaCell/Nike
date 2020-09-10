using System;

namespace Nike.Framework.Domain
{
    public interface IAuditedEntity
    {
        DateTime CreatedAt { get; }
        DateTime UpdatedAt { get; }
    }
}