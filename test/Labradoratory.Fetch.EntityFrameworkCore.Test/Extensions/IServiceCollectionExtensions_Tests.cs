using System.Collections.Generic;
using System.Linq;
using Labradoratory.Fetch.EntityFrameworkCore.Extensions;
using Labradoratory.Fetch.Processors;
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
            Assert.IsType<EntityFrameworkCoreRepositoryRegistrar>(result);
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
    }
}
