using Cads.Application.Commands;
using Cads.Application.Queries;

namespace Cads.Application;

public interface IRequestExecutor
{
    Task<TResponse> ExecuteCommand<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<TResponse> ExecuteQuery<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}