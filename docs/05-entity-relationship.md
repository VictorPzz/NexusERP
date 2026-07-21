# NexusERP - Diagrama Entidad-Relación y Relaciones entre Tablas

## Diagrama Entidad-Relación (Texto)

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              MÓDULO: SECURITY                                   │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌──────────────┐       ┌──────────────┐       ┌──────────────┐               │
│  │    users     │──1:N──│  user_roles  │──N:1──│    roles     │               │
│  │              │       │              │       │              │               │
│  │ id (PK)      │       │ id (PK)      │       │ id (PK)      │               │
│  │ username     │       │ user_id (FK) │       │ name         │               │
│  │ email        │       │ role_id (FK) │       │ description  │               │
│  │ password_hash│       │ assigned_by  │       │ is_active    │               │
│  │ is_active    │       └──────────────┘       └──────────────┘               │
│  │ ...          │                                                               │
│  └──────┬───────┘                                                               │
│         │                                                                       │
│         ├──1:N──┐                                                               │
│         │       │                                                               │
│         │  ┌────┴──────────┐    ┌──────────────┐                              │
│         │  │ refresh_tokens│    │  audit_logs  │                              │
│         │  │               │    │              │                              │
│         │  │ id (PK)       │    │ id (PK)      │                              │
│         │  │ user_id (FK)  │    │ user_id (FK) │                              │
│         │  │ token         │    │ action       │                              │
│         │  │ expires_at    │    │ entity_name  │                              │
│         │  │ is_revoked    │    │ entity_id    │                              │
│         │  └───────────────┘    │ old_values   │                              │
│         │                       │ new_values   │                              │
│         │                       └──────────────┘                              │
│         │                                                                       │
│         └──1:N──┐                                                               │
│                  │                                                               │
│         ┌────────────────────┐                                                  │
│         │system_configurations│                                                 │
│         │                    │                                                  │
│         │ id (PK)            │                                                  │
│         │ module             │                                                  │
│         │ key                │                                                  │
│         │ value              │                                                  │
│         └────────────────────┘                                                  │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                              MÓDULO: PEOPLE                                     │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌──────────────┐                                                               │
│  │   persons    │                                                               │
│  │              │                                                               │
│  │ id (PK)      │                                                               │
│  │ document_type│                                                               │
│  │ document_num │                                                               │
│  │ first_name   │                                                               │
│  │ last_name    │                                                               │
│  │ email        │                                                               │
│  │ phone        │                                                               │
│  └──────┬───────┘                                                               │
│         │                                                                       │
│         ├──1:1──┐                                                               │
│         │       │                                                               │
│         │  ┌────┴──────────┐    ┌──────────────┐                              │
│         │  │  employees    │    │   clients    │                              │
│         │  │               │    │              │                              │
│         │  │ id (PK)       │    │ id (PK)      │                              │
│         │  │ person_id(FK) │    │ person_id(FK)│                              │
│         │  │ employee_code │    │ client_code  │                              │
│         │  │ job_pos_id(FK)│    │ credit_limit │                              │
│         │  │ hire_date     │    │ balance      │                              │
│         │  │ user_id (FK)  │    │ status       │                              │
│         │  └───────┬───────┘    └──────┬───────┘                              │
│         │          │                    │                                       │
│         │          │                    ├──1:N──┐                              │
│         │          │                    │       │                              │
│         │          │                    │  ┌────┴────────────┐                │
│         │          │                    │  │client_addresses │                │
│         │          │                    │  │                 │                │
│         │          │                    │  │ id (PK)         │                │
│         │          │                    │  │ client_id (FK)  │                │
│         │          │                    │  │ address_type    │                │
│         │          │                    │  │ street          │                │
│         │          │                    │  │ city            │                │
│         │          │                    │  └─────────────────┘                │
│         │          │                    │                                       │
│         │          ├──1:1──┐            │                                       │
│         │          │       │            │                                       │
│         │          │  ┌────┴──────────┐ │                                       │
│         │          │  │emp_contacts   │ │                                       │
│         │          │  │               │ │                                       │
│         │          │  │ id (PK)       │ │                                       │
│         │          │  │ employee_id   │ │                                       │
│         │          │  │ contact_type  │ │                                       │
│         │          │  │ contact_value │ │                                       │
│         │          │  └───────────────┘ │                                       │
│         │          │                    │                                       │
│         │          ├──N:1──┐            │                                       │
│         │          │       │            │                                       │
│         │          │  ┌────┴──────────┐ │                                       │
│         │          │  │job_positions  │ │                                       │
│         │          │  │               │ │                                       │
│         │          │  │ id (PK)       │ │                                       │
│         │          │  │ name          │ │                                       │
│         │          │  │ dept_id (FK)  │ │                                       │
│         │          │  └───────┬───────┘ │                                       │
│         │          │          │         │                                       │
│         │          │          │N:1      │                                       │
│         │          │          │         │                                       │
│         │          │  ┌───────┴───────┐ │                                       │
│         │          │  │ departments   │ │                                       │
│         │          │  │               │ │                                       │
│         │          │  │ id (PK)       │ │                                       │
│         │          │  │ name          │ │                                       │
│         │          │  │ parent_id(FK) │ │                                       │
│         │          │  │ (self-ref)    │ │                                       │
│         │          │  └───────────────┘ │                                       │
│         │          │                    │                                       │
│         │          └──1:N──┐            │                                       │
│         │                  │            │                                       │
│         │           ┌──────┴───────┐    │                                       │
│         │           │  suppliers   │    │                                       │
│         │           │              │    │                                       │
│         │           │ id (PK)      │    │                                       │
│         │           │ person_id(FK)│    │                                       │
│         │           │ supplier_code│    │                                       │
│         │           │ company_name │    │                                       │
│         │           │ tax_id       │    │                                       │
│         │           └──────┬───────┘    │                                       │
│         │                  │            │                                       │
│         │                  ├──1:N──┐    │                                       │
│         │                  │       │    │                                       │
│         │                  │  ┌────┴─────────────┐                             │
│         │                  │  │supplier_contacts │                             │
│         │                  │  │                  │                             │
│         │                  │  │ id (PK)          │                             │
│         │                  │  │ supplier_id (FK) │                             │
│         │                  │  │ contact_type     │                             │
│         │                  │  │ contact_value    │                             │
│         │                  │  └──────────────────┘                             │
│         │                  │                                                   │
│         └──────────────────┴───────────────────────────────────────────────────┘
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                              MÓDULO: INVENTORY                                  │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌──────────────┐                                                               │
│  │  categories  │                                                               │
│  │              │                                                               │
│  │ id (PK)      │                                                               │
│  │ name         │                                                               │
│  │ parent_id(FK)│──(self-reference)                                             │
│  └──────┬───────┘                                                               │
│         │                                                                       │
│         │1:N                                                                    │
│         │                                                                       │
│         ▼                                                                       │
│  ┌──────────────┐                                                               │
│  │   products   │                                                               │
│  │              │                                                               │
│  │ id (PK)      │                                                               │
│  │ code         │                                                               │
│  │ name         │                                                               │
│  │ category_id  │──(FK → categories)                                            │
│  │ cost_price   │                                                               │
│  │ selling_price│                                                               │
│  └──────┬───────┘                                                               │
│         │                                                                       │
│         ├──1:1──┐                                                               │
│         │       │                                                               │
│         │  ┌────┴──────────┐    ┌──────────────┐                              │
│         │  │  inventory    │    │  warehouses  │                              │
│         │  │               │    │              │                              │
│         │  │ id (PK)       │    │ id (PK)      │                              │
│         │  │ product_id(FK)│    │ name         │                              │
│         │  │ warehouse_id  │──N:1│ code         │                              │
│         │  │ quantity      │    │ manager_id   │                              │
│         │  │ reserved_qty  │    └──────────────┘                              │
│         │  └───────────────┘                                                   │
│         │                                                                       │
│         ├──M:N──┐                                                               │
│         │       │                                                               │
│         │  ┌────┴──────────────┐    ┌──────────────┐                          │
│         │  │ product_suppliers │    │  suppliers   │                          │
│         │  │                   │    │              │                          │
│         │  │ id (PK)           │    │ id (PK)      │                          │
│         │  │ product_id (FK)   │    │ ...          │                          │
│         │  │ supplier_id (FK)──│──N:1│              │                          │
│         │  │ supplier_price    │    └──────────────┘                          │
│         │  │ lead_time_days    │                                               │
│         │  └───────────────────┘                                               │
│         │                                                                       │
│         └──1:N──┐                                                               │
│                  │                                                               │
│         ┌────────┴─────────────┐                                                │
│         │ inventory_movements  │                                                │
│         │                      │                                                │
│         │ id (PK)              │                                                │
│         │ product_id (FK)      │                                                │
│         │ warehouse_id (FK)    │                                                │
│         │ movement_type        │                                                │
│         │ quantity             │                                                │
│         │ performed_by (FK→users)                                               │
│         └──────────────────────┘                                                │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                              MÓDULO: PURCHASES                                  │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌──────────────────┐    ┌───────────────────────┐                            │
│  │ purchase_orders  │    │purchase_order_details  │                            │
│  │                  │    │                       │                            │
│  │ id (PK)          │──1:N│ id (PK)               │                            │
│  │ order_number     │    │ purchase_order_id (FK) │                            │
│  │ supplier_id (FK) │    │ product_id (FK)        │                            │
│  │ status           │    │ quantity               │                            │
│  │ total            │    │ unit_price             │                            │
│  └────────┬─────────┘    └───────────────────────┘                            │
│           │                                                                     │
│           │1:1                                                                  │
│           │                                                                     │
│  ┌────────┴─────────┐    ┌───────────────────────┐                            │
│  │    purchases     │    │   purchase_details    │                            │
│  │                  │    │                       │                            │
│  │ id (PK)          │──1:N│ id (PK)               │                            │
│  │ purchase_number  │    │ purchase_id (FK)       │                            │
│  │ supplier_id (FK) │    │ product_id (FK)        │                            │
│  │ purchase_order_id│    │ quantity               │                            │
│  │ total            │    │ unit_price             │                            │
│  └──────────────────┘    └───────────────────────┘                            │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                              MÓDULO: SALES                                      │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌──────────────┐    ┌──────────────────┐                                     │
│  │    orders    │    │  order_details   │                                     │
│  │              │    │                  │                                     │
│  │ id (PK)      │──1:N│ id (PK)          │                                     │
│  │ order_number │    │ order_id (FK)    │                                     │
│  │ order_type   │    │ product_id (FK)  │                                     │
│  │ client_id(FK)│    │ quantity         │                                     │
│  │ supplier_id  │    │ unit_price       │                                     │
│  │ status       │    └──────────────────┘                                     │
│  │ total        │                                                               │
│  └──────────────┘                                                               │
│                                                                                 │
│  ┌──────────────┐    ┌──────────────────┐                                     │
│  │    sales     │    │   sale_details   │                                     │
│  │              │    │                  │                                     │
│  │ id (PK)      │──1:N│ id (PK)          │                                     │
│  │ sale_number  │    │ sale_id (FK)     │                                     │
│  │ client_id(FK)│    │ product_id (FK)  │                                     │
│  │ status       │    │ quantity         │                                     │
│  │ total        │    │ unit_price       │                                     │
│  └──────────────┘    │ cost_price       │                                     │
│                      │ profit           │                                     │
│                      └──────────────────┘                                     │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                              MÓDULO: BILLING                                    │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌──────────────┐    ┌──────────────────┐    ┌──────────────┐                │
│  │   invoices   │    │ invoice_details  │    │   payments   │                │
│  │              │    │                  │    │              │                │
│  │ id (PK)      │──1:N│ id (PK)          │    │ id (PK)      │                │
│  │ invoice_num  │    │ invoice_id (FK)  │    │ payment_num  │                │
│  │ sale_id (FK) │    │ product_id (FK)  │    │ invoice_id   │──N:1            │
│  │ client_id(FK)│    │ quantity         │    │ amount       │                │
│  │ total        │    │ unit_price       │    │ payment_method│                │
│  │ amount_paid  │    │ total            │    │ payment_date │                │
│  │ balance_due  │    └──────────────────┘    └──────────────┘                │
│  └──────────────┘                                                               │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## Tabla Resumen de Relaciones

