using System.Linq;
using Labradoratory.Fetch.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test.Extensions
{
    public class IServiceCollectionExtensions_Tests
    {
        [Fact]
        public void AddFetchForEntityFrameworkCore_ReturnsServiceCollection()
        {
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Loose);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            var expectedServiceCollection = mockServiceCollection.Object;
            var result = expectedServiceCollection.AddFetchForEntityFrameworkCore();
            Assert.Same(expectedServiceCollection, result);
        }
    }
}
