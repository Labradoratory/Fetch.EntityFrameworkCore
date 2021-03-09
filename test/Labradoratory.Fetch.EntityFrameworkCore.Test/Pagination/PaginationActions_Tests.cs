using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.EntityFrameworkCore.Pagination;
using Labradoratory.Fetch.Processors;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test.Pagination
{
    public class PaginationActions_Tests
    {
        private async Task<DbContext> CreateTestContextAsync(int entityCount, string testDbName)
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase(testDbName);

            var r = new Random();
            var entities = new List<TestEntity>();
            for (var i = 0; i < entityCount; i++)
            {
                entities.Add(new TestEntity { IntValue = r.Next() });
            }

            var context = new TestContext(optionsBuilder.Options);
            foreach (var entity in entities)
            {
                context.Add(entity);
            }

            await context.SaveChangesAsync();

            foreach (var entity in entities)
            {
                Assert.NotEqual(0, entity.Id);
            }

            return context;
        }

        [Fact]
        public async Task CountAsync_Success()
        {
            var expectedCount = new Random().Next(15);
            using var context = await CreateTestContextAsync(expectedCount, "TestPaginationCount");

            var subject = new PaginationActions<TestEntity, int>(() => context.Set<TestEntity>(), e => e.IntValue);

            var result = await subject.CountAsync(cancellationToken: CancellationToken.None);

            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task CountAsync_AppliesFilter()
        {
            using var context = await CreateTestContextAsync(20, "TestPaginationCountWithFilter");

            var average = context.Set<TestEntity>().Average(t => t.IntValue);
            var expectedCount = context.Set<TestEntity>().Count(t => t.IntValue < average);

            var subject = new PaginationActions<TestEntity, int>(() => context.Set<TestEntity>(), e => e.IntValue);

            var result = await subject.CountAsync(query => query.Where(t => t.IntValue < average), CancellationToken.None);

            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task GetPageAsync_Success()
        {
            var entityCount = 100;
            var pageSize = 10;
            var totalPages = entityCount / pageSize;

            using var context = await CreateTestContextAsync(entityCount, "TestPaginationPaging");

            var entitiesInOrder = await context.Set<TestEntity>().OrderBy(e => e.IntValue).ToListAsync();

            var subject = new PaginationActions<TestEntity, int>(() => context.Set<TestEntity>(), e => e.IntValue);

            var page = 0;
            while (true)
            {
                var result = await subject.GetPageAsync(page, pageSize, cancellationToken: CancellationToken.None);
                var resultCount = result.Results.Count();
                if (resultCount == 0)
                    break;

                Assert.Equal(page, result.Page);
                Assert.Equal(pageSize, result.PageSize);
                Assert.Equal(pageSize, resultCount);

                var fragment = entitiesInOrder.Skip(page * pageSize).Take(pageSize);
                Assert.Equal(fragment, result.Results);

                page++;

                if (page > 10)
                    Assert.True(false, "Test loop ran more times than expected.");
            }
        }

        [Fact]
        public async Task GetPageAsync_AppliesFilter()
        {
            var entityCount = 20;
            var pageSize = 10;
            var totalPages = entityCount / pageSize;

            using var context = await CreateTestContextAsync(entityCount, "TestPaginationPagingWithFilter");

            var entitiesInOrder = await context.Set<TestEntity>().OrderBy(e => e.IntValue).ToListAsync();

            var average = context.Set<TestEntity>().Average(t => t.IntValue);

            var subject = new PaginationActions<TestEntity, int>(() => context.Set<TestEntity>(), e => e.IntValue);

            var page = 0;
            while (true)
            {
                var result = await subject.GetPageAsync(page, pageSize, query => query.Where(t => t.IntValue < average), CancellationToken.None);
                var resultCount = result.Results.Count();
                if (resultCount == 0)
                    break;

                Assert.Equal(page, result.Page);
                Assert.Equal(pageSize, result.PageSize);
                Assert.True(result.Results.All(t => t.IntValue < average));

                page++;

                if (page > 2)
                    Assert.True(false, "Test loop ran more times than expected.");
            }
        }
    }
}
