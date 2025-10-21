using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para criar ordem de serviço
/// Contém a lógica de negócio extraída do controller
/// </summary>
public class CriarOrdemServicoCommandHandler : ICommandHandler<CriarOrdemServicoCommand, OrdemServicoResponseDto>
{
    private readonly IOrdemServicoRepository _repository;
    private readonly ILogger<CriarOrdemServicoCommandHandler> _logger;

    public CriarOrdemServicoCommandHandler(
        IOrdemServicoRepository repository,
        ILogger<CriarOrdemServicoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OrdemServicoResponseDto> Handle(CriarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando nova ordem de serviço. Título: {Titulo}, TecnicoId: {TecnicoId}", 
            request.Titulo, request.TecnicoId);

        // Usar factory method da entidade (Rich Domain Model)
        var ordemServico = OrdemServico.Criar(request.Titulo, request.Descricao, request.TecnicoId);
        
        // Persistir usando repository pattern
        await _repository.AddAsync(ordemServico);

        _logger.LogInformation("Ordem de serviço criada com sucesso. ID: {Id}", ordemServico.Id);

        // Recarregar com relacionamento para retornar nome do técnico
        var ordemServicoCompleta = await _repository.GetByIdAsync(ordemServico.Id);

        // Retornar DTO de resposta
        return new OrdemServicoResponseDto
        {
            Id = ordemServicoCompleta!.Id,
            Titulo = ordemServicoCompleta.Titulo,
            Descricao = ordemServicoCompleta.Descricao,
            TecnicoId = ordemServicoCompleta.TecnicoId,
            TecnicoNome = ordemServicoCompleta.Tecnico?.Nome ?? "N/A",
            Status = ordemServicoCompleta.GetStatusDisplay(),
            DataCriacao = ordemServicoCompleta.DataCriacao,
            DataAtualizacao = ordemServicoCompleta.DataAtualizacao
        };
    }
}
