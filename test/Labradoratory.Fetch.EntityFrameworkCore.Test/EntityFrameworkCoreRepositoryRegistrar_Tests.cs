using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test
{
    public class EntityFrameworkCoreRepositoryRegistrar_Tests
    {
        [Fact]
        public void RegisterRepository_AddsDefaultImplementation()
        {
            var serviceDescriptors = new List<ServiceDescriptor>();
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(d => serviceDescriptors.Add(d));
            mockServiceCollection
                .SetupGet(sc => sc.Count)
                .Returns(0);

            var subject = new EntityFrameworkCoreRepositoryRegistrar<TestContext>(mockServiceCollection.Object);
            subject.RegisterRepository<TestEntity>();

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(Repository<TestEntity>)
                && v.ImplementationType == typeof(EntityFrameworkCoreRepository<TestEntity, TestContext>))),
                Times.Once());
        }

        [Fact]
        public void RegisterRepository_AddsSpecificImplementation()
        {
            var serviceDescriptors = new List<ServiceDescriptor>();
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(d => serviceDescriptors.Add(d));
            mockServiceCollection
                .SetupGet(sc => sc.Count)
                .Returns(0);

            var subject = new EntityFrameworkCoreRepositoryRegistrar<TestContext>(mockServiceCollection.Object);
            subject.RegisterRepository<TestEntity, TestRepository>();

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(Repository<TestEntity>)
                && v.ImplementationType == typeof(TestRepository))),
                Times.Once());

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(TestRepository)
                && v.ImplementationType == typeof(TestRepository))),
                Times.Once());
        }
    }
}
