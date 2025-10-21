using LegacyProcs.Application.Common;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Domain.Exceptions;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para excluir cliente
/// </summary>
public class ExcluirClienteCommandHandler : ICommandHandler<ExcluirClienteCommand>
{
    private readonly IClienteRepository _repository;
    private readonly ILogger<ExcluirClienteCommandHandler> _logger;

    public ExcluirClienteCommandHandler(
        IClienteRepository repository,
        ILogger<ExcluirClienteCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(ExcluirClienteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Excluindo cliente {Id}", request.Id);

        var cliente = await _repository.GetByIdAsync(request.Id);
        
        if (cliente == null)
        {
            throw new ClienteNotFoundException(request.Id);
        }

        await _repository.DeleteAsync(cliente);

        _logger.LogInformation("Cliente {Id} excluído com sucesso", request.Id);
    }
}
