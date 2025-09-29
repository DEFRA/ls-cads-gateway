namespace Cads.Core.Transactions;

public interface ITransactionManager
{
    void BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task AbortTransactionAsync();
}