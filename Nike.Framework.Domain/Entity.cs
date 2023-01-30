#nullable enable
using System;

namespace Nike.Framework.Domain
{
    public interface IEntity
    {
        DateTime CreatedAt { get; }
        DateTime? EditAt { get; }
    }

    public interface IEntity<out TPrimaryKey> : IEntity
    {
        TPrimaryKey Id { get; }
    }

    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>, IEquatable<Entity<TPrimaryKey>>
        // where TPrimaryKey : notnull
    {
        public Entity()
        {
            CreatedAt = DateTime.Now;
        }

        public TPrimaryKey Id { get; protected set; } = default;
        public DateTime CreatedAt { get; protected set; }
        public DateTime? EditAt { get; protected set; }

        public bool Equals(Entity<TPrimaryKey>? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            if (GetType() != other.GetType()) return false;

            return Id.Equals(other.Id);
        }

        public static bool operator !=(Entity<TPrimaryKey> a, Entity<TPrimaryKey> b)
        {
            return !(a == b);
        }

        public static bool operator ==(Entity<TPrimaryKey> a, Entity<TPrimaryKey> b)
        {
            if (a is null && b is null) return true;

            if (a is null || b is null) return false;

            return a.Equals(b);
        }

        public override bool Equals(object? obj)
        {
            return !(obj is null) && Equals(obj as Entity<TPrimaryKey>);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (GetType().GetHashCode() * 97) ^ Id.GetHashCode();
            }
        }
    }
}