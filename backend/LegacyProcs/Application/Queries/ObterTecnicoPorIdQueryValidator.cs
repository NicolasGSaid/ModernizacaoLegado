using FluentValidation;

namespace LegacyProcs.Application.Queries;

public class ObterTecnicoPorIdQueryValidator : AbstractValidator<ObterTecnicoPorIdQuery>
{
    public ObterTecnicoPorIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID deve ser maior que zero");
    }
}
