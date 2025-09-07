---
description: Analyzes a C# class and adds comprehensive XML documentation.
argument-hint: [path/to/file.cs]
---

<task>
Analyze the file referenced below and add comprehensive XML documentation in English.
Ensure that every public method and property is fully documented, and the <remarks> section contains deep business context.
</task>

<detailed_rules_context>
<rule name="Comprehensive XML Documentation">

### 🎯 RULE: Comprehensive XML Documentation

**Rule:** All public classes, methods, and properties MUST have detailed XML documentation in English.
**Required Elements:** `<summary>`, `<param>`, `<returns>`, `<exception>`, `<remarks>`, `<example>`.
</rule>
<rule name="Business Context First">

### 🎯 RULE: Document the "Why", Not Just the "What"

**Rule:** Documentation, especially in the `<remarks>` section, must explain the business context, not just repeat the technical description.
</rule>
</detailed_rules_context>

<file_to_process>
$ARGUMENTS
</file_to_process>
