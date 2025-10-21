using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LegacyProcs.Application.Commands
{
    /// <summary>
    /// Handler para exclusão completa de dados do cliente - Right to Erasure LGPD
    /// Implementa hard delete com auditoria completa
    /// </summary>
    public class ExcluirDadosClienteCommandHandler : IRequestHandler<ExcluirDadosClienteCommand, ExcluirDadosClienteResponse>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IAuditLogRepository _auditRepository;
        private readonly ILogger<ExcluirDadosClienteCommandHandler> _logger;

        public ExcluirDadosClienteCommandHandler(
            IClienteRepository clienteRepository,
            IAuditLogRepository auditRepository,
            ILogger<ExcluirDadosClienteCommandHandler> logger)
        {
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
            _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ExcluirDadosClienteResponse> Handle(ExcluirDadosClienteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando exclusão de dados do cliente {ClienteId} solicitada por {SolicitadoPor}", 
                request.ClienteId, request.SolicitadoPor);

            try
            {
                // Validar confirmação obrigatória
                if (!request.ConfirmarExclusao)
                {
                    return new ExcluirDadosClienteResponse
                    {
                        Sucesso = false,
                        Mensagem = "Confirmação de exclusão é obrigatória para exercer o direito ao esquecimento"
                    };
                }

                // Buscar cliente
                var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
                if (cliente == null)
                {
                    return new ExcluirDadosClienteResponse
                    {
                        Sucesso = false,
                        Mensagem = "Cliente não encontrado"
                    };
                }

                // Capturar dados antes da exclusão para auditoria
                var dadosExcluidos = new
                {
                    cliente.Id,
                    cliente.RazaoSocial,
                    cliente.CNPJ,
                    cliente.Email,
                    cliente.Telefone,
                    cliente.Endereco,
                    cliente.Cidade,
                    cliente.Estado,
                    cliente.CEP,
                    cliente.DataCadastro
                };

                var numeroProtocolo = GerarNumeroProtocolo();

                // Registrar auditoria ANTES da exclusão
                var auditLog = AuditLog.CriarRegistroDelete(
                    "Cliente", 
                    request.ClienteId.ToString(), 
                    request.SolicitadoPor, 
                    request.SolicitadoPor, 
                    "Sistema", 
                    "LGPD-RightToErasure",
                    JsonSerializer.Serialize(new { 
                        Motivo = request.MotivoExclusao,
                        Protocolo = numeroProtocolo,
                        DadosExcluidos = dadosExcluidos 
                    })
                );

                await _auditRepository.AdicionarAsync(auditLog, cancellationToken);

                // Executar exclusão física (hard delete)
                await _clienteRepository.DeleteAsync(cliente);

                _logger.LogWarning("Cliente {ClienteId} excluído permanentemente. Protocolo: {Protocolo}", 
                    request.ClienteId, numeroProtocolo);

                return new ExcluirDadosClienteResponse
                {
                    Sucesso = true,
                    Mensagem = "Dados do cliente excluídos permanentemente conforme LGPD",
                    DadosExcluidos = new List<string>
                    {
                        "Dados pessoais (Razão Social, CNPJ)",
                        "Dados de contato (Email, Telefone)",
                        "Dados de localização (Endereço, Cidade, Estado, CEP)",
                        "Metadados (Data de cadastro)"
                    },
                    DataExclusao = DateTime.UtcNow,
                    NumeroProtocolo = numeroProtocolo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir dados do cliente {ClienteId}", request.ClienteId);
                
                return new ExcluirDadosClienteResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno ao processar exclusão de dados"
                };
            }
        }

        private static string GerarNumeroProtocolo()
        {
            return $"LGPD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
    }
}
