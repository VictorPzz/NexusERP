# NexusERP - Reglas de Negocio Principales

## 1. Reglas de Seguridad

### 1.1 Autenticación
| ID | Regla | Prioridad |
|----|-------|-----------|
| SEC-001 | Las contraseñas se almacenan hasheadas con BCrypt (cost factor 12) | Alta |
| SEC-002 | Los JWT tokens tienen un tiempo de vida de 15 minutos | Alta |
| SEC-003 | Los refresh tokens tienen un tiempo de vida de 7 días | Alta |
| SEC-004 | Un refresh token solo puede usarse una vez (rotación de tokens) | Alta |
| SEC-005 | Los tokens revocados no pueden reutilizarse | Alta |
| SEC-006 | El email debe ser único en el sistema | Alta |
| SEC-007 | El username debe ser único en el sistema | Alta |

### 1.2 Autorización
| ID | Regla | Prioridad |
|----|-------|-----------|
| SEC-010 | Un usuario puede tener múltiples roles | Alta |
| SEC-011 | Un rol puede ser asignado a múltiples usuarios | Alta |
| SEC-012 | Los permisos se heredan del rol | Alta |
| SEC-013 | Solo un Admin puede crear usuarios | Alta |
| SEC-014 | Solo un Admin puede asignar roles | Alta |

### 1.3 Auditoría
| ID | Regla | Prioridad |
|----|-------|-----------|
| SEC-020 | Todo CREATE, UPDATE, DELETE genera un registro de auditoría | Alta |
| SEC-021 | Los registros de auditoría son inmutables | Alta |
| SEC-022 | Se almacena: usuario, acción, entidad, ID, valores anteriores/nuevos, IP, timestamp | Alta |
| SEC-023 | Los logs de auditoría no se pueden editar ni eliminar | Alta |

---

## 2. Reglas de Personas

### 2.1 Personas
| ID | Regla | Prioridad |
|----|-------|-----------|
| PPL-001 | El número de documento (document_type + document_number) debe ser único | Alta |
| PPL-002 | El email debe ser válido (formato) | Media |
| PPL-003 | El teléfono debe tener formato válido | Baja |
| PPL-004 | La fecha de nacimiento no puede ser futura | Media |

### 2.2 Empleados
| ID | Regla | Prioridad |
|----|-------|-----------|
| PPL-010 | Un empleado debe tener un job_position válido | Alta |
| PPL-011 | La fecha de contratación no puede ser futura | Alta |
| PPL-012 | La fecha de terminación debe ser posterior a la de contratación | Alta |
| PPL-013 | Un empleado puede tener opcionalmente un usuario asociado | Media |
| PPL-014 | Un usuario solo puede estar asociado a un empleado | Alta |
| PPL-015 | El employee_code debe ser único | Alta |

### 2.3 Clientes
| ID | Regla | Prioridad |
|----|-------|-----------|
| PPL-020 | Un cliente debe tener una persona asociada | Alta |
| PPL-021 | El client_code debe ser único | Alta |
| PPL-022 | El crédito no puede ser negativo | Alta |
| PPL-023 | El saldo actual no puede ser negativo | Alta |
| PPL-024 | Un cliente puede tener múltiples direcciones | Media |
| PPL-025 | Solo una dirección puede ser principal (is_primary = true) | Media |

### 2.4 Proveedores
| ID | Regla | Prioridad |
|----|-------|-----------|
| PPL-030 | Un proveedor debe tener una persona asociada | Alta |
| PPL-031 | El supplier_code debe ser único | Alta |
| PPL-032 | El tax_id debe ser único si se proporciona | Media |
| PPL-033 | La calificación debe estar entre 0 y 5 | Media |

---

## 3. Reglas de Inventario

### 3.1 Productos
| ID | Regla | Prioridad |
|----|-------|-----------|
| INV-001 | Un producto debe tener una categoría válida | Alta |
| INV-002 | El código del producto debe ser único | Alta |
| INV-003 | El precio de costo no puede ser negativo | Alta |
| INV-004 | El precio de venta no puede ser negativo | Alta |
| INV-005 | El precio de venta debe ser mayor o igual al precio de costo | Media |
| INV-006 | La tasa de impuesto debe estar entre 0% y 100% | Alta |
| INV-007 | El stock mínimo no puede ser negativo | Alta |
| INV-008 | El stock máximo debe ser mayor o igual al stock mínimo | Media |
| INV-009 | El código de barras debe ser único si se proporciona | Media |

### 3.2 Stock
| ID | Regla | Prioridad |
|----|-------|-----------|
| INV-010 | La cantidad en inventario no puede ser negativa | Crítica |
| INV-011 | La cantidad reservada no puede ser mayor que la cantidad disponible | Alta |
| INV-012 | Un producto puede tener stock en múltiples almacenes | Media |
| INV-013 | La combinación producto + almacén debe ser única | Alta |

