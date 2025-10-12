# Synchronous Extensions

The synchronous extension methods provide essential operations for working with `Result<T, E>` types in a functional programming style. These methods enable chaining, transformation, and composition of results without explicit conditional logic.

## Namespace

```csharp
Monads.Results.Extensions.Sync
```

## Overview

All synchronous extension methods follow these principles:

- **Immutability**: Original results are never modified; new results are returned
- **Error Propagation**: Errors automatically propagate through operation chains
- **Null Safety**: All methods validate inputs and prevent null values
- **Functional Composition**: Methods can be chained together fluently

## Extension Methods

### Bind

Chains operations that return `Result<T, E>` types (monadic bind operation).

```csharp
public static Result<U, E> Bind<T, E, U>(
    this Result<T, E> self,
    Func<T, Result<U, E>> operation
)
```

**Purpose:** Enables sequential composition of operations that can fail.

**Behavior:**

- If `self` is `Ok<T, E>`, applies `operation` to the contained value
- If `self` is `Err<T, E>`, propagates the error without calling `operation`

**Parameters:**

- `self` - The input result
- `operation` - Function that transforms the success value to a new result

**Returns:** A new result from the operation, or the propagated error.

**Exceptions:**

- `ArgumentNullException` - When `self` or `operation` is `null`
- `InvalidOperationException` - When operation returns `null`

**Example:**

```csharp
Result<int, string> ParseNumber(string input) => 
    int.TryParse(input, out var result) 
        ? new Ok<int, string>(result)
        : new Err<int, string>("Invalid number");

Result<bool, string> IsEven(int number) => 
    new Ok<bool, string>(number % 2 == 0);

// Chain operations that can fail
Result<string, string> input = new Ok<string, string>("42");
Result<bool, string> result = input
    .Bind(s => ParseNumber(s))    // string -> Result<int, string>
    .Bind(i => IsEven(i));        // int -> Result<bool, string>
// result is Ok(true)

// Error propagation
Result<string, string> badInput = new Ok<string, string>("invalid");
Result<bool, string> errorResult = badInput
    .Bind(s => ParseNumber(s))    // Returns Err("Invalid number")
    .Bind(i => IsEven(i));        // Skipped due to previous error
// errorResult is Err("Invalid number")
```

### Map

Transforms the success value using a regular function (not returning a Result).

```csharp
public static Result<U, E> Map<T, E, U>(
    this Result<T, E> self,
    Func<T, U> operation
)
```

**Purpose:** Applies a transformation to the success value without risk of failure.

**Behavior:**

- If `self` is `Ok<T, E>`, applies `operation` and wraps result in `Ok<U, E>`
- If `self` is `Err<T, E>`, propagates the error without calling `operation`

**Parameters:**

- `self` - The input result
- `operation` - Function that transforms the success value

**Returns:** A new result with the transformed value, or the propagated error.

**Exceptions:**

- `ArgumentNullException` - When `self` or `operation` is `null`
- `InvalidOperationException` - When operation returns `null`

**Example:**

```csharp
Result<int, string> number = new Ok<int, string>(42);
Result<string, string> text = number.Map(x => x.ToString());
// text is Ok("42")

Result<string, string> doubled = number
    .Map(x => x * 2)           // int -> int
    .Map(x => x.ToString());   // int -> string
// doubled is Ok("84")

// Error case
Result<int, string> error = new Err<int, string>("Failed");
Result<string, string> errorMapped = error.Map(x => x.ToString());
// errorMapped is Err("Failed") - operation not called
```

### MapErr

Transforms the error value while leaving success values unchanged.

```csharp
public static Result<T, U> MapErr<T, E, U>(
    this Result<T, E> self,
    Func<E, U> operation
)
```

**Purpose:** Transforms or translates error values to different types.

**Behavior:**

- If `self` is `Ok<T, E>`, propagates the success value unchanged
- If `self` is `Err<T, E>`, applies `operation` to transform the error

**Parameters:**

- `self` - The input result
- `operation` - Function that transforms the error value

**Returns:** A new result with the transformed error, or the original success.

**Example:**

```csharp
Result<int, string> error = new Err<int, string>("404");
Result<int, int> numericError = error.MapErr(err => int.Parse(err));
// numericError is Err(404)

// Success case - error transformation ignored
Result<int, string> success = new Ok<int, string>(42);
Result<int, int> successMapped = success.MapErr(err => int.Parse(err));
// successMapped is Ok(42) - MapErr operation not called
```

### Match

Pattern matches on the result, applying different functions for Ok and Err cases.

```csharp
public static U Match<T, E, U>(
    this Result<T, E> self,
    Func<T, U> onOk,
    Func<E, U> onErr
)
```

**Purpose:** Extracts values from results by handling both success and error cases.

**Behavior:** Always calls exactly one of the provided functions based on the result state.

**Parameters:**

- `self` - The input result
- `onOk` - Function to apply if result is successful
- `onErr` - Function to apply if result is an error

**Returns:** The value returned by the appropriate function.

**Example:**

