# NexusERP - Diseño de Estructura de Carpetas

## 1. Estructura General del Proyecto

```
NexusERP/
├── docs/                                    # Documentación del proyecto
│   ├── 01-domain-analysis.md
│   ├── 02-architecture-design.md
│   ├── 03-modules-design.md
│   ├── 04-database-design.md
│   ├── 05-entity-relationship.md
│   ├── 06-business-rules.md
│   ├── 07-folder-structure.md
│   ├── 08-api-endpoints.md
│   └── 09-roadmap.md
│
├── src/                                     # Código fuente
│   ├── NexusERP.Domain/                     # Capa de Dominio
│   ├── NexusERP.Application/                # Capa de Aplicación
│   ├── NexusERP.Infrastructure/             # Capa de Infraestructura
│   └── NexusERP.Api/                        # Capa de Presentación (API)
│
├── tests/                                   # Pruebas
│   ├── NexusERP.Domain.Tests/
│   ├── NexusERP.Application.Tests/
│   ├── NexusERP.Infrastructure.Tests/
│   └── NexusERP.Api.Tests/
│
├── docker/                                  # Configuración Docker
│   ├── Dockerfile.api
│   ├── Dockerfile.frontend
│   └── nginx.conf
│
├── frontend/                                # Frontend React
│   ├── src/
│   ├── public/
│   ├── package.json
│   ├── tsconfig.json
│   ├── vite.config.ts
│   └── tailwind.config.js
│
├── docker-compose.yml
├── docker-compose.dev.yml
├── .gitignore
├── README.md
└── NexusERP.sln
```

---

## 2. Estructura de NexusERP.Domain

```
src/NexusERP.Domain/
├── NexusERP.Domain.csproj
│
├── Common/                                  # Compartido
│   ├── BaseEntity.cs                        # Clase base para entidades
│   ├── AuditableEntity.cs                   # Entidad con auditoría
│   ├── SoftDeleteEntity.cs                  # Entidad con soft delete
│   ├── Result.cs                            # Result pattern
│   ├── ResultType.cs                        # Tipos de resultado
│   ├── Error.cs                             # Modelo de error
│   └── PagedResult.cs                       # Resultado paginado
│
├── Enums/                                   # Enumeraciones
│   ├── DocumentType.cs
│   ├── Gender.cs
│   ├── EmploymentType.cs
│   ├── EmployeeStatus.cs
│   ├── ClientStatus.cs
│   ├── SupplierStatus.cs
│   ├── MovementType.cs
│   ├── UnitOfMeasure.cs
│   ├── OrderType.cs
│   ├── OrderStatus.cs
│   ├── PurchaseOrderStatus.cs
│   ├── PurchaseStatus.cs
│   ├── PaymentStatus.cs
│   ├── SaleStatus.cs
│   ├── InvoiceType.cs
│   ├── InvoiceStatus.cs
│   ├── PaymentMethod.cs
│   └── AddressType.cs
│
├── Entities/                                # Entidades de dominio
│   ├── Security/
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── UserRole.cs
│   │   ├── RefreshToken.cs
│   │   ├── AuditLog.cs
│   │   └── SystemConfiguration.cs
│   │
│   ├── People/
│   │   ├── Person.cs
│   │   ├── Employee.cs
│   │   ├── Department.cs
│   │   ├── JobPosition.cs
│   │   ├── EmployeeContact.cs
│   │   ├── Client.cs
│   │   ├── ClientAddress.cs
│   │   ├── Supplier.cs
│   │   └── SupplierContact.cs
│   │
│   ├── Inventory/
│   │   ├── Category.cs
│   │   ├── Product.cs
│   │   ├── ProductSupplier.cs
│   │   ├── Warehouse.cs
│   │   ├── Inventory.cs
│   │   └── InventoryMovement.cs
│   │
│   ├── Purchases/
│   │   ├── PurchaseOrder.cs
│   │   ├── PurchaseOrderDetail.cs
│   │   ├── Purchase.cs
│   │   └── PurchaseDetail.cs
│   │
│   ├── Sales/
│   │   ├── Order.cs
│   │   ├── OrderDetail.cs
│   │   ├── Sale.cs
│   │   └── SaleDetail.cs
│   │
│   └── Billing/
│       ├── Invoice.cs
│       ├── InvoiceDetail.cs
│       └── Payment.cs
│
├── Interfaces/                              # Interfaces
│   ├── Repositories/                        # Interfaces de repositorios
│   │   ├── IRepository.cs                   # Repositorio genérico
│   │   ├── IUnitOfWork.cs                   # Unit of Work
│   │   ├── Security/
│   │   │   ├── IUserRepository.cs
│   │   │   ├── IRoleRepository.cs
│   │   │   └── IAuditLogRepository.cs
│   │   ├── People/
│   │   │   ├── IPersonRepository.cs
│   │   │   ├── IEmployeeRepository.cs
│   │   │   ├── IClientRepository.cs
│   │   │   └── ISupplierRepository.cs
│   │   ├── Inventory/
│   │   │   ├── IProductRepository.cs
│   │   │   ├── ICategoryRepository.cs
│   │   │   ├── IInventoryRepository.cs
│   │   │   ├── IWarehouseRepository.cs
│   │   │   └── IInventoryMovementRepository.cs
│   │   ├── Purchases/
│   │   │   ├── IPurchaseOrderRepository.cs
│   │   │   └── IPurchaseRepository.cs
│   │   ├── Sales/
│   │   │   ├── IOrderRepository.cs
│   │   │   └── ISaleRepository.cs
│   │   └── Billing/
│   │       ├── IInvoiceRepository.cs
│   │       └── IPaymentRepository.cs
│   │
│   └── Services/                            # Interfaces de servicios externos
│       ├── IAuthenticationService.cs
│       ├── ITokenService.cs
│       ├── IPasswordHasher.cs
│       ├── IAuditService.cs
│       ├── IEmailService.cs
│       └── ICurrentUserService.cs
│
├── ValueObjects/                            # Objetos de valor
│   ├── Money.cs
│   ├── Email.cs
│   ├── Address.cs
│   └── PhoneNumber.cs
│
└── Exceptions/                              # Excepciones de dominio
    ├── DomainException.cs
    ├── InsufficientStockException.cs
    ├── InvalidEntityException.cs
    └── BusinessRuleException.cs
```

