# Basic Concepts

This guide introduces the fundamental concepts of ResultMonad and how to work with Result types effectively.

## Table of Contents

- [The Result Type](#the-result-type)
- [Ok and Err](#ok-and-err)
- [Type Safety](#type-safety)
- [Basic Operations](#basic-operations)
- [Pattern Matching](#pattern-matching)
- [Common Use Cases](#common-use-cases)

## The Result Type

`Result<T, E>` is an **abstract discriminated union** that represents one of two possible states:

- **Success**: Contains a value of type `T` (represented by `Ok<T, E>`)
- **Error**: Contains an error of type `E` (represented by `Err<T, E>`)

### Why Use Result?

Traditional C# error handling relies on exceptions:

```csharp
// Traditional approach with exceptions
public int Divide(int a, int b)
{
    if (b == 0)
        throw new DivideByZeroException("Cannot divide by zero");
    
    return a / b;
}

// Caller must remember to catch
try
{
    var result = Divide(10, 0);
}
catch (DivideByZeroException ex)
{
    // Handle error
}
```

**Problems with exceptions:**

- ❌ Not visible in method signature
- ❌ Easy to forget to handle
- ❌ Performance overhead
- ❌ Breaks functional composition

**With Result:**

```csharp
// Result-based approach
public Result<int, string> Divide(int a, int b)
{
    if (b == 0)
        return new Err<int, string>("Cannot divide by zero");
    
    return new Ok<int, string>(a / b);
}

// Error handling is explicit and required
var result = Divide(10, 0);
result.Match(
    ok: value => Console.WriteLine($"Success: {value}"),
    err: error => Console.WriteLine($"Error: {error}")
);
```

**Benefits:**

- ✅ Errors are explicit in the type signature
- ✅ Compiler ensures you handle both cases
- ✅ No performance overhead
- ✅ Composable with functional operations

## Ok and Err

### Ok\<T, E\>: Success Variant

`Ok<T, E>` represents a successful result containing a value.

```csharp
// Creating an Ok result
Result<int, string> success = new Ok<int, string>(42);

Console.WriteLine(success.IsOk);   // True
Console.WriteLine(success.IsErr);  // False
```

**Key Properties:**

- `IsOk`: Always `true`
- `IsErr`: Always `false`
- `Value`: The success value (only accessible in `Ok`)

### Err\<T, E\>: Error Variant

`Err<T, E>` represents a failed result containing an error.

```csharp
// Creating an Err result
Result<int, string> failure = new Err<int, string>("Something went wrong");

Console.WriteLine(failure.IsOk);   // False
Console.WriteLine(failure.IsErr);  // True
```

**Key Properties:**

- `IsOk`: Always `false`
- `IsErr`: Always `true`
- `Error`: The error value (only accessible in `Err`)

### Type Parameters

Both `Ok` and `Err` require **two type parameters**:

```csharp
// T: Type of success value (int)
// E: Type of error value (string)
Result<int, string> result = new Ok<int, string>(42);
```

**Important:** Both type parameters must be **non-nullable** reference or value types.

## Type Safety

ResultMonad enforces type safety through constraints:

```csharp
// ✅ Valid - both types are non-null
Result<int, string> r1 = new Ok<int, string>(42);
Result<User, ErrorCode> r2 = new Ok<User, ErrorCode>(user);

// ❌ Invalid - nullable types not allowed
// Result<int?, string> r3 = ...  // Won't compile
// Result<int, string?> r4 = ...  // Won't compile
```

### Choosing Error Types

You can use any non-nullable type for errors:

```csharp
// String errors (simple, human-readable)
Result<User, string> r1 = new Err<User, string>("User not found");

// Exception types (preserves exception info)
Result<Data, Exception> r2 = new Err<Data, Exception>(new FileNotFoundException());

// Custom error enums (structured, localized)
public enum ErrorCode { NotFound, Unauthorized, ValidationFailed }
Result<User, ErrorCode> r3 = new Err<User, ErrorCode>(ErrorCode.NotFound);

// Custom error classes (rich context)
public record AppError(string Code, string Message, Dictionary<string, string> Details);
Result<User, AppError> r4 = new Err<User, AppError>(
    new AppError("USR001", "Invalid user", new())
);
```

## Basic Operations

### Checking Result State

```csharp
Result<int, string> result = Divide(10, 2);

// Check if success
if (result.IsOk)
{
    Console.WriteLine("Operation succeeded");
}

// Check if error
if (result.IsErr)
{
    Console.WriteLine("Operation failed");
}
```

### Conditional Checking with Predicates

```csharp
Result<int, string> result = new Ok<int, string>(42);

// Check if Ok AND value satisfies condition
bool isEven = result.IsOkAnd(value => value % 2 == 0);  // True

// Check if Err AND error satisfies condition
bool isSpecificError = result.IsErrAnd(error => error.Contains("zero"));  // False
```

### Map: Transform Success Values

`Map` applies a function to the success value, leaving errors unchanged:

```csharp
Result<int, string> result = new Ok<int, string>(5);

// Transform the value
Result<int, string> doubled = result.Map(x => x * 2);  // Ok(10)

// Map does nothing to errors
Result<int, string> error = new Err<int, string>("fail");
Result<int, string> stillError = error.Map(x => x * 2);  // Still Err("fail")
```

**Type transformation:**

```csharp
Result<int, string> intResult = new Ok<int, string>(42);

// Map can change the success type
Result<string, string> stringResult = intResult.Map(x => x.ToString());  // Ok("42")
```

### MapErr: Transform Error Values

`MapErr` transforms the error, leaving success unchanged:

```csharp
Result<int, string> error = new Err<int, string>("error");

// Transform the error
Result<int, string> prefixed = error.MapErr(e => $"[ERROR] {e}");
// Err("[ERROR] error")

// MapErr does nothing to success
Result<int, string> success = new Ok<int, string>(42);
Result<int, string> stillSuccess = success.MapErr(e => $"[ERROR] {e}");  // Still Ok(42)
```

### Bind: Chain Operations

`Bind` (also known as `flatMap` or `andThen`) chains operations that return `Result`:

```csharp
Result<int, string> Divide(int a, int b)
{
    if (b == 0)
        return new Err<int, string>("Division by zero");
    return new Ok<int, string>(a / b);
}

Result<int, string> MultiplyByTwo(int x)
{
    return new Ok<int, string>(x * 2);
}

// Chain operations
var result = Divide(20, 4)          // Ok(5)
    .Bind(x => Divide(x, 5))        // Ok(1)
    .Bind(MultiplyByTwo);           // Ok(2)

// Error short-circuits the chain
var errorResult = Divide(20, 0)     // Err("Division by zero")
    .Bind(x => Divide(x, 5))        // Skipped
    .Bind(MultiplyByTwo);           // Still Err("Division by zero")
```

**Key difference from Map:**

- `Map`: Function returns `T` → Result wraps it automatically
- `Bind`: Function returns `Result<T, E>` → No double wrapping

### OrElse: Provide Fallback

`OrElse` recovers from errors by providing an alternative result:

```csharp
Result<int, string> error = new Err<int, string>("failed");

// Provide fallback on error
Result<int, string> recovered = error.OrElse(e => 
{
    Console.WriteLine($"Recovering from: {e}");
    return new Ok<int, string>(0);  // Default value
});
// Ok(0)

// OrElse does nothing to success
Result<int, string> success = new Ok<int, string>(42);
Result<int, string> unchanged = success.OrElse(e => new Ok<int, string>(0));
// Still Ok(42)
```

### Flatten: Unwrap Nested Results

When you have `Result<Result<T, E>, E>`, use `Flatten` to simplify:

```csharp
Result<Result<int, string>, string> nested = 
    new Ok<Result<int, string>, string>(new Ok<int, string>(42));

Result<int, string> flattened = nested.Flatten();  // Ok(42)
```

## Pattern Matching

`Match` is the primary way to extract values from a Result:

```csharp
Result<int, string> result = Divide(10, 2);

// Pattern match on both cases
string message = result.Match(
    ok: value => $"Success: {value}",
    err: error => $"Error: {error}"
);
```

**Both functions must return the same type:**

```csharp
// ✅ Valid - both return string
string msg = result.Match(
    ok: v => $"Value: {v}",
    err: e => $"Error: {e}"
);

// ✅ Valid - both return void
result.Match(
    ok: v => Console.WriteLine($"Value: {v}"),
    err: e => Console.WriteLine($"Error: {e}")
);

// ❌ Invalid - different return types
// var x = result.Match(
//     ok: v => v,           // Returns int
//     err: e => e           // Returns string
// );
```

## Common Use Cases

### 1. Validation

```csharp
public Result<User, string> ValidateUser(string username, int age)
{
    if (string.IsNullOrWhiteSpace(username))
        return new Err<User, string>("Username cannot be empty");
    
    if (age < 18)
        return new Err<User, string>("User must be 18 or older");
    
    return new Ok<User, string>(new User(username, age));
}
```

### 2. Safe Parsing

```csharp
public Result<int, string> ParseInt(string input)
{
    if (int.TryParse(input, out var value))
        return new Ok<int, string>(value);
    
    return new Err<int, string>($"Cannot parse '{input}' as integer");
}
```

### 3. Database Operations

```csharp
public Result<User, string> GetUserById(int id)
{
    var user = database.Users.Find(id);
    
    if (user == null)
        return new Err<User, string>($"User with ID {id} not found");
    
    return new Ok<User, string>(user);
}
```

### 4. Chaining Validations

```csharp
public Result<Order, string> ProcessOrder(OrderRequest request)
{
    return ValidateRequest(request)
        .Bind(ValidateInventory)
        .Bind(CalculateTotal)
        .Bind(ApplyDiscount)
        .Bind(CreateOrder);
}
```

### 5. Error Recovery

```csharp
public Result<Config, string> LoadConfig(string path)
{
    return ReadConfigFile(path)
        .OrElse(error => 
        {
            Console.WriteLine($"Failed to read config: {error}");
            return GetDefaultConfig();
        });
}
```

## Key Principles

### 1. Explicit is Better Than Implicit

```csharp
// ✅ Good - error possibility is clear
public Result<User, string> FindUser(int id)

// ❌ Bad - caller doesn't know it can fail
public User FindUser(int id)  // Might throw!
```

### 2. Use Result for Expected Errors

```csharp
// ✅ Use Result
Result<int, string> Divide(int a, int b)  // Division by zero is expected

// ❌ Use exceptions
void UpdateDatabase()  // Database connection loss is exceptional
{
    throw new DatabaseException();
}
```

### 3. Compose Operations

```csharp
// ✅ Good - functional composition
var result = ParseInt(input)
    .Bind(ValidateRange)
    .Map(x => x * 2)
    .MapErr(e => $"Processing failed: {e}");

// ❌ Bad - nested if statements
if (ParseInt(input).IsOk)
{
    var value = /* extract value somehow */;
    if (ValidateRange(value).IsOk)
    {
        // ...
    }
}
```

### 4. Always Handle Both Cases

```csharp
// ✅ Good - handles both cases
result.Match(
    ok: value => Process(value),
    err: error => LogError(error)
);

// ⚠️ Dangerous - only checks success
if (result.IsOk)
{
    // What about errors?
}
```

## Next Steps

- 📖 [Monad Pattern](../concepts/monad-pattern.md) - Understand the theory
- 📖 [Error Handling](../concepts/error-handling.md) - Advanced patterns
- 📖 [Async Patterns](../concepts/async-patterns.md) - Working with async operations
- 📖 [API Reference](../api/) - Complete method documentation
- 📖 [Examples](../examples/) - Real-world scenarios

## Summary

| Concept | Purpose | Example |
|---------|---------|---------|
| `Result<T, E>` | Represents success or failure | `Result<int, string>` |
| `Ok<T, E>` | Success with value | `new Ok<int, string>(42)` |
| `Err<T, E>` | Failure with error | `new Err<int, string>("error")` |
| `Map` | Transform success value | `result.Map(x => x * 2)` |
| `MapErr` | Transform error value | `result.MapErr(e => $"Error: {e}")` |
| `Bind` | Chain Result-returning operations | `result.Bind(Divide)` |
| `Match` | Pattern match on both cases | `result.Match(ok: ..., err: ...)` |
| `OrElse` | Provide fallback for errors | `result.OrElse(e => defaultValue)` |
| `IsOk` | Check if success | `if (result.IsOk) ...` |
| `IsErr` | Check if error | `if (result.IsErr) ...` |
