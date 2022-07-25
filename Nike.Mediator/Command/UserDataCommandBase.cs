using Enexure.MicroBus;
using Nike.Mediator.Models;

namespace Nike.Mediator.Command;

public abstract class UserDataCommandBase : ICommand
{
    private UserData _userData;

    public void SetUserData(UserData userInfo)
    {
        _userData = userInfo;
    }

    public UserData GetUserData()
    {
        return _userData;
    }

    public abstract void Validate();
}