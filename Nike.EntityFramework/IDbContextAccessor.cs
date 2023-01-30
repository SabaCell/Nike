namespace Nike.EntityFramework
{
    public interface IDbContextAccessor
    {
        DbContextBase Context { get; }
    }
}