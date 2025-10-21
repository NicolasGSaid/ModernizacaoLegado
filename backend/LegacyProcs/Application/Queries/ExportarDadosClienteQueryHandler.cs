using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Xml.Linq;

namespace LegacyProcs.Application.Queries
{
    /// <summary>
    /// Handler para exportação de dados do cliente - Data Portability LGPD
    /// Suporta múltiplos formatos: JSON, XML, CSV
    /// </summary>
    public class ExportarDadosClienteQueryHandler : IRequestHandler<ExportarDadosClienteQuery, ExportarDadosClienteResponse>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IAuditLogRepository _auditRepository;
        private readonly ILogger<ExportarDadosClienteQueryHandler> _logger;

        public ExportarDadosClienteQueryHandler(
            IClienteRepository clienteRepository,
            IAuditLogRepository auditRepository,
            ILogger<ExportarDadosClienteQueryHandler> logger)
        {
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
            _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ExportarDadosClienteResponse> Handle(ExportarDadosClienteQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando exportação de dados do cliente {ClienteId} no formato {Formato} solicitada por {SolicitadoPor}", 
                request.ClienteId, request.FormatoExportacao, request.SolicitadoPor);

            try
            {
                // Buscar cliente
                var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
                if (cliente == null)
                {
                    return new ExportarDadosClienteResponse
                    {
                        Sucesso = false,
                        Mensagem = "Cliente não encontrado"
                    };
                }

                var numeroProtocolo = GerarNumeroProtocolo();

                // Preparar dados para exportação
                var dadosExportacao = new
                {
                    DadosBasicos = new
                    {
                        cliente.Id,
                        cliente.RazaoSocial,
                        cliente.NomeFantasia,
                        cliente.CNPJ,
                        cliente.Email,
                        cliente.Telefone,
                        cliente.Endereco,
                        cliente.Cidade,
                        cliente.Estado,
                        cliente.CEP,
                        cliente.DataCadastro
                    },
                    Metadados = new
                    {
                        DataExportacao = DateTime.UtcNow,
                        FormatoExportacao = request.FormatoExportacao,
                        SolicitadoPor = request.SolicitadoPor,
                        NumeroProtocolo = numeroProtocolo,
                        VersaoSistema = "1.0.0",
                        ConformidadeLGPD = "Art. 18, V - Direito à portabilidade"
                    }
                };

                // Incluir histórico se solicitado
                object dadosFinais = dadosExportacao;
                if (request.IncluirHistorico)
                {
                    var historicoAuditoria = await _auditRepository.ObterPorEntidadeAsync("Cliente", request.ClienteId.ToString(), cancellationToken);
                    dadosFinais = new
                    {
                        dadosExportacao.DadosBasicos,
                        dadosExportacao.Metadados,
                        HistoricoOperacoes = historicoAuditoria.Select(h => new
                        {
                            h.Operation,
                            h.Timestamp,
                            h.UserId,
                            h.UserName
                        }).ToList()
                    };
                }

                // Exportar no formato solicitado
                var dadosFormatados = request.FormatoExportacao.ToUpperInvariant() switch
                {
                    "JSON" => ExportarParaJson(dadosFinais),
                    "XML" => ExportarParaXml(dadosFinais),
                    "CSV" => ExportarParaCsv(dadosExportacao.DadosBasicos),
                    _ => throw new ArgumentException($"Formato de exportação não suportado: {request.FormatoExportacao}")
                };

                // Registrar auditoria da exportação
                var auditLog = AuditLog.CriarRegistroExport(
                    "Cliente",
                    request.SolicitadoPor,
                    request.SolicitadoPor,
                    "Sistema",
                    "LGPD-DataPortability",
                    JsonSerializer.Serialize(new
                    {
                        ClienteId = request.ClienteId,
                        Formato = request.FormatoExportacao,
                        IncluirHistorico = request.IncluirHistorico,
                        Protocolo = numeroProtocolo
                    })
                );

                await _auditRepository.AdicionarAsync(auditLog, cancellationToken);

                _logger.LogInformation("Exportação de dados do cliente {ClienteId} concluída. Protocolo: {Protocolo}", 
                    request.ClienteId, numeroProtocolo);

                return new ExportarDadosClienteResponse
                {
                    Sucesso = true,
                    Mensagem = "Dados exportados com sucesso conforme LGPD",
                    FormatoExportacao = request.FormatoExportacao,
                    DadosExportados = dadosFormatados,
                    DataExportacao = DateTime.UtcNow,
                    NumeroProtocolo = numeroProtocolo,
                    Metadados = new Dictionary<string, object>
                    {
                        ["TotalRegistros"] = 1,
                        ["IncluiHistorico"] = request.IncluirHistorico,
                        ["ConformidadeLGPD"] = "Art. 18, V",
                        ["ValidadeExportacao"] = DateTime.UtcNow.AddDays(30)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar dados do cliente {ClienteId}", request.ClienteId);
                
                return new ExportarDadosClienteResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno ao processar exportação de dados"
                };
            }
        }

        private static string ExportarParaJson(object dados)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Serialize(dados, options);
        }

        private static string ExportarParaXml(object dados)
        {
            var json = JsonSerializer.Serialize(dados);
            var doc = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            var xml = new XElement("DadosCliente");
            AdicionarElementosXml(xml, doc!);
            
            return xml.ToString();
        }

        private static void AdicionarElementosXml(XElement parent, Dictionary<string, object> dados)
        {
            foreach (var item in dados)
            {
                if (item.Value is JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == JsonValueKind.Object)
                    {
                        var subElement = new XElement(item.Key);
                        var subDados = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonElement.GetRawText());
                        AdicionarElementosXml(subElement, subDados!);
                        parent.Add(subElement);
                    }
                    else
                    {
                        parent.Add(new XElement(item.Key, jsonElement.ToString()));
                    }
                }
                else
                {
                    parent.Add(new XElement(item.Key, item.Value?.ToString() ?? ""));
                }
            }
        }

        private static string ExportarParaCsv(object dadosBasicos)
        {
            var csv = new StringBuilder();
            
            // Cabeçalho
            csv.AppendLine("Campo,Valor");
            
            // Usar reflexão para obter propriedades
            var propriedades = dadosBasicos.GetType().GetProperties();
            foreach (var prop in propriedades)
            {
                var valor = prop.GetValue(dadosBasicos)?.ToString() ?? "";
                csv.AppendLine($"{prop.Name},\"{valor}\"");
            }
            
            return csv.ToString();
        }

        private static string GerarNumeroProtocolo()
        {
            return $"LGPD-EXPORT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
    }
}
