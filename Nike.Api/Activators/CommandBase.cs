using Enexure.MicroBus;

namespace Nike.Api.Activators
{
    public abstract class CommandBase : ICommand
    {
        public abstract void Validate();
    }
}