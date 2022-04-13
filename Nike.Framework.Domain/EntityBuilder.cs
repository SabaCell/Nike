using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nike.Framework.Domain;

public abstract class EntityBuilder<TEntity> where TEntity : class
{
    protected virtual TEntity Entity { get; } = (TEntity) FormatterServices.GetUninitializedObject(typeof(TEntity));

    protected void Set<TProperty, TValue>(Expression<Func<TEntity, TProperty>> expression, TValue value)
    {
        if (Entity is null)
            throw new ArgumentNullException(nameof(Entity));

        var prop = (PropertyInfo) ((MemberExpression) expression.Body).Member;

        if (prop.PropertyType != typeof(TValue))
            throw new Exception("type of values is not same as property type.");

        prop.SetValue(Entity, value, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
    }

    /// <summary>
    ///     Adds an item to a field that is type of collection
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <exception cref="Exception">In case the value is not same type as property type, exception is thrown</exception>
    /// <exception cref="Exception">If no field is found with fieldName, an exception is thrown</exception>
    protected void AddToCollection<TValue>(string fieldName, TValue value)
    {
        var fieldInfo = typeof(TEntity).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        if (fieldInfo is null) throw new Exception("field with this name is not found.");

        if (fieldInfo.FieldType.IsAssignableFrom(typeof(ICollection<>).MakeGenericType(typeof(TValue))))
            throw new Exception($"field type is not a collection of {typeof(TValue).Name}.");

        var fieldValue = fieldInfo.GetValue(Entity);

        if (fieldValue is null)
        {
            fieldValue = Activator.CreateInstance(fieldInfo.FieldType);
            fieldInfo.SetValue(Entity, fieldValue);
        }

        if (fieldInfo.FieldType.GenericTypeArguments[0] != typeof(TValue))
            throw new Exception("type of value is not same as list type.");

        fieldInfo.FieldType.GetMethod(nameof(ICollection<object>.Add))?.Invoke(fieldValue, new[] {(object) value});
    }

    public virtual TEntity Build()
    {
        return Entity;
    }
}