---

## 3. Estructura de NexusERP.Application

```
src/NexusERP.Application/
├── NexusERP.Application.csproj
│
├── Common/                                  # Compartido
│   ├── Behaviors/                           # Pipeline behaviors
│   │   ├── ValidationBehavior.cs
│   │   ├── LoggingBehavior.cs
│   │   └── TransactionBehavior.cs
│   │
│   ├── Exceptions/                          # Excepciones de aplicación
│   │   ├── ValidationException.cs
│   │   ├── NotFoundException.cs
│   │   ├── ForbiddenAccessException.cs
│   │   └── ConflictException.cs
│   │
│   ├── Models/                              # Modelos comunes
│   │   ├── PaginatedList.cs
│   │   ├── PaginationRequest.cs
│   │   └── ApiResponse.cs
│   │
│   └── Mappings/                            # Mappers manuales
│       ├── MappingProfile.cs                # Configuración base
│       └── MappingExtensions.cs             # Extensiones de mapeo
│
├── Modules/                                 # Módulos por funcionalidad
│   ├── Security/
│   │   ├── Commands/
│   │   │   ├── Login/
│   │   │   │   ├── LoginCommand.cs
│   │   │   │   ├── LoginCommandValidator.cs
│   │   │   │   └── LoginCommandHandler.cs
│   │   │   ├── RefreshToken/
│   │   │   │   ├── RefreshTokenCommand.cs
│   │   │   │   ├── RefreshTokenCommandValidator.cs
│   │   │   │   └── RefreshTokenCommandHandler.cs
│   │   │   └── RevokeToken/
│   │   │       ├── RevokeTokenCommand.cs
│   │   │       └── RevokeTokenCommandHandler.cs
│   │   │
│   │   └── Queries/
│   │       └── GetCurrentUser/
│   │           ├── GetCurrentUserQuery.cs
│   │           └── GetCurrentUserQueryHandler.cs
│   │
│   ├── Identity/
│   │   ├── Commands/
│   │   │   ├── CreateUser/
│   │   │   ├── UpdateUser/
│   │   │   ├── DeleteUser/
│   │   │   ├── AssignRole/
│   │   │   └── RemoveRole/
│   │   │
│   │   └── Queries/
│   │       ├── GetAllUsers/
│   │       ├── GetUserById/
│   │       └── GetUserRoles/
│   │
│   ├── People/
│   │   ├── Commands/
│   │   │   ├── Employees/
│   │   │   │   ├── CreateEmployee/
│   │   │   │   ├── UpdateEmployee/
│   │   │   │   └── DeleteEmployee/
│   │   │   ├── Clients/
│   │   │   │   ├── CreateClient/
│   │   │   │   ├── UpdateClient/
│   │   │   │   └── DeleteClient/
│   │   │   └── Suppliers/
│   │   │       ├── CreateSupplier/
│   │   │       ├── UpdateSupplier/
│   │   │       └── DeleteSupplier/
│   │   │
│   │   └── Queries/
│   │       ├── GetEmployeeById/
│   │       ├── GetAllEmployees/
│   │       ├── GetClientById/
│   │       ├── GetAllClients/
│   │       ├── GetSupplierById/
│   │       └── GetAllSuppliers/
│   │
│   ├── Inventory/
│   │   ├── Commands/
│   │   │   ├── Categories/
│   │   │   │   ├── CreateCategory/
│   │   │   │   ├── UpdateCategory/
│   │   │   │   └── DeleteCategory/
│   │   │   ├── Products/
│   │   │   │   ├── CreateProduct/
│   │   │   │   ├── UpdateProduct/
│   │   │   │   └── DeleteProduct/
│   │   │   └── Inventory/
│   │   │       ├── AdjustStock/
│   │   │       ├── TransferStock/
│   │   │       └── ReserveStock/
│   │   │
│   │   └── Queries/
│   │       ├── GetProductById/
│   │       ├── GetAllProducts/
│   │       ├── GetCategoryById/
│   │       ├── GetAllCategories/
│   │       ├── GetStockByWarehouse/
│   │       ├── GetInventoryMovements/
│   │       └── CheckAvailability/
│   │
│   ├── Purchases/
│   │   ├── Commands/
│   │   │   ├── PurchaseOrders/
│   │   │   │   ├── CreatePurchaseOrder/
│   │   │   │   ├── UpdatePurchaseOrder/
│   │   │   │   ├── ApprovePurchaseOrder/
│   │   │   │   ├── ReceivePurchaseOrder/
│   │   │   │   └── CancelPurchaseOrder/
│   │   │   └── Purchases/
│   │   │       ├── CreatePurchase/
│   │   │       ├── ConfirmPurchase/
│   │   │       └── CancelPurchase/
│   │   │
│   │   └── Queries/
│   │       ├── GetPurchaseOrderById/
│   │       ├── GetAllPurchaseOrders/
│   │       ├── GetPurchaseById/
│   │       └── GetAllPurchases/
│   │
│   ├── Sales/
│   │   ├── Commands/
│   │   │   ├── Orders/
│   │   │   │   ├── CreateOrder/
│   │   │   │   ├── UpdateOrder/
│   │   │   │   ├── CompleteOrder/
│   │   │   │   └── CancelOrder/
│   │   │   └── Sales/
│   │   │       ├── CreateSale/
│   │   │       ├── ConfirmSale/
│   │   │       └── CancelSale/
│   │   │
│   │   └── Queries/
│   │       ├── GetOrderById/
│   │       ├── GetAllOrders/
│   │       ├── GetSaleById/
│   │       └── GetAllSales/
│   │
│   ├── Billing/
│   │   ├── Commands/
│   │   │   ├── Invoices/
│   │   │   │   ├── GenerateInvoice/
│   │   │   │   ├── MarkAsPaid/
│   │   │   │   └── CancelInvoice/
│   │   │   └── Payments/
│   │   │       ├── RegisterPayment/
│   │   │       └── RefundPayment/
│   │   │
│   │   └── Queries/
│   │       ├── GetInvoiceById/
│   │       ├── GetAllInvoices/
│   │       ├── GetPaymentById/
│   │       └── GetPaymentsByInvoice/
│   │
│   ├── Dashboard/
│   │   └── Queries/
│   │       └── GetDashboard/
│   │           ├── GetDashboardQuery.cs
│   │           └── GetDashboardQueryHandler.cs
│   │
│   ├── Reports/
│   │   └── Queries/
│   │       ├── SalesReport/
│   │       ├── PurchasesReport/
│   │       ├── InventoryReport/
│   │       └── ClientReport/
│   │
│   └── Settings/
│       ├── Commands/
│       │   ├── SetConfiguration/
│       │   └── DeleteConfiguration/
│       └── Queries/
│           ├── GetAllConfigurations/
│           └── GetConfigurationByKey/
│
├── DependencyInjection.cs                   # Configuración de DI
└── NexusERP.Application.csproj
```

