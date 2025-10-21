using LegacyProcs.Application.Common;
using LegacyProcs.Domain.Exceptions;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para excluir técnico
/// </summary>
public class ExcluirTecnicoCommandHandler : ICommandHandler<ExcluirTecnicoCommand>
{
    private readonly ITecnicoRepository _repository;
    private readonly ILogger<ExcluirTecnicoCommandHandler> _logger;

    public ExcluirTecnicoCommandHandler(
        ITecnicoRepository repository,
        ILogger<ExcluirTecnicoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(ExcluirTecnicoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Excluindo técnico ID: {Id}", request.Id);

        var tecnico = await _repository.GetByIdAsync(request.Id);
        
        if (tecnico == null)
        {
            throw new TecnicoNotFoundException(request.Id);
        }

        await _repository.DeleteAsync(tecnico);

        _logger.LogInformation("Técnico {Id} excluído com sucesso", request.Id);
    }
}
