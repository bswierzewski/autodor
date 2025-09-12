---
description: Adds concise and meaningful XML documentation to C# code.
argument-hint: [path/to/file.cs]
---

<task>
Analyze the file referenced below and add XML documentation only where it adds real value.
- Use concise <summary> (1–2 sentences) describing the purpose and intent of classes, methods, or properties. 
- Add <param>, <returns>, and <exception> only when they provide specific, useful context.
- Avoid obvious or trivial comments (e.g., "Gets or sets" or repeating method name).
- Do not add inline comments for every line of code — only add them when clarifying non-obvious business logic or reasoning.
- If any Polish comments are found, replace them with clear English equivalents.
</task>

<detailed_rules_context>
<rule name="Concise XML Documentation">

### 🎯 RULE: Concise XML Documentation

**Rule:** Add XML documentation to all public classes, methods, and non-trivial properties.  
**Required Elements:**

- `<summary>`: short, clear, purpose-driven.
- `<param>`: describe what the parameter represents, not just its type.
- `<returns>`: explain the result precisely (if meaningful).
- `<exception>`: document only if method explicitly throws exceptions.  
  Avoid boilerplate or meaningless text.
  </rule>

<rule name="Value-Oriented Inline Comments">

### 🎯 RULE: Inline Comments Only for Value

**Rule:** Add inline comments inside methods **only where necessary** to explain business logic, decisions, or non-obvious reasoning.  
Never restate what the code already makes obvious.
</rule>
</detailed_rules_context>

<file_to_process>
$ARGUMENTS
</file_to_process>
