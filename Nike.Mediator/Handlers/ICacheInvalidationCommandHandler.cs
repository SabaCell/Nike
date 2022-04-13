using Enexure.MicroBus;
using Nike.Mediator.Command;

namespace Nike.Mediator.Handlers;

public interface ICacheInvalidationCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICacheInvalidationCommand
{
}