using Microsoft.Extensions.Logging;
using Nike.Benchmark.ConsumerPerformance.Models;
using Nike.EventBus.Handlers;
using Nike.Framework.Domain;

namespace Nike.Benchmark.ConsumerPerformance.Handler
{
    public class Request2MessageHandler : IntegrationEventHandler<MyRequestMessage2>
    {
        private readonly ILogger<Request2MessageHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public Request2MessageHandler(ILogger<Request2MessageHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }
        public override async Task HandleAsync(MyRequestMessage2 @event)
        {
            var domain = new Message2AggregateRoot(@event.Identifier, @event.Name, @event.InstantiateAt,
                @event.ProduceAt, DateTime.Now);
            //_repository.Add(domain);
            domain.Store();
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }
    }
}


//