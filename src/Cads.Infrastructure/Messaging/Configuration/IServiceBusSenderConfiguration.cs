namespace Cads.Infrastructure.Messaging.Configuration;

public interface IServiceBusSenderConfiguration
{
    TopicConfiguration CadsCtsTopic { get; init; }
}