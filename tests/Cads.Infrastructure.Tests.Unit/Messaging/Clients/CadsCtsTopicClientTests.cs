using FluentAssertions;
using Cads.Infrastructure.Messaging.Clients;

namespace Cads.Infrastructure.Tests.Unit.Messaging.Clients;

public class CadsCtsTopicClientTests
{
    [Fact]
    public void ClientName_ReturnsClassName()
    {
        var client = new CadsCtsTopicClient();

        var result = client.ClientName;

        result.Should().Be(nameof(CadsCtsTopicClient));
    }

}