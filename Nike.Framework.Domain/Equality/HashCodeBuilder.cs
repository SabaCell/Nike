#region The Apache Software License

/*
 *  Copyright 2001-2004 The Apache Software Foundation
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *  ported to C# by Artur Trosin
 */

#endregion

using System;
using System.Reflection;

namespace Nike.Framework.Domain.Equality;

#region Usings

#endregion

/// <summary>
///Assists in implementing object.GetHashCode() methods.
/// This class enables a good GetHashCode method to be built for any class. It
///follows the rules laid out in the book
///<a HREF="http://java.sun.com/docs/books/effective/index.html">Effective Java</a>
///by Joshua Bloch. Writing a good GetHashCode method is actually quite
///difficult. This class aims to simplify the process.
///All relevant fields from the object should be included in the
///GetHashCode method. Derived fields may be excluded. In general, any
///field used in the equals method must be used in the GetHashCode
///method.
///To use this class write code as follows:
///
///public class Person {
///String name;
///int age;
///boolean isSmoker;
///...
///public int GetHashCode() {
// you pick a hard-coded, randomly chosen, non-zero, odd number
// ideally different for each class
/// return new HashCodeBuilder(17, 37).
/// Append(name).
/// Append(age).
/// Append(smoker).
/// ToHashCode();
/// }
/// }
/// 
/// If required, the superclass hashCode() can be added
/// using {@link #AppendSuper}.
/// Alternatively, there is a method that uses reflection to determine
/// the fields to test. Because these fields are usually private, the method,
/// reflectionHashCode, uses AccessibleObject.setAccessible to
/// change the visibility of the fields. This will fail under a security manager,
/// unless the appropriate permissions are set up correctly. It is also slower
/// than testing explicitly.
/// A typical invocation for this method would look like:
/// 
/// public int hashCode() {
/// return HashCodeBuilder.reflectionHashCode(this);
/// }
/// 
/// @author Stephen Colebourne
/// @author Gary Gregory
/// @author Pete Gieser    
/// @since 1.0    
/// @version $Id: HashCodeBuilder.java 161243 2005-04-14 04:30:28Z ggregory $
/// </summary>
public class HashCodeBuilder
{
	/// <summary>
	///     Constant to use in building the hashCode.
	/// </summary>
	private readonly int iConstant;

	/// <summary>
	///     Running total of the hashCode.
	/// </summary>
	private int iTotal;

	/// <summary>
	///     Constructor.
	///     This constructor uses two hard coded choices for the constants
	///     needed to build a GetHashCode
	/// </summary>
	public HashCodeBuilder()
    {
        iConstant = 37;
        iTotal = 17;
    }

	/// <summary>
	///     Constructor.
	///     Two randomly chosen, non-zero, odd numbers must be passed in.
	///     Ideally these should be different for each class, however this is
	///     not vital.
	///     Prime numbers are preferred, especially for the multiplier.
	///     throws ArgumentException if the number is zero or even
	/// </summary>
	/// <param name="initialNonZeroOddNumber">a non-zero, odd number used as the initial value</param>
	/// <param name="multiplierNonZeroOddNumber">a non-zero, odd number used as the multiplier</param>
	public HashCodeBuilder(int initialNonZeroOddNumber, int multiplierNonZeroOddNumber)
    {
        if (initialNonZeroOddNumber == 0)
            throw new ArgumentException("HashCodeBuilder requires a non zero initial value");
        if (initialNonZeroOddNumber % 2 == 0)
            throw new ArgumentException("HashCodeBuilder requires an odd initial value");
        if (multiplierNonZeroOddNumber == 0)
            throw new ArgumentException("HashCodeBuilder requires a non zero multiplier");
        if (multiplierNonZeroOddNumber % 2 == 0)
            throw new ArgumentException("HashCodeBuilder requires an odd multiplier");
        iConstant = multiplierNonZeroOddNumber;
        iTotal = initialNonZeroOddNumber;
    }

