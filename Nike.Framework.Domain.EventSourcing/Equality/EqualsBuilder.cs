using System;
using System.Reflection;

namespace Nike.Framework.Domain.EventSourcing.Equality{

#region Usings

#endregion

/// <summary>
///     The is a C# port of the
///     <see href="http://commons.apache.org/lang/api/org/apache/commons/lang/builder/EqualsBuilder.html">
///         Apache Commons EqualsBuilder
///     </see>
///     , which was implemented in Java. This
///     C# implementation was created by
///     <see href="http://weblogs.asp.net/arturtrosin/archive/2009/05/08/apache-hash-code-and-equals-builders.aspx">
///         Artur
///         Trosin
///     </see>
///     .
///     This class provides methods to build a good Equals method for any
///     class. It follows rules laid out in
///     <a href="http://java.sun.com/docs/books/effective/index.html">Effective Java</a>,
///     by Joshua Bloch. In particular the rule for comparing <code>doubles</code>,
///     <code>floats</code>, and arrays can be tricky. Also, making sure that
///     <code>Equals()</code> and <code>GetHashCode()</code> are consistent can be
///     difficult.</p>
///     Two Objects that compare as equal must generate the same hash code,
///     but two Objects with the same hash code do not have to be equal.
///     All relevant fields should be included in the calculation of Equals.
///     Derived fields may be ignored. In particular, any field used in
///     the GetHashCode method must be used in the Equals method, and vice
///     versa.
///     Typical use for the code is as follows:
///     <code>
/// public boolean Equals(Object obj) {
///   if (obj == null) { return false; }
///   if (obj == this) { return true; }
///   if (obj.GetType() != GetType()) {
///     return false;
///   }
///   MyClass rhs = (MyClass) obj;
///   return new EqualsBuilder()
///                 .Append(field1, rhs.field1)
///                 .Append(field2, rhs.field2)
///                 .Append(field3, rhs.field3)
///                 .IsEquals();
///  }
/// </code>
///     If the base class has overridden Objects.Equals(), then you can conveniently
///     chain the base class result of Equals to the derived class result like so:
///     <code>
/// //...
/// return new EqualsBuilder()
///               .AppendSuper(base.Equals(obj)) // chain base class result
///               .Append(field1, rhs.field1)
///               .Append(field2, rhs.field2)
///               .Append(field3, rhs.field3)
///               .IsEquals();
/// </code>
///     Alternatively, there is a method that uses reflection to determine
///     the fields to test. Because these fields are usually private, the method,
///     <code>reflectionEquals</code>, uses <code>BindingFlags.NonPublic</code> to
///     change the visibility of the fields. This will fail under a security
///     manager, unless the appropriate permissions are set up correctly. It is
///     also slower than testing explicitly.
///     A typical invocation for this method would look like:
///     <code>
/// public boolean Equals(Object obj) {
///   return EqualsBuilder.ReflectionEquals(this, obj);
/// }
/// </code>
/// </summary>
public class EqualsBuilder
{
    // If the fields tested are equals.     
    private bool isEqual;

    // Constructor for EqualsBuilder.
    // Starts off assuming that equals is true
    public EqualsBuilder()
    {
        isEqual = true;
    }

    /// <summary>
    ///     This method uses reflection to determine if the two Object
    ///     are equal.
    ///     It uses AccessibleObject.setAccessible to gain access to private
    ///     fields. This means that it will throw a security exception if run under
    ///     a security manager, if the permissions are not set up correctly. It is also
    ///     not as efficient as testing explicitly.
    ///     Transient members will be not be tested, as they are likely derived
    ///     fields, and not part of the value of the Object.
    ///     Static fields will not be tested. Superclass fields will be included.
    /// </summary>
    /// <param name="lhs">this object</param>
    /// <param name="rhs">the other object</param>
    /// <returns>true if the two Objects have tested equals.</returns>
    public static bool ReflectionEquals(object lhs, object rhs)
    {
        return ReflectionEquals(lhs, rhs, false, null);
    }

