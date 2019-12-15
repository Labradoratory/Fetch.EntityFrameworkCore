using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Labradoratory.Fetch.EntityFrameworkCore
{
    /// <summary>
    /// A base class that simply passes <see cref="IServiceCollection"/> calls to a wrapped instance.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.DependencyInjection.IServiceCollection" />
    public abstract class ServiceCollectionWrapper : IServiceCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCollectionWrapper"/> class.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        public ServiceCollectionWrapper(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        private IServiceCollection ServiceCollection { get; }

        /// <inheritdoc />
        public ServiceDescriptor this[int index]
        { 
            get => ServiceCollection[index];
            set => ServiceCollection[index] = value;
        }

        /// <inheritdoc />
        public int Count => ServiceCollection.Count;

        /// <inheritdoc />
        public bool IsReadOnly => ServiceCollection.IsReadOnly;

        /// <inheritdoc />
        public void Add(ServiceDescriptor item)
        {
            ServiceCollection.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            ServiceCollection.Clear();
        }

        /// <inheritdoc />
        public bool Contains(ServiceDescriptor item)
        {
            return ServiceCollection.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            ServiceCollection.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return ServiceCollection.GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf(ServiceDescriptor item)
        {
            return ServiceCollection.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, ServiceDescriptor item)
        {
            ServiceCollection.Insert(index, item);
        }

        /// <inheritdoc />
        public bool Remove(ServiceDescriptor item)
        {
            return ServiceCollection.Remove(item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            ServiceCollection.RemoveAt(index);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ServiceCollection.GetEnumerator();
        }
    }
}
