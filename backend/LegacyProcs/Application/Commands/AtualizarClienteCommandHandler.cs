using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Domain.Exceptions;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Handler para atualizar cliente
/// </summary>
public class AtualizarClienteCommandHandler : ICommandHandler<AtualizarClienteCommand, ClienteResponseDto>
{
    private readonly IClienteRepository _repository;
    private readonly ILogger<AtualizarClienteCommandHandler> _logger;

    public AtualizarClienteCommandHandler(
        IClienteRepository repository,
        ILogger<AtualizarClienteCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ClienteResponseDto> Handle(AtualizarClienteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando cliente {Id}", request.Id);

        var cliente = await _repository.GetByIdAsync(request.Id);
        
        if (cliente == null)
        {
            throw new ClienteNotFoundException(request.Id);
        }

        cliente.AtualizarInformacoes(
            request.RazaoSocial,
            request.NomeFantasia,
            request.Email,
            request.Telefone ?? string.Empty,
            request.Endereco,
            request.Cidade,
            request.Estado,
            request.CEP);

        await _repository.UpdateAsync(cliente);

        _logger.LogInformation("Cliente {Id} atualizado com sucesso", request.Id);

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