### Relaciones Uno a Uno (1:1)

| Entidad A | Entidad B | FK | ON DELETE | Justificación |
|-----------|-----------|-----|-----------|---------------|
| persons | clients | clients.person_id | RESTRICT | Una persona puede ser un cliente |
| persons | suppliers | suppliers.person_id | RESTRICT | Una persona puede ser un proveedor |
| persons | employees | employees.person_id | RESTRICT | Una persona puede ser un empleado |
| employees | employees.user_id | users.id | SET NULL | Un empleado puede tener usuario |
| products | inventory | inventory.product_id | RESTRICT | Un producto tiene stock por almacén |
| sales | invoices | invoices.sale_id | SET NULL | Una venta genera una factura |
| purchases | invoices | invoices.purchase_id | SET NULL | Una compra puede tener factura |

### Relaciones Uno a Muchos (1:N)

| Entidad Padre | Entidad Hijo | FK | ON DELETE | Justificación |
|---------------|--------------|-----|-----------|---------------|
| users | user_roles | user_roles.user_id | CASCADE | Un usuario tiene muchos roles |
| roles | user_roles | user_roles.role_id | CASCADE | Un rol se asigna a muchos usuarios |
| users | refresh_tokens | refresh_tokens.user_id | CASCADE | Un usuario tiene muchos tokens |
| users | audit_logs | audit_logs.user_id | SET NULL | Un usuario genera muchos logs |
| users | sales | sales.created_by | RESTRICT | Un usuario registra muchas ventas |
| departments | employees | employees.job_position_id | RESTRICT | Un departamento tiene muchos empleados |
| job_positions | employees | employees.job_position_id | RESTRICT | Un cargo tiene muchos empleados |
| persons | employee_contacts | employee_contacts.employee_id | CASCADE | Un empleado tiene muchos contactos |
| clients | client_addresses | client_addresses.client_id | CASCADE | Un cliente tiene muchas direcciones |
| suppliers | supplier_contacts | supplier_contacts.supplier_id | CASCADE | Un proveedor tiene muchos contactos |
| categories | products | products.category_id | RESTRICT | Una categoría tiene muchos productos |
| categories | categories (self) | categories.parent_id | SET NULL | Categoría padre |
| warehouses | inventory | inventory.warehouse_id | RESTRICT | Un almacén tiene mucho stock |
| products | inventory_movements | inventory_movements.product_id | RESTRICT | Un producto tiene muchos movimientos |
| warehouses | inventory_movements | inventory_movements.warehouse_id | RESTRICT | Un almacén tiene muchos movimientos |
| suppliers | purchase_orders | purchase_orders.supplier_id | RESTRICT | Un proveedor tiene muchas órdenes |
| purchase_orders | purchase_order_details | purchase_order_details.purchase_order_id | CASCADE | Una orden tiene muchos detalles |
| suppliers | purchases | purchases.supplier_id | RESTRICT | Un proveedor tiene muchas compras |
| purchases | purchase_details | purchase_details.purchase_id | CASCADE | Una compra tiene muchos detalles |
| clients | orders | orders.client_id | SET NULL | Un cliente tiene muchas órdenes |
| suppliers | orders | orders.supplier_id | SET NULL | Un proveedor tiene muchas órdenes |
| orders | order_details | order_details.order_id | CASCADE | Una orden tiene muchos detalles |
| clients | sales | sales.client_id | RESTRICT | Un cliente tiene muchas ventas |
| sales | sale_details | sale_details.sale_id | CASCADE | Una venta tiene muchos detalles |
| invoices | invoice_details | invoice_details.invoice_id | CASCADE | Una factura tiene muchos detalles |
| invoices | payments | payments.invoice_id | RESTRICT | Una factura tiene muchos pagos |

