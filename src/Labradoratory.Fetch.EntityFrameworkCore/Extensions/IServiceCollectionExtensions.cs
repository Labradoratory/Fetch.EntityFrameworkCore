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
        /// <returns>A <see cref="EntityFrameworkCoreDbContextRegistrar"/> which can be used to register <see cref="DbContext"/>.</returns>
        public static EntityFrameworkCoreDbContextRegistrar AddFetchForEntityFrameworkCore(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddFetch();

            return new EntityFrameworkCoreDbContextRegistrar(serviceCollection);
        }        
    }    
}
