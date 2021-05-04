using Enexure.MicroBus;

namespace Nike.Mediator.Command
{
    public abstract class UserDataCommandBase : ICommand
    {
        public UserData UserData { get; set; }

        public void SetUserData(UserData userInfo)
        {
            this.UserData = userInfo;
        }

        public abstract void Validate();
    }
}