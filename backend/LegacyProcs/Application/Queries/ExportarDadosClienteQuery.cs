using MediatR;

namespace LegacyProcs.Application.Queries
{
    /// <summary>
    /// Query para exportação de dados do cliente - Data Portability LGPD
    /// Implementa o direito à portabilidade conforme Art. 18, V da LGPD
    /// </summary>
    public class ExportarDadosClienteQuery : IRequest<ExportarDadosClienteResponse>
    {
        public int ClienteId { get; set; }
        public string FormatoExportacao { get; set; } = "JSON"; // JSON, XML, CSV
        public string SolicitadoPor { get; set; } = string.Empty;
        public bool IncluirHistorico { get; set; } = true;
    }

    public class ExportarDadosClienteResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string FormatoExportacao { get; set; } = string.Empty;
        public string DadosExportados { get; set; } = string.Empty;
        public DateTime DataExportacao { get; set; }
        public string NumeroProtocolo { get; set; } = string.Empty;
        public Dictionary<string, object> Metadados { get; set; } = new();
    }
}
