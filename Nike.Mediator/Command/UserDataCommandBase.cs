using Enexure.MicroBus;

namespace Nike.Mediator.Command
{
    public abstract class UserDataCommandBase : ICommand
    {
        private UserData _userData;

        public void SetUserData(UserData userInfo)
        {
            this._userData = userInfo;
        }
        public UserData GetUserData()
        {
            return _userData;
        }
        
        public abstract void Validate();
    }
}