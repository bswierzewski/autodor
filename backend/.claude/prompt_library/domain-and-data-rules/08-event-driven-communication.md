### 🎯 RULE: Event-Driven Module Communication

**Rule:** Modules MUST communicate in an asynchronous and loosely coupled manner. Direct service calls between modules are forbidden.
*   When a process in one module needs to trigger an action in another, it publishes an **Integration Event**.
*   These events are part of the public contract and are defined in `Shared.Contracts/[ModuleName]/Events/`.
*   Other modules subscribe to these events using dedicated handlers.