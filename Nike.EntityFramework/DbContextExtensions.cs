using System;
using System.Threading.Tasks;
using Nike.Framework.Domain;

namespace Nike.EntityFramework
{
    public static class DbContextExtensions
    {
        public static ModelBuilder SetAllRelationshipDeleteBehavior(this ModelBuilder builder,
            DeleteBehavior deleteBehavior = DeleteBehavior.Restrict)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = deleteBehavior;

            return builder;
        }

        /// <summary>
        ///     This method will prevent the update of 'CreatedAt' property
        /// </summary>
        /// <param name="builder"></param>
        public static ModelBuilder DisallowTimestampPropertiesUpdate(this ModelBuilder builder)
        {
            var properties = builder.Model
                .GetEntityTypes()
                .SelectMany(entityType => entityType.GetProperties())
                .Where(p => p.Name == nameof(IAuditedEntity.CreatedAt));

            foreach (var property in properties) property.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            return builder;
        }

        public static void OnEntityCreate(this DbContext dbContext, Action<object, EntityTrackedEventArgs> callback)
        {
            dbContext.ChangeTracker.Tracked += (sender, args) =>
            {
                if (!args.FromQuery && args.Entry.State == EntityState.Added) callback(sender, args);
            };
        }

        public static void OnEntityUpdate(this DbContext dbContext,
            Action<object, EntityStateChangedEventArgs> callback)
        {
            dbContext.ChangeTracker.StateChanged += (sender, args) =>
            {
                if (args.NewState == EntityState.Modified) callback(sender, args);
            };
        }

        /// <summary>
        ///     set the <see cref="IAuditedEntity.CreatedAt" /> on entity create and <see cref="IAuditedEntity.UpdatedAt" /> on
        ///     entity update
        /// </summary>
        /// <param name="dbContext">database context</param>
        public static void SetAuditPropertiesOnEntityChange(this DbContext dbContext)
        {
            OnEntityUpdate(dbContext, (o, args) =>
            {
                var entity = args.Entry.Entity;
                var isAuditedEntity = entity is IAuditedEntity;

                if (isAuditedEntity) SetUpdatedAtProperty(entity);
            });

            OnEntityCreate(dbContext, (o, args) =>
            {
                var entity = args.Entry.Entity;
                var isAuditedEntity = entity is IAuditedEntity;

                if (isAuditedEntity)
                {
                    SetUpdatedAtProperty(entity);
                    SetCreatedAtProperty(entity);
                }
            });
        }

        /// <summary>
        ///     Creates a sequence with the following name format "type_Sequence" that is starting at 1 and is incremented by 1
        /// </summary>
        /// <typeparam name="TEntity"> The type of values the sequence will generate</typeparam>
        /// <param name="modelBuilder"> The model builder</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static ModelBuilder HasSequence<TEntity, TValue>(this ModelBuilder modelBuilder) where TEntity : class
        {
            var sequenceName = GetSequenceName<TEntity>();

            modelBuilder.HasSequence<TValue>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            return modelBuilder;
        }

        private static string GetSequenceName<TEntity>()
        {
            return $"{typeof(TEntity).Name}_Sequence";
        }

        /// <summary>
        ///     returns next value of sequence based on <see cref="HasSequence{TEntity,TValue}" />
        /// </summary>
        /// <param name="dbContext"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValue">type of sequence value</typeparam>
        /// <returns>next value of sequence</returns>
        public static async Task<TValue> GetSequenceValue<TEntity, TValue>(this DbContext dbContext)
            where TEntity : class
        {
            var sequenceName = GetSequenceName<TEntity>();

            var connection = dbContext.Database.GetDbConnection();
            connection.Open();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT NEXT VALUE FOR {sequenceName}";
            var obj = await cmd.ExecuteScalarAsync();
            var value = (TValue) obj;
            return value;
        }

        private static void SetUpdatedAtProperty(object entity)
        {
            var updateAtProperty = entity.GetType().GetProperty(nameof(IAuditedEntity.UpdatedAt));
            if (updateAtProperty is null)
                throw new Exception($"{nameof(IAuditedEntity.UpdatedAt)} property is not found.");
            updateAtProperty.SetValue(entity, DateTime.Now);
        }

        private static void SetCreatedAtProperty(object entity)
        {
            var createdAtProperty = entity.GetType().GetProperty(nameof(IAuditedEntity.CreatedAt));
            if (createdAtProperty is null)
                throw new Exception($"{nameof(IAuditedEntity.CreatedAt)} property is not found.");
            createdAtProperty.SetValue(entity, DateTime.Now);
        }
    }
}