    /// <summary>
    ///     This method uses reflection to determine if the two Object
    ///     are equal.
    ///     It uses AccessibleObject.setAccessible to gain access to private
    ///     fields. This means that it will throw a security exception if run under
    ///     a security manager, if the permissions are not set up correctly. It is also
    ///     not as efficient as testing explicitly.
    ///     If the TestTransients parameter is set to true, transient
    ///     members will be tested, otherwise they are ignored, as they are likely
    ///     derived fields, and not part of the value of the Object.
    ///     Static fields will not be tested. Superclass fields will be included.
    /// </summary>
    /// <param name="lhs">this object</param>
    /// <param name="rhs">the other object</param>
    /// <param name="testTransients">whether to include transient fields</param>
    /// <returns>true if the two Objects have tested equals.</returns>
    public static bool ReflectionEquals(object lhs, object rhs, bool testTransients)
    {
        return ReflectionEquals(lhs, rhs, testTransients, null);
    }

    /// <summary>
    ///     This method uses reflection to determine if the two Object
    ///     are equal.
    ///     It uses AccessibleObject.setAccessible to gain access to private
    ///     fields. This means that it will throw a security exception if run under
    ///     a security manager, if the permissions are not set up correctly. It is also
    ///     not as efficient as testing explicitly.
    ///     If the testTransients parameter is set to true, transient
    ///     members will be tested, otherwise they are ignored, as they are likely
    ///     derived fields, and not part of the value of the Object.
    ///     Static fields will not be included. Superclass fields will be appended
    ///     up to and including the specified superclass. A null superclass is treated
    ///     as java.lang.Object.
    /// </summary>
    /// <param name="lhs">this object</param>
    /// <param name="rhs">the other object</param>
    /// <param name="testTransients">whether to include transient fields</param>
    /// <param name="reflectUpToClass">the superclass to reflect up to (inclusive), may be null</param>
    /// <returns>true if the two Objects have tested equals.</returns>
    public static bool ReflectionEquals(object lhs, object rhs, bool testTransients, Type reflectUpToClass)
    {
        if (lhs == rhs) return true;
        if (lhs == null || rhs == null) return false;
        // Find the leaf class since there may be transients in the leaf 
        // class or in classes between the leaf and root.
        // If we are not testing transients or a subclass has no ivars, 
        // then a subclass can test equals to a superclass.
        var lhsClass = lhs.GetType();
        var rhsClass = rhs.GetType();
        Type testClass;
        if (lhsClass.IsInstanceOfType(rhs))
        {
            testClass = lhsClass;
            if (!rhsClass.IsInstanceOfType(lhs))
                // rhsClass is a subclass of lhsClass
                testClass = rhsClass;
        }
        else if (rhsClass.IsInstanceOfType(lhs))
        {
            testClass = rhsClass;
            if (!lhsClass.IsInstanceOfType(rhs))
                // lhsClass is a subclass of rhsClass
                testClass = lhsClass;
        }
        else
        {
            // The two classes are not related.
            return false;
        }

        var equalsBuilder = new EqualsBuilder();
        try
        {
            ReflectionAppend(lhs, rhs, testClass, equalsBuilder, testTransients);
            while (testClass.BaseType != null && testClass != reflectUpToClass)
            {
                testClass = testClass.BaseType;
                ReflectionAppend(lhs, rhs, testClass, equalsBuilder, testTransients);
            }
        }
        catch (ArgumentException e)
        {
            // In this case, we tried to test a subclass vs. a superclass and
            // the subclass has ivars or the ivars are transient and 
            // we are testing transients.
            // If a subclass has ivars that we are trying to test them, we get an
            // exception and we know that the objects are not equal.
            return false;
        }

        return equalsBuilder.IsEquals();
    }

