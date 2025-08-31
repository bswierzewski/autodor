### 🎯 RULE: Result Pattern for Business Errors

**Rule:** The application layer (CQRS handlers) MUST NOT throw exceptions for predictable business rule failures (e.g., "product out of stock," "credit limit exceeded").
Instead, these methods MUST return a generic `Result<T>` object, which contains either the successful value or a structured error object. Exceptions are reserved exclusively for unexpected, system-level failures (e.g., database connection lost).