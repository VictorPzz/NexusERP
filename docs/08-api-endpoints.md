# NexusERP - Definición de Endpoints Principales

## Convenciones de la API

### URL Pattern
```
/api/v1/{resource}
```

### Respuesta Estándar
```json
{
    "success": true,
    "data": { ... },
    "message": "Operación exitosa",
    "errors": [],
    "traceId": "abc-123"
}
```

### Respuesta Paginada
```json
{
    "success": true,
    "data": {
        "items": [...],
        "totalCount": 100,
        "pageNumber": 1,
        "pageSize": 10,
        "totalPages": 10
    }
}
```

### Headers de Autenticación
```
Authorization: Bearer {access_token}
```

---

## 1. Autenticación

### POST /api/v1/auth/login
**Autenticación**: No requerida
**Descripción**: Iniciar sesión

**Request Body:**
```json
{
    "email": "user@example.com",
    "password": "password123"
}
```

**Response 200:**
```json
{
    "success": true,
    "data": {
        "accessToken": "eyJhbGciOiJIUzI1NiIs...",
        "refreshToken": "abc123...",
        "expiresAt": "2024-01-01T00:15:00Z",
        "user": {
            "id": 1,
            "username": "jdoe",
            "email": "user@example.com",
            "roles": ["Admin"]
        }
    }
}
```

### POST /api/v1/auth/refresh
**Autenticación**: No requerida
**Descripción**: Refrescar token de acceso

**Request Body:**
```json
{
    "refreshToken": "abc123..."
}
```

### POST /api/v1/auth/revoke
**Autenticación**: Requerida
**Descripción**: Revocar refresh token

**Request Body:**
```json
{
    "refreshToken": "abc123..."
}
```

### POST /api/v1/auth/logout
**Autenticación**: Requerida
**Descripción**: Cerrar sesión (revoca todos los tokens)

---

## 2. Usuarios

### GET /api/v1/users
**Autenticación**: Requerida (Admin)
**Descripción**: Obtener lista paginada de usuarios

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- search (string, optional)
- isActive (bool, optional)

### GET /api/v1/users/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Obtener usuario por ID

### POST /api/v1/users
**Autenticación**: Requerida (Admin)
**Descripción**: Crear usuario

**Request Body:**
```json
{
    "username": "jdoe",
    "email": "jdoe@example.com",
    "password": "Password123!",
    "firstName": "John",
    "lastName": "Doe"
}
```

### PUT /api/v1/users/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Actualizar usuario

### DELETE /api/v1/users/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Eliminar usuario (soft delete)

### PUT /api/v1/users/{id}/activate
**Autenticación**: Requerida (Admin)
**Descripción**: Activar usuario

### PUT /api/v1/users/{id}/deactivate
**Autenticación**: Requerida (Admin)
**Descripción**: Desactivar usuario

### POST /api/v1/users/{id}/roles
**Autenticación**: Requerida (Admin)
**Descripción**: Asignar rol a usuario

**Request Body:**
```json
{
    "roleId": 1
}
```

### DELETE /api/v1/users/{id}/roles/{roleId}
**Autenticación**: Requerida (Admin)
**Descripción**: Remover rol de usuario

### GET /api/v1/users/{id}/roles
**Autenticación**: Requerida (Admin)
**Descripción**: Obtener roles de usuario

---

## 3. Roles

### GET /api/v1/roles
**Autenticación**: Requerida (Admin)
**Descripción**: Obtener todos los roles

### GET /api/v1/roles/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Obtener rol por ID

### POST /api/v1/roles
**Autenticación**: Requerida (Admin)
**Descripción**: Crear rol

**Request Body:**
```json
{
    "name": "Manager",
    "description": "Gerente del sistema"
}
```

### PUT /api/v1/roles/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Actualizar rol

### DELETE /api/v1/roles/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Eliminar rol

---

## 4. Empleados

### GET /api/v1/employees
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de empleados

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- search (string, optional)
- departmentId (int, optional)
- jobPositionId (int, optional)
- status (string: active, inactive, terminated, optional)

### GET /api/v1/employees/{id}
**Autenticación**: Requerida
**Descripción**: Obtener empleado por ID

### POST /api/v1/employees
**Autenticación**: Requerida (Admin)
**Descripción**: Crear empleado

**Request Body:**
```json
{
    "documentType": "CC",
    "documentNumber": "1234567890",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@company.com",
    "phone": "+52-55-1234-5678",
    "dateOfBirth": "1990-01-15",
    "gender": "male",
    "employeeCode": "EMP-001",
    "jobPositionId": 1,
    "hireDate": "2024-01-15",
    "employmentType": "full_time",
    "salary": 50000.00
}
```

