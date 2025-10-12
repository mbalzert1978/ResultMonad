# Ok&lt;T, E&gt; Type

The `Ok<T, E>` type represents a successful result containing a value in the Result monad pattern. It is a sealed record that inherits from `Result<T, E>` and indicates that an operation completed successfully with a value of type `T`.

## Namespace

```csharp
Monads.Results
```

## Syntax

```csharp
public sealed record Ok<T, E>(T Value) : Result<T, E>
    where E : notnull
    where T : notnull
```

## Type Parameters

| Parameter | Description |
|-----------|-------------|
| `T` | The type of the success value. Must be non-null. |
| `E` | The type of the error (not used in this variant but required for Result compatibility). Must be non-null. |

## Constructor

### Ok(T)

Creates a new successful result containing the specified value.

```csharp
public Ok(T Value)
```

**Parameters:**

- `Value` - The success value to contain in the result.

**Exceptions:**

- `ArgumentNullException` - Thrown if `Value` is `null`.

**Example:**

```csharp
// Create a successful result with an integer value
var result = new Ok<int, string>(42);
Console.WriteLine(result.Value); // Output: 42
Console.WriteLine(result.IsOk);  // Output: True
```

## Properties

### Value

Gets the success value contained in this result.

```csharp
public T Value { get; }
```

**Returns:** The success value of type `T`.

**Exceptions:**

- `ArgumentNullException` - Thrown during construction if the provided value is `null`.

**Example:**

```csharp
var result = new Ok<string, int>("Hello World");
string message = result.Value; // "Hello World"
```

### IsOk (Inherited)

Gets a value indicating whether the result represents a success.

```csharp
public override bool IsOk => true;
```

**Returns:** Always returns `true` for `Ok<T, E>` instances.

### IsErr (Inherited)

Gets a value indicating whether the result represents an error.

```csharp
public bool IsErr => !IsOk;
```

**Returns:** Always returns `false` for `Ok<T, E>` instances.

## Methods

### IsOkAnd(Func&lt;T, bool&gt;) (Inherited)

Determines whether the result is successful and satisfies the specified predicate.

```csharp
public override bool IsOkAnd(Func<T, bool> predicate)
```

**Parameters:**

- `predicate` - The function to test the success value.

**Returns:** `true` if the predicate returns `true` for the contained value; otherwise, `false`.

**Exceptions:**

- `ArgumentNullException` - Thrown when `predicate` is `null`.

**Example:**

```csharp
var result = new Ok<int, string>(42);
bool isPositive = result.IsOkAnd(value => value > 0); // true
bool isNegative = result.IsOkAnd(value => value < 0); // false
```

### IsErrAnd(Func&lt;E, bool&gt;) (Inherited)

Determines whether the result is an error and satisfies the specified predicate.

```csharp
public override bool IsErrAnd(Func<E, bool> predicate)
```

**Parameters:**

- `predicate` - The function to test the error value.

**Returns:** Always returns `false` for `Ok<T, E>` instances since they represent success.

**Exceptions:**

- `ArgumentNullException` - Thrown when `predicate` is `null`.

**Example:**

```csharp
var result = new Ok<int, string>(42);
bool hasError = result.IsErrAnd(err => err.Contains("error")); // false
```

## Usage Patterns

### Basic Creation and Access

```csharp
// Direct construction
var result = new Ok<int, string>(100);

// Access the value
if (result.IsOk)
{
    int value = result.Value;
    Console.WriteLine($"Success: {value}");
}
```

### With Factory Methods

```csharp
// Using ResultFactory for consistency
Result<string, int> result = ResultFactory.Success<string, int>("Operation completed");

// Pattern matching (if using switch expressions)
string message = result switch
{
    Ok<string, int>(var value) => $"Success: {value}",
    Err<string, int>(var error) => $"Error: {error}",
    _ => "Unknown result"
};
```

### Conditional Logic

```csharp
var result = new Ok<List<int>, string>(new List<int> { 1, 2, 3, 4, 5 });

// Check if the result contains a non-empty list
bool hasItems = result.IsOkAnd(list => list.Count > 0); // true

// Check if all items are positive
bool allPositive = result.IsOkAnd(list => list.All(x => x > 0)); // true
```

## Integration with Extensions

The `Ok<T, E>` type works seamlessly with all Result extension methods:

```csharp
var result = new Ok<int, string>(10);

// Map the success value
Result<string, string> mapped = result.Map(x => x.ToString());

// Chain operations
Result<int, string> doubled = result.Bind(x => new Ok<int, string>(x * 2));

// Handle both success and error cases
string output = result.Match(
    onOk: value => $"Value is {value}",
    onErr: error => $"Error: {error}"
);
```

## Design Notes

- **Immutability**: `Ok<T, E>` is a record, making it immutable by default
- **Null Safety**: Constructor validation ensures the contained value is never `null`
- **Type Safety**: Generic constraints ensure both `T` and `E` are non-null reference types
- **Performance**: Sealed class enables compiler optimizations and prevents inheritance
- **Compatibility**: Fully compatible with the base `Result<T, E>` type and all extension methods

## See Also

- [Result&lt;T, E&gt; Type](./result.md) - Base result type
- [Err&lt;T, E&gt; Type](./err.md) - Error result implementation
- [ResultFactory Class](./result.md#resultfactory) - Factory methods for creating results
- [Synchronous Extensions](../extensions/sync-extensions.md) - Extension methods for Result operations
- [Asynchronous Extensions](../extensions/async-extensions.md) - Async extension methods
- [Common Scenarios](../../examples/common-scenarios.md) - Real-world usage examples
