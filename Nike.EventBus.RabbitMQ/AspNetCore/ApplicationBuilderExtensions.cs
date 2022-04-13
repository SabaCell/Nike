using System;
using System.Reflection;
using EasyNetQ.AutoSubscribe;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Nike.EventBus.RabbitMQ.AspNetCore;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRabbitMq(this IApplicationBuilder applicationBuilder,
        string subscriptionIdPrefix = null, Assembly consumerAssembly = null)
    {
        consumerAssembly ??= Assembly.GetEntryAssembly();

        if (consumerAssembly is null) throw new ArgumentNullException(nameof(consumerAssembly));

        var services = applicationBuilder.ApplicationServices;

        var lifeTime = services.GetService<IApplicationLifetime>();
        var rabbitMqConnection = services.GetService<IRabbitMqConnection>();

        lifeTime.ApplicationStarted.Register(() =>
        {
            var subscriber = new AutoSubscriber(rabbitMqConnection.Bus, subscriptionIdPrefix)
            {
                AutoSubscriberMessageDispatcher =
                    new MicrosoftDependencyInjectionMessageDispatcher(applicationBuilder.ApplicationServices)
            };

            subscriber.Subscribe(new[] {consumerAssembly});
            subscriber.SubscribeAsync(new[] {consumerAssembly});
        });

        lifeTime.ApplicationStopped.Register(() => rabbitMqConnection.Bus.Dispose());

        return applicationBuilder;
    }
}