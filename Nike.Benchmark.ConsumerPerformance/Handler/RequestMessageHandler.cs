using System;
using System.Threading;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Microsoft.Extensions.Logging;
using Nike.Benchmark.ConsumerPerformance.Models;
using Nike.EventBus.Handlers;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Persistence;

namespace Nike.Benchmark.ConsumerPerformance.Handler
{
    public class RequestMessageHandler : IntegrationEventHandler<MyRequestMessage>
    {
        private readonly ILogger<RequestMessageHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RequestMessageHandler(ILogger<RequestMessageHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        public override async Task HandleAsync(MyRequestMessage @event)
        {
            var domain = new MessageAggregateRoot(@event.Identifier, @event.Name, @event.InstantiateAt,
                @event.ProduceAt, DateTime.Now);
            //_repository.Add(domain);
            domain.Store();
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }
    }
}


//