### Relaciones Muchos a Muchos (M:N) - Tabla Intermedia

| Entidad A | Entidad B | Tabla Intermedia | FKs | Justificación |
|-----------|-----------|------------------|-----|---------------|
| products | suppliers | product_suppliers | product_id, supplier_id | Un producto tiene muchos proveedores, un proveedor tiene muchos productos |

### Relaciones Self-Reference

| Entidad | Campo FK | ON DELETE | Justificación |
|---------|----------|-----------|---------------|
| departments | parent_id | SET NULL | Jerarquía de departamentos |
| categories | parent_id | SET NULL | Jerarquía de categorías |

---

## Diagrama de Cardinalidades

```
users ──────< user_roles >────── roles
  │                                   │
  │ 1:N                               │ 1:N
  │                                   │
  ├──< refresh_tokens                 │
  ├──< audit_logs                     │
  ├──< sales (created_by)             │
  ├──< purchases (created_by)         │
  └──< inventory_movements            │
                                      │
persons ────< clients ────< client_addresses
  │              │
  │ 1:1          │ 1:N
  │              │
  ├──< employees ──< job_positions ──< departments
  │              │                      │
  │ 1:1          │                      │ self-ref
  │              │                      │
  ├──< suppliers ──< supplier_contacts  │
                    │                   │
                    │ 1:N               │
                    │                   │
                    └──< product_suppliers >── products ──< categories
                                          │                      │
                                          │ 1:1                  │ self-ref
                                          │                      │
                                     < inventory >                │
                                          │                      │
                                          │ N:1                  │
                                          │                      │
                                     warehouses                  │
                                                                  │
                                     products ──< inventory_movements
                                                                  │
                                                                  │
                                     purchase_orders ──< purchase_order_details
                                          │
                                          │ 1:1
                                          │
                                     purchases ──< purchase_details
                                          │
                                          │
                                     invoices ──< invoice_details
                                          │
                                          │ 1:N
                                          │
                                     < payments
```

