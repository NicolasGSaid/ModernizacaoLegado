using FluentValidation;

namespace LegacyProcs.Application.Commands
{
    /// <summary>
    /// Validador para ExcluirDadosClienteCommand - Right to Erasure LGPD
    /// Garante que todos os requisitos legais sejam atendidos
    /// </summary>
    public class ExcluirDadosClienteCommandValidator : AbstractValidator<ExcluirDadosClienteCommand>
    {
        public ExcluirDadosClienteCommandValidator()
        {
            RuleFor(x => x.ClienteId)
                .GreaterThan(0)
                .WithMessage("ID do cliente deve ser maior que zero");

            RuleFor(x => x.MotivoExclusao)
                .NotEmpty()
                .WithMessage("Motivo da exclusão é obrigatório para conformidade LGPD")
                .MinimumLength(10)
                .WithMessage("Motivo deve ter pelo menos 10 caracteres")
                .MaximumLength(500)
                .WithMessage("Motivo deve ter no máximo 500 caracteres");

            RuleFor(x => x.SolicitadoPor)
                .NotEmpty()
                .WithMessage("Identificação do solicitante é obrigatória")
                .MaximumLength(200)
                .WithMessage("Nome do solicitante deve ter no máximo 200 caracteres");

            RuleFor(x => x.ConfirmarExclusao)
                .Equal(true)
                .WithMessage("Confirmação explícita é obrigatória para exclusão permanente de dados");
        }
    }
}
