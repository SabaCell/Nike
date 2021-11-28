using Microsoft.Extensions.Logging;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Nike.EventBus.Mqtt.Model;

namespace Nike.EventBus.Mqtt.AspNetCore
{
    public static class ConsumerHelper
    {
        public static MqttServerOptionsBuilder WithUserNameAndPass(this MqttServerOptionsBuilder builder,
            MqttConsumerConfig mqttConfig, ILogger<ConsumerHostedService> logger)
        {
            if (string.IsNullOrEmpty(mqttConfig.Password) == false &&
                string.IsNullOrEmpty(mqttConfig.Username) == false)
            {
                builder = builder.WithConnectionValidator(
                    c =>
                    {
                        if (c.Username != mqttConfig.Username)
                        {
                            c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                            logger.LogError("Invalid UserName or Password!");
                        }
                        else
                        {
                            c.ReasonCode = MqttConnectReasonCode.Success;
                        }

                        if (c.Password != mqttConfig.Password)
                        {
                            c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                            logger.LogError("Invalid UserName or Password!");
                        }
                        else
                        {
                            c.ReasonCode = MqttConnectReasonCode.Success;
                        }
                    });
            }

            return builder;
        }

        public static MqttServerOptionsBuilder WithNikeClientId(this MqttServerOptionsBuilder builder,
            MqttConsumerConfig mqttConfig)
        {
            if (string.IsNullOrEmpty(mqttConfig.ClientId) == false)
            {
                builder = builder.WithClientId(mqttConfig.ClientId);
            }

            return builder;
        }
    }
}