# NexusERP - Análisis del Dominio del Negocio

## 1. Visión del Sistema

NexusERP es un sistema ERP modular diseñado para PyMEs que centraliza la gestión empresarial
a través de módulos interconectados. El sistema está diseñado con arquitectura modular para
permitir la adición de nuevos módulos sin modificar los existentes.

## 2. Identificación de Subdominios

### 2.1 Subdominio de Seguridad (Core)
- Autenticación y autorización
- Gestión de usuarios y roles
- Auditoría de acciones
- Configuración del sistema

### 2.2 Subdominio de Personas
- Gestión de empleados
- Gestión de clientes
- Gestión de proveedores

### 2.3 Subdominio de Inventario
- Categorías de productos
- Gestión de productos
- Control de inventario
- Movimientos de inventario

### 2.4 Subdominio de Comercio
- Compras a proveedores
- Ventas a clientes
- Órdenes de compra/venta
- Facturación

### 2.5 Subdominio de Inteligencia
- Dashboard con indicadores
- Reportes

## 3. Entidades del Dominio por Módulo

### 3.1 Módulo de Seguridad

**User (Usuario)**
- Representa un usuario del sistema
- Se relaciona con Role (muchos a uno)
- Se relaciona con Employee (uno a uno, opcional)
- Se relaciona con RefreshToken (uno a muchos)
- Se relaciona con AuditLog (uno a muchos)

**Role (Rol)**
- Define permisos y roles del sistema
- Se relaciona con User (uno a muchos)
- Se relaciona con UserRole (uno a muchos) para asignación flexible

**UserRole (Asignación de Rol)**
- Tabla intermedia para asignar roles a usuarios
- Permite múltiples roles por usuario

**RefreshToken**
- Almacena tokens de refresco para JWT
- Se relaciona con User (muchos a uno)
- Tiene fecha de expiración

**AuditLog (Registro de Auditoría)**
- Registra todas las acciones importantes del sistema
- Se relaciona con User (muchos a uno)
- Almacena: acción, entidad, ID de entidad, valores anteriores/nuevos, IP, timestamp

**SystemConfiguration (Configuración del Sistema)**
- Almacena configuraciones clave-valor del sistema
- Soporta configuraciones por módulo

### 3.2 Módulo de Personas

**Employee (Empleado)**
- Representa un empleado de la empresa
- Se relaciona con User (uno a uno, opcional)
- Se relaciona con Department (muchos a uno)
- Se relaciona con JobPosition (muchos a uno)
- Se relaciona con EmployeeContact (uno a uno)

**Department (Departamento)**
- Organización interna de la empresa
- Se relaciona con Employee (uno a muchos)
- Se relaciona con JobPosition (uno a muchos)

**JobPosition (Cargo/Puesto)**
- Define los cargos disponibles
- Se relaciona con Department (muchos a uno)
- Se relaciona con Employee (uno a muchos)

**EmployeeContact (Contacto del Empleado)**
- Información de contacto del empleado
- Se relaciona con Employee (uno a uno)

**Client (Cliente)**
- Clientes de la empresa
- Se relaciona con Person (uno a uno) - información personal
- Se relaciona con Sale (uno a muchos)
- Se relaciona con ClientAddress (uno a muchos)

**Supplier (Proveedor)**
- Proveedores de la empresa
- Se relaciona con Person (uno a uno)
- Se relaciona con Purchase (uno a muchos)
- Se relaciona con SupplierContact (uno a muchos)

**Person (Persona)**
- Información personal compartida (Client/Supplier/Employee)
- Se relaciona con Client (uno a uno)
- Se relaciona con Supplier (uno a uno)
- Se relaciona con Employee (uno a uno)

### 3.3 Módulo de Inventario

**Category (Categoría)**
- Categorización de productos
- Se relaciona con Product (uno a muchos)
- Soporta jerarquía (categoría padre)

