using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Exceptions;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para atualizar técnico
/// </summary>
public class AtualizarTecnicoCommandHandler : ICommandHandler<AtualizarTecnicoCommand, TecnicoResponseDto>
{
    private readonly ITecnicoRepository _repository;
    private readonly ILogger<AtualizarTecnicoCommandHandler> _logger;

    public AtualizarTecnicoCommandHandler(
        ITecnicoRepository repository,
        ILogger<AtualizarTecnicoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TecnicoResponseDto> Handle(AtualizarTecnicoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando técnico ID: {Id}", request.Id);

        var tecnico = await _repository.GetByIdAsync(request.Id);
        
        if (tecnico == null)
        {
            throw new TecnicoNotFoundException(request.Id);
        }

        tecnico.AtualizarInformacoes(
            request.Nome,
            request.Email,
            request.Telefone,
            request.Especialidade);

        await _repository.UpdateAsync(tecnico);

        _logger.LogInformation("Técnico {Id} atualizado com sucesso", request.Id);

        return new TecnicoResponseDto
        {
            Id = tecnico.Id,
            Nome = tecnico.Nome,
            Email = tecnico.Email,
            Telefone = tecnico.Telefone,
            Especialidade = tecnico.Especialidade,
            Status = tecnico.Status.ToString(),
            DataCadastro = tecnico.DataCadastro
        };
    }
}
