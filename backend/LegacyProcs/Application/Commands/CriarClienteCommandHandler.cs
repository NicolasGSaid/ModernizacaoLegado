using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para criar cliente
/// </summary>
public class CriarClienteCommandHandler : ICommandHandler<CriarClienteCommand, ClienteResponseDto>
{
    private readonly IClienteRepository _repository;
    private readonly ILogger<CriarClienteCommandHandler> _logger;

    public CriarClienteCommandHandler(
        IClienteRepository repository,
        ILogger<CriarClienteCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ClienteResponseDto> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando novo cliente. Razão Social: {RazaoSocial}, CNPJ: {CNPJ}", 
            request.RazaoSocial, request.CNPJ);

        var cliente = Cliente.Criar(
            request.RazaoSocial,
            request.NomeFantasia,
            request.CNPJ,
            request.Email,
            request.Telefone ?? string.Empty,
            request.Endereco,
            request.Cidade,
            request.Estado,
            request.CEP);

        await _repository.AddAsync(cliente);

        _logger.LogInformation("Cliente criado com sucesso. ID: {Id}", cliente.Id);

        return new ClienteResponseDto
        {
            Id = cliente.Id,
            RazaoSocial = cliente.RazaoSocial,
            NomeFantasia = cliente.NomeFantasia,
            CNPJ = cliente.CNPJ,
            CNPJFormatado = cliente.GetCNPJFormatado(),
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            Endereco = cliente.Endereco,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado,
            CEP = cliente.CEP,
            CEPFormatado = cliente.GetCEPFormatado(),
            DataCadastro = cliente.DataCadastro
        };
    }
}
