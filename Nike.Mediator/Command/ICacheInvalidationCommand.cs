using System.Collections.Generic;
using Enexure.MicroBus;

namespace Nike.Mediator.Command
{
    public interface ICacheInvalidationCommand : ICommand
    {
        IEnumerable<string> GetKeys();
    }
}