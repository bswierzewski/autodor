# Struktura repozytorium

```
/
├─ .github/
│  └─ workflows/
│
├─ apps/
│  ├─ api/
│  ├─ web/
│  └─ apphost/
│
├─ backend/
│  ├─ buildingblocks/            ← git submodule
│  │  ├─ BuildingBlocks.Core/
│  │  ├─ BuildingBlocks.Hosting/
│  │  ├─ BuildingBlocks.Infrastructure/
│  │  ├─ BuildingBlocks.Soap/
│  │  └─ BuildingBlocks.Tests/
│  └─ modules/
│     ├─ contractors/
│     │  ├─ Autodor.Modules.Contractors/
│     │  └─ Autodor.Modules.Contractors.Contracts/
│     ├─ errors/
│     ├─ invoicing/
│     │  ├─ Autodor.Modules.Invoicing/
│     │  └─ Autodor.Modules.Invoicing.Contracts/
│     └─ orders/
│        ├─ Autodor.Modules.Orders/
│        │  ├─ Domain/
│        │  ├─ Features/
│        │  └─ Infrastructure/
│        └─ Autodor.Modules.Orders.Contracts/
│
├─ docs/
├─ openapi/
├─ scripts/
└─ tests/
   ├─ Autodor.Tests.Integration/
   └─ Autodor.Tests.Integration/
```
