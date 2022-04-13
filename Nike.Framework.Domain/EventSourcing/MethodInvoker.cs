using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Nike.Framework.Domain.EventSourcing;

/// <summary>
///     This is class is used to invoke 'When' methods on event sourcing aggregate roots
/// </summary>
internal static class MethodInvoker
{
    private static readonly MethodInfo InternalPreserveStackTraceMethod =
        typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly Dictionary<Type, List<WhenMethodMetadata>> WhenMethods = new();

    [DebuggerNonUserCode]
    internal static void Invoke(object instance, object domainEvent)
    {
        var aggregateRootType = instance.GetType();
        var domainEventType = domainEvent.GetType();

        var method = GetMethod(aggregateRootType, domainEventType);

        if (method == null) throw new Exception($"'When' method for '{domainEventType.Name}' is not implemented");

        try
        {
            method.AssociatedMethodInfo.Invoke(instance, new[] {domainEvent});
        }
        catch (TargetInvocationException ex)
        {
            if (null != InternalPreserveStackTraceMethod)
                InternalPreserveStackTraceMethod.Invoke(ex.InnerException, new object[0]);
            throw ex.InnerException;
        }
    }

    private static WhenMethodMetadata GetMethod(Type aggregateRootType, Type domainEventType)
    {
        var aggregateRootMethods = WhenMethods.GetValueOrDefault(aggregateRootType);

        if (aggregateRootMethods == null)
        {
            aggregateRootMethods = aggregateRootType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "When")
                .Where(m => m.GetParameters().Length == 1)
                .Select(m => new WhenMethodMetadata(m, m.GetParameters().First().ParameterType))
                .ToList();

            WhenMethods[aggregateRootType] = aggregateRootMethods;
        }

        return aggregateRootMethods.SingleOrDefault(m => m.DomainEventType == domainEventType);
    }

    private class WhenMethodMetadata
    {
        public WhenMethodMetadata(MethodInfo associatedMethodInfo, Type domainEventType)
        {
            AssociatedMethodInfo = associatedMethodInfo;
            DomainEventType = domainEventType;
        }

        public MethodInfo AssociatedMethodInfo { get; }
        public Type DomainEventType { get; }
    }
}