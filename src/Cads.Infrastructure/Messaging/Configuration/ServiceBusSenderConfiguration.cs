namespace Cads.Infrastructure.Messaging.Configuration;

public class ServiceBusSenderConfiguration : IServiceBusSenderConfiguration
{
    public TopicConfiguration CadsCtsTopic { get; init; } = new();
}