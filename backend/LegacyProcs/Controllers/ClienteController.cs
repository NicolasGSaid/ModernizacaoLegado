using Microsoft.AspNetCore.Mvc;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Application.Commands;
using LegacyProcs.Application.Queries;
using LegacyProcs.Domain.Exceptions;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace LegacyProcs.Controllers
{
    /// <summary>
    /// Controller para gerenciar Clientes
    /// MODERNIZADO: Padrão CQRS com MediatR
    /// CORRIGIDO: SQL Injection eliminado
    /// CORRIGIDO: Lógica extraída para Commands/Queries
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(IMediator mediator, ILogger<ClienteController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/cliente
        /// Lista todos os clientes com busca opcional
        /// MODERNIZADO: Usa CQRS Query pattern
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(string? busca = null, int page = 1, int pageSize = 10)
        {
            try
            {
                var query = new ListarClientesQuery
                {
                    Busca = busca,
                    Page = page,
                    PageSize = pageSize
                };

                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao listar clientes");
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// GET: api/cliente/5
        /// Obtém um cliente por ID
        /// MODERNIZADO: Usa CQRS Query pattern
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var query = new ObterClientePorIdQuery { Id = id };
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (ClienteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar cliente por ID: {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// POST: api/cliente
        /// Cria um novo cliente
        /// MODERNIZADO: Usa CQRS Command pattern
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ClienteCreateDto dto)
        {
            try
            {
                var command = new CriarClienteCommand
                {
                    RazaoSocial = dto.RazaoSocial,
                    NomeFantasia = dto.NomeFantasia,
                    CNPJ = dto.CNPJ,
                    Email = dto.Email,
                    Telefone = dto.Telefone,
                    Endereco = dto.Endereco,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    CEP = dto.CEP
                };

                var response = await _mediator.Send(command);
                return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar cliente");
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// PUT: api/cliente/5
        /// Atualiza um cliente existente
        /// MODERNIZADO: Usa CQRS Command pattern
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ClienteUpdateDto dto)
        {
            try
            {
                var command = new AtualizarClienteCommand
                {
                    Id = id,
                    RazaoSocial = dto.RazaoSocial,
                    NomeFantasia = dto.NomeFantasia,
                    Email = dto.Email,
                    Telefone = dto.Telefone,
                    Endereco = dto.Endereco,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    CEP = dto.CEP
                };

                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (ClienteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar cliente {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// DELETE: api/cliente/5
        /// Exclui um cliente
        /// MODERNIZADO: Usa CQRS Command pattern
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new ExcluirClienteCommand { Id = id };
                await _mediator.Send(command);
                return Ok(new { message = "Cliente excluído com sucesso" });
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (ClienteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir cliente {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }
    }
}
