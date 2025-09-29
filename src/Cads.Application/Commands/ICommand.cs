using MediatR;

namespace Cads.Application.Commands;

public interface ICommand<TResponse> : IRequest<TResponse> { }