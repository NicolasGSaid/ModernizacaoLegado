using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Infrastructure.Data;
using LegacyProcs.Application.Commands;
using LegacyProcs.Application.Queries;
using LegacyProcs.Application.DTOs;
using LegacyProcs.Domain.Exceptions;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace LegacyProcs.Controllers
{
    /// <summary>
    /// Controller para gerenciar Ordens de Serviço
    /// Modernizado: ASP.NET Core + Entity Framework Core + Rich Domain Model
    /// ✅ CORRIGIDO: SQL Injection eliminado com EF Core
    /// ✅ CORRIGIDO: Validações movidas para camada Domain
    /// ✅ CORRIGIDO: Lógica de negócio encapsulada na entidade
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdemServicoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrdemServicoController> _logger;
        private readonly IMediator _mediator;

        public OrdemServicoController(AppDbContext context, ILogger<OrdemServicoController> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Lista todas as ordens de serviço com paginação e filtro opcional
        /// </summary>
        /// <param name="filtro">Filtro opcional para buscar por título</param>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Itens por página (padrão: 10, máximo: 100)</param>
        /// <returns>Lista paginada de ordens de serviço</returns>
        /// <response code="200">Lista retornada com sucesso</response>
        /// <response code="400">Parâmetros de paginação inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar ordens de serviço",
            Description = "Retorna uma lista paginada de ordens de serviço com filtro opcional por título",
            Tags = new[] { "Ordens de Serviço" }
        )]
        [SwaggerResponse(200, "Lista retornada com sucesso", typeof(PaginatedResultDto<OrdemServicoResponseDto>))]
        [SwaggerResponse(400, "Parâmetros inválidos")]
        [SwaggerResponse(500, "Erro interno do servidor")]
        public async Task<IActionResult> Get(string? filtro = null, int page = 1, int pageSize = 10)
        {
            try
            {
                // ✅ Mapear parâmetros para Query
                var query = new ListarOrdensServicoQuery
                {
                    Filtro = filtro,
                    Page = page,
                    PageSize = pageSize
                };

                // ✅ Executar via MediatR (validação automática + logging)
                var response = await _mediator.Send(query);

                return Ok(response);
            }
            catch (FluentValidation.ValidationException ex)
            {
                // ✅ Erros de validação do FluentValidation
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (Exception ex)
            {
                // ✅ Log estruturado sem exposição de detalhes internos
                _logger.LogError(ex, "Erro inesperado ao listar ordens de serviço");
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// Obtém uma ordem de serviço específica por ID
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Dados da ordem de serviço</returns>
        /// <response code="200">Ordem de serviço encontrada</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obter ordem de serviço por ID",
            Description = "Retorna os dados completos de uma ordem de serviço específica",
            Tags = new[] { "Ordens de Serviço" }
        )]
        [SwaggerResponse(200, "Ordem de serviço encontrada", typeof(OrdemServicoResponseDto))]
        [SwaggerResponse(400, "ID inválido")]
        [SwaggerResponse(404, "Ordem de serviço não encontrada")]
        [SwaggerResponse(500, "Erro interno do servidor")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                // ✅ Criar Query
                var query = new ObterOrdemServicoPorIdQuery { Id = id };

                // ✅ Executar via MediatR (validação automática + logging)
                var response = await _mediator.Send(query);

                return Ok(response);
            }
            catch (FluentValidation.ValidationException ex)
            {
                // ✅ Erros de validação do FluentValidation
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (OrdemServicoNotFoundException ex)
            {
                // ✅ Ordem de serviço não encontrada
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // ✅ Log estruturado sem exposição de detalhes internos
                _logger.LogError(ex, "Erro inesperado ao buscar ordem de serviço por ID: {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// POST: api/ordemservico
        /// Cria uma nova ordem de serviço
        /// ✅ REFATORADO: Usa padrão CQRS com MediatR
        /// ✅ CORRIGIDO: Validações automáticas com FluentValidation
        /// ✅ CORRIGIDO: Lógica extraída para Command Handler
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrdemServicoCreateDto dto)
        {
            try
            {
                // ✅ Mapear DTO para Command
                var command = new CriarOrdemServicoCommand
                {
                    Titulo = dto.Titulo,
                    Descricao = dto.Descricao,
                    TecnicoId = dto.TecnicoId
                };

                // ✅ Executar via MediatR (validação automática + logging)
                var response = await _mediator.Send(command);

                return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
            }
            catch (FluentValidation.ValidationException ex)
            {
                // ✅ Erros de validação do FluentValidation
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (ArgumentException ex)
            {
                // ✅ Erros de validação de domínio
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // ✅ Log estruturado sem exposição de detalhes internos
                _logger.LogError(ex, "Erro inesperado ao criar ordem de serviço");
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// PUT: api/ordemservico/5
        /// Atualiza todos os dados de uma ordem de serviço
        /// ✅ REFATORADO: Usa padrão CQRS com MediatR
        /// ✅ CORRIGIDO: Validações automáticas com FluentValidation
        /// ✅ CORRIGIDO: Lógica extraída para Command Handler
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OrdemServicoUpdateDto dto)
        {
            try
            {
                // ✅ Mapear para Command
                var command = new AtualizarOrdemServicoCommand
                {
                    Id = id,
                    Titulo = dto.Titulo,
                    Descricao = dto.Descricao,
                    TecnicoId = dto.TecnicoId,
                    Status = dto.Status
                };

                // ✅ Executar via MediatR (validação automática + logging)
                var response = await _mediator.Send(command);

                return Ok(response);
            }
            catch (FluentValidation.ValidationException ex)
            {
                // ✅ Erros de validação do FluentValidation
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (OrdemServicoNotFoundException ex)
            {
                // ✅ Ordem de serviço não encontrada
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // ✅ Erros de validação de domínio (status inválido, transição inválida)
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // ✅ Erros de regra de negócio (transição inválida)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // ✅ Log estruturado sem exposição de detalhes internos
                _logger.LogError(ex, "Erro inesperado ao atualizar ordem de serviço {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// PATCH: api/ordemservico/5/status
        /// Atualiza apenas o status de uma ordem de serviço
        /// ✅ REFATORADO: Usa padrão CQRS com MediatR
        /// ✅ CORRIGIDO: Validações automáticas com FluentValidation
        /// ✅ CORRIGIDO: Lógica extraída para Command Handler
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> PatchStatus(int id, [FromBody] OrdemServicoUpdateStatusDto dto)
        {
            try
            {
                // ✅ Mapear para Command
                var command = new AlterarStatusOrdemServicoCommand
                {
                    Id = id,
                    Status = dto.Status
                };

                // ✅ Executar via MediatR (validação automática + logging)
                var response = await _mediator.Send(command);

                return Ok(response);
            }
            catch (FluentValidation.ValidationException ex)
            {
                // ✅ Erros de validação do FluentValidation
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (OrdemServicoNotFoundException ex)
            {
                // ✅ Ordem de serviço não encontrada
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // ✅ Erros de validação de domínio (status inválido, transição inválida)
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // ✅ Erros de regra de negócio (transição inválida)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // ✅ Log estruturado sem exposição de detalhes internos
                _logger.LogError(ex, "Erro inesperado ao alterar status da ordem de serviço {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// DELETE: api/ordemservico/5
        /// Exclui uma ordem de serviço
        /// ✅ REFATORADO: Usa padrão CQRS com MediatR
        /// ✅ CORRIGIDO: Validações automáticas com FluentValidation
        /// ✅ CORRIGIDO: Lógica extraída para Command Handler
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // ✅ Criar Command
                var command = new ExcluirOrdemServicoCommand { Id = id };

                // ✅ Executar via MediatR (validação automática + logging)
                await _mediator.Send(command);

                return Ok(new { message = "Ordem de serviço excluída com sucesso" });
            }
            catch (FluentValidation.ValidationException ex)
            {
                // ✅ Erros de validação do FluentValidation
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (OrdemServicoNotFoundException ex)
            {
                // ✅ Ordem de serviço não encontrada
                return NotFound(ex.Message);
            }
            catch (BusinessRuleViolationException ex)
            {
                // ✅ Violação de regra de negócio (não pode excluir)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // ✅ Log estruturado sem exposição de detalhes internos
                _logger.LogError(ex, "Erro inesperado ao excluir ordem de serviço {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }
    }
}
