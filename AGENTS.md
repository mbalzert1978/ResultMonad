# Monads: AI Agent Guide

This document helps AI agents understand the Monads project structure, conventions, and development workflows.

## Project Overview

**Monads** is a functional programming library for C# implementing the Result monad pattern for railway-oriented programming. It provides type-safe error handling without exceptions.

- **Language**: C# 13 with modern features (records, file-scoped namespaces, switch expressions)
- **.NET Target**: 10.0 (see `global.json` for SDK version)
- **Test Framework**: xunit v3 + AwesomeAssertions
- **Code Quality**: SonarAnalyzer enforcement, 100% documentation, strict error handling

## Build & Test Commands

```bash
# Build the solution
dotnet build

# Run all tests
dotnet test

# Pack NuGet package
dotnet pack
```

The solution file is `Monads.slnx` (modern format).

## Architecture: Key Concepts

### Core Type Hierarchy

The library centers on three core types:

1. **`Result<T, E>`** — Abstract record representing either success or failure
   - Generic over value type `T` (must not be null)
   - Generic over error type `E` (must not be null)
   - Two sealed variants: `Ok<T, E>` and `Err<T, E>`

2. **`Ok<T, E>(T Value)`** — Success variant
3. **`Err<T, E>(E Error)`** — Error variant

### Folder Structure

```text
src/Monads/
├── Models/
│   ├── Results/             # Core Result, Ok, Err types + predicates
│   ├── Options/             # Option type (partial implementation)
│   └── Unit.cs              # Void-like type for "no meaningful value"
├── Extensions/
│   ├── Results/Sync/        # Map, Bind, Match, OrElse, Flatten, Or
│   ├── Results/Async/       # Task<T> and ValueTask<T> variants
│   └── Options/Sync/        # Option pattern matching
└── Strings/Constants.cs     # Error messages

tests/Tests.Monads.Result/
├── Models/                  # Ok, Err, predicates, Unit tests
└── Extensions/
    ├── Sync/                # Mirror source structure
    └── Async/
```

## Coding Conventions

### File Organization

- **Namespaces**: File-scoped (e.g., `namespace Monads.Models.Results;`)
- **Naming**: Classes and files use PascalCase matching filename
- **Copyright**: Every file starts with copyright header block

  ```csharp
  // <copyright file="FileName.cs" company="Markus - Iorio">
  // Copyright (c) Markus - Iorio. All rights reserved.
  // </copyright>
  ```

### Code Style Enforcement

**Strict rules** (configured in `.editorconfig`):

- **4-space indentation**, CRLF line endings, final newline
- **No `this.` qualification** except where required by language
- **Expression-bodied members** for properties, operators, indexers, accessors
- **NOT expression-bodied** for methods or constructors (expand them)
- **Predefined types** (`int` not `Int32`)
- **Implicit null propagation** and coalesce expressions preferred
- **No unnecessary parentheses**
- **TreatWarningsAsErrors: true** — All code analysis warnings are treated as errors

**Disabled analyzers** (by design):

- S1694: Abstract class requirements
- CA1715: Interface/delegate naming prefixes

### Parameter Validation

Every public method validates parameters:

```csharp
ArgumentNullException.ThrowIfNull(self);
ArgumentNullException.ThrowIfNull(operation);
```

## Extension Methods: Sync Pattern

**One extension class per feature** (named `[Feature]Extension.cs`):

```csharp
public static class BindExtension
{
    /// <summary>Chains operations that return Result.</summary>
    public static Result<U, E> Bind<T, E, U>(
        this Result<T, E> self,
        Func<T, Result<U, E>> operation)
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(operation);
        return self.Match(operation, error => ResultFactory.Failure<U, E>(error));
    }
}
```

### Key Extension Methods

| Method | Purpose | Variants |
| -------- | --------- | ---------- |
| `Map(Func<T, U>)` | Transform Ok value | Sync, Task, ValueTask |
| `MapErr(Func<E, F>)` | Transform Err value | Sync, Task, ValueTask |
| `Bind(Func<T, Result<U, E>>)` | Chain Result-returning operations (flatMap) | Sync, Task, ValueTask |
| `Match(Func<T, U>, Func<E, U>)` | Pattern match both branches | Sync, Task, ValueTask |
| `Or(Result<T, E>)` | Provide alternative result | Sync only |
| `OrElse(Func<E, Result<T, F>>)` | Recover from error | Sync, Task, ValueTask |
| `Flatten()` | Unwrap nested Result | Sync, Async |

## Async Extension Pattern

Async methods exist in parallel files with consistent naming:

- **`MapTaskExtension.cs`** — Async Map for `Task<Result<T, E>>`
- **`MapValueTaskExtension.cs`** — Async Map for `ValueTask<Result<T, E>>`
- Similarly for `BindTaskExtension`, `MatchTaskExtension`, etc.

**Key async patterns**:

1. **All async methods call `.ConfigureAwait(false)`** on every await
2. **Use `.MatchAsync()` for pattern matching** on awaited results
3. **Support mixed sync/async combinations**:
   - `Task<Result>` + sync operation
   - Result + async operation (returns Task)
   - `Task<Result>` + async operation

