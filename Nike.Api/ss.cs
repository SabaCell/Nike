using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nike.EventBus.Events;
using Nike.Mediator.Handlers;

namespace Nike.Api;

public class SwitchScanResponseIntegrationEvent : IntegrationEvent
{
    public Guid RequestId { get; set; }
    public Switch Switch { get; set; }
}

public class Switch
{
    public string Description { get; set; }
    public string Host { get; set; }
    public string UpTime { get; }
    public string HostName { get; set; }
    public List<InterfaceInfo> Interfaces { get; set; }
}

public class InterfaceInfo
{
    public int Index { get; set; }

    public string Port { get; set; }
    // public int PortNumber => int.Parse(Port);

    public string Description { get; set; }
    public string Status { get; set; }
    public List<Device> Devices { get; }
    public string Speed { get; set; }
}

public class Device
{
    public string MacAddress { get; set; }
    public string Port { get; set; }
}

public class SwitchScanResponseIntegrationEventHandler : IntegrationEventHandler<SwitchScanResponseIntegrationEvent>
{
    public override Task HandleAsync(SwitchScanResponseIntegrationEvent @event)
    {
        Console.WriteLine("hello worlkd");
        return Task.CompletedTask;
    }
}