---

## 4. Estructura de NexusERP.Infrastructure

```
src/NexusERP.Infrastructure/
├── NexusERP.Infrastructure.csproj
│
├── Persistence/                             # Persistencia
│   ├── NexusERPDbContext.cs                 # DbContext principal
│   ├── Configurations/                      # Configuraciones EF Core
│   │   ├── BaseEntityConfiguration.cs       # Config base
│   │   ├── Security/
│   │   │   ├── UserConfiguration.cs
│   │   │   ├── RoleConfiguration.cs
│   │   │   ├── UserRoleConfiguration.cs
│   │   │   ├── RefreshTokenConfiguration.cs
│   │   │   ├── AuditLogConfiguration.cs
│   │   │   └── SystemConfigurationConfiguration.cs
│   │   ├── People/
│   │   │   ├── PersonConfiguration.cs
│   │   │   ├── EmployeeConfiguration.cs
│   │   │   ├── DepartmentConfiguration.cs
│   │   │   ├── JobPositionConfiguration.cs
│   │   │   ├── EmployeeContactConfiguration.cs
│   │   │   ├── ClientConfiguration.cs
│   │   │   ├── ClientAddressConfiguration.cs
│   │   │   ├── SupplierConfiguration.cs
│   │   │   └── SupplierContactConfiguration.cs
│   │   ├── Inventory/
│   │   │   ├── CategoryConfiguration.cs
│   │   │   ├── ProductConfiguration.cs
│   │   │   ├── ProductSupplierConfiguration.cs
│   │   │   ├── WarehouseConfiguration.cs
│   │   │   ├── InventoryConfiguration.cs
│   │   │   └── InventoryMovementConfiguration.cs
│   │   ├── Purchases/
│   │   │   ├── PurchaseOrderConfiguration.cs
│   │   │   ├── PurchaseOrderDetailConfiguration.cs
│   │   │   ├── PurchaseConfiguration.cs
│   │   │   └── PurchaseDetailConfiguration.cs
│   │   ├── Sales/
│   │   │   ├── OrderConfiguration.cs
│   │   │   ├── OrderDetailConfiguration.cs
│   │   │   ├── SaleConfiguration.cs
│   │   │   └── SaleDetailConfiguration.cs
│   │   └── Billing/
│   │       ├── InvoiceConfiguration.cs
│   │       ├── InvoiceDetailConfiguration.cs
│   │       └── PaymentConfiguration.cs
│   │
│   ├── Migrations/                          # Migraciones
│   │   └── {timestamp}_{migration_name}.cs
│   │
│   └── Interceptors/                        # Interceptors
│       ├── AuditableEntityInterceptor.cs
│       ├── SoftDeleteInterceptor.cs
│       └── DispatchDomainEventsInterceptor.cs
│
├── Repositories/                            # Implementaciones de repositorios
│   ├── Repository.cs                        # Repositorio genérico
│   ├── UnitOfWork.cs                        # Unit of Work
│   ├── Security/
│   │   ├── UserRepository.cs
│   │   ├── RoleRepository.cs
│   │   └── AuditLogRepository.cs
│   ├── People/
│   │   ├── PersonRepository.cs
│   │   ├── EmployeeRepository.cs
│   │   ├── ClientRepository.cs
│   │   └── SupplierRepository.cs
│   ├── Inventory/
│   │   ├── ProductRepository.cs
│   │   ├── CategoryRepository.cs
│   │   ├── InventoryRepository.cs
│   │   ├── WarehouseRepository.cs
│   │   └── InventoryMovementRepository.cs
│   ├── Purchases/
│   │   ├── PurchaseOrderRepository.cs
│   │   └── PurchaseRepository.cs
│   ├── Sales/
│   │   ├── OrderRepository.cs
│   │   └── SaleRepository.cs
│   └── Billing/
│       ├── InvoiceRepository.cs
│       └── PaymentRepository.cs
│
├── Services/                                # Servicios externos
│   ├── Authentication/
│   │   ├── JwtTokenService.cs
│   │   └── BCryptPasswordHasher.cs
│   ├── Email/
│   │   └── SmtpEmailService.cs
│   └── CurrentUser/
│       └── CurrentUserService.cs
│
├── Extensions/                              # Extensiones
│   ├── ServiceCollectionExtensions.cs
│   └── DbContextExtensions.cs
│
└── NexusERP.Infrastructure.csproj
```

