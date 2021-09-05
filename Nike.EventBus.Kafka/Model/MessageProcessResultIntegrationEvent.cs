﻿using Nike.EventBus.Events;
using System;

namespace Nike.EventBus.Kafka.Model
{
    public class MessageProcessResultIntegrationEvent : IntegrationEvent
    {
        public static MessageProcessResultIntegrationEvent Success(Guid eventId)
        {
            return new MessageProcessResultIntegrationEvent { EventId = eventId, IsSuccess = true };
        }

        public static MessageProcessResultIntegrationEvent Fail(Guid eventId, string failureReason)
        {
            return new MessageProcessResultIntegrationEvent { EventId = eventId, IsSuccess = false, FailureReason = failureReason };
        }

        public Guid EventId { get; set; }

        public bool IsSuccess { get; set; }

        public string FailureReason { get; set; }
    }
}