    /// <summary>
    ///     Appends the fields and values defined by the given object of the given Class.
    /// </summary>
    /// <param name="builder">the builder to Append to</param>
    /// <param name="clazz">the class to Append details of</param>
    /// <param name="lhs">the left hand object</param>
    /// <param name="rhs">the right hand object</param>
    /// <param name="useTransients">whether to test transient fields</param>
    private static void ReflectionAppend(
        object lhs,
        object rhs,
        Type clazz,
        EqualsBuilder builder,
        bool useTransients)
    {
        /* 
         * In Java version of this ReflectionAppend, we have to call 
         * AccessibleObject.setAccessible() right after class.GetFields() to 
         * make non-public fields accessible. In C#, it is easier to do. We simply
         * add BindingFlags.NonPublic, which makes non-public fields accessible 
         * (subject to security manager restrictions, of course).
         */
        var fields = clazz.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                     BindingFlags.DeclaredOnly);
        for (var i = 0; i < fields.Length && builder.isEqual; i++)
        {
            var f = fields[i];
            if (f.Name.IndexOf('$') == -1
                && (useTransients || !isTransient(f))
                && !f.IsStatic)
                try
                {
                    builder.Append(f.GetValue(lhs), f.GetValue(rhs));
                }
                /* 
                     * According to FieldInfo's documentation, getValue() can throw the 
                     * following exceptions: TargetException, NotSupportedException, 
                     * FieldAccessException and ArgumentException.
                     * 
                     * TargetException is thrown if the field is non-static and obj is
                     * a null reference. In our case, the field is non-static (because of
                     * BindingFlags.Instance) but obj should never be null because of
                     * null checks in the calling method (i.e. reflectionEquals()). 
                     * I guess we can just throw an unexpected exception.
                     * 
                     * NotSupportedException is thrown if the field is marked Literal, but
                     * the field does not have one of the accepted literal types. Literal
                     * means that the field's value is a compile-time (static or early 
                     * bound) constant. I think this exception can be just eaten because
                     * constants should always be equal in lhs and rhs and default value
                     * of isEqual is true.
                     * 
                     * FieldAccessException is thrown if the caller does not have 
                     * permission to access this field. If this code is fully trusted
                     * (not sure how to verify this), access restrictions are ignored.
                     * This means that private fields are accessible. If this code is not
                     * fully trusted, it can still access private fields if this code
                     * has been granted ReflectedPermission with the 
                     * ReflectionPermisionFlag.RestrictedMemberAccess flag and if the 
                     * grant set of the non-public members is restricted to the caller's
                     * grant set, or subset thereof. Whew, that's a mouthful to say!
                     * I guess it's best just to let FieldAccessException bubble up
                     * to callers so that user can grant permissions, if desired.
                     * 
                     * Finally, ArgumentException is thrown if the method is neither 
                     * declared nor inherited by the class of obj. This could happen
                     * if lhs is a subclass of rhs (or vice-versa) and the field is 
                     * declared in the subclass. In Java, Field.get() would throw 
                     * IllegalArgumentException in this case. In Java version of
                     * reflectionAppend(), IllegalArgumentException
                     * bubbles up to reflectionEquals(), where it is dealt with.
                     * It seems logical that use the same technique in the C#
                     * version. That is, we allow ArgumentException to bubble up
                     * to ReflectionEquals() and deal with it there.
                     */
                catch (TargetException te)
                {
                    throw new Exception("Unexpected TargetException", te);
                }
                catch (NotSupportedException nse)
                {
                    // eat it!
                }
            /* Don't catch FieldAccessException and ArgumentException so that
                 * they can bubble up to caller. Alternatively, we could catch and
                 * rethrow.
                 */
            //catch (FieldAccessException fae) { throw; }
            //catch (ArgumentException fae) { throw; }
        }
    }

    /// <summary>
    ///     Adds the result of super.equals() to this builder.
    /// </summary>
    /// <param name="superEquals">the result of calling super.equals()</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder AppendSuper(bool superEquals)
    {
        if (isEqual == false) return this;
        isEqual = superEquals;
        return this;
    }

    /// <summary>
    ///     Test if two Object are equal using their equals method.
    /// </summary>
    /// <param name="lhs">the left hand object</param>
    /// <param name="rhs">the right hand object</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(object lhs, object rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        var lhsClass = lhs.GetType();
        if (!lhsClass.IsArray)
        {
            //the simple case, not an array, just test the element
            isEqual = lhs.Equals(rhs);
        }
        else
        {
            EnsureArraysSameDemention(lhs, rhs);
            if (isEqual == false) return this;

            //'Switch' on type of array, to dispatch to the correct handler
            // This handles multi dimensional arrays
            if (lhs is long[])
                Append((long[]) lhs, rhs as long[]);
            else if (lhs is int[])
                Append((int[]) lhs, rhs as int[]);
            else if (lhs is short[])
                Append((short[]) lhs, rhs as short[]);
            else if (lhs is char[])
                Append((char[]) lhs, rhs as char[]);
            else if (lhs is byte[])
                Append((byte[]) lhs, rhs as byte[]);
            else if (lhs is double[])
                Append((double[]) lhs, rhs as double[]);
            else if (lhs is float[])
                Append((float[]) lhs, rhs as float[]);
            else if (lhs is bool[])
                Append((bool[]) lhs, rhs as bool[]);
            else if (lhs is object[]) Append((object[]) lhs, rhs as object[]);
            {
                // Not an simple array of primitives
                CompareArrays(lhs, rhs, 0, 0);
            }
        }

