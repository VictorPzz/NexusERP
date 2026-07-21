# NexusERP - Diseño de Arquitectura

## 1. Arquitectura Seleccionada: Clean Architecture + Modular

### 1.1 Justificación de Clean Architecture

**¿Por qué Clean Architecture?**
- Separa concerns: negocio, persistencia, presentación
- Facilita testing unitario (cada capa se puede testear aisladamente)
- Permite cambiar implementaciones sin afectar la lógica de negocio
- El dominio no depende de infraestructura (inversión de dependencias)
- Preparado para producción y escalabilidad

**¿Por qué no Microservicios?**
- Para una PyME, un monolito modular es más simple de mantener
- Los microservicios agregan complejidad de infraestructura innecesaria
- Un monolito bien diseñado puede escalar horizontalmente si es necesario
- Facilita las transacciones distribuidas entre módulos

### 1.2 Justificación de Arquitectura Modular

**¿Por qué Modular?**
- Cada módulo es una responsabilidad separada
- Facilita el desarrollo paralelo por equipos
- Permite habilitar/deshabilitar módulos
- Los módulos se comunican a través de interfaces (contracts)
- Preparado para extraer módulos a servicios independientes en el futuro

## 2. Capas de la Arquitectura

### 2.1 Domain (Núcleo)
```
Responsabilidad: Entidades, Value Objects, Interfaces de repositorio, Reglas de negocio
Dependencias: Ninguna (es el núcleo puro)
```

**Componentes:**
- **Entities**: Entidades de dominio con comportamiento (no solo DTOs)
- **Value Objects**: Objetos de valor (Money, Address, Email)
- **Interfaces**: Contratos para repositorios y servicios externos
- **Enums**: Enumeraciones del dominio
- **Events**: Eventos de dominio (para futura implementación de Event-Driven)

### 2.2 Application (Aplicación)
```
Responsabilidad: Casos de uso, Orquestación, DTOs, Validación
Dependencias: Domain
```

**Componentes:**
- **Services**: Servicios de aplicación (casos de uso)
- **DTOs**: Data Transfer Objects para request/response
- **Mappings**: AutoMapper profiles o mappers manuales
- **Validators**: Validaciones usando FluentValidation
- **Interfaces**: Contratos de servicios externos (email, file, etc.)

### 2.3 Infrastructure (Infraestructura)
```
Responsabilidad: Persistencia, External Services, Implementaciones
Dependencias: Domain (invierte dependencias)
```

**Componentes:**
- **Persistence**: DbContext, Configuraciones EF Core, Migraciones
- **Repositories**: Implementaciones de repositorios
- **Services**: Servicios externos (email, file system, cache)
- **Authentication**: Configuración JWT, Token Service
- **Extensions**: Extensiones de configuración

### 2.4 Presentation (Presentación/API)
```
Responsabilidad: Endpoints, Middleware, Configuración
Dependencias: Application, Infrastructure
```

**Componentes:**
- **Controllers**: Controladores API
- **Middleware**: Middleware personalizado (error handling, logging)
- **Filters**: Filtros de acción y excepción
- **Configuration**: Configuración de servicios, DI, Swagger

## 3. Flujo de Dependencias

```
Presentation → Application → Domain
                  ↓
            Infrastructure → Domain
```

**Regla de oro**: El dominio NUNCA depende de otras capas. Las dependencias siempre apuntan hacia adentro.

## 4. Patrones de Diseño Aplicados

### 4.1 Repository Pattern
```csharp
// Interface en Domain
public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByCodeAsync(string code);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
}

// Implementación en Infrastructure
public class ProductRepository : Repository<Product>, IProductRepository
{
    // Implementación específica
}
```

**Justificación**: Separa la lógica de acceso a datos del dominio. Permite cambiar de EF Core a Dapper o a otro ORM sin afectar el negocio.

### 4.2 Unit of Work Pattern
```csharp
// Interface en Domain
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IInventoryRepository Inventory { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

**Justificación**: Gestiona transacciones de manera consistente. Permite agrupar múltiples operaciones en una sola transacción.

### 4.3 Specification Pattern (para consultas complejas)
```csharp
public class ProductsByCategorySpec : Specification<Product>
{
    public ProductsByCategorySpec(int categoryId)
    {
        Query.Where(p => p.CategoryId == categoryId)
             .Include(p => p.Category)
             .OrderBy(p => p.Name);
    }
}
```

### 4.4 Result Pattern (para manejo de errores)
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ResultType Type { get; }

    public static Result<T> Success(T value) => new(true, value, null, ResultType.Ok);
    public static Result<T> NotFound(string error) => new(false, default, error, ResultType.NotFound);
    public static Result<T> Failure(string error) => new(false, default, error, ResultType.Failure);
    public static Result<T> ValidationFailed(string error) => new(false, default, error, ResultType.ValidationFailed);
}
```

**Justificación**: Elimina el uso de excepciones para control de flujo. Más explícito y funcional.

