using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Handlers;

public static class TopicHelper
{
    public static Dictionary<string, Type> GetLiveTopics()
    {
        var topics = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes().Where(p =>
                p.IsGenericType == false && IsSubclassOfRawGeneric(typeof(IntegrationEventHandler<>), p))).Distinct()
            .ToList();

        var results = new Dictionary<string, Type>();
        foreach (var topic in topics)
        {
            var topicName = "";
            var type = topic.BaseType?.GetGenericArguments();

            if (type == null) continue;
            var attribute = GetAttribute(type[0]);
            topicName = attribute == null ? type[0].Name : attribute.TopicName;
            results.Add(topicName, type[0]);
        }

        return results;
    }
    private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur) return true;

            toCheck = toCheck.BaseType;
        }

        return false;
    }

    public static TopicAttribute GetAttribute(Type type)
    {
        var attributes = type.GetCustomAttributes();

        foreach (var attribute in attributes)
            if (attribute is TopicAttribute topicAttribute)
                return topicAttribute;

        return null;
    }
}