	/// <summary>
	///     This method uses reflection to build a valid hash code.
	///     This constructor uses two hard coded choices for the constants
	///     needed to build a hash code.
	///     It uses AccessibleObject.setAccessible to gain access to private
	///     fields. This means that it will throw a security exception if run under
	///     a security manager, if the permissions are not set up correctly. It is
	///     also not as efficient as testing explicitly.
	///     Transient members will be not be used, as they are likely derived
	///     fields, and not part of the value of the Object
	///     Static fields will not be tested. Superclass fields will be included.
	///     throws ArgumentException if the object is null
	/// </summary>
	/// <param name="obj">the Object to create a GetHashCode</param>
	/// <returns>hash code</returns>
	public static int ReflectionHashCode(object obj)
    {
        return ReflectionHashCode(17, 37, obj, false, null);
    }

	/// <summary>
	///     This method uses reflection to build a valid hash code.
	///     This constructor uses two hard coded choices for the constants needed
	///     to build a hash code.
	///     It uses AccessibleObject.setAccessible to gain access to private
	///     fields. This means that it will throw a security exception if run under
	///     a security manager, if the permissions are not set up correctly. It is
	///     also not as efficient as testing explicitly.
	///     If the TestTransients parameter is set to true, transient
	///     members will be tested, otherwise they are ignored, as they are likely
	///     derived fields, and not part of the value of the Object.
	///     Static fields will not be tested. Superclass fields will be included.
	///     Throws IllegalArgumentException if the object is null
	/// </summary>
	/// <param name="obj">the Object to create a hashCode</param>
	/// <param name="testTransients">whether to include transient fields</param>
	/// <returns>hash code</returns>
	public static int ReflectionHashCode(object obj, bool testTransients)
    {
        return ReflectionHashCode(17, 37, obj, testTransients, null);
    }

	/// <summary>
	///     This method uses reflection to build a valid hash code.
	///     It uses AccessibleObject.setAccessible to gain access to private
	///     fields. This means that it will throw a security exception if run under
	///     a security manager, if the permissions are not set up correctly. It is
	///     also not as efficient as testing explicitly.
	///     Transient members will be not be used, as they are likely derived
	///     fields, and not part of the value of the Object.
	///     Static fields will not be tested. Superclass fields will be included.
	///     Two randomly chosen, non-zero, odd numbers must be passed in. Ideally
	///     these should be different for each class, however this is not vital.
	///     Prime numbers are preferred, especially for the multiplier.
	///     Throws IllegalArgumentException if the Object is null
	///     Throws IllegalArgumentException if the number is zero or even
	/// </summary>
	/// <param name="initialNonZeroOddNumber">a non-zero, odd number used as the initial value</param>
	/// <param name="multiplierNonZeroOddNumber">a non-zero, odd number used as the multiplier</param>
	/// <param name="obj">the Object to create a hashCode for</param>
	/// <returns>hash code</returns>
	public static int ReflectionHashCode(
        int initialNonZeroOddNumber, int multiplierNonZeroOddNumber, object obj)
    {
        return ReflectionHashCode(initialNonZeroOddNumber, multiplierNonZeroOddNumber, obj, false, null);
    }

	/// <summary>
	///     This method uses reflection to build a valid hash code.
	///     It uses AccessibleObject.setAccessible to gain access to private
	///     fields. This means that it will throw a security exception if run under
	///     a security manager, if the permissions are not set up correctly. It is also
	///     not as efficient as testing explicitly.
	///     If the TestTransients parameter is set to true, transient
	///     members will be tested, otherwise they are ignored, as they are likely
	///     derived fields, and not part of the value of the Object.
	///     Static fields will not be tested. Superclass fields will be included.
	///     Two randomly chosen, non-zero, odd numbers must be passed in. Ideally
	///     these should be different for each class, however this is not vital.
	///     Prime numbers are preferred, especially for the multiplier.
	///     Throws IllegalArgumentException if the Object is null
	///     Throws IllegalArgumentException if the number is zero or even
	/// </summary>
	/// <param name="initialNonZeroOddNumber">a non-zero, odd number used as the initial value</param>
	/// <param name="multiplierNonZeroOddNumber">a non-zero, odd number used as the multiplier</param>
	/// <param name="obj">the Object to create a hashCode for</param>
	/// <param name="testTransients">whether to include transient fields</param>
	/// <returns>hash code</returns>
	public static int ReflectionHashCode(
        int initialNonZeroOddNumber, int multiplierNonZeroOddNumber,
        object obj, bool testTransients)
    {
        return ReflectionHashCode(initialNonZeroOddNumber, multiplierNonZeroOddNumber, obj, testTransients, null);
    }

