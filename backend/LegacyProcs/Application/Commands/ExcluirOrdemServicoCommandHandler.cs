using LegacyProcs.Application.Common;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Domain.Exceptions;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para excluir ordem de serviço
/// Contém lógica de negócio extraída do controller DELETE
/// </summary>
public class ExcluirOrdemServicoCommandHandler : ICommandHandler<ExcluirOrdemServicoCommand>
{
    private readonly IOrdemServicoRepository _repository;
    private readonly ILogger<ExcluirOrdemServicoCommandHandler> _logger;

    public ExcluirOrdemServicoCommandHandler(
        IOrdemServicoRepository repository,
        ILogger<ExcluirOrdemServicoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(ExcluirOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Excluindo ordem de serviço {Id}", request.Id);

        // Buscar ordem de serviço
        var ordemServico = await _repository.GetByIdAsync(request.Id);
        
        if (ordemServico == null)
        {
            throw new OrdemServicoNotFoundException(request.Id);
        }

        // Validar regra de negócio para exclusão
        if (!ordemServico.PodeSerExcluida())
        {
            throw new BusinessRuleViolationException(
                "ExclusaoOrdemServico", 
                $"Não é possível excluir ordem de serviço com status '{ordemServico.GetStatusDisplay()}'. Apenas ordens pendentes podem ser excluídas.");
        }

        // Excluir usando repository
        await _repository.DeleteAsync(ordemServico);

        _logger.LogInformation("Ordem de serviço {Id} excluída com sucesso", request.Id);
    }
}
