# Unit Type

The `Unit` type represents the absence of a meaningful value, similar to `void` in C# methods but as a proper type that can be used in generic contexts. It is commonly used in functional programming to indicate operations that perform side effects but don't return a meaningful value.

## Namespace

```csharp
Monads.Results
```

## Syntax

```csharp
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, ISpanFormattable
```

## Overview

The `Unit` type is a singleton - all instances are equal and represent the same concept of "no meaningful value." It's particularly useful in the Result monad pattern when operations need to indicate success without returning data.

## Properties

### Default

Gets the default `Unit` instance.

```csharp
public static readonly Unit Default;
```

**Returns:** The singleton instance of `Unit`.

**Example:**

```csharp
Unit unit = Unit.Default;
Console.WriteLine(unit); // Output: ()
```

## Methods

### CompareTo(Unit)

Compares the current instance with another `Unit` instance.

```csharp
public int CompareTo(Unit other)
```

**Parameters:**

- `other` - The `Unit` instance to compare with.

**Returns:** Always returns `0` since all `Unit` instances are equal.

**Example:**

```csharp
Unit unit1 = Unit.Default;
Unit unit2 = default(Unit);
int comparison = unit1.CompareTo(unit2); // Always 0
```

### Equals(Unit)

Determines whether the current instance is equal to another `Unit` instance.

```csharp
public bool Equals(Unit other)
```

**Parameters:**

- `other` - The `Unit` instance to compare with.

**Returns:** Always returns `true` since all `Unit` instances are equal.

### Equals(object?)

Determines whether the specified object is equal to the current instance.

```csharp
public override bool Equals(object? obj)
```

**Parameters:**

- `obj` - The object to compare with the current instance.

**Returns:** `true` if the specified object is a `Unit`; otherwise, `false`.

### GetHashCode()

Returns the hash code for the current instance.

```csharp
public override int GetHashCode()
```

**Returns:** Always returns `0` since all `Unit` instances are equal.

### ToString()

Returns the string representation of the current instance.

```csharp
public override string ToString()
```

**Returns:** Always returns `"()"`.

**Example:**

```csharp
Unit unit = Unit.Default;
Console.WriteLine(unit.ToString()); // Output: ()
```

### ToString(string?, IFormatProvider?)

Formats the value of the current instance using the specified format.

```csharp
public string ToString(string? format, IFormatProvider? formatProvider)
```

**Parameters:**

- `format` - The format to use (ignored).
- `formatProvider` - The format provider to use (ignored).

**Returns:** Always returns `"()"`.

### TryFormat(Span&lt;char&gt;, out int, ReadOnlySpan&lt;char&gt;, IFormatProvider?)

Tries to format the value of the current instance into the provided span of characters.

```csharp
public bool TryFormat(
    Span<char> destination,
    out int charsWritten,
    ReadOnlySpan<char> format,
    IFormatProvider? provider
)
```

**Parameters:**

- `destination` - The span to write the formatted value into.
- `charsWritten` - The number of characters written to the destination.
- `format` - The format to use (ignored).
- `provider` - The format provider to use (ignored).

**Returns:** `true` if formatting was successful; otherwise, `false`.

## Operators

### Equality Operators

```csharp
public static bool operator ==(Unit left, Unit right) // Always returns true
public static bool operator !=(Unit left, Unit right) // Always returns false
```

All `Unit` instances are considered equal.

### Comparison Operators

```csharp
public static bool operator <(Unit left, Unit right)   // Always returns false
public static bool operator <=(Unit left, Unit right)  // Always returns true
public static bool operator >(Unit left, Unit right)   // Always returns false
public static bool operator >=(Unit left, Unit right)  // Always returns true
```

### Addition Operator

```csharp
public static Unit operator +(Unit left, Unit right)
```

Adds two `Unit` instances, returning a `Unit`.

### Implicit Conversions

