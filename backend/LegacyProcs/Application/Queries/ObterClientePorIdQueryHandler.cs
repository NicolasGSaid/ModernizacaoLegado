using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Domain.Exceptions;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Handler para obter cliente por ID
/// </summary>
public class ObterClientePorIdQueryHandler : IQueryHandler<ObterClientePorIdQuery, ClienteResponseDto>
{
    private readonly IClienteRepository _repository;
    private readonly ILogger<ObterClientePorIdQueryHandler> _logger;

    public ObterClientePorIdQueryHandler(
        IClienteRepository repository,
        ILogger<ObterClientePorIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ClienteResponseDto> Handle(ObterClientePorIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando cliente por ID: {Id}", request.Id);

        var cliente = await _repository.GetByIdAsync(request.Id);
        
        if (cliente == null)
        {
            throw new ClienteNotFoundException(request.Id);
        }

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
