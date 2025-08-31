### 🎯 RULE: API Simplicity

**Rule:** Methods in public API interfaces (`I[Module]Api.cs`) must have simple, concise, and intention-revealing names. All implementation complexity is hidden inside the handlers within the `Application` layer.

*   **GOOD:** `GetProductAsync()`, `CreateInvoiceAsync()`
*   **BAD:** `GetProductWithStockLevelAndPricingFromExternalSystemAsync()`, `ExecuteProductRetrievalWorkflowAsync()`