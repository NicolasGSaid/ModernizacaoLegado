using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LegacyProcs.Domain.Entities
{
    /// <summary>
    /// Entidade de domínio Cliente modernizada
    /// Migrada de model anêmico para Rich Domain Model
    /// Implementa validações de CNPJ, email e encapsulamento
    /// </summary>
    public class Cliente
    {
        // Construtor privado para EF Core
        private Cliente() 
        {
            RazaoSocial = string.Empty;
            NomeFantasia = string.Empty;
            CNPJ = string.Empty;
            Email = string.Empty;
            Telefone = string.Empty;
            Endereco = string.Empty;
            Cidade = string.Empty;
            Estado = string.Empty;
            CEP = string.Empty;
        }

        // Construtor para criação via factory method
        private Cliente(string razaoSocial, string? nomeFantasia, string cnpj, string email, 
                       string telefone, string endereco, string cidade, string estado, string cep)
        {
            ValidarRazaoSocial(razaoSocial);
            ValidarCNPJ(cnpj);
            ValidarEmail(email);
            ValidarTelefone(telefone);
            ValidarCEP(cep);

            RazaoSocial = razaoSocial.Trim();
            NomeFantasia = nomeFantasia?.Trim();
            CNPJ = LimparCNPJ(cnpj);
            Email = email.Trim().ToLowerInvariant();
            Telefone = LimparTelefone(telefone);
            Endereco = endereco.Trim();
            Cidade = cidade.Trim();
            Estado = estado.Trim().ToUpperInvariant();
            CEP = LimparCEP(cep);
            DataCadastro = DateTime.UtcNow;
        }

        // Propriedades com encapsulamento
        public int Id { get; private set; }
        
        [Required(ErrorMessage = "Razão Social é obrigatória")]
        [StringLength(200, ErrorMessage = "Razão Social deve ter no máximo 200 caracteres")]
        public string RazaoSocial { get; private set; }
        
        [StringLength(200, ErrorMessage = "Nome Fantasia deve ter no máximo 200 caracteres")]
        public string? NomeFantasia { get; private set; }
        
        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
        public string CNPJ { get; private set; }
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
        public string Email { get; private set; }
        
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Telefone { get; private set; }
        
        [Required(ErrorMessage = "Endereço é obrigatório")]
        [StringLength(300, ErrorMessage = "Endereço deve ter no máximo 300 caracteres")]
        public string Endereco { get; private set; }
        
        [Required(ErrorMessage = "Cidade é obrigatória")]
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; private set; }
        
        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter exatamente 2 caracteres")]
        public string Estado { get; private set; }
        
        [Required(ErrorMessage = "CEP é obrigatório")]
        [StringLength(10, ErrorMessage = "CEP deve ter no máximo 10 caracteres")]
        public string CEP { get; private set; }
        
        public DateTime DataCadastro { get; private set; }

        // Factory method para criação
        public static Cliente Criar(string razaoSocial, string? nomeFantasia, string cnpj, string email,
                                   string telefone, string endereco, string cidade, string estado, string cep)
        {
            return new Cliente(razaoSocial, nomeFantasia, cnpj, email, telefone, endereco, cidade, estado, cep);
        }

        // Métodos de negócio
        public void AtualizarInformacoes(string razaoSocial, string? nomeFantasia, string email,
                                        string telefone, string endereco, string cidade, string estado, string cep)
        {
            ValidarRazaoSocial(razaoSocial);
            ValidarEmail(email);
            ValidarTelefone(telefone);
            ValidarCEP(cep);

            RazaoSocial = razaoSocial.Trim();
            NomeFantasia = nomeFantasia?.Trim();
            Email = email.Trim().ToLowerInvariant();
            Telefone = LimparTelefone(telefone);
            Endereco = endereco.Trim();
            Cidade = cidade.Trim();
            Estado = estado.Trim().ToUpperInvariant();
            CEP = LimparCEP(cep);
        }

        public string GetCNPJFormatado()
        {
            if (string.IsNullOrEmpty(CNPJ) || CNPJ.Length != 14)
                return CNPJ;
            
            return $"{CNPJ.Substring(0, 2)}.{CNPJ.Substring(2, 3)}.{CNPJ.Substring(5, 3)}/{CNPJ.Substring(8, 4)}-{CNPJ.Substring(12, 2)}";
        }

        public string GetCEPFormatado()
        {
            if (string.IsNullOrEmpty(CEP) || CEP.Length != 8)
                return CEP;
            
            return $"{CEP.Substring(0, 5)}-{CEP.Substring(5, 3)}";
        }

        // Validações de domínio privadas
        private static void ValidarRazaoSocial(string razaoSocial)
        {
            if (string.IsNullOrWhiteSpace(razaoSocial))
                throw new ArgumentException("Razão Social é obrigatória", nameof(razaoSocial));
            
            if (razaoSocial.Length > 200)
                throw new ArgumentException("Razão Social deve ter no máximo 200 caracteres", nameof(razaoSocial));
        }

        private static void ValidarCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ArgumentException("CNPJ é obrigatório", nameof(cnpj));

            var cnpjLimpo = LimparCNPJ(cnpj);
            
            if (cnpjLimpo.Length != 14)
                throw new ArgumentException("CNPJ deve ter 14 dígitos", nameof(cnpj));

            if (!Regex.IsMatch(cnpjLimpo, @"^\d{14}$"))
                throw new ArgumentException("CNPJ deve conter apenas números", nameof(cnpj));

            // Validação básica de CNPJ (algoritmo simplificado)
            if (cnpjLimpo.All(c => c == cnpjLimpo[0]))
                throw new ArgumentException("CNPJ inválido", nameof(cnpj));
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

        private static void ValidarCEP(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                throw new ArgumentException("CEP é obrigatório", nameof(cep));

            var cepLimpo = LimparCEP(cep);
            if (cepLimpo.Length != 8)
                throw new ArgumentException("CEP deve ter 8 dígitos", nameof(cep));

            if (!Regex.IsMatch(cepLimpo, @"^\d{8}$"))
                throw new ArgumentException("CEP deve conter apenas números", nameof(cep));
        }

        // Métodos de limpeza
        private static string LimparCNPJ(string cnpj)
        {
            return Regex.Replace(cnpj ?? "", @"[^\d]", "");
        }

        private static string LimparTelefone(string telefone)
        {
            return Regex.Replace(telefone ?? "", @"[^\d]", "");
        }

        private static string LimparCEP(string cep)
        {
            return Regex.Replace(cep ?? "", @"[^\d]", "");
        }
    }
}
