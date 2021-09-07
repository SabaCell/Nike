using System;
using System.Text.Json.Serialization;

namespace Nike.EventBus.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationAt = DateTime.UtcNow;
        }

        public IntegrationEvent(Guid id, bool isReplyAble, DateTime createDate)
        {
            Id = id;
            IsReplyAble = isReplyAble;
            CreationAt = createDate;
        }

        [JsonPropertyName("Id")]
        public Guid Id { get; set; }

        public bool IsReplyAble { get; set; }

        [JsonPropertyName("CreationAt")]
        public DateTime CreationAt { get; private set; }
    }
}