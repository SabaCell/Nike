namespace Nike.EventBus.Abstractions
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TopicAttribute : System.Attribute
    {
        public string TopicName { get; }
        public QualityOfServiceLevel ServiceLevel { get; }


        public TopicAttribute(string topicName, QualityOfServiceLevel serviceLevel = QualityOfServiceLevel.AtLeastOnce)
        {
            TopicName = topicName;
            ServiceLevel = serviceLevel;
        }
    }
}