	/// <summary>
	///     This method uses reflection to build a valid hash code.
	///     It uses AccessibleObject.setAccessible to gain access to private
	///     fields. This means that it will throw a security exception if run under
	///     a security manager, if the permissions are not set up correctly. It is also
	///     not as efficient as testing explicitly.
	///     If the TestTransients parameter is set to true, transient
	///     members will be tested, otherwise they are ignored, as they are likely
	///     derived fields, and not part of the value of the Object.
	///     Static fields will not be included. Superclass fields will be included
	///     up to and including the specified superclass. A null superclass is treated
	///     as java.lang.Object.
	///     Two randomly chosen, non-zero, odd numbers must be passed in. Ideally
	///     these should be different for each class, however this is not vital.
	///     Prime numbers are preferred, especially for the multiplier.
	///     Throws IllegalArgumentException if the Object is null
	///     Throws IllegalArgumentException if the number is zero or even since 2.0
	/// </summary>
	/// <param name="initialNonZeroOddNumber">a non-zero, odd number used as the initial value</param>
	/// <param name="multiplierNonZeroOddNumber">a non-zero, odd number used as the multiplier</param>
	/// <param name="obj">the Object to create a hashCode for</param>
	/// <param name="reflectUpToClass">whether to include transient fields</param>
	/// <param name="testTransients">the superclass to reflect up to (inclusive),may be null</param>
	/// <returns>hash code</returns>
	public static int ReflectionHashCode(
        int initialNonZeroOddNumber,
        int multiplierNonZeroOddNumber,
        object obj,
        bool testTransients,
        Type reflectUpToClass)
    {
        if (obj == null) throw new ArgumentException("The object to build a hash code for must not be null");
        var builder = new HashCodeBuilder(initialNonZeroOddNumber, multiplierNonZeroOddNumber);
        var clazz = obj.GetType();
        reflectionAppend(obj, clazz, builder, testTransients);
        while (clazz.BaseType != null && clazz != reflectUpToClass)
        {
            clazz = clazz.BaseType;
            reflectionAppend(obj, clazz, builder, testTransients);
        }

        return builder.ToHashCode();
    }

