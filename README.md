# NexusERP - Documentación de Arquitectura (Fase 1)

## Resumen del Proyecto

**NexusERP** es un sistema ERP modular para PyMEs, diseñado con Clean Architecture, arquitectura modular y buenas prácticas de desarrollo de software empresarial.

**Stack Tecnológico:**
- Backend: ASP.NET Core 9 + Entity Framework Core
- Frontend: React + TypeScript + Vite + Tailwind CSS
- Base de datos: PostgreSQL
- Infraestructura: Docker + Docker Compose
- Autenticación: JWT + Refresh Tokens

---

## Documentos de la Fase 1

| # | Documento | Descripción |
|---|-----------|-------------|
| 1 | [Análisis del Dominio](docs/01-domain-analysis.md) | Subdominios, entidades, reglas de negocio, flujos |
| 2 | [Diseño de Arquitectura](docs/02-architecture-design.md) | Clean Architecture, capas, patrones, decisiones técnicas |
| 3 | [Diseño de Módulos](docs/03-modules-design.md) | Definición de cada módulo, interfaces, dependencias |
| 4 | [Diseño de Base de Datos](docs/04-database-design.md) | 30 tablas, columnas, tipos, restricciones, índices |
| 5 | [Diagrama Entidad-Relación](docs/05-entity-relationship.md) | Diagramas ER, cardinalidades, integridad referencial |
| 6 | [Reglas de Negocio](docs/06-business-rules.md) | Reglas por módulo, transacciones, estados, cálculos |
| 7 | [Estructura de Carpetas](docs/07-folder-structure.md) | Organización del proyecto por capas y módulos |
| 8 | [Endpoints API](docs/08-api-endpoints.md) | 99 endpoints REST documentados |
| 9 | [Roadmap Técnico](docs/09-roadmap.md) | 6 fases, estimaciones, dependencias, criterios |

---

## Arquitectura Resumida

```
┌─────────────────────────────────────────────────────┐
│                  NexusERP.sln                        │
├─────────────────────────────────────────────────────┤
│                                                      │
│  ┌──────────────┐                                   │
│  │  Api (Web)   │  ← Controllers, Middleware, DI    │
│  └──────┬───────┘                                   │
│         │                                            │
│  ┌──────┴───────┐                                   │
│  │ Application  │  ← Services, Commands, Queries    │
│  └──────┬───────┘                                   │
│         │                                            │
│  ┌──────┴───────┐                                   │
│  │   Domain     │  ← Entities, Interfaces, Rules    │
│  └──────┬───────┘                                   │
│         │                                            │
│  ┌──────┴───────┐                                   │
│  │Infrastructure│  ← EF Core, Repos, External Svc   │
│  └──────────────┘                                   │
│                                                      │
└─────────────────────────────────────────────────────┘
```

---

## Módulos del Sistema

| Módulo | Tablas | Endpoints | Entidades |
|--------|--------|-----------|-----------|
| Security | 5 | 18 | User, Role, UserRole, RefreshToken, AuditLog |
| People | 9 | 22 | Person, Employee, Department, JobPosition, Client, Supplier |
| Inventory | 5 | 10 | Category, Product, Warehouse, Inventory, InventoryMovement |
| Purchases | 4 | 11 | PurchaseOrder, Purchase, Details |
| Sales | 4 | 10 | Order, Sale, Details |
| Billing | 3 | 7 | Invoice, Payment, Details |
| Dashboard | - | 1 | Metrics |
| Reports | - | 4 | Sales, Purchases, Inventory, Client |
| Settings | 1 | 4 | SystemConfiguration |
| **Total** | **30** | **99** | |

---

## Base de Datos

- **30 tablas** en total
- **Relaciones**: 1:1, 1:N, M:N, Self-Reference
- **Integridad referencial**: CASCADE, RESTRICT, SET NULL
- **Índices**: Optimizados para consultas frecuentes
- **Soft delete**: Todas las entidades principales
- **Auditoría**: Tracking de cambios con valores anteriores/nuevos

---

## Decisiones Técnicas Clave

| Decisión | Alternativa Rechazada | Justificación |
|----------|----------------------|---------------|
| Clean Architecture | N-Capas | Mejor separación de concerns, testing |
| Monolito Modular | Microservicios | Simpleza para PyME, menos infraestructura |
| EF Core | Dapper | ORM nativo, migraciones, change tracking |
| FluentValidation | DataAnnotations | Más potente, separación de validaciones |
| Mappers manuales | AutoMapper | Más explícito, mejor rendimiento |
| MediatR | Servicios directos | CQRS ligero, pipeline behaviors |
| Result pattern | Excepciones | Más explícito, control de flujo funcional |

---

## Próximos Pasos

1. Crear repositorio en GitHub
2. Implementar Fase 2: Infraestructura Base
3. Configurar Docker Compose
4. Implementar Fase 3: Módulo de Seguridad
5. Continuar con las fases del roadmap

---

## Estadísticas del Diseño

- **Archivos de documentación**: 9
- **Tablas de base de datos**: 30
- **Endpoints API**: 99
- **Entidades de dominio**: ~35
- **Interfaces de repositorio**: ~20
- **Módulos**: 10
- **Reglas de negocio documentadas**: ~80
