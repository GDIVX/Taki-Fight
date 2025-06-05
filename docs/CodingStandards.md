# Coding Standards

This document outlines the coding conventions for the **Taki Fight** project. These guidelines apply to all C# code in this repository.

## Formatting

- **Indentation:** Use four spaces per indentation level. Do not use tabs.
- **Braces:** Place opening braces on a new line (Allman style).
- **Line length:** Try to keep lines under 120 characters when possible.
- **Whitespace:** Remove trailing whitespace at the end of lines.

## Naming

- **Classes, methods and properties:** Use **PascalCase**.
- **Local variables and parameters:** Use **camelCase**.
- **Constants:** Use **PascalCase**.
- **Private fields:** Use **camelCase** prefixed with an underscore (`_`).

## Access Modifiers

Always specify an access modifier (`public`, `private`, `protected`, `internal`) for types and members. Prefer the most restrictive modifier that still allows the code to function.

## File Organization

- Place one top-level class per file.
- Name the file to match the class it contains.
- Use `using` statements at the top of the file, grouped by namespace and sorted alphabetically.

## Testing

Run unit tests with the `.NET CLI`:

```bash
dotnet test TakiFight.Tests/TakiFight.Tests.csproj
```

## Unity Assets

Large binary assets such as models, audio files and textures are tracked with Git LFS. Do not modify LFS-tracked files in pull requests.

