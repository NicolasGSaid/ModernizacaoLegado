using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Exceptions;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Handler para obter técnico por ID
/// </summary>
public class ObterTecnicoPorIdQueryHandler : IQueryHandler<ObterTecnicoPorIdQuery, TecnicoResponseDto>
{
    private readonly ITecnicoRepository _repository;
    private readonly ILogger<ObterTecnicoPorIdQueryHandler> _logger;

    public ObterTecnicoPorIdQueryHandler(
        ITecnicoRepository repository,
        ILogger<ObterTecnicoPorIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TecnicoResponseDto> Handle(ObterTecnicoPorIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo técnico por ID: {Id}", request.Id);

        var tecnico = await _repository.GetByIdAsync(request.Id);
        
        if (tecnico == null)
        {
            throw new TecnicoNotFoundException(request.Id);
        }

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