### PUT /api/v1/employees/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Actualizar empleado

### DELETE /api/v1/employees/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Eliminar empleado (soft delete)

### PUT /api/v1/employees/{id}/terminate
**Autenticación**: Requerida (Admin)
**Descripción**: Terminar empleado

---

## 5. Clientes

### GET /api/v1/clients
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de clientes

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- search (string, optional)
- status (string: active, inactive, suspended, optional)

### GET /api/v1/clients/{id}
**Autenticación**: Requerida
**Descripción**: Obtener cliente por ID

### POST /api/v1/clients
**Autenticación**: Requerida
**Descripción**: Crear cliente

**Request Body:**
```json
{
    "documentType": "CC",
    "documentNumber": "9876543210",
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@email.com",
    "phone": "+52-55-8765-4321",
    "clientCode": "CLI-001",
    "creditLimit": 100000.00,
    "addresses": [
        {
            "addressType": "billing",
            "street": "Av. Principal 123",
            "city": "Ciudad de México",
            "state": "CDMX",
            "postalCode": "06600",
            "country": "México",
            "isPrimary": true
        }
    ]
}
```

### PUT /api/v1/clients/{id}
**Autenticación**: Requerida
**Descripción**: Actualizar cliente

### DELETE /api/v1/clients/{id}
**Autenticación**: Requerida
**Descripción**: Eliminar cliente (soft delete)

### GET /api/v1/clients/{id}/addresses
**Autenticación**: Requerida
**Descripción**: Obtener direcciones de cliente

### POST /api/v1/clients/{id}/addresses
**Autenticación**: Requerida
**Descripción**: Agregar dirección a cliente

### PUT /api/v1/clients/{clientId}/addresses/{addressId}
**Autenticación**: Requerida
**Descripción**: Actualizar dirección de cliente

### DELETE /api/v1/clients/{clientId}/addresses/{addressId}
**Autenticación**: Requerida
**Descripción**: Eliminar dirección de cliente

---

## 6. Proveedores

### GET /api/v1/suppliers
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de proveedores

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- search (string, optional)
- status (string: active, inactive, suspended, optional)

### GET /api/v1/suppliers/{id}
**Autenticación**: Requerida
**Descripción**: Obtener proveedor por ID

### POST /api/v1/suppliers
**Autenticación**: Requerida
**Descripción**: Crear proveedor

**Request Body:**
```json
{
    "documentType": "NIT",
    "documentNumber": "900123456-7",
    "firstName": "Proveedor",
    "lastName": "S.A.S.",
    "email": "contacto@proveedor.com",
    "supplierCode": "SUP-001",
    "companyName": "Proveedor S.A.S.",
    "taxId": "900123456-7",
    "paymentTerms": "Net 30",
    "rating": 4.5,
    "contacts": [
        {
            "contactType": "phone",
            "contactValue": "+57-1-234-5678",
            "contactName": "Juan Pérez",
            "isPrimary": true
        }
    ]
}
```

### PUT /api/v1/suppliers/{id}
**Autenticación**: Requerida
**Descripción**: Actualizar proveedor

### DELETE /api/v1/suppliers/{id}
**Autenticación**: Requerida
**Descripción**: Eliminar proveedor (soft delete)

---

## 7. Departamentos

### GET /api/v1/departments
**Autenticación**: Requerida
**Descripción**: Obtener todos los departamentos

### GET /api/v1/departments/{id}
**Autenticación**: Requerida
**Descripción**: Obtener departamento por ID

### POST /api/v1/departments
**Autenticación**: Requerida (Admin)
**Descripción**: Crear departamento

**Request Body:**
```json
{
    "name": "Ventas",
    "description": "Departamento de Ventas",
    "parentId": null
}
```

### PUT /api/v1/departments/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Actualizar departamento

### DELETE /api/v1/departments/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Eliminar departamento

---

## 8. Categorías

### GET /api/v1/categories
**Autenticación**: Requerida
**Descripción**: Obtener todas las categorías (árbol)

### GET /api/v1/categories/{id}
**Autenticación**: Requerida
**Descripción**: Obtener categoría por ID

### POST /api/v1/categories
**Autenticación**: Requerida (Admin)
**Descripción**: Crear categoría

**Request Body:**
```json
{
    "name": "Electrónica",
    "description": "Productos electrónicos",
    "parentId": null
}
```

### PUT /api/v1/categories/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Actualizar categoría

### DELETE /api/v1/categories/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Eliminar categoría

---

## 9. Productos

