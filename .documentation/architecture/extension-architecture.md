# Extension Architecture

This document explains the design patterns and architectural principles behind the extension method system that provides the fluent API for the Result monad library. Understanding these patterns helps developers use the library effectively and guides future extensions.

## Overview

The extension method architecture is built around several key principles:

- **Fluent Interface**: Natural method chaining for readable code
- **Type Safety**: Compile-time guarantees through generic constraints
- **Discoverability**: IntelliSense-driven API exploration
- **Modularity**: Selective imports and focused functionality
- **Performance**: Efficient implementations with minimal overhead
- **Composability**: Operations that work well together

## Core Extension Patterns

### 1. Functor Pattern (Map Operations)

The Map pattern transforms the success value while preserving the error structure.

**Synchronous Map**:

```csharp
public static Result<TResult, TError> Map<T, TResult, TError>(
    this Result<T, TError> result,
    Func<T, TResult> mapFunc)
{
    ArgumentNullException.ThrowIfNull(mapFunc);
    
    return result.IsOk
        ? new Ok<TResult, TError>(mapFunc(result.Match(x => x, _ => throw new UnreachableException())))
        : result.MapErr(e => e);
}
```

**Design Rationale**:

- **Preservation**: Error type remains unchanged
- **Transformation**: Only success values are transformed
- **Null Safety**: Argument validation prevents null reference exceptions
- **Type Inference**: Generic parameters enable type inference from usage

**Usage Pattern**:

```csharp
Result<string, ValidationError> nameResult = GetUserName(id);
Result<string, ValidationError> upperName = nameResult.Map(name => name.ToUpper());
```

### 2. Monad Pattern (Bind Operations)

The Bind pattern enables chaining operations that can themselves fail.

**Synchronous Bind**:

```csharp
public static Result<TResult, TError> Bind<T, TResult, TError>(
    this Result<T, TError> result,
    Func<T, Result<TResult, TError>> bindFunc)
{
    ArgumentNullException.ThrowIfNull(bindFunc);
    
    return result.IsOk
        ? bindFunc(result.Match(x => x, _ => throw new UnreachableException()))
        : result.MapErr(e => e);
}
```

**Design Rationale**:

- **Composition**: Enables chaining fallible operations
- **Error Propagation**: Errors short-circuit the chain automatically
- **Flat Structure**: Prevents nested Result<Result<T, E>, E> structures
- **Railway Pattern**: Success and error paths are clearly separated

**Usage Pattern**:

```csharp
return GetUser(id)
    .Bind(user => ValidateUser(user))
    .Bind(validUser => SaveUser(validUser));
```

### 3. Async Composition Patterns

The library provides comprehensive async support for both Task and ValueTask scenarios.

**Task-Based Bind**:

```csharp
public static async Task<Result<TResult, TError>> BindAsync<T, TResult, TError>(
    this Task<Result<T, TError>> resultTask,
    Func<T, Task<Result<TResult, TError>>> bindFunc)
{
    ArgumentNullException.ThrowIfNull(bindFunc);
    
    Result<T, TError> result = await resultTask.ConfigureAwait(false);
    
    return result.IsOk
        ? await bindFunc(result.Match(x => x, _ => throw new UnreachableException())).ConfigureAwait(false)
        : result.MapErr(e => e);
}
```

**ValueTask-Based Bind**:

```csharp
public static async ValueTask<Result<TResult, TError>> BindAsync<T, TResult, TError>(
    this ValueTask<Result<T, TError>> resultTask,
    Func<T, ValueTask<Result<TResult, TError>>> bindFunc)
{
    ArgumentNullException.ThrowIfNull(bindFunc);
    
    Result<T, TError> result = await resultTask.ConfigureAwait(false);
    
    return result.IsOk
        ? await bindFunc(result.Match(x => x, _ => throw new UnreachableException())).ConfigureAwait(false)
        : result.MapErr(e => e);
}
```

**Design Considerations**:

- **ConfigureAwait(false)**: Library code doesn't capture SynchronizationContext
- **Performance**: ValueTask variants avoid allocations in hot paths
- **Consistency**: Same API surface for both Task and ValueTask
- **Async Propagation**: Maintains async context through the chain

### 4. Error Handling Patterns

Error transformation and recovery patterns provide flexible error management.

