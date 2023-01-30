using System.Collections.Generic;

namespace Nike.Framework.Domain.EventSourcing
{
    public struct HashCode
    {
        public const int InitialValue = 31;

        public const int MultiplyerValue = 59;

        public const int AlternativeMultiplyerValue = 114;

        private readonly int hashCode;

        public HashCode(int hashCode)
        {
            this.hashCode = hashCode;
        }

        public static HashCode Start { get; } = new HashCode(InitialValue);

        public static implicit operator int(HashCode hashCode)
        {
            return hashCode.GetHashCode();
        }

        public HashCode WithHash<T>(T item, bool useAlternativeMultiplyerValue = false)
        {
            var hashValue = EqualityComparer<T>.Default.GetHashCode(item);
            unchecked
            {
                var multiplyerValue = useAlternativeMultiplyerValue ? AlternativeMultiplyerValue : MultiplyerValue;
                hashValue += hashCode * multiplyerValue;
            }

            var result = new HashCode(hashValue);
            return result;
        }

        public HashCode WithHashProperties<T>(T item, bool useAlternativeMultiplyer = false,
            bool changeMultiplyerValues = false)
        {
            var properties = GetType().GetProperties();
            var result = this;
            foreach (var property in properties)
            {
                var obj = property.GetValue(item);
                result = result.WithHash(obj, useAlternativeMultiplyer);
                if (changeMultiplyerValues) useAlternativeMultiplyer = !useAlternativeMultiplyer;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}