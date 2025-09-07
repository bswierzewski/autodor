---
description: Analyzes a C# class and adds comprehensive XML documentation.
argument-hint: [path/to/file.cs]
---

<task>
Analyze the file referenced below and add XML documentation with summary and parameters.
Additionally, add valuable inline comments within methods to explain their operation and business logic.
If any Polish comments are found, replace them with English equivalents.
</task>

<detailed_rules_context>
<rule name="Comprehensive XML Documentation">

### 🎯 RULE: Comprehensive XML Documentation

**Rule:** All public classes, methods, and properties MUST have XML documentation in English.
**Required Elements:** `<summary>` and `<param>` for parameters. Additionally, add inline comments within method bodies to explain business logic and operation flow.
</rule>
<rule name="Business Context First">

### 🎯 RULE: Document the "Why", Not Just the "What"

**Rule:** Inline comments within methods must explain the business context and reasoning behind the implementation, not just repeat what the code does.
</rule>
</detailed_rules_context>

<file_to_process>
$ARGUMENTS
</file_to_process>
