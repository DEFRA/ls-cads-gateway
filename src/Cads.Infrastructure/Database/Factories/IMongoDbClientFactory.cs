using MongoDB.Driver;

namespace Cads.Infrastructure.Database.Factories;

public interface IMongoDbClientFactory
{
    IMongoClient CreateClient();
}