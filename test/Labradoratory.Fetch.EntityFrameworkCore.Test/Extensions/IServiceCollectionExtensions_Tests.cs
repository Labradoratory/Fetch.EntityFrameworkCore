using System.Collections.Generic;
using System.Linq;
using Labradoratory.Fetch.EntityFrameworkCore.Extensions;
using Labradoratory.Fetch.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test.Extensions
{
    public class IServiceCollectionExtensions_Tests
    {
        [Fact]
        public void AddFetchForEntityFrameworkCore_ReturnsEntityFrameworkCoreRepositoryRegistrar()
        {
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Loose);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            var expectedServiceCollection = mockServiceCollection.Object;
            var result = expectedServiceCollection.AddFetchForEntityFrameworkCore();
            Assert.Same(expectedServiceCollection, result);
        }

        [Fact]
        public void AddFetchForEntityFrameworkCore_CallsUseFetch()
        {
            var serviceDescriptors = new List<ServiceDescriptor>();
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(d => serviceDescriptors.Add(d));
            var expectedServiceCollection = mockServiceCollection.Object;
            var result = expectedServiceCollection.AddFetchForEntityFrameworkCore();

            Assert.Contains(serviceDescriptors, d => d.ImplementationType == typeof(ProcessorPipeline));
            mockServiceCollection.Verify(sc => sc.Add(
                It.IsAny<ServiceDescriptor>()),
                Times.AtLeastOnce());
        }

        [Fact]
        public void AddDbContext_Success()
        {
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var subject = mockServiceCollection.Object;
            var result = subject.AddDbContext<TestContext>(
                repoRegistrar =>
                {
                    repoRegistrar.RegisterRepository<TestEntity>();
                    repoRegistrar.RegisterRepository<TestEntity, TestRepository>();
                });

            Assert.Same(subject, result);

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(TestContext))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions<TestContext>))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd =>
                    sd.ServiceType == typeof(Repository<TestEntity>)
                    && sd.ImplementationType == typeof(EntityFrameworkCoreRepository<TestEntity, TestContext>))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd =>
                    sd.ServiceType == typeof(Repository<TestEntity>)
                    && sd.ImplementationType == typeof(TestRepository))),
                Times.Once());
        }        

        [Fact]
        public void AddDbContext_ServiceAndImpl_OptionsActionServiceLifetimeOptionsLifeTime_Success()
        {
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var subject = mockServiceCollection.Object;
            var result = subject.AddDbContext<TestContext, TestContext>(
                repoRegistrar =>
                {
                    repoRegistrar.RegisterRepository<TestEntity>();
                    repoRegistrar.RegisterRepository<TestEntity, TestRepository>();
                });

            Assert.Same(subject, result);

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(TestContext))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions<TestContext>))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd =>
                    sd.ServiceType == typeof(Repository<TestEntity>)
                    && sd.ImplementationType == typeof(TestRepository))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd =>
                    sd.ServiceType == typeof(EntityFrameworkCoreRepository<TestEntity, TestContext>)
                    && sd.ImplementationType == typeof(TestRepository))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd =>
                    sd.ServiceType == typeof(TestRepository)
                    && sd.ImplementationType == typeof(TestRepository))),
                Times.Once());
        }

        [Fact]
        public void WithContext_Success()
        {
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            object expectedRepoRegistrar = null;

            var subject = mockServiceCollection.Object;
            var result = subject.WithContext<TestContext>(repoRegistrar => expectedRepoRegistrar = repoRegistrar);

            Assert.Same(subject, result);
            Assert.IsType<EntityFrameworkCoreRepositoryRegistrar<TestContext>>(expectedRepoRegistrar);
        }
    }
}
