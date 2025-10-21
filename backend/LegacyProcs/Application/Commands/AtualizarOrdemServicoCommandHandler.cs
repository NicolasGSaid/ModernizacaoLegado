using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Enums;
using LegacyProcs.Domain.Exceptions;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para atualizar todos os dados de uma ordem de serviço
/// </summary>
public class AtualizarOrdemServicoCommandHandler : ICommandHandler<AtualizarOrdemServicoCommand, OrdemServicoResponseDto>
{
    private readonly IOrdemServicoRepository _repository;
    private readonly ILogger<AtualizarOrdemServicoCommandHandler> _logger;

    public AtualizarOrdemServicoCommandHandler(
        IOrdemServicoRepository repository,
        ILogger<AtualizarOrdemServicoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OrdemServicoResponseDto> Handle(AtualizarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando ordem de serviço ID: {Id}", request.Id);

        var ordemServico = await _repository.GetByIdAsync(request.Id);
        
        if (ordemServico == null)
        {
            throw new OrdemServicoNotFoundException(request.Id);
        }

        // Atualizar informações básicas
        ordemServico.AtualizarInformacoes(
            request.Titulo,
            request.Descricao,
            request.TecnicoId);

        // Atualizar status APENAS se realmente mudou
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var novoStatus = StatusOSExtensions.FromString(request.Status);
            var statusAtual = ordemServico.Status;
            
            // Só alterar se o status for diferente do atual
            if (statusAtual != novoStatus)
            {
                ordemServico.AlterarStatus(novoStatus);
            }
        }

        await _repository.UpdateAsync(ordemServico);

        _logger.LogInformation("Ordem de serviço {Id} atualizada com sucesso", request.Id);

        // Recarregar com relacionamento
        var ordemServicoCompleta = await _repository.GetByIdAsync(ordemServico.Id);

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