---

## 5. Estructura de NexusERP.Api

```
src/NexusERP.Api/
├── NexusERP.Api.csproj
│
├── Controllers/                             # Controladores
│   ├── BaseController.cs                    # Controlador base
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── RolesController.cs
│   ├── EmployeesController.cs
│   ├── ClientsController.cs
│   ├── SuppliersController.cs
│   ├── DepartmentsController.cs
│   ├── CategoriesController.cs
│   ├── ProductsController.cs
│   ├── WarehousesController.cs
│   ├── InventoryController.cs
│   ├── PurchaseOrdersController.cs
│   ├── PurchasesController.cs
│   ├── OrdersController.cs
│   ├── SalesController.cs
│   ├── InvoicesController.cs
│   ├── PaymentsController.cs
│   ├── DashboardController.cs
│   ├── ReportsController.cs
│   └── SettingsController.cs
│
├── Middleware/                               # Middleware
│   ├── ExceptionHandlingMiddleware.cs
│   ├── RequestLoggingMiddleware.cs
│   └── CorrelationIdMiddleware.cs
│
├── Filters/                                 # Filtros
│   ├── ValidationFilter.cs
│   └── AuditFilter.cs
│
├── Models/                                  # Modelos de presentación
│   ├── Requests/                            # Request DTOs
│   │   └── {Entity}Request.cs
│   └── Responses/                           # Response DTOs
│       └── {Entity}Response.cs
│
├── Configuration/                           # Configuración
│   ├── DependencyInjection.cs
│   ├── SwaggerConfiguration.cs
│   ├── JwtConfiguration.cs
│   └── CorsConfiguration.cs
│
├── Program.cs                               # Punto de entrada
├── appsettings.json                         # Config base
├── appsettings.Development.json             # Config dev
└── appsettings.Production.json              # Config prod
```

