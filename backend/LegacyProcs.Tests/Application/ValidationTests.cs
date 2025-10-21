using FluentAssertions;
using FluentValidation;
using LegacyProcs.Application.OrdemServico.Commands.CreateOrdemServico;
using LegacyProcs.Application.Cliente.Commands.CreateCliente;
using LegacyProcs.Application.Tecnico.Commands.CreateTecnico;
using Xunit;

namespace LegacyProcs.Tests.Application;

public class ValidationTests
{
    [Fact]
    public void Validation_ShouldFail_WhenTituloIsEmpty()
    {
        // Arrange
        var command = new CreateOrdemServicoCommand
        {
            Titulo = "",
            Descricao = "Descrição válida",
            ClienteId = 1,
            TecnicoId = 1,
            Status = "Pendente"
        };
        var validator = new CreateOrdemServicoCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Titulo");
    }

    [Fact]
    public void Validation_ShouldFail_WhenTituloIsTooLong()
    {
        // Arrange
        var command = new CreateOrdemServicoCommand
        {
            Titulo = new string('A', 201),
            Descricao = "Descrição válida",
            ClienteId = 1,
            TecnicoId = 1,
            Status = "Pendente"
        };
        var validator = new CreateOrdemServicoCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Titulo");
    }

    [Fact]
    public void Validation_ShouldFail_WhenClienteIdIsZero()
    {
        // Arrange
        var command = new CreateOrdemServicoCommand
        {
            Titulo = "Título válido",
            Descricao = "Descrição válida",
            ClienteId = 0,
            TecnicoId = 1,
            Status = "Pendente"
        };
        var validator = new CreateOrdemServicoCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ClienteId");
    }

    [Fact]
    public void Validation_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateClienteCommand
        {
            Nome = "Cliente Teste",
            Email = "email-invalido",
            Telefone = "11999999999",
            Endereco = "Rua Teste, 123"
        };
        var validator = new CreateClienteCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validation_ShouldPass_WhenEmailIsValid()
    {
        // Arrange
        var command = new CreateClienteCommand
        {
            Nome = "Cliente Teste",
            Email = "cliente@teste.com",
            Telefone = "11999999999",
            Endereco = "Rua Teste, 123"
        };
        var validator = new CreateClienteCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validation_ShouldFail_WhenNomeIsNullOrEmpty(string nome)
    {
        // Arrange
        var command = new CreateClienteCommand
        {
            Nome = nome,
            Email = "cliente@teste.com",
            Telefone = "11999999999",
            Endereco = "Rua Teste, 123"
        };
        var validator = new CreateClienteCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nome");
    }

    [Fact]
    public void Validation_ShouldFail_WhenTelefoneIsInvalid()
    {
        // Arrange
        var command = new CreateClienteCommand
        {
            Nome = "Cliente Teste",
            Email = "cliente@teste.com",
            Telefone = "123",
            Endereco = "Rua Teste, 123"
        };
        var validator = new CreateClienteCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Telefone");
    }

    [Fact]
    public void Validation_ShouldFail_WhenEspecialidadeIsEmpty()
    {
        // Arrange
        var command = new CreateTecnicoCommand
        {
            Nome = "Técnico Teste",
            Email = "tecnico@teste.com",
            Telefone = "11999999999",
            Especialidade = "",
            Status = "Ativo"
        };
        var validator = new CreateTecnicoCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Especialidade");
    }

    [Theory]
    [InlineData("Ativo")]
    [InlineData("Inativo")]
    [InlineData("Férias")]
    public void Validation_ShouldPass_WhenStatusIsValid(string status)
    {
        // Arrange
        var command = new CreateTecnicoCommand
        {
            Nome = "Técnico Teste",
            Email = "tecnico@teste.com",
            Telefone = "11999999999",
            Especialidade = "Elétrica",
            Status = status
        };
        var validator = new CreateTecnicoCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validation_ShouldFail_WhenMultipleFieldsAreInvalid()
    {
        // Arrange
        var command = new CreateOrdemServicoCommand
        {
            Titulo = "",
            Descricao = "",
            ClienteId = 0,
            TecnicoId = 0,
            Status = ""
        };
        var validator = new CreateOrdemServicoCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void Validation_ShouldProvide_DetailedErrorMessages()
    {
        // Arrange
        var command = new CreateClienteCommand
        {
            Nome = "",
            Email = "invalid",
            Telefone = "123",
            Endereco = ""
        };
        var validator = new CreateClienteCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().AllSatisfy(error =>
        {
            error.ErrorMessage.Should().NotBeNullOrEmpty();
            error.PropertyName.Should().NotBeNullOrEmpty();
        });
    }
}