### 3.3 Movimientos
| ID | Regla | Prioridad |
|----|-------|-----------|
| INV-020 | Todo movimiento de inventario debe ser registrado | Crítica |
| INV-021 | Los movimientos son inmutables (no se editan ni eliminan) | Crítica |
| INV-022 | La cantidad del movimiento debe ser mayor a 0 | Alta |
| INV-023 | El tipo de movimiento debe ser válido (entry, exit, adjustment, transfer_in, transfer_out) | Alta |
| INV-024 | Todo movimiento debe tener un usuario que lo realizó | Alta |

---

## 4. Reglas de Compras

### 4.1 Órdenes de Compra
| ID | Regla | Prioridad |
|----|-------|-----------|
| PUR-001 | El order_number debe ser único | Alta |
| PUR-002 | Debe tener al menos un detalle | Alta |
| PUR-003 | El supplier_id debe ser válido | Alta |
| PUR-004 | Los estados válidos son: pending, approved, received, cancelled | Alta |
| PUR-005 | Solo se puede aprobar si está en estado pending | Alta |
| PUR-006 | Solo se puede recibir si está en estado approved | Alta |
| PUR-007 | Solo se puede cancelar si está en estado pending o approved | Alta |
| PUR-008 | La fecha esperada debe ser posterior a la fecha de orden | Media |
| PUR-009 | Los totales se calculan automáticamente (suma de detalles) | Alta |

### 4.2 Compras
| ID | Regla | Prioridad |
|----|-------|-----------|
| PUR-020 | El purchase_number debe ser único | Alta |
| PUR-021 | Debe tener al menos un detalle | Alta |
| PUR-022 | Al confirmar una compra, se incrementa el stock automáticamente | Crítica |
| PUR-023 | La confirmación de compra y actualización de stock son atómicas (transacción) | Crítica |
| PUR-024 | Al cancelar una compra, no se modifica el stock | Alta |
| PUR-025 | Solo se puede cancelar si está en estado pending | Alta |

---

## 5. Reglas de Ventas

### 5.1 Ventas
| ID | Regla | Prioridad |
|----|-------|-----------|
| SAL-001 | El sale_number debe ser único | Alta |
| SAL-002 | Debe tener al menos un detalle | Alta |
| SAL-003 | El client_id debe ser válido | Alta |
| SAL-004 | Los estados válidos son: pending, confirmed, cancelled | Alta |
| SAL-005 | Al confirmar una venta, se decrementa el stock automáticamente | Crítica |
| SAL-006 | Se valida disponibilidad de stock antes de confirmar | Crítica |
| SAL-007 | La confirmación de venta y actualización de stock son atómicas (transacción) | Crítica |
| SAL-008 | Al cancelar una venta, se incrementa el stock (reversión) | Alta |
| SAL-009 | Solo se puede cancelar si está en estado pending | Alta |
| SAL-010 | Los precios de venta deben ser mayores o iguales al precio de compra | Media |
| SAL-011 | El descuento no puede ser mayor al subtotal | Alta |
| SAL-012 | El profit se calcula automáticamente (selling_price - cost_price) * quantity | Alta |

### 5.2 Órdenes
| ID | Regla | Prioridad |
|----|-------|-----------|
| SAL-020 | El order_number debe ser único | Alta |
| SAL-021 | Debe tener al menos un detalle | Alta |
| SAL-022 | Si es tipo 'sale', debe tener client_id | Alta |
| SAL-023 | Si es tipo 'purchase', debe tener supplier_id | Alta |
| SAL-024 | Los estados válidos son: pending, confirmed, processing, completed, cancelled | Alta |

---

## 6. Reglas de Facturación

### 6.1 Facturas
| ID | Regla | Prioridad |
|----|-------|-----------|
| BIL-001 | El invoice_number debe ser único y secuencial | Alta |
| BIL-002 | Una factura se genera automáticamente al confirmar una venta | Alta |
| BIL-003 | Los estados válidos son: issued, paid, overdue, cancelled | Alta |
| BIL-004 | El balance_due se calcula automáticamente (total - amount_paid) | Alta |
| BIL-005 | Las facturas anuladas no pueden ser modificadas | Alta |
| BIL-006 | El due_date debe ser posterior al invoice_date | Media |

