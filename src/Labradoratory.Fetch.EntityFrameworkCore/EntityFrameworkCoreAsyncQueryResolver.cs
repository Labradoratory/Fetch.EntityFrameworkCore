using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Labradoratory.Fetch.EntityFrameworkCore
{
    /// <summary>
    /// An implementation of <see cref="IAsyncQueryResolver{T}"/> that targets Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">The type of value being resolved.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.IAsyncQueryResolver{T}" />
    public class EntityFrameworkCoreAsyncQueryResolver<T> : IAsyncQueryResolver<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkCoreAsyncQueryResolver{T}"/> class.
        /// </summary>
        /// <param name="query">The query to resolve asynchronously.</param>
        public EntityFrameworkCoreAsyncQueryResolver(IQueryable<T> query)
        {
            Query = query;
        }

        private IQueryable<T> Query { get; }

        /// <inheritdoc />
        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return Query.AnyAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Query.AnyAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Query.CountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Query.CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> FirstAsync(CancellationToken cancellationToken = default)
        {
            return Query.FirstAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> FirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Query.FirstAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return Query.FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> SingleAsync(CancellationToken cancellationToken = default)
        {
            return Query.SingleAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> SingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Query.SingleAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken)
        {
            return Query.SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Query.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IList<T>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await Query.ToListAsync(cancellationToken);
        }
    }
}
