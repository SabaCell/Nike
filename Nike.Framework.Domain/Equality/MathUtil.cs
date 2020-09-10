using System;

namespace Nike.Framework.Domain.Equality
{
    public class MathUtil
    {
        public static bool FloatEqualTo(float left, float right, float epsilon)
        {
            return Math.Abs(left - right) <= epsilon;
        }

        public static bool FloatGreaterThan(float left, float right, float epsilon)
        {
            // delegate
            return FloatGreaterThan(left, right, epsilon, false);
        }

        public static bool FloatGreaterThanOrEqualTo(float left, float right, float epsilon)
        {
            // delegate
            return FloatGreaterThan(left, right, epsilon, true);
        }

        private static bool FloatGreaterThan(float left, float right, float epsilon, bool orEqualTo)
        {
            if (FloatEqualTo(left, right, epsilon))
            {
                // within epsilon, so considered equal
                return orEqualTo;
            }
            return left > right;
        }

        public static bool FloatLessThan(float left, float right, float epsilon)
        {
            // delegate
            return FloatLessThan(left, right, epsilon, false);
        }

        public static bool FloatLessThanOrEqualTo(float left, float right, float epsilon)
        {
            // delegate
            return FloatLessThan(left, right, epsilon, true);
        }

        private static bool FloatLessThan(float left, float right, float epsilon, bool orEqualTo)
        {
            if (FloatEqualTo(left, right, epsilon))
            {
                // within epsilon, so considered equal
                return orEqualTo;
            }
            return left < right;
        }

        public static bool DoubleEqualTo(double left, double right, double epsilon)
        {
            return Math.Abs(left - right) <= epsilon;
        }

        public static bool DoubleGreaterThan(double left, double right, double epsilon)
        {
            // delegate
            return DoubleGreaterThan(left, right, epsilon, false);
        }

        public static bool DoubleGreaterThanOrEqualTo(double left, double right, double epsilon)
        {
            // delegate
            return DoubleGreaterThan(left, right, epsilon, true);
        }

        private static bool DoubleGreaterThan(double left, double right, double epsilon, bool orEqualTo)
        {
            if (DoubleEqualTo(left, right, epsilon))
            {
                // within epsilon, so considered equal
                return orEqualTo;
            }
            return left > right;
        }

        public static bool DoubleLessThan(double left, double right, double epsilon)
        {
            // delegate
            return DoubleLessThan(left, right, epsilon, false);
        }

        public static bool DoubleLessThanOrEqualTo(double left, double right, double epsilon)
        {
            // delegate
            return DoubleLessThan(left, right, epsilon, true);
        }

        private static bool DoubleLessThan(double left, double right, double epsilon, bool orEqualTo)
        {
            if (DoubleEqualTo(left, right, epsilon))
            {
                // within epsilon, so considered equal
                return orEqualTo;
            }
            return left < right;
        }
    }
}
