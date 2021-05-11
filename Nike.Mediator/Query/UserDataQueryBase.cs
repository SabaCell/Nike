namespace Nike.Mediator.Query
{
    public abstract class UserDataQueryBase<T> : QueryBase<T> where T : class
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