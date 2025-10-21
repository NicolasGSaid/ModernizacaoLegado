using FluentValidation;

namespace LegacyProcs.Application.Queries;

public class ObterClientePorIdQueryValidator : AbstractValidator<ObterClientePorIdQuery>
{
    public ObterClientePorIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID deve ser maior que zero");
    }
}
