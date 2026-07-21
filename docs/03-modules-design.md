# NexusERP - Diseño de Módulos

## Estructura Modular

Cada módulo sigue el mismo patrón de arquitectura y se comunica a través de interfaces definidas en el proyecto Domain.

```
Modules/
├── Security/          → Autenticación, autorización, auditoría
├── Identity/          → Usuarios, roles, empleados
├── People/            → Clientes, proveedores, personas
├── Inventory/         → Productos, categorías, stock, movimientos
├── Purchases/         → Compras, órdenes de compra
├── Sales/             → Ventas, órdenes de venta
├── Billing/           → Facturación, pagos
├── Dashboard/         → Métricas, indicadores
├── Reports/           → Generación de reportes
└── Settings/          → Configuración del sistema
```

---

## Módulo 1: Security

### Responsabilidades
- Autenticación de usuarios (login/logout)
- Generación y validación de JWT tokens
- Gestión de refresh tokens
- Registro de auditoría de acciones
- Configuración de políticas de seguridad

### Entidades
- User
- Role
- UserRole
- RefreshToken
- AuditLog

### Interfaces que Expone
```csharp
public interface IAuthenticationService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<Result> RevokeTokenAsync(string token);
}

public interface IAuditService
{
    Task LogAsync(AuditLogEntry entry);
}
```

### Dependencias
- Ninguna (es un módulo base)

---

## Módulo 2: Identity

### Responsabilidades
- CRUD de usuarios
- Asignación de roles a usuarios
- Gestión de contraseñas (cambiar, recuperar)
- Perfil de usuario

### Entidades
- User (referencia desde Security)
- Role (referencia desde Security)
- UserRole (referencia desde Security)

### Interfaces que Expone
```csharp
public interface IUserService
{
    Task<Result<UserDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<UserDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreateUserCommand command);
    Task<Result> UpdateAsync(int id, UpdateUserCommand command);
    Task<Result> DeleteAsync(int id);
    Task<Result> AssignRoleAsync(int userId, int roleId);
    Task<Result> RemoveRoleAsync(int userId, int roleId);
}
```

### Dependencias
- Security Module (para User, Role)

---

## Módulo 3: People

### Responsabilidades
- Gestión de información personal (Person)
- Gestión de empleados (Employee, Department, JobPosition)
- Gestión de clientes (Client)
- Gestión de proveedores (Supplier)

### Entidades
- Person
- Employee
- Department
- JobPosition
- EmployeeContact
- Client
- ClientAddress
- Supplier
- SupplierContact

### Interfaces que Expone
```csharp
public interface IEmployeeService
{
    Task<Result<EmployeeDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<EmployeeDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreateEmployeeCommand command);
    Task<Result> UpdateAsync(int id, UpdateEmployeeCommand command);
    Task<Result> DeleteAsync(int id);
}

public interface IClientService
{
    Task<Result<ClientDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<ClientDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreateClientCommand command);
    Task<Result> UpdateAsync(int id, UpdateClientCommand command);
    Task<Result> DeleteAsync(int id);
}

public interface ISupplierService
{
    Task<Result<SupplierDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<SupplierDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreateSupplierCommand command);
    Task<Result> UpdateAsync(int id, UpdateSupplierCommand command);
    Task<Result> DeleteAsync(int id);
}
```

### Dependencias
- Security Module (para user association)

---

## Módulo 4: Inventory

### Responsabilidades
- Gestión de categorías de productos
- CRUD de productos
- Control de stock por almacén
- Registro de movimientos de inventario
- Validación de disponibilidad de stock

### Entidades
- Category
- Product
- ProductSupplier
- Warehouse
- Inventory
- InventoryMovement

### Interfaces que Expone
```csharp
public interface IProductService
{
    Task<Result<ProductDto>> GetByIdAsync(int id);
    Task<Result<ProductDto>> GetByCodeAsync(string code);
    Task<Result<PaginatedList<ProductDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreateProductCommand command);
    Task<Result> UpdateAsync(int id, UpdateProductCommand command);
    Task<Result> DeleteAsync(int id);
}

public interface ICategoryService
{
    Task<Result<CategoryDto>> GetByIdAsync(int id);
    Task<Result<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<Result<int>> CreateAsync(CreateCategoryCommand command);
    Task<Result> UpdateAsync(int id, UpdateCategoryCommand command);
    Task<Result> DeleteAsync(int id);
}

public interface IInventoryService
{
    Task<Result<InventoryDto>> GetStockAsync(int productId, int warehouseId);
    Task<Result<IEnumerable<InventoryDto>>> GetStockByWarehouseAsync(int warehouseId);
    Task<Result> AdjustStockAsync(AdjustStockCommand command);
    Task<Result> TransferStockAsync(TransferStockCommand command);
    Task<Result<bool>> CheckAvailabilityAsync(int productId, int warehouseId, int quantity);
}

public interface IInventoryMovementService
{
    Task<Result<PaginatedList<InventoryMovementDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<PaginatedList<InventoryMovementDto>>> GetByProductAsync(int productId, PaginationRequest request);
}
```

### Dependencias
- People Module (para Supplier en ProductSupplier)

---

## Módulo 5: Purchases

### Responsabilidades
- Gestión de órdenes de compra
- Registro de compras a proveedores
- Actualización automática de inventario
- Generación de facturas de compra

### Entidades
- PurchaseOrder
- PurchaseOrderDetail
- Purchase
- PurchaseDetail

