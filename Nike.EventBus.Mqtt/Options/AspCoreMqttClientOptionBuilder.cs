using System;
using MQTTnet.Client.Options;

namespace Nike.EventBus.Mqtt.Options
{
    public class AspCoreMqttClientOptionBuilder : MqttClientOptionsBuilder
    {
        public IServiceProvider ServiceProvider { get; }

        public AspCoreMqttClientOptionBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
