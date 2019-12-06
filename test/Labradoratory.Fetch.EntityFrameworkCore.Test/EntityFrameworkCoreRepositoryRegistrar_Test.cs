using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test
{
    public class EntityFrameworkCoreRepositoryRegistrar_Test
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

            var subject = new EntityFrameworkCoreRepositoryRegistrar(mockServiceCollection.Object);
            subject.RegisterRepository<TestEntity, TestContext>();

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

            var subject = new EntityFrameworkCoreRepositoryRegistrar(mockServiceCollection.Object);
            subject.RegisterRepository<TestEntity, TestContext, TestRepository>();

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
