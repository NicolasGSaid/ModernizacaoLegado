using MediatR;

namespace LegacyProcs.Application.Common;

/// <summary>
/// Interface base para Queries (operações de leitura)
/// </summary>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Interface base para Query Handlers
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
