using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para criar técnico
/// </summary>
public class CriarTecnicoCommandHandler : ICommandHandler<CriarTecnicoCommand, TecnicoResponseDto>
{
    private readonly ITecnicoRepository _repository;
    private readonly ILogger<CriarTecnicoCommandHandler> _logger;

    public CriarTecnicoCommandHandler(
        ITecnicoRepository repository,
        ILogger<CriarTecnicoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TecnicoResponseDto> Handle(CriarTecnicoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando novo técnico. Nome: {Nome}, Email: {Email}", 
            request.Nome, request.Email);

        var tecnico = Tecnico.Criar(
            request.Nome,
            request.Email,
            request.Telefone,
            request.Especialidade);

        await _repository.AddAsync(tecnico);

        _logger.LogInformation("Técnico criado com sucesso. ID: {Id}", tecnico.Id);

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