### Interfaces que Expone
```csharp
public interface IPurchaseOrderService
{
    Task<Result<PurchaseOrderDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<PurchaseOrderDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreatePurchaseOrderCommand command);
    Task<Result> UpdateAsync(int id, UpdatePurchaseOrderCommand command);
    Task<Result> CancelAsync(int id);
    Task<Result> ApproveAsync(int id);
    Task<Result> ReceiveAsync(int id);
}

public interface IPurchaseService
{
    Task<Result<PurchaseDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<PurchaseDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreatePurchaseCommand command);
    Task<Result> ConfirmAsync(int id);
    Task<Result> CancelAsync(int id);
}
```

### Dependencias
- Inventory Module (para actualizar stock)
- People Module (para Supplier)
- Billing Module (para generar factura)

---

## Módulo 6: Sales

### Responsabilidades
- Registro de ventas a clientes
- Validación de stock antes de confirmar
- Actualización automática de inventario
- Generación de facturas de venta

### Entidades
- Sale
- SaleDetail
- Order
- OrderDetail

### Interfaces que Expone
```csharp
public interface ISaleService
{
    Task<Result<SaleDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<SaleDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreateSaleCommand command);
    Task<Result> ConfirmAsync(int id);
    Task<Result> CancelAsync(int id);
}

public interface IOrderService
{
    Task<Result<OrderDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<OrderDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> CreateAsync(CreateOrderCommand command);
    Task<Result> UpdateAsync(int id, UpdateOrderCommand command);
    Task<Result> CancelAsync(int id);
    Task<Result> CompleteAsync(int id);
}
```

### Dependencias
- Inventory Module (para validar y actualizar stock)
- People Module (para Client)
- Billing Module (para generar factura)

---

## Módulo 7: Billing

### Responsabilidades
- Generación de facturas
- Gestión de estados de factura
- Registro de pagos
- Consulta de saldos

### Entidades
- Invoice
- InvoiceDetail
- Payment

### Interfaces que Expone
```csharp
public interface IInvoiceService
{
    Task<Result<InvoiceDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<InvoiceDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<int>> GenerateFromSaleAsync(int saleId);
    Task<Result> MarkAsPaidAsync(int invoiceId);
    Task<Result> CancelAsync(int invoiceId);
}

public interface IPaymentService
{
    Task<Result<PaymentDto>> GetByIdAsync(int id);
    Task<Result<PaginatedList<PaymentDto>>> GetByInvoiceAsync(int invoiceId, PaginationRequest request);
    Task<Result<int>> RegisterPaymentAsync(RegisterPaymentCommand command);
}
```

### Dependencias
- Sales Module (para Sale)
- People Module (para Client/Supplier)

---

## Módulo 8: Dashboard

### Responsabilidades
- Consolidación de métricas de negocio
- KPIs principales
- Tendencias y comparativas

### Datos que Expone
```csharp
public interface IDashboardService
{
    Task<Result<DashboardDto>> GetDashboardAsync(DateTime startDate, DateTime endDate);
}

public class DashboardDto
{
    public decimal TotalSales { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalClients { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public List<TopProductDto> TopProducts { get; set; }
    public List<DailyMetricDto> DailyMetrics { get; set; }
}
```

### Dependencias
- Sales Module
- Purchases Module
- Inventory Module
- People Module

---

## Módulo 9: Reports

### Responsabilidades
- Generación de reportes personalizados
- Exportación (futuro: PDF, Excel)
- Reportes predefinidos

### Reportes Disponibles
```csharp
public interface IReportService
{
    Task<Result<SalesReportDto>> GenerateSalesReportAsync(ReportFilters filters);
    Task<Result<PurchasesReportDto>> GeneratePurchasesReportAsync(ReportFilters filters);
    Task<Result<InventoryReportDto>> GenerateInventoryReportAsync(ReportFilters filters);
    Task<Result<ClientReportDto>> GenerateClientReportAsync(ReportFilters filters);
}
```

### Dependencias
- Sales Module
- Purchases Module
- Inventory Module
- People Module

---

## Módulo 10: Settings

### Responsabilidades
- Configuración clave-valor del sistema
- Configuración por módulo
- Configuración de la empresa

### Entidades
- SystemConfiguration

### Interfaces que Expone
```csharp
public interface ISettingsService
{
    Task<Result<Dictionary<string, string>>> GetAllAsync();
    Task<Result<string>> GetAsync(string key);
    Task<Result> SetAsync(string key, string value);
    Task<Result> DeleteAsync(string key);
}
```

### Dependencias
- Ninguna (módulo independiente)

---

## Comunicación entre Módulos

### 1. Directa (vía interfaces)
```
Sales Service → IInventoryService.CheckAvailabilityAsync()
Sales Service → IInvoiceService.GenerateFromSaleAsync()
```

### 2. Event-Driven (futuro)
```
SaleConfirmedEvent → InventoryService (decrementar stock)
SaleConfirmedEvent → InvoiceService (generar factura)
PurchaseConfirmedEvent → InventoryService (incrementar stock)
```

### 3. Regla de Comunicación
- Los módulos NUNCA se referencian directamente entre sí
- Siempre se comunican a través de interfaces definidas en Domain
- Las dependencias se inyectan vía DI Container

---

## Matriz de Dependencias

```
Security ← (no tiene dependencias)
Identity ← Security
People ← Security
Inventory ← People
Purchases ← Inventory, People, Billing
Sales ← Inventory, People, Billing
Billing ← Sales, People
Dashboard ← Sales, Purchases, Inventory, People
Reports ← Sales, Purchases, Inventory, People
Settings ← (no tiene dependencias)
```
