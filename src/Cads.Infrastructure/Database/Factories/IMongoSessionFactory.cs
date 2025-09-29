using MongoDB.Driver;

namespace Cads.Infrastructure.Database.Factories;

public interface IMongoSessionFactory
{
    IClientSessionHandle GetSession();
}