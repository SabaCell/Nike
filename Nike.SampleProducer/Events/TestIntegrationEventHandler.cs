using System.Threading.Tasks;
using Nike.EventBus.Events;
using Nike.Mediator.Handlers;

namespace Nike.SampleProducer.Events
{

  

    public class TestIntegrationEventHandler:IntegrationEventHandler<TestIntegrationEvent> 
    {
        public override Task HandleAsync(TestIntegrationEvent @event)
        {
            throw new System.NotImplementedException();
        }
    }
}