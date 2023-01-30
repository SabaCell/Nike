using Nike.Framework.Domain.EventSourcing.Equality;

namespace Nike.Framework.Domain.EventSourcing{

public abstract class ValueObject<T>
{
    public override bool Equals(object obj)
    {
        return EqualsBuilder.ReflectionEquals(this, obj);
    }

    public override int GetHashCode()
    {
        return HashCodeBuilder.ReflectionHashCode(this);
    }

    protected abstract void Validate();
}
}