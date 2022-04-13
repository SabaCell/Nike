using Nike.EventBus.Events;

namespace Nike.SampleProducer.Events;

//  [Topic(@"mostahas\sadasd\asda\")]
public class MonitorIntegrationEvent : IntegrationEvent
{
    public NetworkDevice? DeviceAddress { get; set; }
    public ushort IntervalInSeconds { get; set; }
}

public class NetworkDevice
{
    public string DeviceIP { get; set; }
    public string DeviceName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}