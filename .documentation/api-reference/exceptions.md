# Exception Reference

This document provides a comprehensive reference of all exceptions that can be thrown when using the Monads library. Understanding these exceptions helps in proper error handling and debugging.

## Overview

The Monads library follows .NET exception handling conventions and throws standard framework exceptions in specific scenarios. All exceptions are well-documented and occur in predictable situations.

## Exception Types

### ArgumentNullException

The most common exception in the library, thrown when `null` values are passed where they are not allowed.

**Namespace:** `System`

**When Thrown:**

- Passing `null` to Result constructors (`Ok<T, E>`, `Err<T, E>`)
- Passing `null` predicate functions to `IsOkAnd()` or `IsErrAnd()`
- Passing `null` operation functions to extension methods
- Passing `null` as the `self` parameter to any extension method

**Common Scenarios:**

```csharp
// Constructor scenarios
var result1 = new Ok<string, int>(null); // ❌ ArgumentNullException
var result2 = new Err<int, string>(null); // ❌ ArgumentNullException

// Predicate scenarios  
Result<int, string> result = new Ok<int, string>(42);
result.IsOkAnd(null); // ❌ ArgumentNullException

// Extension method scenarios
result.Map(null); // ❌ ArgumentNullException
result.Bind(null); // ❌ ArgumentNullException
result.Match(value => "ok", null); // ❌ ArgumentNullException
```

**Prevention:**

```csharp
// Always validate inputs
ArgumentNullException.ThrowIfNull(value);
var result = new Ok<string, int>(value);

// Use null-conditional operators when appropriate
result?.Map(x => x.ToUpper());
```

### InvalidOperationException

Thrown when operation functions return `null` values, which violates the non-null constraints of the Result types.

**Namespace:** `System`

**When Thrown:**

- Extension method operations return `null`
- Match operations return `null` from either branch
- Factory methods receive operations that return `null`

**Error Messages:**

- `"The operation function returned null, which is not allowed."`

**Common Scenarios:**

```csharp
Result<string, int> result = new Ok<string, int>("test");

// Map operation returns null
result.Map<string>(x => null); // ❌ InvalidOperationException

// Bind operation returns null  
result.Bind<string>(x => null); // ❌ InvalidOperationException

// Match operations return null
result.Match(
    onOk: x => null,     // ❌ InvalidOperationException
    onErr: e => "error"
);

result.Match(
    onOk: x => "success",
    onErr: e => null     // ❌ InvalidOperationException  
);
```

**Prevention:**

```csharp
// Ensure operations never return null
result.Map(x => x?.ToUpper() ?? string.Empty);

// Use proper Result types for operations that can fail
result.Bind(x => 
    string.IsNullOrEmpty(x) 
        ? new Err<string, int>(404)
        : new Ok<string, int>(x.ToUpper())
);

// Provide non-null default values in Match
result.Match(
    onOk: x => x ?? "default",
    onErr: e => $"Error: {e}"
);
```

### UnreachableException

Thrown in theoretically unreachable code paths, typically indicating a bug in the library or corrupted state.

**Namespace:** `System.Diagnostics`

**When Thrown:**

- A `Result<T, E>` instance is neither `Ok<T, E>` nor `Err<T, E>` (should be impossible)
- Pattern matching exhaustion in internal library code

**Error Messages:**

- `"Result must be either Ok or Err."`

**Common Scenarios:**

This exception should never occur in normal usage. If you encounter it:

```csharp
// This should never happen in normal code
Result<int, string> result = GetSomeResult();
result.Match(onOk: x => x, onErr: e => 0); // Might throw UnreachableException if result is corrupted
```

**If You Encounter This Exception:**

1. **Check for memory corruption** - This could indicate unsafe operations or P/Invoke issues
2. **Verify library version** - Ensure you're using a stable version of the Monads library  
3. **Report the bug** - This likely indicates a library defect that should be reported
4. **Check for threading issues** - Concurrent modification of results could cause corruption

### TaskCanceledException / OperationCanceledException

Can occur when using async extension methods with cancellation tokens.

**Namespace:** `System.Threading.Tasks` / `System`

**When Thrown:**

- Async operations are cancelled via `CancellationToken`
- Timeouts occur in async operations

**Common Scenarios:**

```csharp
// With explicit cancellation
CancellationTokenSource cts = new();
Task<Result<Data, string>> dataTask = GetDataAsync(cts.Token);
cts.Cancel(); // May cause TaskCanceledException in async extensions

// With timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
await GetDataAsync()
    .BindAsync(data => ProcessDataAsync(data, cts.Token))
    .MapAsync(processed => SaveDataAsync(processed, cts.Token));
```

**Handling:**

```csharp
try
{
    var result = await GetDataAsync()
        .BindAsync(ProcessDataAsync)
        .MapAsync(FormatDataAsync);
}
catch (OperationCanceledException)
{
    // Handle cancellation
    return new Err<string, string>("Operation was cancelled");
}
```

