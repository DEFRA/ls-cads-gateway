using Amazon.SimpleNotificationService;
using Cads.Core.Messaging;
using Cads.Core.Messaging.Exceptions;
using Cads.Infrastructure.Messaging.Clients;
using Cads.Infrastructure.Messaging.Configuration;
using Cads.Infrastructure.Messaging.Factories;

namespace Cads.Infrastructure.Messaging.Publishers;

public class CadsCtsMessagePublisher(IAmazonSimpleNotificationService amazonSimpleNotificationService,
    IMessageFactory messageFactory, IServiceBusSenderConfiguration serviceBusSenderConfiguration) : IMessagePublisher<CadsCtsTopicClient>
{
    private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService = amazonSimpleNotificationService;
    private readonly IMessageFactory _messageFactory = messageFactory;
    private readonly IServiceBusSenderConfiguration _serviceBusSenderConfiguration = serviceBusSenderConfiguration;

    public string TopicArn => _serviceBusSenderConfiguration.CadsCtsTopic.TopicArn;

    public async Task PublishAsync<TMessage>(TMessage? message, CancellationToken cancellationToken = default)
    {
        if (message == null) throw new ArgumentException("Message payload was null", nameof(message));

        if (string.IsNullOrWhiteSpace(TopicArn)) throw new PublishFailedException("TopicArn is missing", false);

        try
        {
            var publishRequest = _messageFactory.CreateMessage(TopicArn, message);
            await _amazonSimpleNotificationService.PublishAsync(publishRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new PublishFailedException($"Failed to publish event on {TopicArn}.", false, ex);
        }
    }
}