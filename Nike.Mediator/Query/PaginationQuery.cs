namespace Nike.Mediator.Query;

public abstract class PaginationQuery<T> : QueryBase<T> where T : class
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }
}