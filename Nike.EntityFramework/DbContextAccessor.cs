using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Nike.Framework.Domain;

namespace Nike.EntityFramework
{
    public class DbContextAccessor : IDbContextAccessor
    {
        public DbContextAccessor(DbContextBase context)
        {
            Context = context;
        }

        public DbContextBase Context { get; }
    }

    public abstract class DbContextBase : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        protected DbContextBase()
        {
        }

        protected DbContextBase(DbContextOptions options) : base(options)
        {
            //   ChangeTracker.Tracked += OnEntityCreate;
            //   ChangeTracker.StateChanged += OnEntityUpdate;
        }

        protected abstract Assembly ConfigurationsAssembly { get; }

        public bool HasActiveTransaction => _currentTransaction != null;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            builder.Model
                .GetEntityTypes()
                .SelectMany(et => et.GetProperties())
                .Where(p => p.Name == nameof(IEntity<object>.CreatedAt)).ToList()
                .ForEach(p => p.SetAfterSaveBehavior(PropertySaveBehavior.Ignore));

            builder.ApplyConfigurationsFromAssembly(ConfigurationsAssembly);
        }
 

        protected virtual void OnEntityUpdate(object sender, EntityStateChangedEventArgs e)
        {
            var entity = e.Entry.Entity;
            var entityType = entity.GetType();

            var implementsIEntity =
                entityType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IEntity<>));

            if (e.NewState == EntityState.Modified && implementsIEntity)
            {
                var modifiedProperty = entityType.GetProperty(nameof(IEntity<object>.EditAt));
                modifiedProperty.SetValue(entity, DateTime.Now);
            }
        }

        protected virtual void OnEntityCreate(object sender, EntityTrackedEventArgs e)
        {
            var entity = e.Entry.Entity;
            var entityType = entity.GetType();

            var implementsIEntity =
                entityType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IEntity<>));

            if (!e.FromQuery && e.Entry.State == EntityState.Added && implementsIEntity)
            {
                var modifiedProperty = entityType.GetProperty(nameof(IEntity<object>.EditAt));
                modifiedProperty.SetValue(entity, DateTime.Now);

                var createdAtProperty = entityType.GetProperty(nameof(IEntity<object>.CreatedAt));
                createdAtProperty.SetValue(entity, DateTime.Now);
            }
        }

        public IDbContextTransaction GetCurrentTransaction()
        {
            return _currentTransaction;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        // public abstract TContext CreateDbContext(string[] args);
    }
}