```csharp
Result<int, string> success = new Ok<int, string>(42);
Result<int, string> error = new Err<int, string>("Failed");

string successMessage = success.Match(
    onOk: value => $"Success: {value}",
    onErr: err => $"Error: {err}"
);
// successMessage is "Success: 42"

string errorMessage = error.Match(
    onOk: value => $"Success: {value}",
    onErr: err => $"Error: {err}"
);
// errorMessage is "Error: Failed"

// Extract different types
int result = success.Match(
    onOk: value => value * 2,
    onErr: err => -1
);
// result is 84
```

### Flatten

Flattens nested Result types (removes one level of nesting).

```csharp
public static Result<T, E> Flatten<T, E>(
    this Result<Result<T, E>, E> self
)
```

**Purpose:** Simplifies nested Result structures that can occur from certain operations.

**Behavior:**

- If outer result is `Ok<Result<T, E>, E>`, returns the inner result
- If outer result is `Err<Result<T, E>, E>`, returns the error

**Parameters:**

- `self` - The nested result to flatten

**Returns:** A flattened result.

**Example:**

```csharp
// Create nested result
Result<Result<int, string>, string> nested = 
    new Ok<Result<int, string>, string>(
        new Ok<int, string>(42)
    );

Result<int, string> flattened = nested.Flatten();
// flattened is Ok(42)

// Nested error case
Result<Result<int, string>, string> nestedError = 
    new Ok<Result<int, string>, string>(
        new Err<int, string>("Inner error")
    );

Result<int, string> flattenedError = nestedError.Flatten();
// flattenedError is Err("Inner error")
```

### OrElse

Provides a fallback result when the current result is an error.

```csharp
public static Result<T, E> OrElse<T, E>(
    this Result<T, E> self,
    Func<Result<T, E>> fallback
)
```

**Purpose:** Implements fallback logic for error recovery.

**Behavior:**

- If `self` is `Ok<T, E>`, returns `self` unchanged
- If `self` is `Err<T, E>`, calls `fallback` and returns its result

**Parameters:**

- `self` - The input result
- `fallback` - Function that provides an alternative result

**Returns:** The original result if successful, otherwise the fallback result.

**Example:**

```csharp
Result<int, string> primary = new Err<int, string>("Primary failed");
Result<int, string> backup = primary.OrElse(() => new Ok<int, string>(0));
// backup is Ok(0)

// Success case - fallback not called
Result<int, string> success = new Ok<int, string>(42);
Result<int, string> withFallback = success.OrElse(() => new Ok<int, string>(0));
// withFallback is Ok(42) - fallback function not called

// Chaining fallbacks
Result<int, string> multipleBackups = primary
    .OrElse(() => new Err<int, string>("Backup 1 failed"))
    .OrElse(() => new Err<int, string>("Backup 2 failed"))
    .OrElse(() => new Ok<int, string>(99));
// multipleBackups is Ok(99)
```

## Chaining Operations

The real power of these extensions comes from chaining them together:

```csharp
Result<string, string> ProcessData(string input)
{
    return ValidateInput(input)           // string -> Result<string, string>
        .Bind(ParseData)                  // string -> Result<Data, string>
        .Map(data => data.Process())      // Data -> ProcessedData
        .Map(processed => processed.Id)   // ProcessedData -> string
        .MapErr(err => $"Processing failed: {err}")
        .OrElse(() => new Ok<string, string>("DEFAULT_ID"));
}
```

## Error Handling Patterns

### Early Return Pattern

```csharp
public Result<ComplexResult, Error> ComplexOperation(Input input)
{
    return ValidateInput(input)
        .Bind(ProcessStep1)
        .Bind(ProcessStep2)
        .Bind(ProcessStep3)
        .Map(FinalTransform);
    // If any step fails, subsequent steps are skipped
}
```

### Fallback Chain Pattern

```csharp
public Result<Data, string> GetDataWithFallbacks(string id)
{
    return GetFromCache(id)
        .OrElse(() => GetFromDatabase(id))
        .OrElse(() => GetFromApi(id))
        .OrElse(() => GetDefaultData(id));
}
```

### Validation Pattern

```csharp
public Result<User, string> CreateUser(UserInput input)
{
    return ValidateEmail(input.Email)
        .Bind(_ => ValidateAge(input.Age))
        .Bind(_ => ValidateUsername(input.Username))
        .Map(_ => new User(input));
}
```

## Performance Considerations

- **Minimal Allocation**: Extension methods minimize object allocation
- **Short-Circuiting**: Error cases skip subsequent operations
- **JIT Optimization**: Simple delegate calls are often inlined
- **No Boxing**: Value types remain on the stack when possible

## Thread Safety

All synchronous extension methods are thread-safe as they:

- Don't modify existing results (immutability)
- Don't share mutable state
- Create new instances for return values

## See Also

- [Result<T, E> Type](../models/result.md) - Base result type
- [Asynchronous Extensions](./async-extensions.md) - Async versions of these operations
- [Ok<T, E> Type](../models/ok.md) - Success result implementation
- [Err<T, E> Type](../models/err.md) - Error result implementation
- [Common Scenarios](../../examples/common-scenarios.md) - Real-world usage patterns
- [Error Handling Patterns](../../examples/error-handling-patterns.md) - Error handling best practices
