using Cads.Core.Repositories;
using Cads.Core.Transactions;
using Cads.Infrastructure.Behaviors;
using Cads.Infrastructure.Database.Configuration;
using Cads.Infrastructure.Database.Factories;
using Cads.Infrastructure.Database.Factories.Implementations;
using Cads.Infrastructure.Database.Repositories;
using Cads.Infrastructure.Database.Transactions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System.Diagnostics.CodeAnalysis;

namespace Cads.Infrastructure.Database.Setup;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    private static bool s_mongoSerializersRegistered;

    public static void AddDatabaseDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterMongoDbGlobals();

        var mongoConfig = configuration.GetSection("Mongo").Get<MongoConfig>()!;
        services.Configure<MongoConfig>(configuration.GetSection("Mongo"));

        services.AddSingleton<IMongoDbClientFactory, MongoDbClientFactory>();
        services.AddScoped<IMongoSessionFactory, MongoSessionFactory>();

        services.AddScoped(sp => sp.GetRequiredService<IMongoSessionFactory>().GetSession());
        services.AddSingleton(sp => sp.GetRequiredService<IMongoDbClientFactory>().CreateClient());
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
        services.AddScoped(sp => (ITransactionManager)sp.GetRequiredService<IUnitOfWork>());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkTransactionBehavior<,>));

        if (mongoConfig.HealthcheckEnabled)
        {
            services.AddHealthChecks()
                .AddCheck<MongoDbHealthCheck>("mongodb", tags: ["db", "mongo"]);
        }
    }

    private static void RegisterMongoDbGlobals()
    {
        if (!s_mongoSerializersRegistered)
        {
            lock (typeof(ServiceCollectionExtensions))
            {
                if (!s_mongoSerializersRegistered)
                {
                    BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
                    ConventionRegistry.Register("CamelCase", new ConventionPack { new CamelCaseElementNameConvention() }, _ => true);
                    s_mongoSerializersRegistered = true;
                }
            }
        }
    }
}