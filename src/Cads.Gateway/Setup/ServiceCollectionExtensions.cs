using Asp.Versioning;
using Cads.Application.Setup;
using Cads.Infrastructure.Database.Setup;
using Cads.Infrastructure.Messaging.Setup;
using System.Text.Json.Serialization;

namespace Cads.Gateway.Setup;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(opts =>
            {
                var enumConverter = new JsonStringEnumConverter();
                opts.JsonSerializerOptions.Converters.Add(enumConverter);
            });

        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddDefaultAWSOptions(configuration.GetAWSOptions());

        services.ConfigureHealthChecks();

        services.AddApplicationLayer();

        services.AddDatabaseDependencies(configuration);

        services.AddMessagingDependencies(configuration);
    }

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
    }
}