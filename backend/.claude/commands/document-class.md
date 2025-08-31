---
description: Analyzes a C# class and adds comprehensive XML documentation.
argument-hint: [path/to/file.cs]
---

<zadanie>
Przeanalizuj zawartość pliku, do którego referencja znajduje się poniżej, i dodaj do niego kompletną dokumentację XML w języku angielskim.
Upewnij się, że każda publiczna metoda i właściwość jest w pełni udokumentowana, a sekcja <remarks> zawiera głęboki kontekst biznesowy.
</zadanie>

<kontekst_szczegolowych_regul>
<regula nazwa="Comprehensive XML Documentation">

### 🎯 RULE: Comprehensive XML Documentation

**Rule:** All public classes, methods, and properties MUST have detailed XML documentation in English.
**Required Elements:** `<summary>`, `<param>`, `<returns>`, `<exception>`, `<remarks>`, `<example>`.
</regula>
<regula nazwa="Business Context First">

### 🎯 RULE: Document the "Why", Not Just the "What"

**Rule:** Documentation, especially in the `<remarks>` section, must explain the business context, not just repeat the technical description.
</regula>
</kontekst_szczegolowych_regul>

<plik_do_przetworzenia>
@$1
</plik_do_przetworzenia>