4. **Factory methods** like `ResultFactory.Success()` and `ResultFactory.Failure()` used directly without await

Example:

```csharp
public static async Task<Result<U, E>> MapAsync<T, E, U>(
    this Task<Result<T, E>> self,
    Func<T, U> mapper)
    where T : notnull
    where E : notnull
    where U : notnull
{
    ArgumentNullException.ThrowIfNull(self);
    ArgumentNullException.ThrowIfNull(mapper);
    
    var result = await self.ConfigureAwait(false);
    return result.Match(
        ok: value => ResultFactory.Success<U, E>(mapper(value)),
        err: error => ResultFactory.Failure<U, E>(error)
    );
}
```

## Testing Conventions

### Test Structure

- **Class**: `sealed class [FeatureName]Tests`
- **Namespace**: Mirrors source structure (e.g., `Monads.Results.Tests.Extensions.Sync`)
- **Framework**: xunit + AwesomeAssertions fluent assertions (`.Should().*`)

### Test Naming Pattern

```text
[FeatureName]_When[Condition]_Should[ExpectedOutcome]
```

Examples:

- `Bind_WhenCalledWithOkResult_ShouldBindValue()`
- `Bind_WhenChainedAndEncountersError_ShouldStopPropagation()`
- `MapAsync_WhenCalledWithTaskOkAndAsyncFunction_ShouldMapValue()`

### Standard Test Coverage

Each extension should have tests for:

1. ✅ Happy path (Ok case)
2. ✅ Error path (Err case)
3. ✅ Null parameter handling (throws `ArgumentNullException`)
4. ✅ Method chaining/composition
5. ✅ Type transformations (if applicable)
6. ✅ For async: all Task/ValueTask combinations

Example test:

```csharp
[Fact]
public void Map_WhenCalledWithOkResult_ShouldTransformValue()
{
    // Arrange
    var result = new Ok<int, string>(5);
    var mapper = (int x) => x * 2;

    // Act
    var mapped = result.Map(mapper);

    // Assert
    mapped.Should()
        .BeOfType<Ok<int, string>>()
        .And.Match<Ok<int, string>>(r => r.Value == 10);
}
```

## Common Tasks

### Adding a New Extension Method

1. **Create sync version first** in `src/Monads/Extensions/Results/Sync/[MethodName].cs`
   - Static class named `[MethodName]Extension`
   - Validate all parameters with `ArgumentNullException.ThrowIfNull()`
   - Use `self.Match()` pattern for branching on Ok/Err
   - Add comprehensive XML documentation

2. **Create corresponding async variants** in `src/Monads/Extensions/Results/Async/`
   - `[MethodName]TaskExtension.cs` for `Task<Result>`
   - `[MethodName]ValueTaskExtension.cs` for `ValueTask<Result>`
   - Include all sync+async combinations

3. **Add tests** mirroring the source structure
   - `tests/Tests.Monads.Result/Extensions/Sync/[MethodName]Tests.cs`
   - `tests/Tests.Monads.Result/Extensions/Async/[MethodName]TaskExtensionTests.cs`
   - `tests/Tests.Monads.Result/Extensions/Async/[MethodName]ValueTaskExtensionTests.cs`

4. **Verify code analysis**: `dotnet build` must succeed with zero warnings

### Adding a New Model Type

1. Create abstract record in `src/Monads/Models/[Category]/`
2. Create sealed derived records for each variant
3. Add property predicates (`IsOk`, `IsErr`, etc.)
4. Add comprehensive XML documentation
5. Create corresponding tests
6. Add factory methods to `ResultFactory` if applicable

## Key Files Reference

| File | Purpose |
| ------ | --------- |
| `Directory.Build.props` | Centralized build configuration for all projects |
| `Directory.Packages.props` | Centralized NuGet package versions |
| `.editorconfig` | Code style rules (strict error enforcement) |
| `Monads.slnx` | Solution structure |
| `src/Monads/Monads.csproj` | Main library project |
| `tests/Tests.Monads.Result/Tests.Monads.Result.csproj` | Test project |

## Important Notes

- **No external dependencies** in core library (zero-dependency design)
- **No CI/CD automation files** — designed for local development with `dotnet` CLI
- **100% documentation required** — all public types/methods need XML comments
- **Code analysis enforcement** — all warnings are errors, no exceptions
- **Async-first pattern** — async extensions support `Task<T>` and `ValueTask<T>` variants
- **Records over classes** — all core types use `abstract record` with sealed variants
- **Static using directives** — import factory methods with `using static` for clean code

## External Documentation

For in-depth topics, see the `.documentation/` folder referenced in [README.md](README.md):

- **Concepts**: Railway-oriented programming, Result pattern
- **Getting Started**: Installation, migration guides
- **API Reference**: Complete method documentation
- **Architecture**: Design decisions, extension patterns
- **Contributing**: Coding standards, documentation standards
- **Testing**: Testing philosophy and patterns
- **Examples**: Real-world usage patterns
