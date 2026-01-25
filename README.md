# Struktura projektu Autodor

> Struktura poniżej znajduje się w katalogu `backend/`

```
src/
├── Aspire/
│   ├── Autodor.AppHost/
│   │   ├── Extensions/
│   └── Autodor.ServiceDefaults/
├── Host/
│   └── Autodor.API/
├── Modules/
│   ├── Orders/
│   │   ├── Autodor.Modules.Orders/
│   │   │   ├── Domain/
│   │   │   │   ├── Aggregates/
│   │   │   │   │   ├── ExcludedOrder.cs
│   │   │   │   │   └── ExcludedOrderPosition.cs
│   │   │   │   ├── Entities/
│   │   │   │   └── ValueObjects/
│   │   │   │   │   ├── Order.cs
│   │   │   │   │   └── Product.cs
│   │   │   ├── Features/
│   │   │   │   ├── GetOrders/
│   │   │   │   ├── GetOrder/
│   │   │   │   ├── ExcludeOrder/
│   │   │   │   ├── ExcludeOrderPosition/
│   │   │   │   ├── GenerateWarehouseDocument/
│   │   │   │   └── SyncProducts/
│   │   │   ├── Infrastructure/
│   │   │   │   ├── BackgroundJobs/
│   │   │   │   ├── ExternalServices/
│   │   │   │   │   ├── DistributorsSales/
│   │   │   │   │   │   ├── Generated/
│   │   │   │   │   │   ├── Models/
│   │   │   │   │   │   └── Options/
│   │   │   │   │   └── Products/
│   │   │   │   │       ├── Generated/
│   │   │   │   │       ├── Models/
│   │   │   │   │       └── Options/
│   │   │   │   ├── Persistence/
│   │   │   │   └── InternalServices/
│   │   └── Autodot.Modules.Orders.Contracts/
│   │       ├── Abstractions/
│   │       ├── Events/
│   │       └── Models/
│   ├── Invoicing/
│   │   ├── Autodor.Modules.Invoicing/
│   │   │   ├── Domain/
│   │   │   │   ├── Aggregates/
│   │   │   │   ├── Entities/
│   │   │   │   └── ValueObjects/
│   │   │   ├── Features/
│   │   │   │   ├── CreateInvoice/
│   │   │   │   └── BulkCreateInvoices/
│   │   │   ├── Infrastructure/
│   │   │   │   ├── ExternalServices/
│   │   │   │   │   ├── Abstractions/
│   │   │   │   │   │   └── IInvoiceService.cs
│   │   │   │   │   ├── InvoiceProviderFactory.cs
│   │   │   │   │   ├── Ifirma/
│   │   │   │   │   │   ├── Models/
│   │   │   │   │   │   ├── Options/
│   │   │   │   │   │   └── IfirmaInvoiceService.cs
│   │   │   │   │   └── Infakt/
│   │   │   │   │       ├── Models/
│   │   │   │   │       ├── Options/
│   │   │   │   │       └── InfaktInvoiceService.cs
│   │   │   │   ├── Persistence/
│   │   │   │   └── InternalServices/
│   │   └── Autodor.Modules.Invoicing.Contracts/
│   │       ├── Abstractions/
│   │       ├── Events/
│   │       └── Models/
│   └── Contractors/
│       ├── Autodor.Modules.Contractors/
│       │   ├── Domain/
│       │   │   ├── Aggregates/
│       │   │   ├── Entities/
│       │   │   └── ValueObjects/
│       │   ├── Features/
│       │   │   ├── CreateContractor/
│       │   │   ├── GetContractor/
│       │   │   ├── GetContractors/
│       │   │   ├── UpdateContractor/
│       │   │   └── DeleteContractor/
│       │   ├── Infrastructure/
│       │   │   ├── Persistence/
│       │   │   └── InternalServices/
│       └── Autodor.Modules.Contractors.Contracts/
│           ├── Abstractions/
│           ├── Events/
│           └── Models/
└── BuildingBlocks/
    ├── BuildingBlocks.Infrastructure/
    │   ├── Exceptions/
    │   │   └── Handlers/
    │   ├── Extensions/
    │   ├── Middleware/
    │   ├── Models/
    │   └── Persistence/
    │       └── Interceptors/
    └── BuildingBlocks.Kernel/
        ├── Abstractions/
        ├── Attributes/
        ├── Exceptions/
        ├── Extensions/
        └── Primitives/
tests/
├── Shared/
│   ├── Fixtures/
│   ├── Builders/
│   └── Extensions/
└── Modules/
    ├── Orders/
    │   └── Autodor.Modules.Orders.IntegrationTests/
    │       └── Features/
    │           ├── GetOrders/
    │           ├── GetOrder/
    │           ├── ExcludeOrder/
    │           ├── ExcludeOrderPosition/
    │           ├── GenerateWarehouseDocument/
    │           └── SyncProducts/
    ├── Invoicing/
    │   └── Autodor.Modules.Invoicing.IntegrationTests/
    │       └── Features/
    │           ├── CreateInvoice/
    │           └── BulkCreateInvoices/
    └── Contractors/
        └── Autodor.Modules.Contractors.IntegrationTests/
            └── Features/
                ├── CreateContractor/
                ├── GetContractor/
                ├── GetContractors/
                ├── UpdateContractor/
                └── DeleteContractor/
```

## Opis głównych obszarów

- **src/Aspire** - Konfiguracja orchestration usług
- **src/Host** - Host API głównej aplikacji
- **src/Modules** - Moduły vertikalnych slices:
  - **Orders** - Moduł zamówień (synchronizacja, excludowanie, drukowanie dokumentów magazynowych)
  - **Invoicing** - Moduł fakturowania (tworzenie pojedynczych i masowych faktur)
  - **Contractors** - Moduł wykonawców (CRUD operacje)
- **src/BuildingBlocks** - Kod wspólny (Building Blocks)
- **tests/** - Testy integracyjne (HTTP):
  - **Shared** - Wspólna infrastruktura testowa (Alba, Testcontainers, Respawn)
  - **Modules** - Testy integracyjne dla każdego modułu

> BuildingBlocks jest dodany jako submoduł git i jest dostępny pod tym adresem https://github.com/bswierzewski/building_blocks.git
