# Result&lt;T, E&gt; Type

The `Result<T, E>` type is the core abstraction of the Monads library, providing a discriminated union that models either a success or an error state. This abstract record serves as the foundation for implementing the Result monad pattern in C#.

## Namespace

```csharp
Monads.Results
```

## Syntax

```csharp
public abstract record Result<T, E>
    where E : notnull
    where T : notnull
```

## Type Parameters

| Parameter | Description |
|-----------|-------------|
| `T` | The type of the success value. Must be non-null. |
| `E` | The type of the error value. Must be non-null. |

## Properties

### IsOk

Gets a value indicating whether the result represents a success.

```csharp
public abstract bool IsOk { get; }
```

**Returns:** `true` if the result is `Ok<T, E>`; otherwise, `false`.

### IsErr

Gets a value indicating whether the result represents an error.

```csharp
public bool IsErr => !IsOk;
```

**Returns:** `true` if the result is `Err<T, E>`; otherwise, `false`.

## Methods

### IsOkAnd(Func&lt;T, bool&gt;)

Determines whether the result is `Ok<T, E>` and satisfies the specified predicate.

```csharp
public abstract bool IsOkAnd(Func<T, bool> predicate);
```

**Parameters:**

- `predicate` - The function to test the success value.

**Returns:** `true` if the result is `Ok<T, E>` and the predicate returns `true`; otherwise, `false`.

**Exceptions:**

- `ArgumentNullException` - Thrown when `predicate` is `null`.

**Example:**

```csharp
Result<int, string> result = ResultFactory.Success<int, string>(42);
bool isPositive = result.IsOkAnd(value => value > 0); // true

Result<int, string> errorResult = ResultFactory.Failure<int, string>("error");
bool isStillPositive = errorResult.IsOkAnd(value => value > 0); // false
```

### IsErrAnd(Func&lt;E, bool&gt;)

Determines whether the result is `Err<T, E>` and satisfies the specified predicate.

```csharp
public abstract bool IsErrAnd(Func<E, bool> predicate);
```

**Parameters:**

- `predicate` - The function to test the error value.

**Returns:** `true` if the result is `Err<T, E>` and the predicate returns `true`; otherwise, `false`.

**Exceptions:**

- `ArgumentNullException` - Thrown when `predicate` is `null`.

**Example:**

```csharp
Result<int, string> errorResult = ResultFactory.Failure<int, string>("validation error");
bool isValidationError = errorResult.IsErrAnd(err => err.Contains("validation")); // true

Result<int, string> successResult = ResultFactory.Success<int, string>(42);
bool isStillValidationError = successResult.IsErrAnd(err => err.Contains("validation")); // false
```

## Derived Types

The `Result<T, E>` type has two concrete implementations:

- [`Ok<T, E>`](./ok.md) - Represents a successful result containing a value of type `T`
- [`Err<T, E>`](./err.md) - Represents a failed result containing an error of type `E`

## ResultFactory

The `ResultFactory` static class provides convenient factory methods for creating `Result<T, E>` instances.

### Success&lt;T, E&gt;(T)

Creates a successful result containing the specified value.

```csharp
public static Result<T, E> Success<T, E>(T value)
    where E : notnull
    where T : notnull
```

**Parameters:**

- `value` - The value to wrap in the result.

**Returns:** A new `Ok<T, E>` instance containing the specified value.

**Exceptions:**

- `ArgumentNullException` - Thrown when `value` is `null`.

**Example:**

```csharp
Result<int, string> result = ResultFactory.Success<int, string>(42);
Console.WriteLine(result.IsOk); // True
```

### Failure&lt;T, E&gt;(E)

Creates a failed result containing the specified error.

```csharp
public static Result<T, E> Failure<T, E>(E error)
    where E : notnull
    where T : notnull
```

**Parameters:**

- `error` - The error to wrap in the result.

**Returns:** A new `Err<T, E>` instance containing the specified error.

**Exceptions:**

- `ArgumentNullException` - Thrown when `error` is `null`.

**Example:**

```csharp
Result<int, string> result = ResultFactory.Failure<int, string>("Something went wrong");
Console.WriteLine(result.IsErr); // True
```

## Usage Patterns

### Basic Usage

```csharp
// Create success result
Result<int, string> success = ResultFactory.Success<int, string>(42);

// Create error result  
Result<int, string> error = ResultFactory.Failure<int, string>("Invalid input");

// Check result state
if (success.IsOk)
{
    Console.WriteLine("Operation succeeded");
}

if (error.IsErr)
{
    Console.WriteLine("Operation failed");
}
```

### Conditional Checking

```csharp
Result<int, string> result = GetSomeResult();

// Check if result is ok and meets condition
bool isPositiveNumber = result.IsOkAnd(value => value > 0);

// Check if result is error and meets condition
bool isValidationError = result.IsErrAnd(err => err.StartsWith("Validation"));
```

## Design Philosophy

The `Result<T, E>` type follows functional programming principles:

- **Immutability**: Results are immutable records that cannot be modified after creation
- **Type Safety**: Compile-time guarantees about success/error states
- **Explicit Error Handling**: Forces developers to handle both success and error cases
- **Composition**: Enables chaining operations through extension methods

## See Also

- [Ok&lt;T, E&gt; Type](./ok.md) - Success result implementation
- [Err&lt;T, E&gt; Type](./err.md) - Error result implementation  
- [Synchronous Extensions](../extensions/sync-extensions.md) - Extension methods for Result operations
- [Asynchronous Extensions](../extensions/async-extensions.md) - Async extension methods
- [Error Handling Concepts](../../concepts/error-handling.md) - Error handling patterns and best practices
