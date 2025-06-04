The repository uses C# for Unity development.

## Code Style
- Indent with four spaces.
- Place opening braces on a new line (Allman style).
- Use PascalCase for class, method, and property names.
- Use camelCase for local variables and method parameters.
- Always specify access modifiers.

## Testing
Run the unit tests with the .NET CLI:

```bash
dotnet test TakiFight.Tests/TakiFight.Tests.csproj
```

## Assets
Large binary assets (models, audio, fonts, images, etc.) are tracked with Git LFS as configured in `.gitattributes`. Do not modify these LFS-tracked files in pull requests.

