### 🎯 RULE: The Navigator Pattern & Structure

**Rule:** The folder structure is the primary navigation tool. Strictly adhere to the following locations:
*   **Public API Contract**:
    *   Interface: `Shared.Contracts/[ModuleName]/I[Module]Api.cs`
    *   DTOs: `Shared.Contracts/[ModuleName]/Dtos/`
    *   Events: `Shared.Contracts/[ModuleName]/Events/`
*   **Internal Logic (CQRS)**:
    *   Location: `Application/[Domain]/[ActionName]/`
    *   Contents: `[ActionName]Query.cs`, `[ActionName]QueryHandler.cs`, etc.
    *   The folder name MUST match the action name.
*   **API Implementation (Adapter)**:
    *   Location: `Infrastructure/Api/[Module]Api.cs`