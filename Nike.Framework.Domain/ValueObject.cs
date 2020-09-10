using Nike.Framework.Domain.Equality;

namespace Nike.Framework.Domain
{
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
