#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Nike.Framework.Domain;

[Serializable]
[DebuggerDisplay("{Name} - {Identifier}")]
public abstract class Enumeration<TEnumeration, TIdentifier> : IComparable<TEnumeration>, IEquatable<TEnumeration>
    where TEnumeration : Enumeration<TEnumeration, TIdentifier>
    where TIdentifier : IComparable
{
    public static IReadOnlyCollection<TEnumeration> Values;
    // static Enumeration()
    // {
    //     var enumerationType = typeof(TEnumeration);
    //
    //     var fields = enumerationType
    //         .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
    //         .Where(info => enumerationType.IsAssignableFrom(info.FieldType))
    //         .Select(info => info.GetValue(null))
    //         .Cast<TEnumeration>();
    //
    //     Values = fields.ToList().AsReadOnly();
    // }

    protected Enumeration(string name)
    {
        Name = name;
    }

    public string Name { get; }

    /// <summary>
    ///     Represents the value that identifies current object
    /// </summary>
    /// <remarks>
    ///     this property should return the value the make this object unique
    /// </remarks>
    protected abstract TIdentifier Identifier { get; }

    protected void SetValues()
    {
        var enumerationType = typeof(TEnumeration);

        var f = enumerationType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        var fv = f.Select(info => info.GetValue(null));
        var fvc = fv.Cast<TEnumeration>();

        var fields = enumerationType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(info => enumerationType.IsAssignableFrom(info.FieldType))
            .Select(info => info.GetValue(null))
            .Cast<TEnumeration>();

        Values = fields.ToList().AsReadOnly();
    }

    public override string ToString()
    {
        return Name;
    }

    #region Parse Methods

    public static bool TryParse(TIdentifier identifier, out TEnumeration result)
    {
        result = Values.FirstOrDefault(item => item.Identifier.Equals(identifier));
        // result = Values.SingleOrDefault(item => item.Identifier.Equals(identifier));

        return result != null;
    }

    public static bool TryParseName(string name, out TEnumeration result)
    {
        result = Values.FirstOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        return result != null;
    }

    public static TEnumeration Parse(TIdentifier identifier)
    {
        if (TryParse(identifier, out var enumeration)) return enumeration;

        var message = $"'{identifier}' is not a valid identifier in {typeof(TEnumeration)}";

        throw new ArgumentException(message, nameof(identifier));
    }

    public static TEnumeration ParseName(string name)
    {
        if (TryParseName(name, out var enumeration)) return enumeration;

        var message = $"'{name}' is not a valid name in {typeof(TEnumeration)}";

        throw new ArgumentException(message, nameof(name));
    }

    #endregion

    #region Equality Methods

    public int CompareTo(TEnumeration other)
    {
        return Identifier.CompareTo(other.Identifier);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as TEnumeration);
    }

    public bool Equals(TEnumeration other)
    {
        return other != null && Identifier.Equals(other.Identifier);
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
    }

    #endregion
}

public abstract class Enumeration<TEnumeration> : Enumeration<TEnumeration, int>
    where TEnumeration : Enumeration<TEnumeration, int>
{
    protected Enumeration(int id, string name) : base(name)
    {
        Id = id;
        SetValues();
    }

    public int Id { get; protected set; }

    protected override int Identifier => Id;
}

public abstract class ByteEnumeration<TEnumeration> : Enumeration<TEnumeration, byte>
    where TEnumeration : Enumeration<TEnumeration, byte>
{
    protected ByteEnumeration(byte id, string name) : base(name)
    {
        Id = id;
        SetValues();
    }

    public byte Id { get; protected set; }

    protected override byte Identifier => Id;
}