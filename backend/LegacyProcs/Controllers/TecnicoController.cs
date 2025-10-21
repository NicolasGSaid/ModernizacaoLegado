using Microsoft.AspNetCore.Mvc;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Application.Commands;
using LegacyProcs.Application.Queries;
using LegacyProcs.Domain.Exceptions;
using MediatR;
using FluentValidation;

namespace LegacyProcs.Controllers
{
    /// <summary>
    /// Controller para gerenciar Técnicos
    /// ✅ MODERNIZADO: Padrão CQRS com MediatR
    /// ✅ CORRIGIDO: SQL Injection eliminado
    /// ✅ CORRIGIDO: Lógica extraída para Commands/Queries
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TecnicoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TecnicoController> _logger;

        public TecnicoController(IMediator mediator, ILogger<TecnicoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/tecnico
        /// Lista todos os técnicos com filtro opcional
        /// ✅ MODERNIZADO: Usa CQRS Query pattern
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(string? filtro = null, int page = 1, int pageSize = 10)
        {
            try
            {
                var query = new ListarTecnicosQuery
                {
                    Filtro = filtro,
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
                _logger.LogError(ex, "Erro inesperado ao listar técnicos");
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// GET: api/tecnico/5
        /// Obtém um técnico por ID
        /// ✅ MODERNIZADO: Usa CQRS Query pattern
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var query = new ObterTecnicoPorIdQuery { Id = id };
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (TecnicoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar técnico por ID: {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// POST: api/tecnico
        /// Cria um novo técnico
        /// ✅ MODERNIZADO: Usa CQRS Command pattern
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TecnicoCreateDto dto)
        {
            try
            {
                var command = new CriarTecnicoCommand
                {
                    Nome = dto.Nome,
                    Email = dto.Email,
                    Telefone = dto.Telefone,
                    Especialidade = dto.Especialidade
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
                _logger.LogError(ex, "Erro inesperado ao criar técnico");
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// PUT: api/tecnico/5
        /// Atualiza um técnico existente
        /// ✅ MODERNIZADO: Usa CQRS Command pattern
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TecnicoUpdateDto dto)
        {
            try
            {
                var command = new AtualizarTecnicoCommand
                {
                    Id = id,
                    Nome = dto.Nome,
                    Email = dto.Email,
                    Telefone = dto.Telefone,
                    Especialidade = dto.Especialidade
                };

                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (TecnicoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar técnico {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// PATCH: api/tecnico/5/status
        /// Altera o status de um técnico
        /// ✅ MODERNIZADO: Usa CQRS Command pattern
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AlterarStatus(int id, [FromBody] TecnicoAlterarStatusDto dto)
        {
            try
            {
                var command = new AlterarStatusTecnicoCommand
                {
                    Id = id,
                    NovoStatus = dto.Status
                };

                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (TecnicoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao alterar status do técnico {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }

        /// <summary>
        /// DELETE: api/tecnico/5
        /// Exclui um técnico
        /// ✅ MODERNIZADO: Usa CQRS Command pattern
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new ExcluirTecnicoCommand { Id = id };
                await _mediator.Send(command);
                return Ok(new { message = "Técnico excluído com sucesso" });
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }
            catch (TecnicoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir técnico {Id}", id);
                return StatusCode(500, "Erro interno do servidor. Contate o administrador.");
            }
        }
    }
}
