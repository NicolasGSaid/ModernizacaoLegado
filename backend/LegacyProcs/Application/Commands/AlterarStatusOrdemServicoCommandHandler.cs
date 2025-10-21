using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Domain.Exceptions;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para alterar status de ordem de serviço
/// Contém lógica de negócio extraída do controller PUT
/// </summary>
public class AlterarStatusOrdemServicoCommandHandler : ICommandHandler<AlterarStatusOrdemServicoCommand, OrdemServicoResponseDto>
{
    private readonly IOrdemServicoRepository _repository;
    private readonly ILogger<AlterarStatusOrdemServicoCommandHandler> _logger;

    public AlterarStatusOrdemServicoCommandHandler(
        IOrdemServicoRepository repository,
        ILogger<AlterarStatusOrdemServicoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OrdemServicoResponseDto> Handle(AlterarStatusOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Alterando status da ordem de serviço {Id} para {Status}", 
            request.Id, request.Status);

        // Buscar ordem de serviço
        var ordemServico = await _repository.GetByIdAsync(request.Id);
        
        if (ordemServico == null)
        {
            throw new OrdemServicoNotFoundException(request.Id);
        }

        // Converter string para enum com validação
        var novoStatus = StatusOSExtensions.FromString(request.Status);
        
        // Usar método de domínio (Rich Domain Model)
        ordemServico.AlterarStatus(novoStatus);
        
        // Persistir alterações
        await _repository.UpdateAsync(ordemServico);

        _logger.LogInformation("Status da ordem de serviço {Id} alterado para {Status} com sucesso", 
            request.Id, request.Status);

        // Retornar DTO atualizado
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
