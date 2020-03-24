using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test
{
    public class EntityFrameworkCoreRepository_Tests
    {
        [Fact]
        public void Ctor_SetsContext()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var expectedContext = new TestContext(new DbContextOptions<TestContext>());
            var subject = new TestRepository(expectedContext, mockProcessorProvider.Object);
            Assert.Same(expectedContext, subject.TestGetContext());
        }

        [Fact]
        public async Task AddAsync_Success()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityAddingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityAddingPackage<TestEntity>>>());
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityAddedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityAddedPackage<TestEntity>>>());

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestAdd");

            var expectedEntity = new TestEntity
            {
                StringValue = "My test value",
                IntValue = 12345,
                DoubleValue = 123.45,
                DateTimeValue = DateTimeOffset.UtcNow,
                Child = new TestEntityChild
                {
                    StringValue = "My child test value"
                }
            };

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                    context,
                    new ProcessorPipeline(mockProcessorProvider.Object));

                await subject.AddAsync(expectedEntity, CancellationToken.None);

                mockProcessorProvider.Verify(p => p.GetProcessors<EntityAddingPackage<TestEntity>>(), Times.Once());
                mockProcessorProvider.Verify(p => p.GetProcessors<EntityAddedPackage<TestEntity>>(), Times.Once());

                Assert.NotEqual(0, expectedEntity.Id);
            }

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var entity = context.Find<TestEntity>(expectedEntity.Id);
                Assert.NotNull(entity);
                Assert.Equal(expectedEntity.DateTimeValue, entity.DateTimeValue);
                Assert.Equal(expectedEntity.DoubleValue, entity.DoubleValue);
                Assert.Equal(expectedEntity.IntValue, entity.IntValue);
                Assert.Equal(expectedEntity.StringValue, entity.StringValue);
                Assert.Equal(expectedEntity.Child.StringValue, entity.Child.StringValue);
            }
        }

        [Fact]
        public async Task UpdateAsync_Success()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityUpdatingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityUpdatingPackage<TestEntity>>>());
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityUpdatedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityUpdatedPackage<TestEntity>>>());

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestUpdate");

            var addEntity = new TestEntity
            {
                StringValue = "My test value",
                IntValue = 12345,
                DoubleValue = 123.45,
                DateTimeValue = DateTimeOffset.UtcNow,
                Child = new TestEntityChild
                {
                    StringValue = "My child test value"
                }
            };

            using (var context = new TestContext(optionsBuilder.Options))
            {
                context.Add(addEntity);
                await context.SaveChangesAsync();
                Assert.NotEqual(0, addEntity.Id);
            }

            TestEntity expectedEntity = null;
            using (var context = new TestContext(optionsBuilder.Options))
            {
                expectedEntity = context.Find<TestEntity>(addEntity.Id);
                Assert.NotNull(expectedEntity);
            }

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                    context,
                    new ProcessorPipeline(mockProcessorProvider.Object));

                expectedEntity.IntValue = 54321;
                Assert.NotEqual(expectedEntity.IntValue, addEntity.IntValue);

                var result = await subject.UpdateAsync(expectedEntity, CancellationToken.None);

                mockProcessorProvider.Verify(p => p.GetProcessors<EntityUpdatingPackage<TestEntity>>(), Times.Once());
                mockProcessorProvider.Verify(p => p.GetProcessors<EntityUpdatedPackage<TestEntity>>(), Times.Once());
            }

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var entity = context.Find<TestEntity>(expectedEntity.Id);
                Assert.NotNull(entity);
                Assert.Equal(expectedEntity.DateTimeValue, entity.DateTimeValue);
                Assert.Equal(expectedEntity.DoubleValue, entity.DoubleValue);
                Assert.Equal(expectedEntity.IntValue, entity.IntValue);
                Assert.Equal(expectedEntity.StringValue, entity.StringValue);
                Assert.Equal(expectedEntity.Child.StringValue, entity.Child.StringValue);
            }
        }

        [Fact]
        public async Task DeleteAsync_Success()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityDeletingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityDeletingPackage<TestEntity>>>());
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityDeletedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityDeletedPackage<TestEntity>>>());

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestDelete");

            var addEntity = new TestEntity
            {
                StringValue = "My test value",
                IntValue = 12345,
                DoubleValue = 123.45,
                DateTimeValue = DateTimeOffset.UtcNow,
                Child = new TestEntityChild
                {
                    StringValue = "My child test value"
                }
            };

            using (var context = new TestContext(optionsBuilder.Options))
            {
                context.Add(addEntity);
                await context.SaveChangesAsync();
                Assert.NotEqual(0, addEntity.Id);
            }

            TestEntity expectedEntity = null;
            using (var context = new TestContext(optionsBuilder.Options))
            {
                expectedEntity = context.Find<TestEntity>(addEntity.Id);
                Assert.NotNull(expectedEntity);
            }

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                    context,
                    new ProcessorPipeline(mockProcessorProvider.Object));
                               
                await subject.DeleteAsync(expectedEntity, CancellationToken.None);

                mockProcessorProvider.Verify(p => p.GetProcessors<EntityDeletingPackage<TestEntity>>(), Times.Once());
                mockProcessorProvider.Verify(p => p.GetProcessors<EntityDeletedPackage<TestEntity>>(), Times.Once());
            }

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var entity = context.Find<TestEntity>(expectedEntity.Id);
                Assert.Null(entity);
            }
        }

        [Fact]
        public async Task FindAsync_Success()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestFind");

            var expectedEntity = new TestEntity
            {
                StringValue = "My test value",
                IntValue = 12345,
                DoubleValue = 123.45,
                DateTimeValue = DateTimeOffset.UtcNow,
                Child = new TestEntityChild
                {
                    StringValue = "My child test value"
                }
            };

            using (var context = new TestContext(optionsBuilder.Options))
            {
                context.Add(expectedEntity);
                await context.SaveChangesAsync();
                Assert.NotEqual(0, expectedEntity.Id);
            }

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var findEntity = context.Find<TestEntity>(expectedEntity.Id);
                Assert.NotNull(findEntity);
            }

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                    context,
                    new ProcessorPipeline(mockProcessorProvider.Object));

                var result = await subject.FindAsync(expectedEntity.GetKeys(), CancellationToken.None);
                Assert.NotNull(result);
                Assert.Equal(expectedEntity.DateTimeValue, result.DateTimeValue);
                Assert.Equal(expectedEntity.DoubleValue, result.DoubleValue);
                Assert.Equal(expectedEntity.IntValue, result.IntValue);
                Assert.Equal(expectedEntity.StringValue, result.StringValue);
                Assert.Equal(expectedEntity.Child.StringValue, result.Child.StringValue);
            }
        }

        [Fact]
        public async Task Get_Success()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestGet");

            var entities = new List<TestEntity>
            {
                new TestEntity { IntValue = 1 },
                new TestEntity { IntValue = 2 },
                new TestEntity { IntValue = 3 },
                new TestEntity { IntValue = 4 }
            };

            using (var context = new TestContext(optionsBuilder.Options))
            {
                foreach (var entity in entities)
                {
                    context.Add(entity);
                }

                await context.SaveChangesAsync();

                foreach (var entity in entities)
                {
                    Assert.NotEqual(0, entity.Id);
                }
            }

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                    context,
                    new ProcessorPipeline(mockProcessorProvider.Object));

                foreach(var entity in subject.Get().ToList())
                {
                    var index = entities.FindIndex(e => e.Id == entity.Id);
                    Assert.True(index > -1);
                    entities.RemoveAt(index);
                }

                Assert.Empty(entities);
            }
        }

        [Fact]
        public void GetAsyncQueryResolver_NoParams_Success()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestGetNoParams");

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                       context,
                       new ProcessorPipeline(mockProcessorProvider.Object));

                var result = subject.GetAsyncQueryResolver();
                Assert.IsType<EntityFrameworkCoreAsyncQueryResolver<TestEntity>>(result);
            }
        }

        [Fact]
        public void GetAsyncQueryResolver_WithQuery_Success()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestGetWithQuery");

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                       context,
                       new ProcessorPipeline(mockProcessorProvider.Object));

                var result = subject.GetAsyncQueryResolver(query => query.Select(e => e.IntValue));
                Assert.IsType<EntityFrameworkCoreAsyncQueryResolver<int>>(result);
            }
        }

        [Fact]
        public void GetAsyncQueryResolver_QueryNull_Throws()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestGetQueryNull");

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                       context,
                       new ProcessorPipeline(mockProcessorProvider.Object));

                Assert.Throws<ArgumentNullException>(() => subject.GetAsyncQueryResolver<int>(null));
            }
        }

        /// <summary>
        /// This test makes sure that entity tracking isn't used.  An error was occuring
        /// because an Add operation caused the context to track that entity.  If a user then
        /// did a query that got the already added entity, doing a update would cause an exception:
        /// 
        /// "System.InvalidOperationException : The instance of entity type 'TestEntity' cannot be 
        /// tracked because another instance with the same key value for {'Id'} is already being tracked. 
        /// When attaching existing entities, ensure that only one entity instance with a given key value 
        /// is attached."
        /// </summary>
        [Fact]
        public async Task AddGetUpdate_Succeeds()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityAddingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityAddingPackage<TestEntity>>>());
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityAddedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityAddedPackage<TestEntity>>>());
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityUpdatingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityUpdatingPackage<TestEntity>>>());
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityUpdatedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityUpdatedPackage<TestEntity>>>());

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("TestAdd");

            var expectedEntity = new TestEntity
            {
                StringValue = "My test value",
                IntValue = 12345,
                DoubleValue = 123.45,
                DateTimeValue = DateTimeOffset.UtcNow,
                Child = new TestEntityChild
                {
                    StringValue = "My child test value"
                }
            };

            var newIntValue1 = 54321;
            var newIntValue2 = 15243;

            using (var context = new TestContext(optionsBuilder.Options))
            {
                var subject = new EntityFrameworkCoreRepository<TestEntity, TestContext>(
                    context,
                    new ProcessorPipeline(mockProcessorProvider.Object));

                // Add the entity.  It should not be tracked after this operation.
                await subject.AddAsync(expectedEntity, CancellationToken.None);

                // Get the entity and update to make sure it wasn't tracked by the add.  
                // If the entity IS being tracked, this will throw.  It should not.
                // It should not be tracked after this operation either.
                var sameEntity1 = await subject.GetAsyncQueryResolver().FirstOrDefaultAsync(e => e.Id == expectedEntity.Id);
                sameEntity1.IntValue = newIntValue1;
                await subject.UpdateAsync(sameEntity1, CancellationToken.None);

                // Get the entity and update again to make sure the update above didn't track.  
                // If the entity IS being tracked, this will throw.  It should not.
                var sameEntity2 = await subject.GetAsyncQueryResolver().FirstOrDefaultAsync(e => e.Id == expectedEntity.Id);
                sameEntity2.IntValue = newIntValue2;
                await subject.UpdateAsync(sameEntity2, CancellationToken.None);
            }
        }
        
        // DEBUGGING:  I was just using this test to figure out how to best access properties.
        //[Fact]
        //public void ModifyNonNavigationProperty_Success()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
        //    optionsBuilder.UseInMemoryDatabase("ModifyNonNavigationProperty");

        //    using (var context = new TestContext(optionsBuilder.Options))
        //    {
        //        var test = new TestEntity
        //        {
        //            StringCollection = new ChangeTracking.ChangeTrackingCollection<string>()
        //        };

        //        var entry = context.Entry(test);
        //        var member = entry.Member(nameof(TestEntity.StringCollection));
        //    }
        //}
    }
}
