---
description: Generates comprehensive unit tests for a given C# class using xUnit and NSubstitute.
argument-hint: [path/to/class.cs]
---

<zadanie>
Your task is to create a comprehensive set of unit tests for the C# class provided below.

**Technical Requirements:**

- **Testing Framework:** Use **xUnit**.
- **Mocking Library:** Use **NSubstitute** for all dependencies.

**Test Coverage Requirements:**
Generate tests that cover the following categories:

1.  **Happy Path Tests:** Verify the correct behavior with valid inputs and standard conditions.
2.  **Edge Case Tests:** Test boundaries, nulls, empty collections, zero values, etc.
3.  **Sad Path / Error Handling Tests:** Verify that the code correctly handles invalid inputs, exceptions from dependencies, and failed business rules.

**Code Style Requirements:**

- Follow the `MethodName_Scenario_ExpectedBehavior` naming convention for all test methods.
- Use the Arrange-Act-Assert pattern clearly within each test.
  </zadanie>

<class_do_przetestowania>
@$1
</class_do_przetestowania>
