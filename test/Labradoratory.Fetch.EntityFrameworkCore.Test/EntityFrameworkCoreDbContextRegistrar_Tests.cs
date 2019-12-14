using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test
{
    public class EntityFrameworkCoreDbContextRegistrar_Tests
    {
        [Fact]
        public void AddDbContext_ServiceLifetimeOptionsLifeTime_Success()
        {
            var expectedServiceLifetime = ServiceLifetime.Singleton;
            var expectedOptionsLifetime = ServiceLifetime.Singleton;

            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var subject = new EntityFrameworkCoreDbContextRegistrar(mockServiceCollection.Object);
            var result = subject.AddDbContext<TestContext>(expectedServiceLifetime, expectedOptionsLifetime);

            Assert.IsType<EntityFrameworkCoreRepositoryRegistrar<TestContext>>(result);

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(TestContext))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions<TestContext>))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions))),
                Times.Once());
        }

        [Fact]
        public void AddDbContext_OptionsActionServiceLifetimeOptionsLifeTime_Success()
        {
            var optionsActionCalled = false;
            var expectedAction = new Action<DbContextOptionsBuilder>(buidler => optionsActionCalled = true);
            var expectedServiceLifetime = ServiceLifetime.Singleton;
            var expectedOptionsLifetime = ServiceLifetime.Singleton;

            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var subject = new EntityFrameworkCoreDbContextRegistrar(mockServiceCollection.Object);
            var result = subject.AddDbContext<TestContext>(expectedAction, expectedServiceLifetime, expectedOptionsLifetime);

            Assert.IsType<EntityFrameworkCoreRepositoryRegistrar<TestContext>>(result);
            // TODO: Figure out a way to check if options is called as part of the service descriptor factory.
            //Assert.True(optionsActionCalled);

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(TestContext))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions<TestContext>))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions))),
                Times.Once());
        }

        [Fact]
        public void AddDbContext_OptionsActionWithProviderServiceLifetimeOptionsLifeTime_Success()
        {
            var optionsActionCalled = false;
            var expectedAction = new Action<IServiceProvider, DbContextOptionsBuilder>((provider, buidler) => optionsActionCalled = true);
            var expectedServiceLifetime = ServiceLifetime.Singleton;
            var expectedOptionsLifetime = ServiceLifetime.Singleton;

            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var subject = new EntityFrameworkCoreDbContextRegistrar(mockServiceCollection.Object);
            var result = subject.AddDbContext<TestContext>(expectedAction, expectedServiceLifetime, expectedOptionsLifetime);

            Assert.IsType<EntityFrameworkCoreRepositoryRegistrar<TestContext>>(result);
            // TODO: Figure out a way to check if options is called as part of the service descriptor factory.
            //Assert.True(optionsActionCalled);

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(TestContext))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions<TestContext>))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions))),
                Times.Once());
        }

        [Fact]
        public void AddDbContext_ServiceAndImpl_OptionsActionServiceLifetimeOptionsLifeTime_Success()
        {
            var optionsActionCalled = false;
            var expectedAction = new Action<DbContextOptionsBuilder>(buidler => optionsActionCalled = true);
            var expectedServiceLifetime = ServiceLifetime.Singleton;
            var expectedOptionsLifetime = ServiceLifetime.Singleton;

            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var subject = new EntityFrameworkCoreDbContextRegistrar(mockServiceCollection.Object);
            var result = subject.AddDbContext<TestContext, TestContext>(expectedAction, expectedServiceLifetime, expectedOptionsLifetime);

            Assert.IsType<EntityFrameworkCoreRepositoryRegistrar<TestContext>>(result);
            // TODO: Figure out a way to check if options is called as part of the service descriptor factory.
            //Assert.True(optionsActionCalled);

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(TestContext))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions<TestContext>))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(DbContextOptions))),
                Times.Once());
        }

        [Fact]
        public void WithContext_Success()
        {
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);

            var subject = new EntityFrameworkCoreDbContextRegistrar(mockServiceCollection.Object);
            var result = subject.WithContext<TestContext>();

            Assert.IsType<EntityFrameworkCoreRepositoryRegistrar<TestContext>>(result);
        }
    }
}
