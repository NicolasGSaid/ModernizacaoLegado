using System;
using System.ComponentModel.DataAnnotations;
using LegacyProcs.Domain.Enums;

namespace LegacyProcs.Domain.Entities
{
    /// <summary>
    /// Entidade de domínio OrdemServico modernizada
    /// Migrada de model anêmico para Rich Domain Model
    /// Implementa validações de domínio e encapsulamento
    /// </summary>
    public class OrdemServico
    {
        // Construtor privado para EF Core
        private OrdemServico() 
        {
            Titulo = string.Empty;
        }

        // Construtor para criação via factory method
        private OrdemServico(string titulo, string? descricao, int tecnicoId)
        {
            ValidarTitulo(titulo);
            ValidarTecnicoId(tecnicoId);
            ValidarDescricao(descricao);

            Titulo = titulo.Trim();
            Descricao = descricao?.Trim();
            TecnicoId = tecnicoId;
            Status = StatusOS.Pendente;
            DataCriacao = DateTime.UtcNow;
        }

        // Propriedades com encapsulamento
        public int Id { get; private set; }
        
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Titulo { get; private set; }
        
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Descricao { get; private set; }
        
        [Required(ErrorMessage = "Técnico é obrigatório")]
        public int TecnicoId { get; private set; }
        
        // Propriedade de navegação para relacionamento
        public Tecnico? Tecnico { get; private set; }
        
        public StatusOS Status { get; private set; }
        
        public DateTime DataCriacao { get; private set; }
        
        public DateTime? DataAtualizacao { get; private set; }

        // Factory method para criação
        public static OrdemServico Criar(string titulo, string? descricao, int tecnicoId)
        {
            return new OrdemServico(titulo, descricao, tecnicoId);
        }

        // Métodos de negócio
        public void AlterarStatus(StatusOS novoStatus)
        {
            if (!StatusOSExtensions.IsValidTransition(Status, novoStatus))
                throw new InvalidOperationException($"Transição inválida de {Status.ToDisplayString()} para {novoStatus.ToDisplayString()}");

            Status = novoStatus;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void AtualizarInformacoes(string titulo, string? descricao, int tecnicoId)
        {
            ValidarTitulo(titulo);
            ValidarTecnicoId(tecnicoId);
            ValidarDescricao(descricao);

            Titulo = titulo.Trim();
            Descricao = descricao?.Trim();
            TecnicoId = tecnicoId;
            DataAtualizacao = DateTime.UtcNow;
        }

        public bool PodeSerExcluida()
        {
            // Regra de negócio: só pode excluir se estiver Pendente
            return Status == StatusOS.Pendente;
        }

        public string GetStatusDisplay()
        {
            return Status.ToDisplayString();
        }

        // Validações de domínio privadas
        private static void ValidarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("Título é obrigatório", nameof(titulo));
            
            if (titulo.Length > 200)
                throw new ArgumentException("Título deve ter no máximo 200 caracteres", nameof(titulo));
        }

        private static void ValidarTecnicoId(int tecnicoId)
        {
            if (tecnicoId <= 0)
                throw new ArgumentException("TecnicoId deve ser maior que zero", nameof(tecnicoId));
        }

        private static void ValidarDescricao(string? descricao)
        {
            if (!string.IsNullOrEmpty(descricao) && descricao.Length > 1000)
                throw new ArgumentException("Descrição deve ter no máximo 1000 caracteres", nameof(descricao));
        }

        // Método para compatibilidade com código legado (será removido após migração completa)
        [Obsolete("Use Status enum ao invés de string. Este método será removido na próxima versão.")]
        public void SetStatusFromString(string statusString)
        {
            var novoStatus = StatusOSExtensions.FromString(statusString);
            AlterarStatus(novoStatus);
        }
    }
}
