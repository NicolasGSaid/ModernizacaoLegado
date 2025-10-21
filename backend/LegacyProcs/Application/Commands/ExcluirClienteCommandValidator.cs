using FluentValidation;

namespace LegacyProcs.Application.Commands;

public class ExcluirClienteCommandValidator : AbstractValidator<ExcluirClienteCommand>
{
    public ExcluirClienteCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID deve ser maior que zero");
    }
}