**Error Mapping**:

```csharp
public static Result<T, TErrorResult> MapErr<T, TError, TErrorResult>(
    this Result<T, TError> result,
    Func<TError, TErrorResult> mapErrFunc)
{
    ArgumentNullException.ThrowIfNull(mapErrFunc);
    
    return result.IsErr
        ? new Err<T, TErrorResult>(mapErrFunc(result.Match(_ => throw new UnreachableException(), e => e)))
        : new Ok<T, TErrorResult>(result.Match(x => x, _ => throw new UnreachableException()));
}
```

**Fallback Operations (OrElse)**:

```csharp
public static Result<T, TError> OrElse<T, TError>(
    this Result<T, TError> result,
    Func<TError, Result<T, TError>> fallbackFunc)
{
    ArgumentNullException.ThrowIfNull(fallbackFunc);
    
    return result.IsErr
        ? fallbackFunc(result.Match(_ => throw new UnreachableException(), e => e))
        : result;
}
```

**Design Features**:

- **Lazy Evaluation**: Fallback functions only execute when needed
- **Type Consistency**: Error types must match for composition
- **Resource Efficiency**: Avoids unnecessary computations
- **Flexibility**: Supports complex recovery scenarios

### 5. Pattern Matching Extensions

Exhaustive pattern matching ensures all cases are handled.

**Match with Return Value**:

```csharp
public static TResult Match<T, TError, TResult>(
    this Result<T, TError> result,
    Func<T, TResult> onOk,
    Func<TError, TResult> onErr)
{
    ArgumentNullException.ThrowIfNull(onOk);
    ArgumentNullException.ThrowIfNull(onErr);
    
    return result.IsOk
        ? onOk(result.Match(x => x, _ => throw new UnreachableException()))
        : onErr(result.Match(_ => throw new UnreachableException(), e => e));
}
```

**Match with Side Effects**:

```csharp
public static Unit Match<T, TError>(
    this Result<T, TError> result,
    Action<T> onOk,
    Action<TError> onErr)
{
    ArgumentNullException.ThrowIfNull(onOk);
    ArgumentNullException.ThrowIfNull(onErr);
    
    if (result.IsOk)
    {
        onOk(result.Match(x => x, _ => throw new UnreachableException()));
    }
    else
    {
        onErr(result.Match(_ => throw new UnreachableException(), e => e));
    }
    
    return Unit.Value;
}
```

**Pattern Benefits**:

- **Exhaustiveness**: Compiler ensures both cases are handled
- **Type Safety**: Return types must match in both branches
- **Clarity**: Makes success/error handling explicit
- **Functional Style**: Supports functional programming paradigms

## Advanced Extension Patterns

### 6. Flattening Operations

Flattening removes nested Result structures that can arise from composition.

**Synchronous Flatten**:

```csharp
public static Result<T, TError> Flatten<T, TError>(
    this Result<Result<T, TError>, TError> result)
{
    return result.IsOk
        ? result.Match(x => x, _ => throw new UnreachableException())
        : result.MapErr(e => e);
}
```

**Async Flatten**:

```csharp
public static async Task<Result<T, TError>> FlattenAsync<T, TError>(
    this Task<Result<Task<Result<T, TError>>, TError>> resultTask)
{
    Result<Task<Result<T, TError>>, TError> outerResult = await resultTask.ConfigureAwait(false);
    
    return outerResult.IsOk
        ? await outerResult.Match(x => x, _ => throw new UnreachableException()).ConfigureAwait(false)
        : outerResult.MapErr(e => e);
}
```

**Use Cases**:

- **Nested Operations**: When operations return Result<Result<T, E>, E>
- **Composition Simplification**: Flattens complex operation chains
- **API Cleanup**: Provides cleaner interfaces for consumers

### 7. Predicate Extensions

Predicate extensions provide convenient condition checking.

**Success Predicate Checking**:

```csharp
public static bool IsOkAnd<T, TError>(
    this Result<T, TError> result,
    Func<T, bool> predicate)
{
    ArgumentNullException.ThrowIfNull(predicate);
    
    return result.IsOk && predicate(result.Match(x => x, _ => throw new UnreachableException()));
}
```

**Error Predicate Checking**:

