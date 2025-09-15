using Autodor.Tests.E2E.Core.Factories;
using Xunit;

namespace Autodor.Tests.E2E.Core.Collections
{
    [CollectionDefinition(nameof(E2ECollection))]
    public class E2ECollection : ICollectionFixture<TestWebApplicationFactory>
    {
    }
}