**Product (Producto)**
- Productos del catálogo
- Se relaciona con Category (muchos a uno)
- Se relaciona con ProductSupplier (muchos a muchos)
- Se relaciona con Inventory (uno a uno)
- Se relaciona con SaleDetail (uno a muchos)
- Se relaciona con PurchaseDetail (uno a muchos)

**ProductSupplier (Producto-Proveedor)**
- Relación entre productos y sus proveedores
- Permite múltiples proveedores por producto
- Almacena precio de compra y tiempo de entrega

**Inventory (Inventario)**
- Stock actual de cada producto
- Se relaciona con Product (uno a uno)
- Se relaciona con Warehouse (muchos a uno)
- Se relaciona con InventoryMovement (uno a muchos)

**Warehouse (Almacén)**
- Ubicaciones de almacenamiento
- Se relaciona con Inventory (uno a muchos)
- Se relaciona con InventoryMovement (uno a muchos)

**InventoryMovement (Movimiento de Inventario)**
- Registra entradas y salidas de stock
- Se relaciona con Product (muchos a uno)
- Se relaciona con Warehouse (muchos a uno)
- Se relaciona con User (quien realiza el movimiento)
- Se relaciona con Purchase/Sale (opcional, para trazabilidad)

### 3.4 Módulo de Comercio

**Purchase (Compra)**
- Registro de compras a proveedores
- Se relaciona con Supplier (muchos a uno)
- Se relaciona con PurchaseDetail (uno a muchos)
- Se relaciona con PurchaseOrder (uno a uno)
- Se relaciona con User (quien registra)

**PurchaseDetail (Detalle de Compra)**
- Líneas de detalle de una compra
- Se relaciona con Purchase (muchos a uno)
- Se relaciona con Product (muchos a uno)

**PurchaseOrder (Orden de Compra)**
- Orden previa a la compra
- Se relaciona con Supplier (muchos a uno)
- Se relaciona con PurchaseOrderDetail (uno a muchos)
- Estados: Pendiente, Aprobada, Recibida, Cancelada

**PurchaseOrderDetail (Detalle de Orden de Compra)**
- Líneas de detalle de una orden de compra
- Se relaciona con PurchaseOrder (muchos a uno)
- Se relaciona con Product (muchos a uno)

**Sale (Venta)**
- Registro de ventas a clientes
- Se relaciona con Client (muchos a uno)
- Se relaciona con SaleDetail (uno a muchos)
- Se relaciona con Invoice (uno a uno)
- Se relaciona con User (quien registra)

**SaleDetail (Detalle de Venta)**
- Líneas de detalle de una venta
- Se relaciona con Sale (muchos a uno)
- Se relaciona con Product (muchos a uno)

**Order (Orden)**
- Órdenes de compra/venta pendientes
- Se relaciona con Client/Supplier (depende del tipo)
- Estados: Pendiente, Confirmada, En Proceso, Completada, Cancelada

**OrderDetail (Detalle de Orden)**
- Líneas de detalle de una orden
- Se relaciona con Order (muchos a uno)
- Se relaciona con Product (muchos a uno)

### 3.5 Módulo de Facturación

**Invoice (Factura)**
- Documento fiscal de venta
- Se relaciona con Sale (uno a uno)
- Se relaciona con InvoiceDetail (uno a_many)
- Estados: Emitida, Pagada, Anulada

**InvoiceDetail (Detalle de Factura)**
- Líneas de detalle de una factura
- Se relaciona con Invoice (muchos a uno)
- Se relaciona con Product (muchos a uno)

**Payment (Pago)**
- Registro de pagos recibidos
- Se relaciona con Invoice (muchos a uno)
- Se relaciona con User (quien registra)

### 3.6 Módulo de Inteligencia

**DashboardMetric (Métrica del Dashboard)**
- Almacena métricas precalculadas
- Soporta diferentes períodos
- Se actualiza periódicamente o bajo demanda