---

## Restricciones de Integridad Referencial

### ON DELETE CASCADE
- **user_roles**: Si se elimina un usuario o rol, se eliminan las asignaciones
- **refresh_tokens**: Si se elimina un usuario, se eliminan sus tokens
- **client_addresses**: Si se elimina un cliente, se eliminan sus direcciones
- **supplier_contacts**: Si se elimina un proveedor, se eliminan sus contactos
- **employee_contacts**: Si se elimina un empleado, se eliminan sus contactos
- **purchase_order_details**: Si se elimina una orden, se eliminan sus detalles
- **purchase_details**: Si se elimina una compra, se eliminan sus detalles
- **order_details**: Si se elimina una orden, se eliminan sus detalles
- **sale_details**: Si se elimina una venta, se eliminan sus detalles
- **invoice_details**: Si se elimina una factura, se eliminan sus detalles
- **product_suppliers**: Si se elimina un producto o proveedor, se elimina la relación

### ON DELETE RESTRICT
- **employees.person_id**: No se puede eliminar una persona que es empleado
- **clients.person_id**: No se puede eliminar una persona que es cliente
- **suppliers.person_id**: No se puede eliminar una persona que es proveedor
- **products.category_id**: No se puede eliminar una categoría con productos
- **inventory.product_id**: No se puede eliminar un producto con inventario
- **purchases.supplier_id**: No se puede eliminar un proveedor con compras
- **sales.client_id**: No se puede eliminar un cliente con ventas
- **invoices.sale_id**: No se puede eliminar una venta con factura
- **payments.invoice_id**: No se puede eliminar una factura con pagos

### ON DELETE SET NULL
- **employees.user_id**: Si se elimina un usuario, el empleado pierde la referencia
- **audit_logs.user_id**: Si se elimina un usuario, el log conserva la referencia NULL
- **categories.parent_id**: Si se elimina una categoría padre, las hijas quedan huérfanas
- **departments.parent_id**: Si se elimina un departamento padre, los hijos quedan huérfanos
- **orders.client_id**: Si se elimina un cliente, la orden conserva la referencia NULL
- **orders.supplier_id**: Si se elimina un proveedor, la orden conserva la referencia NULL