```csharp
public static implicit operator ValueTuple(Unit unit)
public static implicit operator Unit(ValueTuple tuple)
```

Provides seamless conversion between `Unit` and `ValueTuple` (empty tuple).

## Usage Patterns

### With Result Types

The `Unit` type is commonly used with `Result<T, E>` when an operation succeeds but doesn't return meaningful data:

```csharp
// Operation that either succeeds (no return value) or fails with an error
Result<Unit, string> SaveData(string data)
{
    try
    {
        // Perform save operation
        return new Ok<Unit, string>(Unit.Default);
    }
    catch (Exception ex)
    {
        return new Err<Unit, string>(ex.Message);
    }
}

// Usage
Result<Unit, string> result = SaveData("some data");
if (result.IsOk)
{
    Console.WriteLine("Data saved successfully");
}
```

### Method Return Types

Use `Unit` instead of `void` when you need a proper type:

```csharp
// Function that performs side effects
Func<string, Unit> logMessage = message =>
{
    Console.WriteLine($"[LOG] {message}");
    return Unit.Default;
};

// Chain with other operations
Unit result = logMessage("Operation started");
```

### Async Operations

```csharp
// Async operation that completes without returning data
async Task<Result<Unit, string>> SendNotificationAsync(string message)
{
    try
    {
        await notificationService.SendAsync(message);
        return new Ok<Unit, string>(Unit.Default);
    }
    catch (Exception ex)
    {
        return new Err<Unit, string>(ex.Message);
    }
}
```

### Collections of Operations

```csharp
// List of operations that don't return meaningful values
List<Func<Unit>> operations = new List<Func<Unit>>
{
    () => { Console.WriteLine("Operation 1"); return Unit.Default; },
    () => { Console.WriteLine("Operation 2"); return Unit.Default; },
    () => { Console.WriteLine("Operation 3"); return Unit.Default; }
};

// Execute all operations
operations.ForEach(op => op());
```

## Integration with Extensions

The `Unit` type works seamlessly with Result extension methods:

```csharp
Result<Unit, string> result = SaveUserData(user);

// Chain operations that don't return values
Result<Unit, string> final = result
    .Bind(_ => LogOperation("User saved"))
    .Bind(_ => SendNotification("User created"))
    .Map(_ => Unit.Default); // Transform if needed

// Handle the result
string message = final.Match(
    onOk: _ => "All operations completed successfully",
    onErr: error => $"Operation failed: {error}"
);
```

## Design Philosophy

- **Singleton Pattern**: All `Unit` instances are identical, reducing memory usage
- **Immutability**: `Unit` is a readonly struct, ensuring thread safety
- **Interoperability**: Implicit conversion to/from `ValueTuple` for framework compatibility
- **Functional Style**: Enables functional programming patterns in C#
- **Type Safety**: Provides a proper type where `void` cannot be used

## Performance Considerations

- **Zero Allocation**: `Unit` is a struct with no heap allocation
- **Compile-Time Optimization**: Comparisons and operations are optimized away by the JIT
- **Minimal Memory Footprint**: Empty struct takes no additional memory
- **Cache Friendly**: All instances are identical, improving CPU cache performance

## Common Use Cases

1. **Side Effect Operations**: Functions that perform logging, validation, or I/O
2. **Result Types**: Success cases that don't return data
3. **Event Handlers**: Operations triggered by events
4. **Async Workflows**: Async operations that indicate completion
5. **Functional Composition**: Chaining operations in functional style

## See Also

- [Result&lt;T, E&gt; Type](./result.md) - Base result type that commonly uses Unit
- [Ok&lt;T, E&gt; Type](./ok.md) - Success result that can contain Unit
- [Synchronous Extensions](../extensions/sync-extensions.md) - Extensions that work with Unit
- [Asynchronous Extensions](../extensions/async-extensions.md) - Async extensions for Unit operations
- [Common Scenarios](../../examples/common-scenarios.md) - Real-world Unit usage examples
