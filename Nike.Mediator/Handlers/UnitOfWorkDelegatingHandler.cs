using Enexure.MicroBus;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nike.Mediator.Handlers
{
    public class UnitOfWorkDelegatingHandler : IDelegatingHandler
    {
        private readonly IMicroMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBusDispatcher _bus;

        public UnitOfWorkDelegatingHandler(
            IUnitOfWork unitOfWork,
            IMicroMediator mediator,
            IEventBusDispatcher bus)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _bus = bus;
        }

        public async Task<object> Handle(INextHandler next, object message)
        {
            var @event = message as IntegrationEvent;

            try
            {
                var events = _unitOfWork.GetUncommittedEvents().ToList();

                var count = events.Count;
                foreach (var domainEvent in events) await _mediator.PublishEventAsync(domainEvent);

                var result = await next.Handle(message);

                //domain event not occured 
                if (count <= 0)
                    // command event not occured too 
                    if (!(message is ICommand))
                        // so we dont need to Commit to db 
                        return result;

                await _unitOfWork.CommitAsync();

                await _bus.PublishAsync(ProcessedMessageResultIntegrationEvent.Success(@event.Id.ToString()));

                return result;
            }
            catch (DomainException exception)
            {
                _unitOfWork.Rollback();

                await _bus.PublishAsync(ProcessedMessageResultIntegrationEvent.Fail(@event.Id.ToString(), exception.Message));

                throw;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();

                await _bus.PublishAsync(ProcessedMessageResultIntegrationEvent.Fail(@event.Id.ToString(), "متاسفانه خطای سیستمی رخ داده است"));

                throw;
            }
        }
    }

    public class ProcessedMessageResultIntegrationEvent : IntegrationEvent
    {
        public static ProcessedMessageResultIntegrationEvent Success(string eventId)
        {
            return new ProcessedMessageResultIntegrationEvent { EventId = eventId, IsSuccess = true };
        }

        public static ProcessedMessageResultIntegrationEvent Fail(string eventId, string failureReason)
        {
            return new ProcessedMessageResultIntegrationEvent { EventId = eventId, IsSuccess = false, FailureReason = failureReason };
        }

        public string EventId { get; set; }

        public bool IsSuccess { get; set; }

        public string FailureReason { get; set; }
    }
}