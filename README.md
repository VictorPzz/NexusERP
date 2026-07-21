# NexusERP

Sistema ERP completo para pequeñas y medianas empresas (PyMEs). Backend en ASP.NET Core, frontend en React, base de datos PostgreSQL, desplegable con Docker.

## Que es

NexusERP ayuda a manejar todo lo relacionado con la operacion de un negocio:

- **Clientes y Proveedores** - Crear, editar, buscar contactos con documentos, creditos y estados
- **Productos y Catalogos** - Categorias, bodegas, precios, IVA, control de stock
- **Ventas y Ordenes** - Crear ventas con detalle de productos, estados, numeracion automatica
- **Compras y Ordenes de Compra** - Gestionar compras a proveedores con seguimiento
- **Facturacion y Pagos** - Generar facturas, registrar pagos,控制 saldos pendientes
- **Dashboard y Reportes** - Resumen de metricas, ventas por periodo, inventario por categoria

## Stack Tecnologico

| Capa | Tecnologia |
|------|-----------|
| Backend | C# / ASP.NET Core 10 / Entity Framework Core |
| Frontend | React 19 / TypeScript / Vite / TailwindCSS |
| Base de datos | PostgreSQL 16 |
| Autenticacion | JWT + Refresh Tokens |
| Infraestructura | Docker + Docker Compose |
| Tests | xUnit + Moq + FluentAssertions |

## Como ejecutar

### Con Docker (recomendado)

```bash
docker compose up --build
```

- Frontend: http://localhost:3000
- API: http://localhost:8080
- Base de datos: localhost:5432

### Desarrollo local

**Backend:**
```bash
# Asegurarse de que PostgreSQL este corriendo
dotnet run --project src/NexusERP.Api --urls http://localhost:5000
```

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```

### Credenciales de prueba

| Campo | Valor |
|-------|-------|
| Email | admin@nexuserp.com |
| Contrasena | Admin123! |

## Estructura del proyecto

```
NexusERP/
├── src/
│   ├── NexusERP.Domain/           # Entidades, interfaces, reglas de negocio
│   ├── NexusERP.Application/      # Commands, Queries, DTOs, validaciones (MediatR CQRS)
│   ├── NexusERP.Infrastructure/   # EF Core, repositorios, servicios externos
│   └── NexusERP.Api/              # Controllers, middleware, configuracion
├── frontend/
│   └── src/
│       ├── components/            # DataTable, Modal (reutilizables)
│       ├── contexts/              # AuthContext (JWT)
│       ├── layouts/               # Layout con sidebar
│       ├── pages/                 # 14 paginas con CRUD completo
│       ├── services/              # API axios con interceptor JWT
│       └── types/                 # TypeScript interfaces
├── tests/
│   └── NexusERP.Tests/           # 37 tests unitarios
├── docs/                          # 9 documentos de arquitectura
├── docker/                        # Dockerfile para API
├── docker-compose.yml             # Stack completo: PostgreSQL + API + Frontend
└── docker-compose.dev.yml         # Solo PostgreSQL para desarrollo local
```

## Modulos implementados

| Modulo | Crear | Editar | Eliminar | Buscar | Paginacion |
|--------|-------|--------|----------|--------|-----------|
| Clientes | Si | Si | Soft delete | Si | Si |
| Proveedores | Si | Si | Soft delete | Si | Si |
| Productos | Si | Si | Soft delete | Si | Si |
| Categorias | Si | Si | - | - | Lista plana |
| Bodegas | Si | - | - | - | Lista plana |
| Ordenes | Si | Estado | - | Si | Si |
| Ventas | Si | Estado | - | - | Si |
| Ordenes de Compra | Si | Estado | - | - | Si |
| Compras | Si | Estado | - | - | Si |
| Facturas | Si | Estado | - | Filtro | Si |
| Pagos | Si | - | - | - | Si |
| Reportes | - | - | - | - | - |
| Dashboard | - | - | - | - | - |

## Arquitectura

El proyecto sigue **Clean Architecture** con 4 capas separadas:

```
Domain (reglas de negocio)
   ↑
Application (logica de uso - Commands/Queries con MediatR)
   ↑
Infrastructure (acceso a datos - EF Core + PostgreSQL)
   ↑
Api (HTTP - Controllers + JWT)
   ↑
Frontend (React + TypeScript)
```

**Patrones utilizados:**
- Repository Pattern (acceso a datos abstracto)
- Unit of Work (transacciones agrupadas)
- CQRS con MediatR (Commands separados de Queries)
- FluentValidation (validacion de datos)
- Result Pattern (manejo de errores sin excepciones)
- Soft Delete (eliminacion logica)

## Tests

37 tests unitarios cubriendo:
- Validadores de CreateInvoice, CreatePayment, CreateSale, CreatePurchaseOrder
- Handlers de CreateInvoice, CreatePayment, CreateSale
- Queries de GetInvoiceById con mapeo de Client/Supplier/Person

```bash
dotnet test tests/NexusERP.Tests/
```

## Documentacion

9 documentos de arquitectura en la carpeta `docs/`:

1. Analisis del Dominio
2. Diseno de Arquitectura
3. Diseno de Modulos
4. Diseno de Base de Datos (30 tablas)
5. Diagrama Entidad-Relacion
6. Reglas de Negocio
7. Estructura de Carpetas
8. Endpoints API (99 endpoints)
9. Roadmap Tecnico

## Licencia

Proyecto privado.
