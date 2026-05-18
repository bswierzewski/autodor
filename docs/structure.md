# Struktura repozytorium

```
/
├─ .github/
│  └─ workflows/
│
├─ apps/
│  ├─ api/
│  │  ├─ Autodor.API.csproj
│  │  ├─ Program.cs
│  │  ├─ appsettings.json
│  │  ├─ appsettings.Development.json
│  │  ├─ Properties/
│  │  └─ ...
│  │
│  ├─ apphost/
│  │  ├─ Autodor.AppHost.csproj
│  │  ├─ AppHost.cs
│  │  ├─ appsettings.json
│  │  ├─ Properties/
│  │  └─ ...
│  │
│  ├─ web/
│  │  ├─ package.json
│  │  ├─ vite.config.ts
│  │  ├─ public/
│  │  ├─ src/
│  │  └─ ...
│
├─ backend/
│  ├─ building-blocks/           ← git submodule
│  │  ├─ BuildingBlocks.Core/
│  │  ├─ BuildingBlocks.Hosting/
│  │  ├─ BuildingBlocks.Infrastructure/
│  │  ├─ BuildingBlocks.Soap/
│  │  ├─ BuildingBlocks.Tests/
│  │  ├─ BuildingBlocks.slnx
│  │  ├─ Directory.Build.props
│  │  └─ Directory.Packages.props
│  │
│  └─ modules/
│     ├─ contractors/
│     │  ├─ Autodor.Modules.Contractors/
│     │  │  ├─ Autodor.Modules.Contractors.csproj
│     │  │  ├─ Domain/
│     │  │  ├─ Features/
│     │  │  ├─ Infrastructure/
│     │  │  └─ ...
│     │  └─ Autodor.Modules.Contractors.Contracts/
│     │     ├─ Autodor.Modules.Contractors.Contracts.csproj
│     │     └─ ...
│     │
│     ├─ errors/
│     │  ├─ Autodor.Modules.Errors/
│     │  │  ├─ Autodor.Modules.Errors.csproj
│     │  │  ├─ Domain/
│     │  │  ├─ Features/
│     │  │  ├─ Infrastructure/
│     │  │  └─ ...
│     │  └─ Autodor.Modules.Errors.Contracts/
│     │     ├─ Autodor.Modules.Errors.Contracts.csproj
│     │     └─ ...
│     │
│     ├─ invoicing/
│     │  ├─ Autodor.Modules.Invoicing/
│     │  │  ├─ Autodor.Modules.Invoicing.csproj
│     │  │  ├─ Domain/
│     │  │  ├─ Features/
│     │  │  ├─ Infrastructure/
│     │  │  └─ ...
│     │  └─ Autodor.Modules.Invoicing.Contracts/
│     │     ├─ Autodor.Modules.Invoicing.Contracts.csproj
│     │     └─ ...
│     │
│     └─ orders/
│        ├─ Autodor.Modules.Orders/
│        │  ├─ Domain/
│        │  ├─ Features/
│        │  ├─ Infrastructure/
│        │  ├─ Autodor.Modules.Orders.csproj
│        │  └─ ...
│        └─ Autodor.Modules.Orders.Contracts/
│           ├─ Autodor.Modules.Orders.Contracts.csproj
│           └─ ...
│
├─ docs/
│  ├─ structure.md
│  └─ ...
│
├─ openapi/
│  ├─ autodor-api.json
│  └─ clerk-backend-api.json
│
├─ scripts/
│  ├─ add-migrations.ps1
│  └─ ...
│
├─ tests/
│  ├─ integration/
│  │  ├─ Autodor.Tests.Integration.csproj
│  │  ├─ Shared/
│  │  ├─ Contractors/
│  │  ├─ Errors/
│  │  ├─ Invoicing/
│  │  ├─ Orders/
│  │  └─ ...
│
├─ Autodor.slnx
├─ Directory.Build.props
├─ Directory.Packages.props
├─ global.json
├─ aspire.config.json
└─ biome.json
```

## Zasady

- Foldery organizacyjne repozytorium mają nazwy linuksowe: `apps`, `backend`, `modules`, `tests`, `openapi`.
- Aplikacje uruchamialne są trzymane bezpośrednio w `apps/api` i `apps/apphost`, bez dodatkowego folderu z nazwą projektu.
- Moduły backendowe są grupowane domenowo w `backend/modules/<module>`, a bezpośrednio pod nimi znajdują się foldery projektów .NET.
- Testy modułów nie mają osobnych projektów; testy przekrojowe trafiają do `tests/integration`.
- Nazwy projektów .NET, plików `.csproj`, assembly i namespace'ów pozostają w konwencji .NET.
