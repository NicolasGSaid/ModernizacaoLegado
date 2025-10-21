using MediatR;

namespace LegacyProcs.Application.Common;

/// <summary>
/// Interface base para Commands (operações de escrita)
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Interface base para Commands com retorno (operações de escrita)
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Interface base para Command Handlers
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

/// <summary>
/// Interface base para Command Handlers com retorno
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}
