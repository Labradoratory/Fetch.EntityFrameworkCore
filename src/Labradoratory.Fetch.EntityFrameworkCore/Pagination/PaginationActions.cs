using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Labradoratory.Fetch.EntityFrameworkCore.Pagination
{
    /// <summary>
    /// Actions for pagination that work on an Entity Framework <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.AddOn.Pagination.ISupportsPagination{TEntity}" />
    public class PaginationActions<TEntity, TKey> : ISupportsPagination<TEntity> where TEntity : Entity, IPageable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationActions{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="getQuery">The function to get the query to get pages for.</param>
        /// <param name="orderBy">The expression to order by when paging.</param>
        public PaginationActions(Func<IQueryable<TEntity>> getQuery, Expression<Func<TEntity, TKey>> orderBy)
        {
            GetQuery = getQuery;
            OrderBy = orderBy;
        }

        private Func<IQueryable<TEntity>> GetQuery { get; }
        private Expression<Func<TEntity, TKey>> OrderBy { get; }

        /// <inheritdoc />
        public Task<int> CountAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> filter = null, CancellationToken cancellationToken = default)
        {
            if (filter == null)
                filter = new Func<IQueryable<TEntity>, IQueryable<TEntity>>(query => query);

            return filter(GetQuery()).CountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ResultPage<TEntity>> GetPageAsync(PageInfo pageInfo, Func<IQueryable<TEntity>, IQueryable<TEntity>> filter = null, CancellationToken cancellationToken = default)
        {
            if (filter == null)
                filter = new Func<IQueryable<TEntity>, IQueryable<TEntity>>(query => query);

            var query = filter(GetQuery()).OrderBy(OrderBy).Skip(pageInfo.Page * pageInfo.PageSize).Take(pageInfo.PageSize);
            var results = await query.ToListAsync();
            return new ResultPage<TEntity>(pageInfo.Page, pageInfo.PageSize, results);
        }
    }
}
