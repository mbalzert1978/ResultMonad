# Result Type Deep Dive

This guide provides an in-depth explanation of the `Result<T, E>` type, its variants, and implementation details.

## Table of Contents

- [Type Overview](#type-overview)
- [Type Constraints](#type-constraints)
- [The Result Abstract Type](#the-result-abstract-type)
- [Ok Variant](#ok-variant)
- [Err Variant](#err-variant)
- [ResultFactory](#resultfactory)
- [Internal Implementation](#internal-implementation)
- [Design Decisions](#design-decisions)

## Type Overview

`Result<T, E>` is implemented as an **abstract record** with two concrete variants:

```csharp
// Abstract base
public abstract record Result<T, E>
    where T : notnull
    where E : notnull
{
    public abstract bool IsOk { get; }
    public bool IsErr => !IsOk;
    public abstract bool IsOkAnd(Func<T, bool> predicate);
    public abstract bool IsErrAnd(Func<E, bool> predicate);
}

// Success variant
public record Ok<T, E>(T Value) : Result<T, E>
    where T : notnull
    where E : notnull;

// Error variant
public record Err<T, E>(E Error) : Result<T, E>
    where T : notnull
    where E : notnull;
```

### Discriminated Union

Result is a **discriminated union** (also called **tagged union** or **sum type**):

- **Only one variant exists at runtime**: A Result is **either** Ok **or** Err, never both
- **Type-safe**: The compiler ensures you handle both cases
- **No null**: Both value and error must be non-null

## Type Constraints

Both type parameters have **non-null constraints**:

```csharp
where T : notnull
where E : notnull
```

### What This Means

**For value types (int, bool, DateTime, etc.):**

```csharp
// ✅ Valid - value types are always non-null
Result<int, string> r1 = new Ok<int, string>(42);
Result<bool, ErrorCode> r2 = new Ok<bool, ErrorCode>(true);
```

**For reference types:**

```csharp
// ✅ Valid - reference types with non-null guarantee
Result<string, Exception> r1 = new Ok<string, Exception>("hello");

// ❌ Invalid - nullable types not allowed
Result<string?, string> r2 = ...  // Compilation error
Result<int, string?> r3 = ...     // Compilation error
```

**For custom types:**

```csharp
// ✅ Valid - class types
Result<User, AppError> r1 = new Ok<User, AppError>(user);

// ✅ Valid - record types
Result<UserData, ValidationError> r2 = new Ok<UserData, ValidationError>(data);

// ✅ Valid - struct types
Result<Point, string> r3 = new Ok<Point, string>(new Point(1, 2));
```

### Why Non-Null?

1. **Clarity**: Result handles success/failure, not null/non-null
2. **Type safety**: No need to check for null within the monad
3. **Correctness**: Prevents `Result<null, error>` invalid states

## The Result Abstract Type

### Properties

#### IsOk

```csharp
public abstract bool IsOk { get; }
```

Indicates whether the result is a success (`Ok`).

**Usage:**

```csharp
Result<int, string> result = Divide(10, 2);

if (result.IsOk)
{
    Console.WriteLine("Operation succeeded");
}
```

#### IsErr

```csharp
public bool IsErr => !IsOk;
```

Computed property that indicates whether the result is an error (`Err`).

**Implementation note:** `IsErr` is derived from `IsOk` to ensure consistency.

### Methods

#### IsOkAnd

```csharp
public abstract bool IsOkAnd(Func<T, bool> predicate);
```

Checks if the result is `Ok` **and** the value satisfies a condition.

**Parameters:**

- `predicate`: Function to test the success value

**Returns:** `true` if Ok and predicate returns `true`, otherwise `false`

**Throws:** `ArgumentNullException` if predicate is null

**Usage:**

```csharp
Result<int, string> result = new Ok<int, string>(42);

// Check if Ok and even
bool isEven = result.IsOkAnd(x => x % 2 == 0);  // true

// Works with Err too
Result<int, string> error = new Err<int, string>("failed");
bool nope = error.IsOkAnd(x => x % 2 == 0);  // false (not Ok)
```

#### IsErrAnd

```csharp
public abstract bool IsErrAnd(Func<E, bool> predicate);
```

Checks if the result is `Err` **and** the error satisfies a condition.

**Parameters:**

- `predicate`: Function to test the error value

**Returns:** `true` if Err and predicate returns `true`, otherwise `false`

**Throws:** `ArgumentNullException` if predicate is null

**Usage:**

```csharp
Result<int, string> error = new Err<int, string>("Division by zero");

// Check if Err and contains specific text
bool isDivisionError = error.IsErrAnd(e => e.Contains("Division"));  // true

// Works with Ok too
Result<int, string> success = new Ok<int, string>(42);
bool nope = success.IsErrAnd(e => e.Contains("Division"));  // false (not Err)
```

## Ok Variant

The success variant of Result.

### Ok Syntax

```csharp
public record Ok<T, E>(T Value) : Result<T, E>
    where T : notnull
    where E : notnull;
```

### Ok Properties

#### Value

```csharp
public T Value { get; }
```

The success value contained in this Ok result.

**Access:**

```csharp
Ok<int, string> ok = new Ok<int, string>(42);
int value = ok.Value;  // 42
```

**Warning:** Accessing `Value` on an `Err` will throw. Always check `IsOk` first or use pattern matching.

#### Ok.IsOk

Always returns `true` for Ok variants.

```csharp
Ok<int, string> ok = new Ok<int, string>(42);
Console.WriteLine(ok.IsOk);  // True
Console.WriteLine(ok.IsErr); // False
```

### Ok Construction

```csharp
// Direct construction
var result = new Ok<int, string>(42);

// Via factory method
var result = ResultFactory.Success<int, string>(42);

// Implicit from function return
Result<int, string> Divide(int a, int b)
{
    return new Ok<int, string>(a / b);
}
```

### Ok Equality

Ok is a **record**, so it has value-based equality:

```csharp
var ok1 = new Ok<int, string>(42);
var ok2 = new Ok<int, string>(42);
var ok3 = new Ok<int, string>(99);

Console.WriteLine(ok1 == ok2);  // True (same value)
Console.WriteLine(ok1 == ok3);  // False (different value)
```

## Err Variant

The error variant of Result.

### Err Syntax

```csharp
public record Err<T, E>(E Error) : Result<T, E>
    where T : notnull
    where E : notnull;
```

### Err Properties

#### Error

```csharp
public E Error { get; }
```

The error value contained in this Err result.

**Access:**

```csharp
Err<int, string> err = new Err<int, string>("failed");
string error = err.Error;  // "failed"
```

**Warning:** Accessing `Error` on an `Ok` will throw. Always check `IsErr` first or use pattern matching.

#### Err.IsOk

Always returns `false` for Err variants.

```csharp
Err<int, string> err = new Err<int, string>("failed");
Console.WriteLine(err.IsOk);  // False
Console.WriteLine(err.IsErr); // True
```

### Err Construction

```csharp
// Direct construction
var result = new Err<int, string>("Something went wrong");

// Via factory method
var result = ResultFactory.Failure<int, string>("Something went wrong");

// Implicit from function return
Result<int, string> Divide(int a, int b)
{
    if (b == 0)
        return new Err<int, string>("Division by zero");
    return new Ok<int, string>(a / b);
}
```

### Err Equality

Err is also a **record** with value-based equality:

```csharp
var err1 = new Err<int, string>("error");
var err2 = new Err<int, string>("error");
var err3 = new Err<int, string>("different");

Console.WriteLine(err1 == err2);  // True (same error)
Console.WriteLine(err1 == err3);  // False (different error)
```

## ResultFactory

Static factory methods for creating Result instances with validation.

### Success\<T, E\>

```csharp
public static Result<T, E> Success<T, E>(T value)
```

Creates an Ok result with runtime null-checking.

**Usage:**

```csharp
var result = ResultFactory.Success<int, string>(42);
```

**Throws:** `ArgumentNullException` if value is null

**Internal assertion:** Verifies `IsOk == true` in debug builds

### Failure\<T, E\>

```csharp
public static Result<T, E> Failure<T, E>(E error)
```

Creates an Err result with runtime null-checking.

**Usage:**

```csharp
var result = ResultFactory.Failure<int, string>("error");
```

**Throws:** `ArgumentNullException` if error is null

**Internal assertion:** Verifies `IsErr == true` in debug builds

### When to Use Factory Methods

**Use direct construction:**

```csharp
// ✅ When types are obvious
return new Ok<int, string>(42);
return new Err<int, string>("error");
```

**Use factory methods:**

```csharp
// ✅ When you want explicit null checking
var result = ResultFactory.Success<User, string>(maybeNullUser);  // Throws if null

// ✅ When type inference helps
var result = ResultFactory.Failure<int, AppError>(error);
```

## Internal Implementation

### Record Types

Result uses **C# records** for several benefits:

1. **Value-based equality**: Two results with same value are equal
2. **Immutability**: Results can't be modified after creation
3. **with expressions**: Copy with modifications (though rarely needed)
4. **Deconstruction**: Pattern matching support

### Abstract Methods

`IsOkAnd` and `IsErrAnd` are abstract, forcing each variant to implement them:

**Ok implementation:**

```csharp
public override bool IsOkAnd(Func<T, bool> predicate)
{
    ArgumentNullException.ThrowIfNull(predicate);
    return predicate(Value);  // Apply predicate to value
}

public override bool IsErrAnd(Func<E, bool> predicate)
{
    ArgumentNullException.ThrowIfNull(predicate);
    return false;  // Ok is never an error
}
```

**Err implementation:**

```csharp
public override bool IsOkAnd(Func<T, bool> predicate)
{
    ArgumentNullException.ThrowIfNull(predicate);
    return false;  // Err is never ok
}

public override bool IsErrAnd(Func<E, bool> predicate)
{
    ArgumentNullException.ThrowIfNull(predicate);
    return predicate(Error);  // Apply predicate to error
}
```

### Performance Characteristics

- **No heap allocations** beyond the Result instance itself
- **No boxing** for value types
- **Single dispatch** for IsOk/IsErr checks
- **Virtual dispatch** for IsOkAnd/IsErrAnd (abstract methods)

## Design Decisions

### Why Abstract Record?

**Alternatives considered:**

1. **Sealed class with internal discriminator**
   - ❌ More boilerplate
   - ❌ Less type safety
   - ✅ Slightly better performance

2. **Struct with discriminator**
   - ❌ Can't be abstract
   - ❌ Nullable issues
   - ✅ No heap allocation

3. **Abstract record with derived records** (chosen)
   - ✅ Type-safe discriminated union
   - ✅ Value-based equality
   - ✅ Immutability guaranteed
   - ✅ Clean syntax

### Why Two Type Parameters?

Having both `T` and `E` explicit provides:

1. **Flexibility**: Different functions can have different error types
2. **Type safety**: Compiler knows both success and error types
3. **Composability**: Extensions can transform either type

### Why notnull Constraints?

Enforcing non-null ensures:

1. **Result handles success/failure, not null/non-null**
2. **No invalid states** like `Result<null, E>`
3. **Clearer semantics**: Use `Option<T>` for nullable scenarios

### Why Not Sealed?

Leaving Result abstract (not sealed) allows:

- Internal implementation changes
- Potential future extensions
- Testing/mocking scenarios

However, users should **not** derive custom variants. Only `Ok` and `Err` should exist.

## Comparison with Other Languages

### Rust

```rust
enum Result<T, E> {
    Ok(T),
    Err(E),
}
```

C# doesn't have native enums with associated data, so we use abstract records.

### F-Sharp

```fsharp
type Result<'T, 'E> =
    | Ok of 'T
    | Error of 'E
```

Similar discriminated union, but F# has language-level support.

### Haskell

```haskell
data Either a b = Left a | Right b
```

Haskell's Either is more general (no implied success/failure semantics).

## Next Steps

- 📖 [Basic Concepts](../getting-started/basic-concepts.md) - Learn how to use Result
- 📖 [Monad Pattern](monad-pattern.md) - Understand the theory
- 📖 [Error Handling](error-handling.md) - Best practices
- 📖 [API Reference](../api/) - Complete method documentation

## Summary

**Key Points:**

✅ `Result<T, E>` is an abstract discriminated union

✅ Two variants: `Ok<T, E>` (success) and `Err<T, E>` (error)

✅ Both T and E must be non-null types

✅ Value-based equality via C# records

✅ Type-safe: compiler ensures handling both cases

✅ Abstract methods ensure variant-specific behavior

The Result type provides a robust foundation for type-safe error handling in C#, leveraging modern language features like records and generic constraints to ensure correctness at compile time.
