<p align="center">
  <h1 align="center">🏗️ REARQ BIM</h1>
  <p align="center">
    <strong>Plataforma B2B de Economía Circular para el Sector de la Construcción</strong><br/>
    Integración BIM · Trazabilidad Ambiental · Cumplimiento PGI-RCD
  </p>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet" alt=".NET 8"/>
  <img src="https://img.shields.io/badge/Vue.js-3-4FC08D?style=flat-square&logo=vuedotjs&logoColor=white" alt="Vue 3"/>
  <img src="https://img.shields.io/badge/PostgreSQL-15-4169E1?style=flat-square&logo=postgresql&logoColor=white" alt="PostgreSQL"/>
  <img src="https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker&logoColor=white" alt="Docker"/>
  <img src="https://img.shields.io/badge/Licencia-MIT-green?style=flat-square" alt="License"/>
</p>

---

## 📋 Descripción

**REARQ BIM** es un ecosistema digital B2B que convierte los sobrantes y mermas de una obra de construcción en recursos utilizables para otra. La plataforma ofrece:

- **Fichas técnicas paramétricas** con especificaciones dimensionales exactas (mm), masa por unidad y estado de conservación.
- **Integración con modelos BIM** — Descarga de familias Revit (`.RFA`, `.RVT`) e IFC con nivel de detalle LOD 300+.
- **Trazabilidad ambiental auditable** — Certificados de desvío de RCD y reducción de huella de carbono (kg CO₂e) para informes PGI-RCD.

> 🇨🇴 Cumple con la **Resolución 1257 de 2021** (MinAmbiente) y la **Resolución 0472 de 2017** sobre gestión integral de RCD en Colombia.

---

## 🏛️ Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (Vue 3 SPA)                      │
│         Catálogo Técnico · Visor BIM · Dashboard RCD        │
└───────────────────────────┬─────────────────────────────────┘
                            │  HTTP / REST
┌───────────────────────────▼─────────────────────────────────┐
│              Backend (.NET 8 Web API — C#)                   │
│        Clean Architecture · JWT Auth · RBAC Multi-Tenant    │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                  PostgreSQL 15+ (Docker)                     │
│          Esquema relacional 3FN · uuid-ossp · pgcrypto      │
└─────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Stack Tecnológico

| Capa | Tecnología |
|:-----|:-----------|
| **Frontend** | Vue 3 (Composition API, `<script setup>`), Vite 5, Vue Router, Vanilla CSS |
| **Backend** | C# / ASP.NET Core 8 Web API, Clean Architecture |
| **Base de Datos** | PostgreSQL 15+ con extensiones `uuid-ossp` y `pgcrypto` |
| **ORM** | Entity Framework Core 8, Fluent API |
| **Contenedores** | Docker Compose (PostgreSQL) |
| **CI/CD** | GitLab CI/CD multi-etapa |

---

## 📂 Estructura del Proyecto

```
MVP/
├── backend/                    # API .NET 8
│   ├── RearqBim.Api/           # Controladores, Program.cs, configuración
│   ├── RearqBim.Core/          # Entidades de dominio y lógica de negocio
│   └── RearqBim.sln            # Solución de Visual Studio
│
├── frontend/                   # SPA Vue 3
│   ├── src/
│   │   ├── components/         # Componentes reutilizables (MaterialCard, etc.)
│   │   └── views/              # Vistas principales (TransactionsView, etc.)
│   ├── index.html
│   ├── vite.config.js
│   └── package.json
│
├── database/                   # Scripts SQL
│   ├── schema.sql              # DDL — Esquema de base de datos
│   └── seeds.sql               # Datos iniciales de prueba
│
├── docs/                       # Documentación del proyecto
│   ├── DOCUMENTACION_PROYECTO.md
│   └── GUIA_EJECUCION_Y_PRUEBAS_E2E.md
│
├── .gitlab-ci.yml              # Pipeline CI/CD
├── docker-compose.yml          # Infraestructura local (PostgreSQL)
└── .gitignore
```

---

## 🚀 Instalación y Ejecución Local

### Prerrequisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para PostgreSQL)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) y npm

### 1. Clonar el repositorio

```bash
git clone https://gitlab.com/tu-usuario/rearq-bim.git
cd rearq-bim
```

### 2. Levantar la base de datos

```bash
docker compose up -d
```

Esto crea un contenedor PostgreSQL 15 con el esquema y datos de prueba precargados automáticamente.

### 3. Ejecutar el Backend

```bash
cd backend/RearqBim.Api
dotnet restore
dotnet run
```

La API estará disponible en `https://localhost:5001` (o `http://localhost:5000`).

### 4. Ejecutar el Frontend

```bash
cd frontend
npm install
npm run dev
```

La aplicación estará disponible en `http://localhost:5173`.

---

## 📖 Documentación

| Documento | Descripción |
|:----------|:------------|
| [`DOCUMENTACION_PROYECTO.md`](docs/DOCUMENTACION_PROYECTO.md) | Documentación técnica y arquitectónica completa |
| [`GUIA_EJECUCION_Y_PRUEBAS_E2E.md`](docs/GUIA_EJECUCION_Y_PRUEBAS_E2E.md) | Guía de ejecución y pruebas end-to-end |

---

## 💰 Modelo de Negocio

- **Comisión transaccional:** 5% — 8% por lote de material transferido entre obras.
- **Suscripción SaaS:**
  - Plan Pro Project: $1.500.000 COP/mes (descargas BIM + reportes ambientales).
  - Plan Enterprise Corp: $3.000.000 COP/mes (multi-obra, API BIM, auditoría PGI-RCD).
- **SOM proyectado:** $13.200 millones COP.

---

## 👥 Equipo

- Isabella Gómez
- Martín Rivera
- Tomás Ramírez
- Samuel Zuleta
- Esteban Quiceno Loaiza

---

## 📄 Licencia

Este proyecto está bajo la licencia **MIT**. Consulta el archivo [LICENSE](LICENSE) para más detalles.

---

<p align="center">
  <sub>Desarrollado con ♻️ para un sector de la construcción más sostenible en Colombia.</sub>
</p>
