namespace Nike.Mediator.Query
{
    public abstract class UserDataPaginationQuery<T> : UserDataQueryBase<T> where T : class
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}