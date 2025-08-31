---
description: Refactors a C# class according to best practices and project rules.
argument-hint: [path/to/class.cs]
---

<zadanie>
Your task is to refactor the C# class provided below. Your primary goals are to improve its **readability, maintainability, performance**, and ensure it adheres to **SOLID principles** and our project's specific architectural rules.

After refactoring, you must provide two things:

1.  The complete, refactored code for the class.
2.  A "Refactoring Summary" section below the code.

In the summary, you must create a list of the changes you made and, for each change, provide a clear justification explaining **why** you made it, referencing specific software principles (e.g., "Injected dependency X to adhere to the Dependency Inversion Principle") or project rules.
</zadanie>

<kontekst_szczegolowych_regul>
<regula nazwa="Rich Domain Model">

### 🎯 RULE: Rich Domain Model

**Rule:** Business logic, validation, and invariants MUST reside inside domain entities and aggregates. Properties must have `private` setters, and state is modified exclusively through public methods.
</regula>
<regula nazwa="Result Pattern for Business Errors">

### 🎯 RULE: Result Pattern for Business Errors

**Rule:** The application layer (CQRS handlers) MUST NOT throw exceptions for predictable business rule failures. Instead, they MUST return a generic `Result<T>` object.
</regula>
</kontekst_szczegolowych_regul>

<kod_do_refaktoryzacji>
@$1
</kod_do_refaktoryzacji>