```csharp
public static bool IsErrAnd<T, TError>(
    this Result<T, TError> result,
    Func<TError, bool> predicate)
{
    ArgumentNullException.ThrowIfNull(predicate);
    
    return result.IsErr && predicate(result.Match(_ => throw new UnreachableException(), e => e));
}
```

**Benefits**:

- **Conciseness**: Reduces boilerplate for common checks
- **Safety**: Type-safe access to values during predicate checking
- **Readability**: Makes conditional logic more expressive
- **Performance**: Short-circuits evaluation when appropriate

## Extension Organization Strategy

### Namespace Design

The extension methods are organized into focused namespaces:

```csharp
// Core synchronous extensions
namespace Monads.Extensions.Results.Sync
{
    public static class BindExtension { ... }
    public static class MapExtension { ... }
    public static class MatchExtension { ... }
    // ...
}

// Async extensions
namespace Monads.Extensions.Results.Async
{
    public static class BindTaskExtension { ... }
    public static class BindValueTaskExtension { ... }
    public static class MapTaskExtension { ... }
    // ...
}
```

**Organization Benefits**:

- **Selective Imports**: Developers can choose which extensions to include
- **IntelliSense Clarity**: Related methods are grouped together
- **Maintainability**: Clear separation of concerns for development
- **Extensibility**: Easy to add new extension categories

### File-Per-Extension Pattern

Each extension method group is defined in its own file:

```bash
Extensions/Results/Sync/
├── BindExtension.cs        # Bind operations
├── MapExtension.cs         # Map operations  
├── MapErrExtension.cs      # Error mapping operations
├── MatchExtension.cs       # Pattern matching operations
├── FlattenExtension.cs     # Flattening operations
└── OrElseExtension.cs      # Fallback operations
```

**Advantages**:

- **Discoverability**: Easy to find specific functionality
- **Maintainability**: Changes are isolated to specific files
- **Testing**: Each extension can be tested independently
- **Code Review**: Smaller, focused changes are easier to review

### Static Class Design

Each extension file contains a single static class:

```csharp
namespace Monads.Extensions.Results.Sync;

/// <summary>
/// Extension methods for binding operations on Result types.
/// </summary>
public static class BindExtension
{
    /// <summary>
    /// Chains a function that returns a Result, allowing for composition of operations that can fail.
    /// </summary>
    public static Result<TResult, TError> Bind<T, TResult, TError>(
        this Result<T, TError> result,
        Func<T, Result<TResult, TError>> bindFunc)
    {
        // Implementation
    }
    
    // Additional overloads and related methods
}
```

**Design Principles**:

- **Single Responsibility**: Each class focuses on one type of operation
- **Comprehensive Documentation**: XML docs for all public methods
- **Consistent Signatures**: Similar patterns across all extensions
- **Null Safety**: Argument validation in all methods

## Performance Considerations

### Hot Path Optimizations

Critical performance paths are optimized for common scenarios:

**Fast Path for Success Cases**:

```csharp
public static Result<TResult, TError> Map<T, TResult, TError>(
    this Result<T, TError> result,
    Func<T, TResult> mapFunc)
{
    ArgumentNullException.ThrowIfNull(mapFunc);
    
    // Fast path: Check success first (most common case)
    if (result.IsOk)
    {
        return new Ok<TResult, TError>(mapFunc(result.Match(x => x, _ => throw new UnreachableException())));
    }
    
    // Slower path: Error case
    return result.MapErr(e => e);
}
```

### Memory Efficiency

**Struct-Based Intermediate Results** (where applicable):

```csharp
// ValueTask usage to avoid Task allocation
public static async ValueTask<Result<TResult, TError>> MapAsync<T, TResult, TError>(
    this Result<T, TError> result,
    Func<T, ValueTask<TResult>> mapFunc)
{
    ArgumentNullException.ThrowIfNull(mapFunc);
    
    if (result.IsOk)
    {
        TResult mappedValue = await mapFunc(result.Match(x => x, _ => throw new UnreachableException())).ConfigureAwait(false);
        return new Ok<TResult, TError>(mappedValue);
    }
    
    return result.MapErr(e => e);
}
```

### Lazy Evaluation Patterns

**Deferred Execution for Expensive Operations**:

