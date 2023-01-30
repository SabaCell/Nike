using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;

namespace Nike.EventBus.Mqtt.Services
{
    public interface IMqttClientService : IHostedService,
        IMqttClientConnectedHandler,
        IMqttClientDisconnectedHandler,
        IMqttApplicationMessageReceivedHandler
    {
        Task PublishAsync(MqttApplicationMessage msg);
    }
}