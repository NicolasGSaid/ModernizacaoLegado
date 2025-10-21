using LegacyProcs.Application.Common;
using LegacyProcs.Application.DTOs;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Handler para listar clientes
/// </summary>
public class ListarClientesQueryHandler : IQueryHandler<ListarClientesQuery, PaginatedResultDto<ClienteResponseDto>>
{
    private readonly IClienteRepository _repository;
    private readonly ILogger<ListarClientesQueryHandler> _logger;

    public ListarClientesQueryHandler(
        IClienteRepository repository,
        ILogger<ListarClientesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PaginatedResultDto<ClienteResponseDto>> Handle(ListarClientesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando clientes. Busca: {Busca}, Page: {Page}, PageSize: {PageSize}", 
            request.Busca, request.Page, request.PageSize);

        var (clientes, totalItems) = await _repository.GetPagedAsync(request.Page, request.PageSize, request.Busca);

        var clientesDto = clientes.Select(c => new ClienteResponseDto
        {
            Id = c.Id,
            RazaoSocial = c.RazaoSocial,
            NomeFantasia = c.NomeFantasia,
            CNPJ = c.CNPJ,
            CNPJFormatado = c.GetCNPJFormatado(),
            Email = c.Email,
            Telefone = c.Telefone,
            Endereco = c.Endereco,
            Cidade = c.Cidade,
            Estado = c.Estado,
            CEP = c.CEP,
            CEPFormatado = c.GetCEPFormatado(),
            DataCadastro = c.DataCadastro
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        return new PaginatedResultDto<ClienteResponseDto>
        {
            Data = clientesDto,
            TotalItems = totalItems,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }
}
