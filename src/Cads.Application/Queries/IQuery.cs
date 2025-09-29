using MediatR;

namespace Cads.Application.Queries;

public interface IQuery<TResponse> : IRequest<TResponse> { }