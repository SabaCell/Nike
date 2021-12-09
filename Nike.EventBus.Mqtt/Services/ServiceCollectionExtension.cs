using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Mqtt.Model;
using Nike.EventBus.Mqtt.Options;

namespace Nike.EventBus.Mqtt.Services
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMqttClientHostedService(this IServiceCollection services,
            MqttSetting mqttSetting)
        {
            services.AddMqttClientServiceWithConfig(aspOptionBuilder =>
            {
                aspOptionBuilder
                    .WithCredentials(mqttSetting.Username, mqttSetting.Password)
                    .WithClientId(mqttSetting.ClientId)
                    .WithTcpServer(mqttSetting.Host, mqttSetting.Port);
            });
            return services;
        }

        private static IServiceCollection AddMqttClientServiceWithConfig(this IServiceCollection services,
            Action<AspCoreMqttClientOptionBuilder> configure)
        {
            services.AddSingleton(serviceProvider =>
            {
                var optionBuilder = new AspCoreMqttClientOptionBuilder(serviceProvider);
                configure(optionBuilder);
                return optionBuilder.Build();
            });
            services.AddSingleton<IEventBusDispatcher, MqttEventBusDispatcher>();
            services.AddSingleton<MqttClientService>();
            services.AddSingleton<IHostedService>(serviceProvider => serviceProvider.GetService<MqttClientService>());
            services.AddSingleton(serviceProvider =>
            {
                var mqttClientService = serviceProvider.GetService<MqttClientService>();
                var mqttClientServiceProvider = new MqttClientServiceProvider(mqttClientService);
                return mqttClientServiceProvider;
            });
            return services;
        }
    }
}