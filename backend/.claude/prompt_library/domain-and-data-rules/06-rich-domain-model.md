### 🎯 RULE: Rich Domain Model

**Rule:** Business logic, validation, and invariants MUST reside inside domain entities and aggregates.
*   Entity properties must have `private` or `protected` setters.
*   State is modified exclusively through public methods with business meaning (e.g., `Order.AddItem()`, `Order.Cancel()`).
*   Constructors and static factories must ensure that an entity is always created in a valid state.
*   Significant state changes MUST raise Domain Events.