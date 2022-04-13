using System;
using System.Reflection;

namespace Nike.Persistence.EventStore;

public interface IEventTypeResolver
{
    void AddTypesFromAssembly(Assembly assembly);
    Type GetType(string typeName);
}