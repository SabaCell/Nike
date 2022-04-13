using System;
using System.Text.Json.Serialization;

namespace Nike.EventBus.Events;

public class IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationAt = DateTime.UtcNow;
    }

    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationAt = createDate;
    }

    [JsonPropertyName("Id")] public Guid Id { get; set; }


    [JsonPropertyName("CreationAt")] public DateTime CreationAt { get; }
}