using System;
using Labradoratory.Fetch.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Labradoratory.Fetch.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// Methods to make working with <see cref="IServiceCollection"/> a little easier.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the depedencies for the Fetch library targetting Entity Framework Core.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>A <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddFetchForEntityFrameworkCore(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddFetch();

            return serviceCollection;
        }

        /// <summary>
        ///     Registers the given context as a service in the <see cref="IServiceCollection" />.
        ///     You use this method when using dependency injection in your application, such as with ASP.NET.
        ///     For more information on setting up dependency injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be registered. </typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="repositoryRegistrationAction">An action that can we used to register repositories for the context.</param>
        /// <param name="optionsAction">
        ///     <para>
        ///         An optional action to configure the <see cref="DbContextOptions" /> for the context. This provides an
        ///         alternative to performing configuration of the context by overriding the
        ///         <see cref="DbContext.OnConfiguring" /> method in your derived context.
        ///     </para>
        ///     <para>
        ///         If an action is supplied here, the <see cref="DbContext.OnConfiguring" /> method will still be run if it has
        ///         been overridden on the derived context. <see cref="DbContext.OnConfiguring" /> configuration will be applied
        ///         in addition to configuration performed here.
        ///     </para>
        ///     <para>
        ///         In order for the options to be passed into your context, you need to expose a constructor on your context that takes
        ///         <see cref="DbContextOptions{TContext}" /> and passes it to the base constructor of <see cref="DbContext" />.
        ///     </para>
        /// </param>
        /// <param name="contextLifetime"> The lifetime with which to register the DbContext service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the DbContextOptions service in the container. </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddDbContext<TContext>(
            this IServiceCollection serviceCollection,
            Action<EntityFrameworkCoreRepositoryRegistrar<TContext>> repositoryRegistrationAction = null,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            return serviceCollection.AddDbContext<TContext, TContext>(repositoryRegistrationAction, optionsAction, contextLifetime, optionsLifetime);
        }

        /// <summary>
        ///     Registers the given context as a service in the <see cref="IServiceCollection" />.
        ///     You use this method when using dependency injection in your application, such as with ASP.NET.
        ///     For more information on setting up dependency injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        /// </summary>
        /// <typeparam name="TContextService"> The class or interface that will be used to resolve the context from the container. </typeparam>
        /// <typeparam name="TContextImplementation"> The concrete implementation type to create. </typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="repositoryRegistrationAction">An action that can we used to register repositories for the context.</param>
        /// <param name="optionsAction">
        ///     <para>
        ///         An optional action to configure the <see cref="DbContextOptions" /> for the context. This provides an
        ///         alternative to performing configuration of the context by overriding the
        ///         <see cref="DbContext.OnConfiguring" /> method in your derived context.
        ///     </para>
        ///     <para>
        ///         If an action is supplied here, the <see cref="DbContext.OnConfiguring" /> method will still be run if it has
        ///         been overridden on the derived context. <see cref="DbContext.OnConfiguring" /> configuration will be applied
        ///         in addition to configuration performed here.
        ///     </para>
        ///     <para>
        ///         In order for the options to be passed into your context, you need to expose a constructor on your context that takes
        ///         <see cref="DbContextOptions{TContext}" /> and passes it to the base constructor of <see cref="DbContext" />.
        ///     </para>
        /// </param>
        /// <param name="contextLifetime"> The lifetime with which to register the DbContext service in the container. </param>
        /// <param name="optionsLifetime"> The lifetime with which to register the DbContextOptions service in the container. </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddDbContext<TContextService, TContextImplementation>(
            this IServiceCollection serviceCollection,
            Action<EntityFrameworkCoreRepositoryRegistrar<TContextImplementation>> repositoryRegistrationAction = null,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : DbContext, TContextService
        {
            repositoryRegistrationAction?.Invoke(new EntityFrameworkCoreRepositoryRegistrar<TContextImplementation>(serviceCollection));

            var oa = optionsAction == null
                    ? (Action<IServiceProvider, DbContextOptionsBuilder>)null
                    : (p, b) => optionsAction.Invoke(b);

            return serviceCollection.AddDbContext<TContextService, TContextImplementation>(
                oa,
                contextLifetime,
                optionsLifetime);
        }

        /// <summary>
        /// Allows registration of repositories for a <see cref="DbContext"/> that has already been configured
        /// with Entity Framework Core.
        /// </summary>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="repositoryRegistrationAction">An action that can we used to register repositories for the context.</param>
        /// <returns>The same <paramref name="serviceCollection"/> so that multiple calls can be chained.</returns>
        public static IServiceCollection WithContext<TContext>(
            this IServiceCollection serviceCollection,
            Action<EntityFrameworkCoreRepositoryRegistrar<TContext>> repositoryRegistrationAction)
            where TContext : DbContext
        {
            repositoryRegistrationAction?.Invoke(new EntityFrameworkCoreRepositoryRegistrar<TContext>(serviceCollection));
            return serviceCollection;
        }
    }    
}
