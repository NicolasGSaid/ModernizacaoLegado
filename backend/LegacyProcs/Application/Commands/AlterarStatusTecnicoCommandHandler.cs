using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Exceptions;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para alterar status do técnico
/// </summary>
public class AlterarStatusTecnicoCommandHandler : ICommandHandler<AlterarStatusTecnicoCommand, TecnicoResponseDto>
{
    private readonly ITecnicoRepository _repository;
    private readonly ILogger<AlterarStatusTecnicoCommandHandler> _logger;

    public AlterarStatusTecnicoCommandHandler(
        ITecnicoRepository repository,
        ILogger<AlterarStatusTecnicoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TecnicoResponseDto> Handle(AlterarStatusTecnicoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Alterando status do técnico ID: {Id} para {NovoStatus}", request.Id, request.NovoStatus);

        var tecnico = await _repository.GetByIdAsync(request.Id);
        
        if (tecnico == null)
        {
            throw new TecnicoNotFoundException(request.Id);
        }

        tecnico.AlterarStatus(request.NovoStatus);

        await _repository.UpdateAsync(tecnico);

        _logger.LogInformation("Status do técnico {Id} alterado para {Status}", request.Id, request.NovoStatus);

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