### GET /api/v1/products
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de productos

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- search (string, optional)
- categoryId (int, optional)
- isActive (bool, optional)
- minPrice (decimal, optional)
- maxPrice (decimal, optional)

### GET /api/v1/products/{id}
**Autenticación**: Requerida
**Descripción**: Obtener producto por ID

### GET /api/v1/products/by-code/{code}
**Autenticación**: Requerida
**Descripción**: Obtener producto por código

### POST /api/v1/products
**Autenticación**: Requerida (Admin)
**Descripción**: Crear producto

**Request Body:**
```json
{
    "code": "PROD-001",
    "name": "Laptop HP",
    "description": "Laptop HP 15 pulgadas",
    "categoryId": 1,
    "unitOfMeasure": "unit",
    "costPrice": 15000.00,
    "sellingPrice": 22000.00,
    "taxRate": 16.00,
    "minStock": 5,
    "maxStock": 50,
    "barcode": "7501234567890",
    "suppliers": [
        {
            "supplierId": 1,
            "supplierPrice": 14500.00,
            "leadTimeDays": 7,
            "isPreferred": true
        }
    ]
}
```

### PUT /api/v1/products/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Actualizar producto

### DELETE /api/v1/products/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Eliminar producto (soft delete)

---

## 10. Almacenes

### GET /api/v1/warehouses
**Autenticación**: Requerida
**Descripción**: Obtener todos los almacenes

### GET /api/v1/warehouses/{id}
**Autenticación**: Requerida
**Descripción**: Obtener almacén por ID

### POST /api/v1/warehouses
**Autenticación**: Requerida (Admin)
**Descripción**: Crear almacén

**Request Body:**
```json
{
    "name": "Almacén Central",
    "code": "ALM-001",
    "address": "Av. Industrial 456",
    "managerId": 1,
    "isDefault": true
}
```

### PUT /api/v1/warehouses/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Actualizar almacén

### DELETE /api/v1/warehouses/{id}
**Autenticación**: Requerida (Admin)
**Descripción**: Eliminar almacén

---

## 11. Inventario

### GET /api/v1/inventory
**Autenticación**: Requerida
**Descripción**: Obtener inventario por almacén

**Query Parameters:**
- warehouseId (int, required)
- page (int, default: 1)
- pageSize (int, default: 10)
- search (string, optional)
- lowStock (bool, optional)

### GET /api/v1/inventory/{productId}/{warehouseId}
**Autenticación**: Requerida
**Descripción**: Obtener stock de producto en almacén

### POST /api/v1/inventory/adjust
**Autenticación**: Requerida (Admin)
**Descripción**: Ajustar stock

**Request Body:**
```json
{
    "productId": 1,
    "warehouseId": 1,
    "quantity": 10,
    "notes": "Ajuste por inventario físico"
}
```

### POST /api/v1/inventory/transfer
**Autenticación**: Requerida (Admin)
**Descripción**: Transferir stock entre almacenes

**Request Body:**
```json
{
    "productId": 1,
    "fromWarehouseId": 1,
    "toWarehouseId": 2,
    "quantity": 5,
    "notes": "Transferencia a sucursal"
}
```

### GET /api/v1/inventory/movements
**Autenticación**: Requerida
**Descripción**: Obtener movimientos de inventario

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- productId (int, optional)
- warehouseId (int, optional)
- movementType (string, optional)
- startDate (date, optional)
- endDate (date, optional)

---

## 12. Órdenes de Compra

### GET /api/v1/purchase-orders
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de órdenes de compra

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- status (string, optional)
- supplierId (int, optional)
- startDate (date, optional)
- endDate (date, optional)

### GET /api/v1/purchase-orders/{id}
**Autenticación**: Requerida
**Descripción**: Obtener orden de compra por ID

### POST /api/v1/purchase-orders
**Autenticación**: Requerida
**Descripción**: Crear orden de compra

**Request Body:**
```json
{
    "supplierId": 1,
    "orderDate": "2024-01-15",
    "expectedDate": "2024-01-22",
    "notes": "Compra mensual",
    "details": [
        {
            "productId": 1,
            "quantity": 10,
            "unitPrice": 15000.00,
            "taxRate": 16.00
        }
    ]
}
```

### PUT /api/v1/purchase-orders/{id}
**Autenticación**: Requerida
**Descripción**: Actualizar orden de compra

### PUT /api/v1/purchase-orders/{id}/approve
**Autenticación**: Requerida (Admin)
**Descripción**: Aprobar orden de compra

### PUT /api/v1/purchase-orders/{id}/receive
**Autenticación**: Requerida (Admin)
**Descripción**: Recibir orden de compra

