using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Domain.Exceptions;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Handler para obter ordem de serviço por ID
/// Contém lógica de consulta extraída do controller GET por ID
/// </summary>
public class ObterOrdemServicoPorIdQueryHandler : IQueryHandler<ObterOrdemServicoPorIdQuery, OrdemServicoResponseDto>
{
    private readonly IOrdemServicoRepository _repository;
    private readonly ILogger<ObterOrdemServicoPorIdQueryHandler> _logger;

    public ObterOrdemServicoPorIdQueryHandler(
        IOrdemServicoRepository repository,
        ILogger<ObterOrdemServicoPorIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OrdemServicoResponseDto> Handle(ObterOrdemServicoPorIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando ordem de serviço por ID: {Id}", request.Id);

        // Buscar no repository
        var ordemServico = await _repository.GetByIdAsync(request.Id);
        
        if (ordemServico == null)
        {
            throw new OrdemServicoNotFoundException(request.Id);
        }

        _logger.LogInformation("Ordem de serviço {Id} encontrada: {Titulo}", request.Id, ordemServico.Titulo);

        // Mapear para DTO
        return new OrdemServicoResponseDto
        {
            Id = ordemServico.Id,
            Titulo = ordemServico.Titulo,
            Descricao = ordemServico.Descricao,
            TecnicoId = ordemServico.TecnicoId,
            TecnicoNome = ordemServico.Tecnico?.Nome ?? "N/A",
            Status = ordemServico.GetStatusDisplay(),
            DataCriacao = ordemServico.DataCriacao,
            DataAtualizacao = ordemServico.DataAtualizacao
        };
    }
}