### 6.2 Pagos
| ID | Regla | Prioridad |
|----|-------|-----------|
| BIL-010 | El payment_number debe ser único | Alta |
| BIL-011 | El monto del pago debe ser mayor a 0 | Alta |
| BIL-012 | El pago no puede exceder el balance_due de la factura | Alta |
| BIL-013 | Al registrar un pago, se actualiza amount_paid y balance_due | Alta |
| BIL-014 | Si amount_paid = total, la factura cambia a estado 'paid' | Alta |
| BIL-015 | Los métodos de pago válidos son: cash, card, transfer, check, other | Alta |

---

## 7. Reglas de Transacciones

### 7.1 Transacciones Críticas
Estas operaciones DEBEN ejecutarse dentro de una transacción:

| Operación | Tablas Afectadas | Acción |
|-----------|------------------|--------|
| Confirmar Venta | sales, sale_details, inventory, inventory_movements | Decrementar stock + registrar movimiento |
| Cancelar Venta | sales, inventory, inventory_movements | Incrementar stock + registrar movimiento |
| Confirmar Compra | purchases, purchase_details, inventory, inventory_movements | Incrementar stock + registrar movimiento |
| Registrar Pago | payments, invoices | Actualizar amount_paid + balance_due |
| Transferir Stock | inventory (x2), inventory_movements | Decrementar origen + incrementar destino |

### 7.2 Patrón de Transacción
```csharp
// Ejemplo: Confirmar Venta
public async Task<Result> ConfirmSaleAsync(int saleId)
{
    await _unitOfWork.BeginTransactionAsync();
    try
    {
        // 1. Obtener la venta con detalles
        var sale = await _unitOfWork.Sales.GetByIdWithDetailsAsync(saleId);
        
        // 2. Validar stock para cada detalle
        foreach (var detail in sale.Details)
        {
            var available = await _unitOfWork.Inventory
                .GetAvailableQuantityAsync(detail.ProductId, warehouseId);
            if (available < detail.Quantity)
                throw new InsufficientStockException(detail.ProductId);
        }
        
        // 3. Decrementar stock y registrar movimientos
        foreach (var detail in sale.Details)
        {
            await _unitOfWork.Inventory.DecrementStockAsync(
                detail.ProductId, warehouseId, detail.Quantity);
            
            await _unitOfWork.InventoryMovements.AddAsync(new InventoryMovement
            {
                ProductId = detail.ProductId,
                WarehouseId = warehouseId,
                MovementType = "exit",
                Quantity = detail.Quantity,
                ReferenceType = "Sale",
                ReferenceId = saleId
            });
        }
        
        // 4. Actualizar estado de la venta
        sale.Status = "confirmed";
        await _unitOfWork.SaveChangesAsync();
        
        // 5. Generar factura
        await _invoiceService.GenerateFromSaleAsync(saleId);
        
        await _unitOfWork.CommitTransactionAsync();
        return Result.Success();
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

---

## 8. Reglas de Validación

### 8.1 Validación en Capa de Presentación (FluentValidation)
- Formato de email
- Formato de teléfono
- Longitud de campos
- Campos requeridos
- Rangos numéricos

### 8.2 Validación en Capa de Dominio (Entidades)
- Reglas de negocio que involucran múltiples campos
- Invariantes de dominio
- Estados válidos

### 8.3 Validación en Capa de Aplicación (Services)
- Reglas de negocio que involucran múltiples entidades
- Verificación de disponibilidad
- Cálculos automáticos

---

## 9. Reglas de Cálculo

### 9.1 Totales de Venta
```
subtotal = SUM(quantity * unit_price) para cada detalle
tax_amount = SUM(subtotal * tax_rate / 100) para cada detalle
discount_amount = SUM(subtotal * discount_rate / 100) para cada detalle
total = subtotal + tax_amount - discount_amount
profit = SUM((unit_price - cost_price) * quantity) para cada detalle
```

### 9.2 Totales de Compra
```
subtotal = SUM(quantity * unit_price) para cada detalle
tax_amount = SUM(subtotal * tax_rate / 100) para cada detalle
total = subtotal + tax_amount
```

### 9.3 Totales de Factura
```
subtotal = SUM(subtotal) de los detalles
tax_amount = SUM(tax_amount) de los detalles
total = subtotal + tax_amount
balance_due = total - amount_paid
```

---

## 10. Estados de las Entidades

### 10.1 Diagrama de Estados - Venta
```
pending → confirmed → cancelled
   ↓
cancelled
```

### 10.2 Diagrama de Estados - Orden de Compra
```
pending → approved → received
   ↓          ↓
cancelled  cancelled
```

### 10.3 Diagrama de Estados - Factura
```
issued → paid
   ↓
overdue
   ↓
cancelled
```

### 10.4 Diagrama de Estados - Órdenes
```
pending → confirmed → processing → completed
   ↓          ↓           ↓
cancelled  cancelled   cancelled
```
