using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using LegacyProcs.Domain.Enums;

namespace LegacyProcs.Domain.Entities
{
    /// <summary>
    /// Entidade de domínio Tecnico modernizada
    /// Migrada de model anêmico para Rich Domain Model
    /// Implementa validações de email, telefone e encapsulamento
    /// </summary>
    public class Tecnico
    {
        // Construtor privado para EF Core
        private Tecnico() 
        {
            Nome = string.Empty;
            Email = string.Empty;
            Telefone = string.Empty;
            Especialidade = string.Empty;
        }

        // Construtor para criação via factory method
        private Tecnico(string nome, string email, string telefone, string especialidade)
        {
            ValidarNome(nome);
            ValidarEmail(email);
            ValidarTelefone(telefone);
            ValidarEspecialidade(especialidade);

            Nome = nome.Trim();
            Email = email.Trim().ToLowerInvariant();
            Telefone = LimparTelefone(telefone);
            Especialidade = especialidade.Trim();
            Status = StatusTecnico.Ativo;
            DataCadastro = DateTime.UtcNow;
        }

        // Propriedades com encapsulamento
        public int Id { get; private set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; private set; }
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
        public string Email { get; private set; }
        
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Telefone { get; private set; }
        
        [Required(ErrorMessage = "Especialidade é obrigatória")]
        [StringLength(100, ErrorMessage = "Especialidade deve ter no máximo 100 caracteres")]
        public string Especialidade { get; private set; }
        
        public StatusTecnico Status { get; private set; }
        
        public DateTime DataCadastro { get; private set; }

        // Factory method para criação
        public static Tecnico Criar(string nome, string email, string telefone, string especialidade)
        {
            return new Tecnico(nome, email, telefone, especialidade);
        }

        // Métodos de negócio
        public void AlterarStatus(StatusTecnico novoStatus)
        {
            if (Status == novoStatus)
                throw new InvalidOperationException($"Técnico já está com status {novoStatus.ToDisplayString()}");

            Status = novoStatus;
        }

        public void AlterarStatus(string novoStatus)
        {
            var statusEnum = StatusTecnicoExtensions.FromString(novoStatus);
            AlterarStatus(statusEnum);
        }

        public void AtualizarInformacoes(string nome, string email, string telefone, string especialidade)
        {
            ValidarNome(nome);
            ValidarEmail(email);
            ValidarTelefone(telefone);
            ValidarEspecialidade(especialidade);

            Nome = nome.Trim();
            Email = email.Trim().ToLowerInvariant();
            Telefone = LimparTelefone(telefone);
            Especialidade = especialidade.Trim();
        }

        public bool PodeTrabalhar()
        {
            return Status.PodeTrabalhar();
        }

        public string GetStatusDisplay()
        {
            return Status.ToDisplayString();
        }

        // Validações de domínio privadas
        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório", nameof(nome));
            
            if (nome.Length > 100)
                throw new ArgumentException("Nome deve ter no máximo 100 caracteres", nameof(nome));
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório", nameof(email));

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email deve ter formato válido", nameof(email));
        }

        private static void ValidarTelefone(string telefone)
        {
            if (!string.IsNullOrEmpty(telefone))
            {
                var telefoneLimpo = LimparTelefone(telefone);
                if (telefoneLimpo.Length < 10 || telefoneLimpo.Length > 11)
                    throw new ArgumentException("Telefone deve ter 10 ou 11 dígitos", nameof(telefone));
            }
        }

        private static void ValidarEspecialidade(string especialidade)
        {
            if (string.IsNullOrWhiteSpace(especialidade))
                throw new ArgumentException("Especialidade é obrigatória", nameof(especialidade));
            
            if (especialidade.Length > 100)
                throw new ArgumentException("Especialidade deve ter no máximo 100 caracteres", nameof(especialidade));
        }

        // Método de limpeza
        private static string LimparTelefone(string telefone)
        {
            return Regex.Replace(telefone ?? "", @"[^\d]", "");
        }

        // Método para compatibilidade com código legado (será removido após migração completa)
        [Obsolete("Use Status enum ao invés de string. Este método será removido na próxima versão.")]
        public void SetStatusFromString(string statusString)
        {
            var novoStatus = StatusTecnicoExtensions.FromString(statusString);
            AlterarStatus(novoStatus);
        }
    }
}
