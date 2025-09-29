using Cads.Core.Messaging;

namespace Cads.Infrastructure.Messaging.Clients;

public class CadsCtsTopicClient : ITopicClient
{
    public string ClientName => GetType().Name;
}