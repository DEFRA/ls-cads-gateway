using MongoDB.Driver;

namespace Cads.Core.Transactions;

public interface IUnitOfWork
{
    Task CommitAsync();
    Task RollbackAsync();
    IClientSessionHandle Session { get; }
}