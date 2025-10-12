# Err<T, E> Type

The `Err<T, E>` type represents a failed result containing an error in the Result monad pattern. It is a sealed record that inherits from `Result<T, E>` and indicates that an operation failed with an error of type `E`.

## Namespace

```csharp
Monads.Results
```

## Syntax

```csharp
public sealed record Err<T, E>(E Error) : Result<T, E>
    where E : notnull
    where T : notnull
```

## Type Parameters

| Parameter | Description |
|-----------|-------------|
| `T` | The type of the success value (not used in this variant but required for Result compatibility). Must be non-null. |
| `E` | The type of the error. Must be non-null. |

## Constructor

### Err(E)

Creates a new failed result containing the specified error.

```csharp
public Err(E Error)
```

**Parameters:**

- `Error` - The error value to contain in the result.

**Exceptions:**

- `ArgumentNullException` - Thrown if `Error` is `null`.

**Example:**

```csharp
// Create a failed result with a string error
var result = new Err<int, string>("Operation failed");
Console.WriteLine(result.Error); // Output: Operation failed
Console.WriteLine(result.IsErr); // Output: True
```

## Properties

### Error

Gets the error value contained in this result.

```csharp
public E Error { get; }
```

**Returns:** The error value of type `E`.

**Exceptions:**

- `ArgumentNullException` - Thrown during construction if the provided error is `null`.

**Example:**

```csharp
var result = new Err<int, string>("Database connection failed");
string errorMessage = result.Error; // "Database connection failed"
```

### IsOk (Inherited)

Gets a value indicating whether the result represents a success.

```csharp
public override bool IsOk => false;
```

**Returns:** Always returns `false` for `Err<T, E>` instances.

### IsErr (Inherited)

Gets a value indicating whether the result represents an error.

```csharp
public bool IsErr => !IsOk;
```

**Returns:** Always returns `true` for `Err<T, E>` instances.

## Methods

### IsErrAnd(Func<E, bool>) (Inherited)

Determines whether the result is an error and satisfies the specified predicate.

```csharp
public override bool IsErrAnd(Func<E, bool> predicate)
```

**Parameters:**

- `predicate` - The function to test the error value.

**Returns:** `true` if the predicate returns `true` for the contained error; otherwise, `false`.

**Exceptions:**

- `ArgumentNullException` - Thrown when `predicate` is `null`.

**Example:**

```csharp
var result = new Err<int, string>("Validation error: Invalid input");
bool isValidationError = result.IsErrAnd(err => err.Contains("Validation")); // true
bool isNetworkError = result.IsErrAnd(err => err.Contains("Network")); // false
```

### IsOkAnd(Func<T, bool>) (Inherited)

Determines whether the result is successful and satisfies the specified predicate.

```csharp
public override bool IsOkAnd(Func<T, bool> predicate)
```

**Parameters:**

- `predicate` - The function to test the success value.

**Returns:** Always returns `false` for `Err<T, E>` instances since they represent failure.

**Exceptions:**

- `ArgumentNullException` - Thrown when `predicate` is `null`.

**Example:**

```csharp
var result = new Err<int, string>("Operation failed");
bool isPositive = result.IsOkAnd(value => value > 0); // false
```

## Usage Patterns

### Basic Creation and Access

```csharp
// Direct construction
var result = new Err<string, int>(404);

// Access the error
if (result.IsErr)
{
    int errorCode = result.Error;
    Console.WriteLine($"Error occurred: {errorCode}");
}
```

### With Factory Methods

```csharp
// Using ResultFactory for consistency
Result<string, int> result = ResultFactory.Failure<string, int>(500);

// Pattern matching (if using switch expressions)
string message = result switch
{
    Ok<string, int>(var value) => $"Success: {value}",
    Err<string, int>(var error) => $"Error code: {error}",
    _ => "Unknown result"
};
```

### Error Classification

```csharp
var result = new Err<UserData, string>("USER_NOT_FOUND");

// Classify error types
bool isUserError = result.IsErrAnd(err => err.StartsWith("USER_")); // true
bool isSystemError = result.IsErrAnd(err => err.StartsWith("SYSTEM_")); // false

// Check for specific error patterns
bool isNotFoundError = result.IsErrAnd(err => err.Contains("NOT_FOUND")); // true
```

### Custom Error Types

```csharp
// Using custom error types
public record ValidationError(string Field, string Message);

var result = new Err<User, ValidationError>(
    new ValidationError("Email", "Invalid email format")
);

bool isEmailError = result.IsErrAnd(err => err.Field == "Email"); // true
```

## Integration with Extensions

The `Err<T, E>` type works seamlessly with all Result extension methods:

```csharp
var result = new Err<int, string>("Operation failed");

// MapErr transforms the error value
Result<int, int> mappedError = result.MapErr(err => err.Length);

// OrElse provides fallback values
Result<int, string> fallback = result.OrElse(() => new Ok<int, string>(0));

// Match handles both success and error cases
string output = result.Match(
    onOk: value => $"Success: {value}",
    onErr: error => $"Failed: {error}"
);
```

## Error Propagation

```csharp
// Errors propagate through chains of operations
var result = new Err<int, string>("Initial error");

Result<string, string> final = result
    .Map(x => x.ToString())        // Skipped due to error
    .Map(s => s.ToUpper())         // Skipped due to error
    .MapErr(err => err.ToUpper()); // Transforms error: "INITIAL ERROR"

Console.WriteLine(final.IsErr); // True
```

## Design Notes

- **Immutability**: `Err<T, E>` is a record, making it immutable by default
- **Null Safety**: Constructor validation ensures the contained error is never `null`
- **Type Safety**: Generic constraints ensure both `T` and `E` are non-null reference types
- **Performance**: Sealed class enables compiler optimizations and prevents inheritance
- **Short-Circuiting**: Operations on `Err` instances typically short-circuit, preserving the error

## Common Error Patterns

### String Errors

```csharp
// Simple string error messages
var result = new Err<User, string>("User not found");
```

### Enum Error Codes

```csharp
public enum ErrorCode
{
    NotFound,
    Unauthorized,
    ValidationFailed
}

var result = new Err<User, ErrorCode>(ErrorCode.NotFound);
```

### Structured Error Objects

```csharp
public record ApiError(int Code, string Message, DateTime Timestamp);

var result = new Err<Data, ApiError>(
    new ApiError(404, "Resource not found", DateTime.UtcNow)
);
```

## See Also

- [Result<T, E> Type](./result.md) - Base result type
- [Ok<T, E> Type](./ok.md) - Success result implementation
- [ResultFactory Class](./result.md#resultfactory) - Factory methods for creating results
- [Synchronous Extensions](../extensions/sync-extensions.md) - Extension methods for Result operations
- [Asynchronous Extensions](../extensions/async-extensions.md) - Async extension methods
- [Error Handling Patterns](../../examples/error-handling-patterns.md) - Error handling best practices