---

## 6. Justificación de la Estructura

### 6.1 Separación por Capas
- **Domain**: Solo contiene lo esencial del dominio, sin dependencias externas
- **Application**: Contiene la lógica de negocio y casos de uso
- **Infrastructure**: Implementa las dependencias externas (BD, email, etc.)
- **Api**: Capa de presentación, solo orquesta y expone endpoints

### 6.2 Separación por Módulos
- Cada módulo tiene su propia carpeta en Application
- Los comandos y queries están separados (CQRS ligero)
- Cada comando/query tiene su propio handler, validator y DTO

### 6.3 Ventajas de esta Estructura
1. **Escalabilidad**: Fácil agregar nuevos módulos sin afectar los existentes
2. **Mantenibilidad**: Código organizado por responsabilidad
3. **Testing**: Cada capa se puede testear aisladamente
4. **Reutilización**: Los módulos pueden reutilizarse en otros proyectos
5. **Onboarding**: Nuevo desarrollador puede entender la estructura rápidamente

### 6.4 Convenciones de Nomenclatura
- **Commands**: Acciones que modifican datos (Create, Update, Delete)
- **Queries**: Acciones que solo leen datos (Get, GetAll)
- **Handlers**: Lógica que procesa cada command/query
- **Validators**: Validaciones usando FluentValidation
- **Configurations**: Configuración de EF Core para cada entidad
