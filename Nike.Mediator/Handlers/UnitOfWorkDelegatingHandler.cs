using Enexure.MicroBus;
using Microsoft.Extensions.Caching.Distributed;
using Nike.EventBus.Events;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Exceptions;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nike.Mediator.Handlers
{
    public class UnitOfWorkDelegatingHandler : IDelegatingHandler
    {
        private readonly IMicroMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _distributedCache;
        private const int ProcessedMessageAbsoluteExpirationTime = 30;
        private const string Key = "processed-message-results:{0}";

        public UnitOfWorkDelegatingHandler(IUnitOfWork unitOfWork, IMicroMediator mediator, IDistributedCache distributedCache)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _distributedCache = distributedCache;
        }

        public async Task<object> Handle(INextHandler next, object message)
        {
            var @event = message as IntegrationEvent;
            var key = string.Format(Key, @event.Id);

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

                await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(ProcessedMessageResult.Success()),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(ProcessedMessageAbsoluteExpirationTime)
                    });

                return result;
            }
            catch (DomainException exception)
            {
                _unitOfWork.Rollback();

                await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(ProcessedMessageResult.Fail(exception.Message)),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(ProcessedMessageAbsoluteExpirationTime)
                    });

                throw;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();

                await _distributedCache.SetStringAsync(key,
                    JsonSerializer.Serialize(ProcessedMessageResult.Fail("متاسفانه خطای سیستمی رخ داده است")),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(ProcessedMessageAbsoluteExpirationTime)
                    });

                throw;
            }
        }
    }

    public class ProcessedMessageResult
    {
        public static ProcessedMessageResult Success()
        {
            return new ProcessedMessageResult { IsSuccess = true };
        }

        public static ProcessedMessageResult Fail(string failureReason)
        {
            return new ProcessedMessageResult { IsSuccess = false, FailureReason = failureReason };
        }

        public bool IsSuccess { get; set; }

        public string FailureReason { get; set; }
    }
}