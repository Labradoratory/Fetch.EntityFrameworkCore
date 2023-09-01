using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test
{
    public class EntityFrameworkCoreAsyncQueryResolver_Tests
    {
        [Fact]
        public async Task AnyAsync_NoParams_Success()
        {
            using(var repository = await CreateAndPopulateTestDatabaseAsync(nameof(AnyAsync_NoParams_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.True(await subject.AnyAsync());
            }
        }

        [Fact]
        public async Task AnyAsync_WithPredicate_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(AnyAsync_WithPredicate_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.True(await subject.AnyAsync(e => e.IntValue < EntityCount));
            }
        }

        [Fact]
        public async Task CountAsync_NoParams_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(CountAsync_NoParams_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Equal(EntityCount, await subject.CountAsync());
            }
        }

        [Fact]
        public async Task CountAsync_WithPredicate_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(CountAsync_WithPredicate_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Equal(1, await subject.CountAsync(e => e.IntValue == 4));
            }
        }

        [Fact]
        public async Task FirstAsync_NoParams_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(FirstAsync_NoParams_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Equal(ExpectedEntities[0].Id, (await subject.FirstAsync()).Id);
            }
        }

        [Fact]
        public async Task FirstAsync_WithPredicate_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(FirstAsync_WithPredicate_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Equal(ExpectedEntities[4].Id, (await subject.FirstAsync(e => e.IntValue == 4)).Id);
            }
        }

        [Fact]
        public async Task FirstAsync_WithPredicate_ThrowsWhenNotFound()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(FirstAsync_WithPredicate_ThrowsWhenNotFound)))
            {
                var subject = repository.GetAsyncQueryResolver();
                await Assert.ThrowsAsync<InvalidOperationException>(() => subject.FirstAsync(e => e.IntValue == EntityCount + 1));
            }
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NoParams_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(FirstOrDefaultAsync_NoParams_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Equal(ExpectedEntities[0].Id, (await subject.FirstOrDefaultAsync(CancellationToken.None)).Id);
            }
        }

        [Fact]
        public async Task FirstOrDefaultAsync_WithPredicate_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(FirstOrDefaultAsync_WithPredicate_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Equal(ExpectedEntities[4].Id, (await subject.FirstOrDefaultAsync(e => e.IntValue == 4)).Id);
            }
        }

        [Fact]
        public async Task FirstOrDefaultAsync_WithPredicate_NotFoundReturnNull()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(FirstOrDefaultAsync_WithPredicate_NotFoundReturnNull)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Null(await subject.FirstOrDefaultAsync(e => e.IntValue == EntityCount + 1));
            }
        }

        [Fact]
        public async Task SingleAsync_NoParams_ThrowsWhenMoreThanOne()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(SingleAsync_NoParams_ThrowsWhenMoreThanOne)))
            {
                var subject = repository.GetAsyncQueryResolver();
                await Assert.ThrowsAsync<InvalidOperationException>(() => subject.SingleAsync(CancellationToken.None));
            }
        }

        [Fact]
        public async Task SingleAsync_WithPredicate_ThrowsWhenMoreThanOne()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(SingleAsync_WithPredicate_ThrowsWhenMoreThanOne)))
            {
                var subject = repository.GetAsyncQueryResolver();
                await Assert.ThrowsAsync<InvalidOperationException>(() => subject.SingleAsync(e => e.IntValue < EntityCount));
            }
        }

        [Fact]
        public async Task SingleAsync_WithPredicate_ThrowsWhenNotFound()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(SingleAsync_WithPredicate_ThrowsWhenMoreThanOne)))
            {
                var subject = repository.GetAsyncQueryResolver();
                await Assert.ThrowsAsync<InvalidOperationException>(() => subject.SingleAsync(e => e.IntValue == EntityCount + 1));
            }
        }

        [Fact]
        public async Task SingleAsync_WithPredicate_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(SingleAsync_WithPredicate_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Equal(ExpectedEntities[4].Id, (await subject.SingleAsync(e => e.IntValue == 4)).Id);
            }
        }

        [Fact]
        public async Task SingleOrDefaultAsync_NoParams_ThrowsWhenMoreThanOne()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(SingleOrDefaultAsync_NoParams_ThrowsWhenMoreThanOne)))
            {
                var subject = repository.GetAsyncQueryResolver();
                await Assert.ThrowsAsync<InvalidOperationException>(() => subject.SingleOrDefaultAsync(CancellationToken.None));
            }
        }

        [Fact]
        public async Task SingleOrDefaultAsync_WithPredicate_ThrowsWhenMoreThanOne()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(SingleOrDefaultAsync_WithPredicate_ThrowsWhenMoreThanOne)))
            {
                var subject = repository.GetAsyncQueryResolver();
                await Assert.ThrowsAsync<InvalidOperationException>(() => subject.SingleOrDefaultAsync(e => e.IntValue < EntityCount));
            }
        }

        [Fact]
        public async Task SingleOrDefaultAsync_WithPredicate_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(SingleOrDefaultAsync_WithPredicate_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Equal(ExpectedEntities[4].Id, (await subject.SingleOrDefaultAsync(e => e.IntValue == 4)).Id);
            }
        }

        [Fact]
        public async Task SingleOrDefaultAsync_WithPredicate_NullWhenNotFound()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(SingleOrDefaultAsync_WithPredicate_NullWhenNotFound)))
            {
                var subject = repository.GetAsyncQueryResolver();
                Assert.Null(await subject.SingleOrDefaultAsync(e => e.IntValue == EntityCount + 1));
            }
        }

        [Fact]
        public async Task ToListAsync_Success()
        {
            using (var repository = await CreateAndPopulateTestDatabaseAsync(nameof(ToListAsync_Success)))
            {
                var subject = repository.GetAsyncQueryResolver();
                var result = await subject.ToListAsync();
                Assert.NotNull(result);
                Assert.IsAssignableFrom<IList<TestEntity>>(result);
                Assert.Equal(EntityCount, result.Count);
            }
        }

        private List<TestEntity> ExpectedEntities { get; } = CreateEntities();

        private const int EntityCount = 10;
        private static List<TestEntity> CreateEntities()
        {
            var list = new List<TestEntity>();
            for(var i = 0; i < EntityCount; i++)
            {
                list.Add(new TestEntity
                {
                    IntValue = i,
                    DoubleValue = i / 2.0,
                    StringValue = $"Value number {i}",
                    DateTimeValue = DateTimeOffset.UtcNow + TimeSpan.FromMinutes(i),
                    Child = new TestEntityChild
                    {
                        StringValue = $"Child value number {i}"
                    }
                });
            }

            return list;
        }

        private async Task<TestRepository> CreateAndPopulateTestDatabaseAsync(string testName)
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase(testName);

            using (var context = new TestContext(optionsBuilder.Options))
            {
                foreach (var entity in ExpectedEntities)
                {
                    context.Add(entity);
                }

                await context.SaveChangesAsync();

                foreach (var entity in ExpectedEntities)
                {
                    Assert.NotEqual(0, entity.Id);
                }
            }

            return new TestRepository(
                new TestContext(optionsBuilder.Options), 
                mockProcessorProvider.Object);
        }
    }
}
