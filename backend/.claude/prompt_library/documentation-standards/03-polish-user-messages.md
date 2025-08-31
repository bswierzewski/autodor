### 🎯 RULE: Polish for User-Facing Messages

**Rule:** All messages directed at the end-user or used in logs for business analysis MUST be in Polish.

This applies to:
*   Error messages in business exceptions (`throw new BusinessRuleException("Nie można usunąć aktywnego zamówienia.");`)
*   Validation messages.
*   Logs at `Information`, `Warning`, and `Error` levels that describe business events.