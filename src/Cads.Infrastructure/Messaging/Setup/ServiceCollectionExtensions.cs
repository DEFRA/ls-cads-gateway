using Amazon.SimpleNotificationService;
using Cads.Core.Messaging;
using Cads.Infrastructure.Messaging.Clients;
using Cads.Infrastructure.Messaging.Configuration;
using Cads.Infrastructure.Messaging.Factories;
using Cads.Infrastructure.Messaging.Factories.Implementations;
using Cads.Infrastructure.Messaging.Publishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Cads.Infrastructure.Messaging.Setup;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddMessagingDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceBusSenderDependencies(configuration);
    }

    private static void AddServiceBusSenderDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusSenderConfiguration = configuration.GetSection(nameof(ServiceBusSenderConfiguration)).Get<ServiceBusSenderConfiguration>()!;
        services.AddSingleton<IServiceBusSenderConfiguration>(serviceBusSenderConfiguration);

        if (configuration["LOCALSTACK_ENDPOINT"] != null)
        {
            services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            {
                var config = new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = configuration["AWS:ServiceURL"],
                    AuthenticationRegion = configuration["AWS:Region"],
                    UseHttp = true
                };
                return new AmazonSimpleNotificationServiceClient(config);
            });
        }
        else
        {
            services.AddAWSService<IAmazonSimpleNotificationService>();
        }

        if (serviceBusSenderConfiguration.CadsCtsTopic.HealthcheckEnabled)
        {
            services.AddHealthChecks()
                .AddCheck<AwsSnsHealthCheck>("aws_sns", tags: ["aws", "sns"]);
        }

        services.AddServiceBusEventPublishers();
    }

    private static IServiceCollection AddServiceBusEventPublishers(this IServiceCollection services)
    {
        services.AddTransient<IMessageFactory, MessageFactory>();

        services.AddSingleton<IMessagePublisher<CadsCtsTopicClient>, CadsCtsMessagePublisher>();

        return services;
    }
}