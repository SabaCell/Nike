using System;

namespace Nike.EventBus.Abstractions
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class TopicAttribute : Attribute
    {
        public TopicAttribute(string topicName, QualityOfServiceLevel serviceLevel = QualityOfServiceLevel.AtLeastOnce)
        {
            TopicName = topicName;
            ServiceLevel = serviceLevel;
        }

        public string TopicName { get; }
        public QualityOfServiceLevel ServiceLevel { get; }
    }
}