        return this;
    }


    private void EnsureArraysSameDemention(object lhs, object rhs)
    {
        var isArray1 = lhs is Array;
        var isArray2 = rhs is Array;

        if (isArray1 != isArray2)
        {
            isEqual = false;
            return;
        }

        var array1 = (Array) lhs;
        var array2 = (Array) lhs;

        if (array1.Rank != array2.Rank) isEqual = false;

        if (array1.Length != array2.Length) isEqual = false;
    }

    private void CompareArrays(object parray1, object parray2, int prank, int pindex)
    {
        if (isEqual == false) return;
        if (parray1 == parray2) return;
        if (parray1 == null || parray2 == null)
        {
            isEqual = false;
            return;
        }

        var array1 = (Array) parray1;
        var array2 = (Array) parray2;
        var rank1 = array1.Rank;
        var rank2 = array2.Rank;

        if (rank1 != rank2)
        {
            isEqual = false;
            return;
        }

        var size1 = array1.GetLength(prank);
        var size2 = array2.GetLength(prank);

        if (size1 != size2)
        {
            isEqual = false;
            return;
        }

        if (prank == rank1 - 1)
        {
            var index = 0;

            var min = pindex;
            var max = min + size1;


            var enumerator1 = array1.GetEnumerator();
            var enumerator2 = array2.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                if (isEqual == false) return;
                enumerator2.MoveNext();


                if (index >= min && index < max)
                {
                    var obj1 = enumerator1.Current;
                    var obj2 = enumerator2.Current;

                    var isArray1 = obj1 is Array;
                    var isArray2 = obj2 is Array;

                    if (isArray1 != isArray2)
                    {
                        isEqual = false;
                        return;
                    }

                    if (isArray1)
                        CompareArrays(obj1, obj2, 0, 0);
                    else
                        Append(obj1, obj2);
                }

                index++;
            }
        }
        else
        {
            var mux = 1;

            var currentRank = rank1 - 1;

            do
            {
                var sizeMux1 = array1.GetLength(currentRank);
                var sizeMux2 = array2.GetLength(currentRank);

                if (sizeMux1 != sizeMux2)
                {
                    isEqual = false;
                    return;
                }

                mux *= sizeMux1;
                currentRank--;
            } while (currentRank > prank);

            for (var i = 0; i < size1; i++)
            {
                Console.Write("{ ");
                CompareArrays(parray1, parray2, prank + 1, pindex + i * mux);
                Console.Write("} ");
            }
        }
    }

    /// &ltp&gtTest if two &ltcode&gtlong
    /// </code&gtsareequal.
    /// </p>
    /// @param lhs  the left hand &ltcode&gtlong
    /// </code>
    /// @param rhs  the right hand &ltcode&gtlong
    /// </code>
    /// @return EqualsBuilder - used to chain calls.
    /// <summary>
    ///     Test if two long are equal.
    /// </summary>
    /// <param name="lhs">the left hand long</param>
    /// <param name="rhs">the right hand long</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(long lhs, long rhs)
    {
        if (isEqual == false) return this;
        isEqual = lhs == rhs;
        return this;
    }

    /// <summary>
    ///     Test if two int are equal.
    /// </summary>
    /// <param name="lhs">the left hand int</param>
    /// <param name="rhs">the right hand int</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(int lhs, int rhs)
    {
        if (isEqual == false) return this;
        isEqual = lhs == rhs;
        return this;
    }

    /// <summary>
    ///     Test if two short are equal.
    /// </summary>
    /// <param name="lhs">the left hand short</param>
    /// <param name="rhs">the right hand short</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(short lhs, short rhs)
    {
        if (isEqual == false) return this;
        isEqual = lhs == rhs;
        return this;
    }

    /// <summary>
    ///     Test if two char are equal.
    /// </summary>
    /// <param name="lhs">the left hand char</param>
    /// <param name="rhs">the right hand char</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(char lhs, char rhs)
    {
        if (isEqual == false) return this;
        isEqual = lhs == rhs;
        return this;
    }

    /// <summary>
    ///     Test if two byte are equal.
    /// </summary>
    /// <param name="lhs">the left hand byte</param>
    /// <param name="rhs">the right hand byte</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(byte lhs, byte rhs)
    {
        if (isEqual == false) return this;
        isEqual = lhs == rhs;
        return this;
    }

    /// <summary>
    ///     Test if two double are equal by testing that the
    ///     pattern of bits returned by BitConverter.DoubleToInt64Bits(double) are
    ///     equal.
    ///     This handles NaNs, Infinties, and &ltcode>-0.0.
    ///     It is compatible with the hash code generated by
    ///     HashCodeBuilder.
    /// </summary>
    /// <param name="lhs">the left hand double</param>
    /// <param name="rhs">the right hand double</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(double lhs, double rhs)
    {
        if (isEqual == false) return this;
        // java: return append(Double.doubleToLongBits(lhs), Double.doubleToLongBits(rhs));
        return Append(BitConverter.DoubleToInt64Bits(lhs), BitConverter.DoubleToInt64Bits(rhs));
    }

    /// <summary>
    ///     Test if two double are equal, within a given difference (i.e.
    ///     approximately equal).
    /// </summary>
    /// <param name="lhs">the left hand double</param>
    /// <param name="rhs">the right hand double</param>
    /// <param name="epsilon">
    ///     the difference within which the two values are
    ///     considered equal
    /// </param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    /// <remarks>This method is not in the Java version.</remarks>
    public EqualsBuilder Append(double lhs, double rhs, double epsilon)
    {
        if (isEqual == false) return this;
        isEqual = MathUtil.DoubleEqualTo(lhs, rhs, epsilon);
        return this;
    }

    /// <summary>
    ///     Test if two float are equal by testing that the
    ///     pattern of bits returned by
    ///     BitConverter.ToInt32(BitConverter.GetBytes(float), 0) are equal
    ///     (BitConverter does not have SingleToInt32Bits() so we use this
    ///     2-call workaround
    ///     <see href="http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=426391" />
    ///     to roll our own implementation).
    ///     This handles NaNs, Infinties, and &ltcode>-0.0.
    ///     It is compatible with the hash code generated by
    ///     HashCodeBuilder.
    /// </summary>
    /// <param name="lhs">the left hand float</param>
    /// <param name="rhs">the right hand float</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(float lhs, float rhs)
    {
        if (isEqual == false) return this;
        // java: return append(Float.floatToIntBits(lhs), Float.floatToIntBits(rhs));
        return Append(
            BitConverterUtil.SingleToInt32Bits(lhs),
            BitConverterUtil.SingleToInt32Bits(rhs));
    }

    /// <summary>
    ///     Test if two float are equal, within a given difference (i.e.
    ///     approximately equal).
    /// </summary>
    /// <param name="lhs">the left hand float</param>
    /// <param name="rhs">the right hand float</param>
    /// <param name="epsilon">
    ///     the difference within which the two values are
    ///     considered equal
    /// </param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    /// <remarks>This method is not in the Java version.</remarks>
    public EqualsBuilder Append(float lhs, float rhs, float epsilon)
    {
        if (isEqual == false) return this;
        isEqual = MathUtil.FloatEqualTo(lhs, rhs, epsilon);
        return this;
    }

    /// <summary>
    ///     Test if two bools are equal.
    /// </summary>
    /// <param name="lhs">the left hand bool</param>
    /// <param name="rhs">the right hand bool</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(bool lhs, bool rhs)
    {
        if (isEqual == false) return this;
        isEqual = lhs == rhs;
        return this;
    }

    /// <summary>
    ///     Performs a deep comparison of two Object arrays.
    ///     This also will be called for the top level of
    ///     multi-dimensional, ragged, and multi-typed arrays.
    /// </summary>
    /// <param name="lhs">the left hand Object[]</param>
    /// <param name="rhs">the right hand Object[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(object[] lhs, object[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i)
        {
            if (lhs[i] != null)
            {
                var lhsClass = lhs[i].GetType();
                if (!lhsClass.IsInstanceOfType(rhs[i]))
                {
                    isEqual = false; //If the types don't match, not equal
                    break;
                }
            }

            Append(lhs[i], rhs[i]);
        }

        return this;
    }

    /// <summary>
    ///     Deep comparison of array of long. Length and all
    ///     values are compared.
    ///     The method {@link #Append(long, long)} is used.
    /// </summary>
    /// <param name="lhs">the left hand long[]</param>
    /// <param name="rhs">the right hand long[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(long[] lhs, long[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i) Append(lhs[i], rhs[i]);
        return this;
    }

    /// <summary>
    ///     Deep comparison of array of long. Length and all
    ///     values are compared.
    ///     The method {@link #Append(int, int)} is used.
    /// </summary>
    /// <param name="lhs">the left hand int[]</param>
    /// <param name="rhs">the right hand int[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(int[] lhs, int[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i) Append(lhs[i], rhs[i]);
        return this;
    }

    /// <summary>
    ///     Deep comparison of array of long. Length and all
    ///     values are compared.
    ///     The method {@link #Append(short, short)} is used.
    /// </summary>
    /// <param name="lhs">the left hand short[]</param>
    /// <param name="rhs">the right hand short[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(short[] lhs, short[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i) Append(lhs[i], rhs[i]);
        return this;
    }

    /// <summary>
    ///     Deep comparison of array of long. Length and all
    ///     values are compared.
    ///     The method {@link #Append(char, char)} is used.
    /// </summary>
    /// <param name="lhs">the left hand char[]</param>
    /// <param name="rhs">the right hand char[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(char[] lhs, char[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i) Append(lhs[i], rhs[i]);
        return this;
    }

    /// <summary>
    ///     Deep comparison of array of long. Length and all
    ///     values are compared.
    ///     The method {@link #Append(byte, byte)} is used.
    /// </summary>
    /// <param name="lhs">the left hand byte[]</param>
    /// <param name="rhs">the right hand byte[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(byte[] lhs, byte[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i) Append(lhs[i], rhs[i]);
        return this;
    }

    /// <summary>
    ///     Deep comparison of array of long. Length and all
    ///     values are compared.
    ///     The method {@link #Append(double, double)} is used.
    /// </summary>
    /// <param name="lhs">the left hand double[]</param>
    /// <param name="rhs">the right hand double[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(double[] lhs, double[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i) Append(lhs[i], rhs[i]);
        return this;
    }

    /// <summary>
    ///     Deep comparison of array of long. Length and all
    ///     values are compared.
    ///     The method {@link #Append(float, float)} is used.
    /// </summary>
    /// <param name="lhs">the left hand float[]</param>
    /// <param name="rhs">the right hand float[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(float[] lhs, float[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i) Append(lhs[i], rhs[i]);
        return this;
    }

    /// <summary>
    ///     Deep comparison of array of long. Length and all
    ///     values are compared.
    ///     The method {@link #Append(boolean, boolean)} is used.
    /// </summary>
    /// <param name="lhs">the left hand boolean[]</param>
    /// <param name="rhs">the right hand boolean[]</param>
    /// <returns>EqualsBuilder - used to chain calls.</returns>
    public EqualsBuilder Append(bool[] lhs, bool[] rhs)
    {
        if (isEqual == false) return this;
        if (lhs == rhs) return this;
        if (lhs == null || rhs == null)
        {
            isEqual = false;
            return this;
        }

        if (lhs.Length != rhs.Length)
        {
            isEqual = false;
            return this;
        }

        for (var i = 0; i < lhs.Length && isEqual; ++i) Append(lhs[i], rhs[i]);
        return this;
    }

    /// <summary>
    ///     Return true if the fields that have been checked are all equal.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsEquals()
    {
        return isEqual;
    }


    private static bool isTransient(FieldInfo fieldInfo)
    {
        return (fieldInfo.Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized;
    }
}
}