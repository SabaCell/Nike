// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashCode.cs" company="Souran">
//   Souran copyright
// </copyright>
// <summary>
//   Defines the HashCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Nike.Framework.Domain.EventSourcing
{
    /// <summary>
    /// کد مخدوش شده
    /// </summary>
    public struct HashCode
    {
        /// <summary>
        /// مقدار اولیه-زمان تعریف
        /// </summary>
        public const int InitialValue = 31;

        /// <summary>
        /// مقدار ضریب تولید مقادیر مخدوش شده ی ثانویه
        /// </summary>
        public const int MultiplyerValue = 59;

        /// <summary>
        /// مقدار ضریب تولید مقادیر مخدوش شده ی ثانویه ی جایگزین
        /// </summary>
        public const int AlternativeMultiplyerValue = 114;

        /// <summary>
        /// The hash code.
        /// </summary>
        private readonly int hashCode;

        /// <summary>
        /// ایجاد کد مخدوش شده
        /// </summary>
        /// <param name="hashCode">مقدار مخدوش شده</param>
        public HashCode(int hashCode)
        {
            this.hashCode = hashCode;
        }

        /// <summary>
        /// ارائه ی کد مخدوش آغازین.
        /// </summary>
        public static HashCode Start { get; } = new HashCode(InitialValue);

        /// <summary>
        /// تعریف عملگر ارائه ی مقدار کد مخدوش
        /// </summary>
        /// <param name="hashCode">کد مخدوش</param>
        public static implicit operator int(HashCode hashCode) => hashCode.GetHashCode();

        /// <summary>
        /// ارائه ی کد مخدوش با در نظر گرفتن یک موجودیت جدید
        /// </summary>
        /// <typeparam name="T">نوع موجودیت جدید</typeparam>
        /// <param name="item">موجودیت/فیلد جدید</param>
        /// <param name="useAlternativeMultiplyerValue">The Alternative Multiplyer Value.</param>
        /// <returns>کد مخدوش جدید</returns>
        public HashCode WithHash<T>(T item, bool useAlternativeMultiplyerValue = false)
        {
            var hashValue = EqualityComparer<T>.Default.GetHashCode(item);
            unchecked
            {
                var multiplyerValue = useAlternativeMultiplyerValue ? AlternativeMultiplyerValue : MultiplyerValue;
                hashValue += this.hashCode * multiplyerValue;
            }

            var result = new HashCode(hashValue);
            return result;
        }

        /// <summary>
        /// ارائه ی کد مخدوش با در نظر گرفتن یک موجودیت جدید
        /// </summary>
        /// <typeparam name="T">نوع موجودیت جدید</typeparam>
        /// <param name="item">موجودیت/فیلد جدید</param>
        /// <param name="useAlternativeMultiplyer">استفاده از ضریب جایگزین</param>
        /// <param name="changeMultiplyerValues">تغییر متناوب مقدار ضرایب</param>
        /// <returns>کد مخدوش جدید</returns>
        public HashCode WithHashProperties<T>(T item, bool useAlternativeMultiplyer = false, bool changeMultiplyerValues = false)
        {
            var properties = this.GetType().GetProperties();
            var result = this;
            foreach (var property in properties)
            {
                var obj = property.GetValue(item);
                result = result.WithHash(obj, useAlternativeMultiplyer);
                if (changeMultiplyerValues)
                {
                    useAlternativeMultiplyer = !useAlternativeMultiplyer;
                }
            }

            return result;
        }

        /// <summary>
        /// ارائه ی مقدار کد مخدوش
        /// </summary>
        /// <returns>کد مخدوش جاری</returns>
        public override int GetHashCode() => this.hashCode;
    }
}