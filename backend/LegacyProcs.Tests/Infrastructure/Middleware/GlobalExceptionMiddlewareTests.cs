using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using LegacyProcs.Infrastructure.Middleware;
using LegacyProcs.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace LegacyProcs.Tests.Infrastructure.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly GlobalExceptionMiddleware _middleware;

    public GlobalExceptionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        
        _middleware = new GlobalExceptionMiddleware(
            (context) => throw new InvalidOperationException("Test exception"),
            _loggerMock.Object,
            _environmentMock.Object
        );
    }

    [Fact]
    public async Task InvokeAsync_ComValidationException_DeveRetornar400()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var validationException = new ValidationException("Validation failed");
        var middleware = new GlobalExceptionMiddleware(
            (ctx) => throw validationException,
            _loggerMock.Object,
            _environmentMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task InvokeAsync_ComEntityNotFoundException_DeveRetornar404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var notFoundException = new EntityNotFoundException("Cliente", 123);
        var middleware = new GlobalExceptionMiddleware(
            (ctx) => throw notFoundException,
            _loggerMock.Object,
            _environmentMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(404);
        
        // Verificar conteúdo da resposta
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be(404);
        errorResponse.Message.Should().Contain("Cliente");
        errorResponse.TraceId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_ComBusinessRuleViolationException_DeveRetornar400()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var businessException = new BusinessRuleViolationException("ValidacaoIdade", "Idade deve ser maior que 18");
        var middleware = new GlobalExceptionMiddleware(
            (ctx) => throw businessException,
            _loggerMock.Object,
            _environmentMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task InvokeAsync_ComUnauthorizedAccessException_DeveRetornar401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var unauthorizedException = new UnauthorizedAccessException("Access denied");
        var middleware = new GlobalExceptionMiddleware(
            (ctx) => throw unauthorizedException,
            _loggerMock.Object,
            _environmentMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task InvokeAsync_ComExcecaoGenerica_DeveRetornar500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var genericException = new InvalidOperationException("Something went wrong");
        var middleware = new GlobalExceptionMiddleware(
            (ctx) => throw genericException,
            _loggerMock.Object,
            _environmentMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task InvokeAsync_EmDesenvolvimento_DeveIncluirDetalhesDoErro()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");
        
        var exception = new InvalidOperationException("Test exception");
        var middleware = new GlobalExceptionMiddleware(
            (ctx) => throw exception,
            _loggerMock.Object,
            _environmentMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        
        responseBody.Should().Contain("Test exception");
        responseBody.Should().Contain("InvalidOperationException");
    }

    [Fact]
    public async Task InvokeAsync_EmProducao_NaoDeveExporDetalhesDoErro()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");
        
        var exception = new InvalidOperationException("Sensitive internal error");
        var middleware = new GlobalExceptionMiddleware(
            (ctx) => throw exception,
            _loggerMock.Object,
            _environmentMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        
        responseBody.Should().NotContain("Sensitive internal error");
        responseBody.Should().Contain("Erro interno do servidor");
    }

    [Fact]
    public async Task InvokeAsync_DeveLogarExcecoes()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";
        
        var exception = new InvalidOperationException("Test exception");
        var middleware = new GlobalExceptionMiddleware(
            (ctx) => throw exception,
            _loggerMock.Object,
            _environmentMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro não tratado na requisição")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
