using FluentValidation;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Validator para CriarOrdemServicoCommand
/// Substitui validações do ModelState por FluentValidation
/// </summary>
public class CriarOrdemServicoCommandValidator : AbstractValidator<CriarOrdemServicoCommand>
{
    public CriarOrdemServicoCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("Título é obrigatório")
            .MaximumLength(200)
            .WithMessage("Título deve ter no máximo 200 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.TecnicoId)
            .GreaterThan(0)
            .WithMessage("TecnicoId deve ser maior que zero");
    }
}
