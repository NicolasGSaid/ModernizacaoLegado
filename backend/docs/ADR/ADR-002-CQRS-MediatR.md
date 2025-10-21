# ADR-002: Implementação de CQRS com MediatR

## Status
✅ **ACEITO** - Implementado em 15/10/2025

## Contexto
O sistema legado misturava operações de leitura e escrita no mesmo método, causando:
- Dificuldade para otimizar queries específicas
- Acoplamento entre diferentes responsabilidades
- Complexidade desnecessária em operações simples
- Dificuldade para implementar cache e auditoria

## Decisão
Implementar **CQRS (Command Query Responsibility Segregation)** utilizando **MediatR** como mediador.

### Separação Clara
- **Commands**: Operações que modificam estado (Create, Update, Delete)
- **Queries**: Operações que apenas leem dados (Get, List, Search)

### Estrutura Implementada

#### Commands
```csharp
// Exemplo: CriarClienteCommand
public class CriarClienteCommand : IRequest<CriarClienteResponse>
{
    public string RazaoSocial { get; set; }
    public string CNPJ { get; set; }
    // ... outros campos
}

public class CriarClienteCommandHandler : IRequestHandler<CriarClienteCommand, CriarClienteResponse>
{
    // Implementação focada em escrita
}
```

#### Queries
```csharp
// Exemplo: ListarClientesQuery
public class ListarClientesQuery : IRequest<PaginatedResultDto<ClienteResponseDto>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Busca { get; set; }
}

public class ListarClientesQueryHandler : IRequestHandler<ListarClientesQuery, PaginatedResultDto<ClienteResponseDto>>
{
    // Implementação focada em leitura
}
```

## Benefícios Implementados

### 1. **Separação de Responsabilidades**
- Commands focam em validação e persistência
- Queries focam em performance e projeção de dados
- Cada handler tem uma única responsabilidade

### 2. **Flexibilidade de Otimização**
- Queries podem usar diferentes estratégias (cache, read replicas)
- Commands podem implementar validações complexas
- Possibilidade de diferentes modelos de dados

### 3. **Testabilidade**
- Cada handler pode ser testado isoladamente
- Mocks específicos para cada operação
- Testes mais focados e rápidos

### 4. **Auditoria e Cross-Cutting Concerns**
```csharp
// Behaviors implementados
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
```

## Implementação Técnica

### Registro no DI Container
```csharp
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});
```

### Uso nos Controllers
```csharp
[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CriarClienteCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ListarClientesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
```

## Handlers Implementados

### Cliente
- ✅ `CriarClienteCommandHandler`
- ✅ `AtualizarClienteCommandHandler`
- ✅ `ExcluirClienteCommandHandler`
- ✅ `ListarClientesQueryHandler`
- ✅ `ObterClientePorIdQueryHandler`

### Técnico
- ✅ `CriarTecnicoCommandHandler`
- ✅ `AtualizarTecnicoCommandHandler`
- ✅ `AlterarStatusTecnicoCommandHandler`
- ✅ `ExcluirTecnicoCommandHandler`
- ✅ `ListarTecnicosQueryHandler`
- ✅ `ObterTecnicoPorIdQueryHandler`

### OrdemServico
- ✅ `CriarOrdemServicoCommandHandler`
- ✅ `AlterarStatusOrdemServicoCommandHandler`
- ✅ `ExcluirOrdemServicoCommandHandler`
- ✅ `ListarOrdensServicoQueryHandler`
- ✅ `ObterOrdemServicoPorIdQueryHandler`

## Validação com FluentValidation

### Integração Automática
```csharp
public class CriarClienteCommandValidator : AbstractValidator<CriarClienteCommand>
{
    public CriarClienteCommandValidator()
    {
        RuleFor(x => x.RazaoSocial)
            .NotEmpty()
            .MaximumLength(200);
            
        RuleFor(x => x.CNPJ)
            .NotEmpty()
            .Must(BeValidCNPJ);
    }
}
```

### Behavior de Validação
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // Executa validações antes do handler
    // Retorna BadRequest se inválido
}
```

## Métricas de Sucesso
- ✅ **16 Handlers** implementados (100% cobertura)
- ✅ **Validações automáticas** em todos os commands
- ✅ **Logs estruturados** em todas as operações
- ✅ **Testes unitários** para cada handler
- ✅ **Performance** mantida ou melhorada

## Consequências

### ✅ **Positivas**
- Código mais limpo e focado
- Fácil adição de novos casos de uso
- Testabilidade excelente
- Auditoria automática via behaviors
- Validação centralizada

### ⚠️ **Considerações**
- Mais arquivos (um handler por operação)
- Curva de aprendizado para MediatR
- Overhead mínimo do mediator

### 🔧 **Mitigações**
- Templates para novos handlers
- Documentação com exemplos
- Code snippets no IDE

## Próximos Passos
- [ ] Implementar cache em queries específicas
- [ ] Adicionar metrics/telemetria
- [ ] Considerar Event Sourcing para auditoria avançada

## Referências
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Library](https://github.com/jbogard/MediatR)
- [FluentValidation](https://fluentvalidation.net/)

---
**Autor**: Nicolas Dias  
**Data**: 15/10/2025  
**Revisores**: Equipe de Desenvolvimento  
**Próxima Revisão**: 15/01/2026
