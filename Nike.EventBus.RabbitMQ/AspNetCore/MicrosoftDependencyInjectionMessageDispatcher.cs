using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;

namespace Nike.EventBus.RabbitMQ.AspNetCore
{
    public class MicrosoftDependencyInjectionMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftDependencyInjectionMessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message,
            CancellationToken cancellationToken = default) where TMessage : class
            where TConsumer : class, IConsume<TMessage>
        {
            using (var scop = _serviceProvider.CreateScope())
            {
                var consumer = scop.ServiceProvider.GetRequiredService<TConsumer>();
                consumer.Consume(message);
            }
        }

        public async Task DispatchAsync<TMessage, TConsumer>(TMessage message,
            CancellationToken cancellationToken = default) where TMessage : class
            where TConsumer : class, IConsumeAsync<TMessage>
        {
            using (var scop = _serviceProvider.CreateScope())
            {
                var consumer = scop.ServiceProvider.GetRequiredService<TConsumer>();
                await consumer.ConsumeAsync(message);
            }
        }
    }
}