### PUT /api/v1/purchase-orders/{id}/cancel
**Autenticación**: Requerida (Admin)
**Descripción**: Cancelar orden de compra

---

## 13. Compras

### GET /api/v1/purchases
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de compras

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- status (string, optional)
- supplierId (int, optional)
- startDate (date, optional)
- endDate (date, optional)

### GET /api/v1/purchases/{id}
**Autenticación**: Requerida
**Descripción**: Obtener compra por ID

### POST /api/v1/purchases
**Autenticación**: Requerida
**Descripción**: Crear compra

**Request Body:**
```json
{
    "supplierId": 1,
    "purchaseOrderId": 1,
    "purchaseDate": "2024-01-15",
    "notes": "Recepción de mercancía",
    "details": [
        {
            "productId": 1,
            "quantity": 10,
            "unitPrice": 15000.00,
            "taxRate": 16.00
        }
    ]
}
```

### PUT /api/v1/purchases/{id}/confirm
**Autenticación**: Requerida (Admin)
**Descripción**: Confirmar compra (actualiza stock)

### PUT /api/v1/purchases/{id}/cancel
**Autenticación**: Requerida (Admin)
**Descripción**: Cancelar compra

---

## 14. Órdenes

### GET /api/v1/orders
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de órdenes

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- orderType (string: sale, purchase, optional)
- status (string, optional)
- clientId (int, optional)
- supplierId (int, optional)
- startDate (date, optional)
- endDate (date, optional)

### GET /api/v1/orders/{id}
**Autenticación**: Requerida
**Descripción**: Obtener orden por ID

### POST /api/v1/orders
**Autenticación**: Requerida
**Descripción**: Crear orden

**Request Body:**
```json
{
    "orderType": "sale",
    "clientId": 1,
    "orderDate": "2024-01-15",
    "expectedDate": "2024-01-20",
    "notes": "Orden de cliente",
    "details": [
        {
            "productId": 1,
            "quantity": 2,
            "unitPrice": 22000.00,
            "taxRate": 16.00
        }
    ]
}
```

### PUT /api/v1/orders/{id}
**Autenticación**: Requerida
**Descripción**: Actualizar orden

### PUT /api/v1/orders/{id}/complete
**Autenticación**: Requerida (Admin)
**Descripción**: Completar orden

### PUT /api/v1/orders/{id}/cancel
**Autenticación**: Requerida (Admin)
**Descripción**: Cancelar orden

---

## 15. Ventas

### GET /api/v1/sales
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de ventas

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- status (string, optional)
- clientId (int, optional)
- paymentStatus (string, optional)
- startDate (date, optional)
- endDate (date, optional)

### GET /api/v1/sales/{id}
**Autenticación**: Requerida
**Descripción**: Obtener venta por ID

### POST /api/v1/sales
**Autenticación**: Requerida
**Descripción**: Crear venta

**Request Body:**
```json
{
    "clientId": 1,
    "saleDate": "2024-01-15",
    "discountAmount": 0,
    "notes": "Venta al contado",
    "details": [
        {
            "productId": 1,
            "quantity": 2,
            "unitPrice": 22000.00,
            "taxRate": 16.00,
            "discountRate": 0
        }
    ]
}
```

### PUT /api/v1/sales/{id}/confirm
**Autenticación**: Requerida (Admin)
**Descripción**: Confirmar venta (actualiza stock y genera factura)

### PUT /api/v1/sales/{id}/cancel
**Autenticación**: Requerida (Admin)
**Descripción**: Cancelar venta

---

## 16. Facturas

### GET /api/v1/invoices
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de facturas

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- invoiceType (string: sale, purchase, optional)
- status (string, optional)
- clientId (int, optional)
- supplierId (int, optional)
- startDate (date, optional)
- endDate (date, optional)

### GET /api/v1/invoices/{id}
**Autenticación**: Requerida
**Descripción**: Obtener factura por ID

### PUT /api/v1/invoices/{id}/mark-paid
**Autenticación**: Requerida (Admin)
**Descripción**: Marcar factura como pagada

### PUT /api/v1/invoices/{id}/cancel
**Autenticación**: Requerida (Admin)
**Descripción**: Cancelar factura

---

## 17. Pagos

### GET /api/v1/payments
**Autenticación**: Requerida
**Descripción**: Obtener lista paginada de pagos

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- invoiceId (int, optional)
- paymentMethod (string, optional)
- startDate (date, optional)
- endDate (date, optional)

### GET /api/v1/payments/{id}
**Autenticación**: Requerida
**Descripción**: Obtener pago por ID