**Report (Reporte)**
- Definición de reportes guardados
- Se relaciona con User (quien creó el reporte)
- Almacena parámetros de filtrado

## 4. Reglas de Negocio Principales

### 4.1 Seguridad
- Un usuario puede tener múltiples roles
- Un rol puede ser asignado a múltiples usuarios
- Los refresh tokens expirados se eliminan periódicamente
- Todo cambio crítico genera un registro de auditoría
- Las contraseñas se almacenan hasheadas (BCrypt)
- Los JWT tokens tienen un tiempo de vida corto (15 min)
- Los refresh tokens tienen un tiempo de vida mayor (7 días)

### 4.2 Inventario
- Un producto debe tener al menos una categoría
- El stock no puede ser negativo (restricción a nivel de DB)
- Todo movimiento de inventario debe ser registrado
- Los movimientos de inventario son inmutables (no se editan ni eliminan)
- Al registrar una compra, se incrementa el stock
- Al registrar una venta, se decrementa el stock
- Si el stock es insuficiente, la venta no se puede procesar

### 4.3 Compras
- Una compra debe tener al menos un detalle
- El total de la compra se calcula automáticamente (suma de detalles)
- Al confirmar una compra, se genera un movimiento de entrada en inventario
- Las compras pueden estar asociadas a una orden de compra previa

### 4.4 Ventas
- Una venta debe tener al menos un detalle
- El total de la venta se calcula automáticamente
- Al confirmar una venta, se genera un movimiento de salida en inventario
- La venta genera automáticamente una factura
- Los precios de venta deben ser mayores o iguales al precio de compra
- Se valida el stock antes de procesar la venta

### 4.5 Facturación
- Una factura se genera automáticamente al confirmar una venta
- El número de factura es secuencial y único
- Las facturas anuladas no pueden ser modificadas
- Los pagos se registran contra una factura específica

### 4.6 Auditoría
- Se registran CREATE, UPDATE y DELETE de entidades críticas
- Se almacena el usuario, timestamp, IP y valores anteriores/nuevos
- Los registros de auditoría son inmutables

## 5. Flujos de Negocio Principales

### 5.1 Flujo de Compra
```
1. Crear Orden de Compra → Estado: Pendiente
2. Aprobar Orden de Compra → Estado: Aprobada
3. Registrar Recepción (Compra) → Estado: Recibida
4. Actualizar Inventario (entrada automática)
5. Generar Factura de Proveedor (opcional)
```

### 5.2 Flujo de Venta
```
1. Crear Venta (con detalles)
2. Validar Stock disponible
3. Confirmar Venta → Estado: Confirmada
4. Decrementar Inventario (salida automática)
5. Generar Factura de Cliente
6. Registrar Pago (parcial o total)
```

### 5.3 Flujo de Inventario
```
1. Entrada: Compra → Incrementar stock
2. Salida: Venta → Decrementar stock
3. Ajuste: Corrección manual de stock
4. Transferencia: Entre almacenes
5. Todo movimiento genera registro de auditoría
```

## 6. Restricciones de Integridad

### 6.1 Integridad de Dominio
- Email debe ser único y válido
- Número de documento (RFC/RIF) debe ser único
- Código de producto debe ser único
- Número de factura debe ser secuencial y único
- Stock no puede ser negativo
- Precios deben ser positivos
- Fechas de expiración deben ser futuras para tokens

### 6.2 Integridad Referencial
- Todas las Foreign Keys tienen ON DELETE apropiado
- Cascade: Solo para entidades dependientes (detalles)
- Restrict: Para entidades con transacciones
- Set NULL: Para relaciones opcionales

### 6.3 Integridad de Transacciones
- Crear venta + decrementar stock = Transacción
- Crear compra + incrementar stock = Transacción
- Confirmar orden + cambiar estado = Transacción
- Todo movimiento de inventario es atómico
