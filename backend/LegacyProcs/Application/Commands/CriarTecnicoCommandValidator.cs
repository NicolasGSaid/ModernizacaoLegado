using FluentValidation;

namespace LegacyProcs.Application.Commands;

public class CriarTecnicoCommandValidator : AbstractValidator<CriarTecnicoCommand>
{
    public CriarTecnicoCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres");

        RuleFor(x => x.Especialidade)
            .NotEmpty().WithMessage("Especialidade é obrigatória")
            .MaximumLength(100).WithMessage("Especialidade deve ter no máximo 100 caracteres");
    }
}
