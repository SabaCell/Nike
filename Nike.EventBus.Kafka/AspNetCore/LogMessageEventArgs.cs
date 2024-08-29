using System;

namespace Nike.EventBus.Kafka.AspNetCore;

internal enum MqLogType
{
    ConsumeError,
    ConsumeRetries,
    ServerConnError,
}

internal class LogMessageEventArgs : EventArgs
{
    public string Reason { get; set; }

    public MqLogType LogType { get; set; }
}