using FluentValidation;

namespace LegacyProcs.Application.Commands;

public class CriarClienteCommandValidator : AbstractValidator<CriarClienteCommand>
{
    public CriarClienteCommandValidator()
    {
        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .MaximumLength(200).WithMessage("Razão Social deve ter no máximo 200 caracteres");

        RuleFor(x => x.CNPJ)
            .NotEmpty().WithMessage("CNPJ é obrigatório")
            .Length(14).WithMessage("CNPJ deve ter 14 dígitos")
            .Matches(@"^\d{14}$").WithMessage("CNPJ deve conter apenas números");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefone));

        RuleFor(x => x.Endereco)
            .NotEmpty().WithMessage("Endereço é obrigatório")
            .MaximumLength(300).WithMessage("Endereço deve ter no máximo 300 caracteres");

        RuleFor(x => x.Cidade)
            .NotEmpty().WithMessage("Cidade é obrigatória")
            .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("Estado é obrigatório")
            .Length(2).WithMessage("Estado deve ter 2 caracteres (UF)");

        RuleFor(x => x.CEP)
            .NotEmpty().WithMessage("CEP é obrigatório")
            .Length(8).WithMessage("CEP deve ter 8 dígitos")
            .Matches(@"^\d{8}$").WithMessage("CEP deve conter apenas números");
    }
}
