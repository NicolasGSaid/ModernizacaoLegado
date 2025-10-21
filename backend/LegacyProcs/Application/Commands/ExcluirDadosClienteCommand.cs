using MediatR;

namespace LegacyProcs.Application.Commands
{
    /// <summary>
    /// Command para exclusão completa de dados do cliente - Right to Erasure LGPD
    /// Implementa o direito ao esquecimento conforme Art. 18, VI da LGPD
    /// </summary>
    public class ExcluirDadosClienteCommand : IRequest<ExcluirDadosClienteResponse>
    {
        public int ClienteId { get; set; }
        public string MotivoExclusao { get; set; } = string.Empty;
        public string SolicitadoPor { get; set; } = string.Empty;
        public bool ConfirmarExclusao { get; set; }
    }

    public class ExcluirDadosClienteResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public List<string> DadosExcluidos { get; set; } = new();
        public DateTime DataExclusao { get; set; }
        public string NumeroProtocolo { get; set; } = string.Empty;
    }
}
