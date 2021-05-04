namespace Nike.Mediator.Query
{
    public abstract class UserDataQueryBase<T> : QueryBase<T> where T : class
    {
        public UserData UserData { get; set; }

        public void SetUserData(UserData userInfo)
        {
            this.UserData = userInfo;
        }

        public abstract void Validate();
    }
}