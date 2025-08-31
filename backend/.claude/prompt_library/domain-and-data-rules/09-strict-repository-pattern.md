### 🎯 RULE: Strict Repository & Unit of Work

**Rule:** A strict Repository pattern must be followed.
*   Repositories are created only for Aggregate Roots.
*   Repository methods MUST NEVER return `IQueryable<T>`. They must always return materialized data (e.g., `Task<List<T>>` or `Task<T?>`).
*   All EF Core configuration is isolated in separate classes implementing `IEntityTypeConfiguration<T>`.
*   The `Unit of Work` (`SaveChangesAsync`) is called centrally (e.g., via a MediatR pipeline behavior), not from individual handlers.