### POST /api/v1/payments
**Autenticación**: Requerida
**Descripción**: Registrar pago

**Request Body:**
```json
{
    "invoiceId": 1,
    "amount": 25520.00,
    "paymentMethod": "transfer",
    "paymentDate": "2024-01-15",
    "reference": "TRF-123456",
    "notes": "Pago por transferencia bancaria"
}
```

---

## 18. Dashboard

### GET /api/v1/dashboard
**Autenticación**: Requerida
**Descripción**: Obtener métricas del dashboard

**Query Parameters:**
- startDate (date, required)
- endDate (date, required)

**Response 200:**
```json
{
    "success": true,
    "data": {
        "totalSales": 1500000.00,
        "totalPurchases": 800000.00,
        "totalRevenue": 700000.00,
        "totalClients": 150,
        "totalProducts": 500,
        "lowStockProducts": 12,
        "topProducts": [
            {
                "productId": 1,
                "name": "Laptop HP",
                "quantitySold": 25,
                "totalRevenue": 550000.00
            }
        ],
        "dailyMetrics": [
            {
                "date": "2024-01-15",
                "sales": 50000.00,
                "purchases": 20000.00
            }
        ]
    }
}
```

---

## 19. Reportes

### GET /api/v1/reports/sales
**Autenticación**: Requerida
**Descripción**: Generar reporte de ventas

**Query Parameters:**
- startDate (date, required)
- endDate (date, required)
- clientId (int, optional)
- groupBy (string: day, week, month, optional)

### GET /api/v1/reports/purchases
**Autenticación**: Requerida
**Descripción**: Generar reporte de compras

**Query Parameters:**
- startDate (date, required)
- endDate (date, required)
- supplierId (int, optional)
- groupBy (string: day, week, month, optional)

### GET /api/v1/reports/inventory
**Autenticación**: Requerida
**Descripción**: Generar reporte de inventario

**Query Parameters:**
- warehouseId (int, optional)
- categoryId (int, optional)
- lowStock (bool, optional)

### GET /api/v1/reports/clients
**Autenticación**: Requerida
**Descripción**: Generar reporte de clientes

**Query Parameters:**
- startDate (date, required)
- endDate (date, required)
- topN (int, default: 10)

---

## 20. Configuración

### GET /api/v1/settings
**Autenticación**: Requerida (Admin)
**Descripción**: Obtener todas las configuraciones

### GET /api/v1/settings/{key}
**Autenticación**: Requerida (Admin)
**Descripción**: Obtener configuración por clave

### PUT /api/v1/settings/{key}
**Autenticación**: Requerida (Admin)
**Descripción**: Actualizar configuración

**Request Body:**
```json
{
    "value": "Mi Empresa S.A. de C.V.",
    "description": "Nombre de la empresa"
}
```

### DELETE /api/v1/settings/{key}
**Autenticación**: Requerida (Admin)
**Descripción**: Eliminar configuración

---

## 21. Auditoría

### GET /api/v1/audit-logs
**Autenticación**: Requerida (Admin)
**Descripción**: Obtener logs de auditoría

**Query Parameters:**
- page (int, default: 1)
- pageSize (int, default: 10)
- userId (int, optional)
- action (string, optional)
- entityName (string, optional)
- startDate (date, optional)
- endDate (date, optional)

---

## Resumen de Endpoints

| Módulo | Endpoints | Métodos |
|--------|-----------|---------|
| Auth | 4 | POST |
| Users | 9 | GET, POST, PUT, DELETE |
| Roles | 5 | GET, POST, PUT, DELETE |
| Employees | 5 | GET, POST, PUT, DELETE |
| Clients | 7 | GET, POST, PUT, DELETE |
| Suppliers | 5 | GET, POST, PUT, DELETE |
| Departments | 5 | GET, POST, PUT, DELETE |
| Categories | 5 | GET, POST, PUT, DELETE |
| Products | 6 | GET, POST, PUT, DELETE |
| Warehouses | 5 | GET, POST, PUT, DELETE |
| Inventory | 4 | GET, POST |
| Purchase Orders | 6 | GET, POST, PUT |
| Purchases | 5 | GET, POST, PUT |
| Orders | 5 | GET, POST, PUT |
| Sales | 5 | GET, POST, PUT |
| Invoices | 4 | GET, PUT |
| Payments | 3 | GET, POST |
| Dashboard | 1 | GET |
| Reports | 4 | GET |
| Settings | 4 | GET, PUT, DELETE |
| Audit Logs | 1 | GET |
| **Total** | **99** | |