## Exception Handling Patterns

### Defensive Programming

```csharp
public static class SafeExtensions
{
    public static Result<U, E> SafeMap<T, E, U>(
        this Result<T, E> self,
        Func<T, U> operation)
        where T : notnull
        where E : notnull  
        where U : notnull
    {
        try
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);
            
            return self.Map(operation);
        }
        catch (ArgumentNullException ex)
        {
            // Convert to Result error instead of throwing
            return new Err<U, E>((E)(object)ex.Message);
        }
    }
}
```

### Exception Wrapping

```csharp
public static Result<T, string> TryExecute<T>(Func<T> operation)
    where T : notnull
{
    try
    {
        ArgumentNullException.ThrowIfNull(operation);
        
        T result = operation();
        return new Ok<T, string>(result);
    }
    catch (ArgumentNullException ex)
    {
        return new Err<T, string>($"Null argument: {ex.ParamName}");
    }
    catch (InvalidOperationException ex)
    {
        return new Err<T, string>($"Invalid operation: {ex.Message}");
    }
    catch (Exception ex)
    {
        return new Err<T, string>($"Unexpected error: {ex.Message}");
    }
}
```

### Async Exception Handling

```csharp
public static async Task<Result<T, string>> TryExecuteAsync<T>(
    Func<Task<T>> operation)
    where T : notnull
{
    try
    {
        ArgumentNullException.ThrowIfNull(operation);
        
        T result = await operation().ConfigureAwait(false);
        return new Ok<T, string>(result);
    }
    catch (ArgumentNullException ex)
    {
        return new Err<T, string>($"Null argument: {ex.ParamName}");
    }
    catch (InvalidOperationException ex)
    {
        return new Err<T, string>($"Invalid operation: {ex.Message}");
    }
    catch (OperationCanceledException)
    {
        return new Err<T, string>("Operation was cancelled");
    }
    catch (Exception ex)
    {
        return new Err<T, string>($"Unexpected error: {ex.Message}");
    }
}
```

## Best Practices

### Input Validation

**Always validate inputs early:**

```csharp
public Result<ProcessedData, string> ProcessData(RawData input)
{
    // Validate at method boundary
    ArgumentNullException.ThrowIfNull(input);
    
    return ValidateData(input)
        .Bind(ProcessStep1)
        .Bind(ProcessStep2)
        .Map(FinalizeData);
}
```

### Null Checking

**Use consistent null checking patterns:**

```csharp
// Good: Use ArgumentNullException.ThrowIfNull
ArgumentNullException.ThrowIfNull(parameter);

// Good: Use null-conditional operators where appropriate  
return input?.Trim() ?? string.Empty;

// Avoid: Manual null checks that might be inconsistent
if (parameter == null) throw new ArgumentNullException(nameof(parameter));
```

### Operation Safety

**Ensure operations never return null:**

```csharp
// Bad: Can return null
result.Map(x => x.SomeProperty); // SomeProperty might be null

// Good: Provide defaults or use nullable-aware operations
result.Map(x => x.SomeProperty ?? "default");
result.Bind(x => 
    x.SomeProperty is not null 
        ? new Ok<string, string>(x.SomeProperty)
        : new Err<string, string>("Property is null"));
```

### Exception Documentation

**Always document exceptions in your APIs:**

```csharp
/// <summary>
/// Processes user data and returns a formatted result.
/// </summary>
/// <param name="userData">The user data to process.</param>
/// <returns>A result containing the formatted data or error message.</returns>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="userData"/> is null.</exception>
/// <exception cref="InvalidOperationException">Thrown when user data is in an invalid state.</exception>
public Result<FormattedData, string> ProcessUserData(UserData userData)
{
    ArgumentNullException.ThrowIfNull(userData);
    // ... implementation
}
```

## Debugging Tips

### Common Issues

1. **Null Reference Issues**: Check all operation functions for null returns
2. **Type Constraint Violations**: Ensure all generic type parameters meet `notnull` constraints  
3. **Async Deadlocks**: Use `ConfigureAwait(false)` in library code
4. **Memory Corruption**: Check for unsafe operations or threading issues if you see `UnreachableException`

### Diagnostic Tools

```csharp
#if DEBUG
public static class ResultDiagnostics
{
    public static Result<T, E> WithDebugInfo<T, E>(this Result<T, E> result, string context = "")
        where T : notnull
        where E : notnull
    {
        Debug.WriteLine($"[Result] {context}: IsOk={result.IsOk}");
        return result;
    }
}
#endif
```

## See Also

- [Result<T, E> Type](./models/result.md) - Base result type documentation
- [Synchronous Extensions](./extensions/sync-extensions.md) - Extension methods that can throw exceptions
- [Asynchronous Extensions](./extensions/async-extensions.md) - Async extension methods and cancellation
- [Error Handling Patterns](../examples/error-handling-patterns.md) - Best practices for error handling
