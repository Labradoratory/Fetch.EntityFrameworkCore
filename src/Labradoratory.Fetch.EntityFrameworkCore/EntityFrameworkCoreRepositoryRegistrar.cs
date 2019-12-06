using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Labradoratory.Fetch.EntityFrameworkCore
{
    /// <summary>
    /// A class to help register Fetch repositories that target Microsoft's Entity Framework Core.
    /// </summary>
    public class EntityFrameworkCoreRepositoryRegistrar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkCoreRepositoryRegistrar"/> class.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        public EntityFrameworkCoreRepositoryRegistrar(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        private IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// Registers a default <see cref="EntityFrameworkCoreRepository{TEntity, TContext}"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <returns>The <see cref="EntityFrameworkCoreRepositoryRegistrar"/>.</returns>
        public EntityFrameworkCoreRepositoryRegistrar RegisterRepository<TEntity, TContext>()
            where TEntity : Entity
            where TContext : DbContext
        {
            ServiceCollection.TryAddTransient<Repository<TEntity>, EntityFrameworkCoreRepository<TEntity, TContext>>();
            return this;
        }

        /// <summary>
        /// Registers a repository of type <typeparamref name="TRepository"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <returns>The <see cref="EntityFrameworkCoreRepositoryRegistrar"/>.</returns>
        /// <remarks>
        /// <para>This method also registers the repository as <see cref="Repository{TEntity}"/>.</para>
        /// </remarks>
        public EntityFrameworkCoreRepositoryRegistrar RegisterRepository<TEntity, TContext, TRepository>()
            where TEntity : Entity
            where TContext : DbContext
            where TRepository : EntityFrameworkCoreRepository<TEntity, TContext>
        {
            ServiceCollection.TryAddTransient<Repository<TEntity>, TRepository>();
            ServiceCollection.TryAddTransient<TRepository>();
            return this;
        }
    }
}