```csharp
public static Result<T, TError> OrElse<T, TError>(
    this Result<T, TError> result,
    Func<Result<T, TError>> fallbackFactory)  // Factory function, not direct value
{
    ArgumentNullException.ThrowIfNull(fallbackFactory);
    
    return result.IsOk ? result : fallbackFactory();  // Only execute when needed
}
```

## Extensibility Guidelines

### Adding New Extensions

When adding new extension methods, follow these patterns:

1. **Consistent Naming**: Follow established conventions (Map, Bind, Match, etc.)
2. **Generic Constraints**: Use appropriate constraints for type safety
3. **Null Safety**: Always validate function arguments
4. **Documentation**: Provide comprehensive XML documentation
5. **Testing**: Create thorough test coverage
6. **Async Support**: Consider both Task and ValueTask variants

**Example New Extension**:

```csharp
/// <summary>
/// Filters the result based on a predicate, converting success to error if predicate fails.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
/// <param name="result">The result to filter.</param>
/// <param name="predicate">The predicate to test the success value.</param>
/// <param name="errorFactory">Factory function to create error when predicate fails.</param>
/// <returns>Original result if predicate passes, error result if predicate fails or result is error.</returns>
public static Result<T, TError> Filter<T, TError>(
    this Result<T, TError> result,
    Func<T, bool> predicate,
    Func<T, TError> errorFactory)
{
    ArgumentNullException.ThrowIfNull(predicate);
    ArgumentNullException.ThrowIfNull(errorFactory);
    
    return result.IsOk
        ? (predicate(result.Match(x => x, _ => throw new UnreachableException()))
            ? result
            : new Err<T, TError>(errorFactory(result.Match(x => x, _ => throw new UnreachableException()))))
        : result;
}
```

### Third-Party Extensions

The architecture supports third-party extensions through the same patterns:

```csharp
// Third-party package: Monads.Extensions.Validation
namespace Monads.Extensions.Validation;

public static class ValidationExtensions
{
    public static Result<T, ValidationError> ValidateNotNull<T>(
        this Result<T?, ValidationError> result)
        where T : class
    {
        return result.Bind(value =>
            value is not null
                ? new Ok<T, ValidationError>(value)
                : new Err<T, ValidationError>(new ValidationError("Value cannot be null")));
    }
}
```

## Testing Extension Methods

### Unit Testing Patterns

Each extension method should have comprehensive test coverage:

```csharp
[Test]
public void Map_WithOkResult_TransformsValue()
{
    // Arrange
    var result = new Ok<int, string>(42);
    
    // Act
    Result<string, string> mapped = result.Map(x => x.ToString());
    
    // Assert
    Assert.That(mapped.IsOk, Is.True);
    mapped.Match(
        onOk: value => Assert.That(value, Is.EqualTo("42")),
        onErr: error => Assert.Fail($"Expected success but got error: {error}")
    );
}

[Test]
public void Map_WithErrResult_PreservesError()
{
    // Arrange
    var result = new Err<int, string>("error message");
    
    // Act
    Result<string, string> mapped = result.Map(x => x.ToString());
    
    // Assert
    Assert.That(mapped.IsErr, Is.True);
    mapped.Match(
        onOk: value => Assert.Fail($"Expected error but got success: {value}"),
        onErr: error => Assert.That(error, Is.EqualTo("error message"))
    );
}

[Test]
public void Map_WithNullFunction_ThrowsArgumentNullException()
{
    // Arrange
    var result = new Ok<int, string>(42);
    
    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => result.Map<string>(null!));
}
```

### Property-Based Testing

Mathematical properties should be tested with property-based tests:

```csharp
[Test]
public void Map_CompositionLaw_MapOfCompositionEqualsCompositionOfMaps()
{
    // Property: result.Map(f).Map(g) == result.Map(x => g(f(x)))
    
    var result = new Ok<int, string>(42);
    Func<int, string> f = x => x.ToString();
    Func<string, int> g = x => x.Length;
    
    Result<int, string> left = result.Map(f).Map(g);
    Result<int, string> right = result.Map(x => g(f(x)));
    
    Assert.That(left.Match(x => x, e => -1), Is.EqualTo(right.Match(x => x, e => -1)));
}
```

This extension architecture provides a solid foundation for building fluent, type-safe, and performant APIs while maintaining flexibility for future enhancements and third-party extensions.