	/// <summary>
	///     Appends the fields and values defined by the given object of the given Class.
	/// </summary>
	/// <param name="builder">the builder to Append to</param>
	/// <param name="clazz">the class to Append details of</param>
	/// <param name="obj">the object to Append details of</param>
	/// <param name="useTransients">whether to use transient fields</param>
	private static void reflectionAppend(object obj, Type clazz, HashCodeBuilder builder, bool useTransients)
    {
        var fields =
            clazz.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                            | BindingFlags.GetField);
        //AccessibleObject.setAccessible(fields, true);
        for (var i = 0; i < fields.Length; i++)
        {
            var f = fields[i];

            if (f.Name.IndexOf('$') == -1
                && (useTransients || !isTransient(f))
                && !f.IsStatic)
                try
                {
                    builder.Append(f.GetValue(obj));
                }
                catch (Exception e)
                {
                    //this can't happen. Would get a Security exception instead
                    //throw a runtime exception in case the impossible happens.
                    throw new Exception("Unexpected IllegalAccessException");
                }
        }
    }

	/// <summary>
	///     Adds the result of super.hashCode() to this builder.
	/// </summary>
	/// <param name="superHashCode">the result of calling super.hashCode()</param>
	/// <returns>HashCodeBuilder, used to chain calls.</returns>
	public HashCodeBuilder AppendSuper(int superHashCode)
    {
        iTotal = iTotal * iConstant + superHashCode;
        return this;
    }

	/// <summary>
	///     Append a hashCode for an Object.
	/// </summary>
	/// <param name="obj">the Object to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(object obj)
    {
        if (obj == null)
        {
            iTotal = iTotal * iConstant;
        }
        else
        {
            if (obj.GetType().IsArray == false)
            {
                //the simple case, not an array, just the element
                iTotal = iTotal * iConstant + obj.GetHashCode();
            }
            else
            {
                //'Switch' on type of array, to dispatch to the correct handler
                // This handles multi dimensional arrays
                if (obj is long[])
                    Append((long[]) obj);
                else if (obj is int[])
                    Append((int[]) obj);
                else if (obj is short[])
                    Append((short[]) obj);
                else if (obj is char[])
                    Append((char[]) obj);
                else if (obj is byte[])
                    Append((byte[]) obj);
                else if (obj is double[])
                    Append((double[]) obj);
                else if (obj is float[])
                    Append((float[]) obj);
                else if (obj is bool[])
                    Append((bool[]) obj);
                else
                    // Not an array of primitives
                    Append((object[]) obj);
            }
        }

        return this;
    }

	/// <summary>
	///     Append a hashCode for a long.
	/// </summary>
	/// <param name="value">the long to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(long value)
    {
        iTotal = iTotal * iConstant + (int) (value ^ (value >> 32));
        return this;
    }

	/// <summary>
	///     Append a hashCode for an int.
	/// </summary>
	/// <param name="value">the int to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(int value)
    {
        iTotal = iTotal * iConstant + value;
        return this;
    }

	/// <summary>
	///     Append a hashCode for a short.
	/// </summary>
	/// <param name="value">the short to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(short value)
    {
        iTotal = iTotal * iConstant + value;
        return this;
    }

	/// <summary>
	///     Append a hashCode for a char.
	/// </summary>
	/// <param name="value">the char to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(char value)
    {
        iTotal = iTotal * iConstant + value;
        return this;
    }

	/// <summary>
	///     Append a hashCode for a byte.
	/// </summary>
	/// <param name="value">the byte to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(byte value)
    {
        iTotal = iTotal * iConstant + value;
        return this;
    }

	/// <summary>
	///     Append a hashCode for a double.
	/// </summary>
	/// <param name="value">the double to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(double value)
    {
        return Append(Convert.ToInt64(value));
    }

	/// <summary>
	///     Append a hashCode for a float.
	/// </summary>
	/// <param name="value">the float to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(float value)
    {
        iTotal = iTotal * iConstant + Convert.ToInt32(value); /* Float.floatToIntBits(value);*/
        return this;
    }

	/// <summary>
	///     Append a hashCode for a boolean.
	/// </summary>
	/// <param name="value">the boolean to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(bool value)
    {
        iTotal = iTotal * iConstant + (value ? 0 : 1);
        return this;
    }

	/// <summary>
	///     Append a hashCode for an Object array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(object[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Append a hashCode for a long array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(long[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Append a hashCode for a int array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(int[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Append a hashCode for a short array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(short[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Append a hashCode for a char array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(char[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Append a hashCode for a byte array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(byte[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Append a hashCode for a double array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(double[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Append a hashCode for a float array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(float[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Append a hashCode for a boolean array.
	/// </summary>
	/// <param name="array">the array to add to the hashCode</param>
	/// <returns>this</returns>
	public HashCodeBuilder Append(bool[] array)
    {
        if (array == null)
            iTotal = iTotal * iConstant;
        else
            for (var i = 0; i < array.Length; i++)
                Append(array[i]);
        return this;
    }

	/// <summary>
	///     Return the computed hashCode.
	/// </summary>
	/// <returns>hashCode based on the fields appended</returns>
	public int ToHashCode()
    {
        return iTotal;
    }

    private static bool isTransient(FieldInfo fieldInfo)
    {
        return (fieldInfo.Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized;
    }
}