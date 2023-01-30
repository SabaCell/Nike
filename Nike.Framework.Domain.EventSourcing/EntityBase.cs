using System;
using Nike.Framework.Domain.EventSourcing.Exceptions;

namespace Nike.Framework.Domain.EventSourcing{

public abstract class EntityBase<TId> : IEquatable<EntityBase<TId>>
    where TId : IEquatable<TId>
{
    protected EntityBase(TId id)
    {
        Id = id;
    }

    public TId Id { get; protected set; }

    #region IEquatable and Override Equals operators

    public static bool operator ==(EntityBase<TId> entity1, EntityBase<TId> entity2)
    {
        if ((object) entity1 == null && (object) entity2 == null) return true;

        if ((object) entity1 == null || (object) entity2 == null) return false;

        return entity1.Id.ToString() == entity2.Id.ToString();
    }

    public static bool operator !=(EntityBase<TId> entity1, EntityBase<TId> entity2)
    {
        return !(entity1 == entity2);
    }

    /// <inheritdoc />
    public bool Equals(EntityBase<TId> other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public override bool Equals(object entity)
    {
        return entity is EntityBase<TId> && Equals((EntityBase<TId>) entity);
    }

    /// <inheritdoc />
    //// ReSharper disable NonReadonlyMemberInGetHashCode
    public override int GetHashCode()
    {
        return HashCode.Start.WithHash(Id);
        ////return this.Id == null ? 0 : this.Id.GetHashCode();
    }

    #endregion

    public void SetId(TId id)
    {
        if (Id.Equals(default)) Id = id;
    }

    /// <summary>
    ///     بررسی صحت موجودیت
    /// </summary>
    private void Validate()
    {
        if (Id.Equals(default)) throw new DomainException("Invalid Entity.");
    }
}
}