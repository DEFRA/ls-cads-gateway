using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Moq;
using System.Net;

namespace Cads.Gateway.Tests.Component;

public class AppWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IAmazonSimpleNotificationService>? AmazonSimpleNotificationServiceMock;

    public Mock<IMongoClient>? MongoClientMock;

    private const string CadsCtsTopicName = "ls-cads-cts-events";
    private const string CadsCtsTopicArn = $"arn:aws:sns:eu-west-2:000000000000:{CadsCtsTopicName}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        SetTestEnvironmentVariables();

        builder.ConfigureTestServices(services =>
        {
            RemoveService<IHealthCheckPublisher>(services);

            ConfigureAwsOptions(services);

            ConfigureSimpleNotificationService(services);

            ConfigureDatabase(services);
        });
    }

    protected T GetService<T>() where T : notnull
    {
        return Services.GetRequiredService<T>();
    }

    private static void SetTestEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("AWS__ServiceURL", "http://localhost:4566");
        Environment.SetEnvironmentVariable("Mongo__DatabaseUri", "mongodb://localhost:27017");
        Environment.SetEnvironmentVariable("ServiceBusSenderConfiguration__CadsCtsTopic__TopicName", CadsCtsTopicName);
        Environment.SetEnvironmentVariable("ServiceBusSenderConfiguration__CadsCtsTopic__TopicArn", string.Empty);
    }

    private static void ConfigureAwsOptions(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var awsOptions = provider.GetRequiredService<AWSOptions>();
        awsOptions.Credentials = new BasicAWSCredentials("test", "test");
        services.Replace(new ServiceDescriptor(typeof(AWSOptions), awsOptions));
    }

    private void ConfigureSimpleNotificationService(IServiceCollection services)
    {
        RemoveService<IAmazonSimpleNotificationService>(services);

        AmazonSimpleNotificationServiceMock = new Mock<IAmazonSimpleNotificationService>();

        AmazonSimpleNotificationServiceMock
            .Setup(x => x.ListTopicsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListTopicsResponse { HttpStatusCode = HttpStatusCode.OK, Topics = [new Topic { TopicArn = CadsCtsTopicArn }] });

        AmazonSimpleNotificationServiceMock
            .Setup(x => x.GetTopicAttributesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTopicAttributesResponse { HttpStatusCode = HttpStatusCode.OK });

        services.Replace(new ServiceDescriptor(typeof(IAmazonSimpleNotificationService), AmazonSimpleNotificationServiceMock.Object));
    }

    private void ConfigureDatabase(IServiceCollection services)
    {
        MongoClientMock = new Mock<IMongoClient>();

        MongoClientMock.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
            .Returns(() => new Mock<IMongoDatabase>().Object);

        services.Replace(new ServiceDescriptor(typeof(IMongoClient), MongoClientMock.Object));
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var service = services.FirstOrDefault(x => x.ServiceType == typeof(T));
        if (service != null)
        {
            services.Remove(service);
        }
    }
}