### 4.5 CQRS Ligero (Command Query Responsibility Segregation)
Separación de lecturas (Queries) y escrituras (Commands) a nivel de servicios, no a nivel de bases de datos.

```csharp
// Command
public class CreateSaleCommand : IRequest<Result<int>>
{
    public int ClientId { get; set; }
    public List<SaleDetailDto> Details { get; set; }
}

// Query
public class GetSalesQuery : IRequest<Result<PaginatedList<SaleDto>>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

## 5. Decisiones Técnicas

### 5.1 Entity Framework Core vs Dapper
**Decisión**: EF Core como ORM principal.

**Justificación**:
- EF Core es el ORM nativo de .NET
- Soporte completo para PostgreSQL via Npgsql
- Migraciones automáticas de BD
- Change tracking para Unit of Work
- Mejor integración con LINQ
- Para consultas muy complejas, se puede usar Dapper como complemento

### 5.2 FluentValidation vs DataAnnotations
**Decisión**: FluentValidation.

**Justificación**:
- Más poderoso que DataAnnotations
- Separación de validaciones de la entidad
- Soporte para validaciones asíncronas
- Más fácil de testear
- Reglas de negocio complejas en un solo lugar

### 5.3 AutoMapper vs Mappers Manuales
**Decisión**: Mappers manuales (extension methods).

**Justificación**:
- AutoMapper puede generar problemas difíciles de debuggear
- Los mappers manuales son más explícitos
- Mejor rendimiento (sin reflexión)
- Más control sobre el mapeo
- Más fácil de mantener a largo plazo

### 5.4 MediatR vs Servicios Directos
**Decisión**: MediatR para mediación entre capas.

**Justificación**:
- Implementa CQRS de manera limpia
- Separa el manejo de requests
- Facilita el uso de pipeline behaviors (validation, logging, transactions)
- Reduce el acoplamiento entre controllers y services

### 5.5 Swagger/OpenAPI
**Decisión**: Swashbuckle para documentación.

**Justificación**:
- Integración nativa con ASP.NET Core
- Generación automática de documentación
- Soporte para autenticación JWT
- UI interactiva para testing

## 6. Autenticación y Autorización

### 6.1 JWT + Refresh Tokens
```
Flow:
1. User Login → Access Token (15 min) + Refresh Token (7 días)
2. API Request → Access Token en Header Authorization
3. Token Expirado → Usa Refresh Token para obtener nuevo Access Token
4. Refresh Token Expirado → Re-login
```

### 6.2 Autorización basada en Roles
```csharp
[Authorize(Roles = "Admin")]
[HttpPost]
public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
{
    // Solo usuarios con rol Admin pueden crear productos
}
```

### 6.3 Autorización basada en Policies (futuro)
```csharp
[Authorize(Policy = "CanManageInventory")]
public async Task<IActionResult> UpdateStock([FromBody] UpdateStockCommand command)
{
    // Usuarios con permiso específico
}
```

## 7. Manejo de Errores

### 7.1 Middleware Global de Excepciones
```csharp
public class ExceptionHandlingMiddleware
{
    // Captura excepciones no manejadas
    // Retorna respuesta consistente
    // Registra el error
}
```

### 7.2 Respuesta Estándar de Error
```json
{
    "type": "ValidationFailed",
    "message": "Error de validación",
    "errors": [
        { "field": "Name", "message": "El nombre es requerido" }
    ],
    "traceId": "abc-123"
}
```

## 8. Logging y Monitoreo

### 8.1 Serilog
- Structured logging
- Sink para consola y archivos
- Enrichment con contexto de la petición

### 8.2 Health Checks
```csharp
// Endpoint: /health
// Verifica: BD PostgreSQL, servicios externos
```

## 9. Configuración

### 9.1 appsettings.json por Ambiente
```
appsettings.json           → Config base
appsettings.Development.json → Dev
appsettings.Production.json  → Prod
```

### 9.2 Variables de Entorno
- Connection strings
- JWT secrets
- API keys
- Configuraciones sensibles

## 10. Optimización de Rendimiento

### 10.1 Caching
- In-Memory Cache para datos estáticos (categorías, configuraciones)
- Redis (futuro) para cache distribuido

### 10.2 Paginación
- Todas las listas soportan paginación
- Cursor-based pagination para listas grandes

### 10.3 Async/Await
- Todas las operaciones de I/O son asíncronas
- Mejora el rendimiento bajo carga

## 11. Seguridad

### 11.1 HTTPS
- Forzado en producción

### 11.2 CORS
- Configuración por origen permitido

### 11.3 Rate Limiting
- Prevención de abuso de API

### 11.4 Input Validation
- Validación en capa de presentación (FluentValidation)
- Validación en capa de dominio (entidades)
- Sanitización de inputs

### 11.5 SQL Injection
- EF Core usa parameterized queries
- Nunca concatenar strings en queries
