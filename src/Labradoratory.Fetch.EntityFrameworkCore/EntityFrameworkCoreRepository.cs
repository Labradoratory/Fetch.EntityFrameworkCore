using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Labradoratory.Fetch.EntityFrameworkCore
{
    /// <summary>
    /// A default implementation of <see cref="Repository{T}"/> targting the 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TContext">The type of <see cref="DbContext"/> handling the entity.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Repository{TEntity}" />
    public class EntityFrameworkCoreRepository<TEntity, TContext> : Repository<TEntity>
        where TEntity : Entity
        where TContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkCoreRepository{TEntity, TContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="processorPipeline">The processor pipeline.</param>
        public EntityFrameworkCoreRepository(TContext context, ProcessorPipeline processorPipeline)
            : base(processorPipeline)
        {
            Context = context;
        }

        /// <summary>
        /// The context to use to access the database.
        /// </summary>
        protected TContext Context { get; }

        /// <inheritdoc />
        public override Task<TEntity> FindAsync(object[] keys, CancellationToken cancellationToken)
        {
            return Context.FindAsync<TEntity>(keys, cancellationToken).AsTask();
        }

        /// <inheritdoc />
        public override IQueryable<TEntity> Get()
        {
            return Context.Set<TEntity>().AsNoTracking();
        }

        /// <inheritdoc />
        public override IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return new EntityFrameworkCoreAsyncQueryResolver<TResult>(query(Get()));
        }

        /// <inheritdoc />
        protected override async Task ExecuteAddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await Context.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task ExecuteDeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            Context.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task<ChangeSet> ExecuteUpdateAsync(TEntity entity, ChangeSet changes, CancellationToken cancellationToken)
        {
            // TODO: This needs a bunch of work.  It is currently only set up to handle
            // basic properties and owned objects.
            //
            // It may turn out that doing a detect changes and relying on EF change tracking
            // would be just as fast.  We'll do some more experimenting when we have time.

            var entry = Context.Entry(entity);
            foreach (var change in changes)
            {
                FindProperty(entry, change.Key).IsModified = true;
            }

            await Context.SaveChangesAsync(cancellationToken);
            return changes;
        }

        protected virtual PropertyEntry FindProperty(EntityEntry<TEntity> entry, ChangePath path)
        {
            EntityEntry next = entry;
            PropertyEntry lastProperty = null;
            for(var i = 0; i < path.Parts.Count; i++)
            {
                switch(path.Parts[i])
                {
                    case ChangePathProperty property:
                        var member = next.Member(property.Property);
                        next = member.EntityEntry;
                        if (member is PropertyEntry)
                            lastProperty = member as PropertyEntry;
                        break;
                    case ChangePathIndex index:
                        // TODO: Not supported yet.  At the moment, I am unaware of a use case for this, 
                        // but if it comes up we should handle it.  Just skip it.
                        break;
                    case ChangePathKey key:
                        // TODO: Not supported yet.  At the moment, I am unaware of a use case for this, 
                        // but if it comes up we should handle it.  Just skip it.
                        break;
                }
            }

            return lastProperty;
        }
    }
}
