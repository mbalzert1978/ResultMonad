# The Monad Pattern

This guide explains the monad pattern, its theoretical foundations, and why Monads implements it.

## Table of Contents

- [What is a Monad?](#what-is-a-monad)
- [The Three Monad Laws](#the-three-monad-laws)
- [Result as a Monad](#result-as-a-monad)
- [Benefits of Monads](#benefits-of-monads)
- [Common Monads in Programming](#common-monads-in-programming)
- [Railway-Oriented Programming](#railway-oriented-programming)

## What is a Monad?

A **monad** is a design pattern from functional programming that provides a way to:

1. **Wrap values** in a context (like `Result<T, E>`)
2. **Transform** wrapped values while maintaining the context
3. **Chain operations** that work with wrapped values

### The Simple Definition

A monad is any type that implements two operations:

1. **Return** (or **Unit**): Wraps a value in the monad
2. **Bind** (or **FlatMap**): Chains operations on wrapped values

In C#/Monads:

```csharp
// Return: Wrap a value
Result<int, string> Wrap(int value) => new Ok<int, string>(value);

// Bind: Chain operations
Result<int, string> result = Wrap(5)
    .Bind(x => Wrap(x * 2))
    .Bind(x => Wrap(x + 10));
```

### Why "Monad"?

The term comes from category theory in mathematics. While the theory is complex, the practical application is straightforward: **monads let you chain operations cleanly while handling context automatically**.

## The Three Monad Laws

For a type to be a proper monad, it must satisfy three mathematical laws. Monads adheres to all three:

### 1. Left Identity Law

Wrapping a value and then binding a function should be the same as just applying the function:

```csharp
// Return(a).Bind(f) == f(a)

int value = 5;
Func<int, Result<int, string>> f = x => new Ok<int, string>(x * 2);

// These should be equivalent:
var left = new Ok<int, string>(value).Bind(f);  // Ok(10)
var right = f(value);                            // Ok(10)

// left == right ✓
```

**What it means:** Wrapping and immediately unwrapping shouldn't change the behavior.

### 2. Right Identity Law

Binding a wrapped value with the wrapping function should return the original:

```csharp
// m.Bind(Return) == m

Result<int, string> m = new Ok<int, string>(5);

// These should be equivalent:
var left = m.Bind(x => new Ok<int, string>(x));  // Ok(5)
var right = m;                                    // Ok(5)

// left == right ✓
```

**What it means:** Wrapping an already-wrapped value doesn't add extra layers.

### 3. Associativity Law

The order of binding operations shouldn't matter:

```csharp
// m.Bind(f).Bind(g) == m.Bind(x => f(x).Bind(g))

Result<int, string> m = new Ok<int, string>(5);
Func<int, Result<int, string>> f = x => new Ok<int, string>(x * 2);
Func<int, Result<int, string>> g = x => new Ok<int, string>(x + 3);

// These should be equivalent:
var left = m.Bind(f).Bind(g);                    // Ok(13)
var right = m.Bind(x => f(x).Bind(g));           // Ok(13)

// left == right ✓
```

**What it means:** You can refactor chains of operations without changing behavior.

## Result as a Monad

`Result<T, E>` is a monad that handles **computation with potential errors**.

### The Context: Success or Failure

The "context" that Result provides is the possibility of failure:

```csharp
// Context: Operation might fail
Result<int, string> Divide(int a, int b)
{
    if (b == 0)
        return new Err<int, string>("Division by zero");  // Failure context
    return new Ok<int, string>(a / b);                    // Success context
}
```

### Return (Unit): Creating Results

```csharp
// Return: Wrap a value in success context
Result<int, string> Return(int value) => new Ok<int, string>(value);

// Or wrap in error context
Result<int, string> Fail(string error) => new Err<int, string>(error);
```

### Bind: Chaining Failable Operations

```csharp
Result<int, string> result = Divide(20, 4)    // Ok(5)
    .Bind(x => Divide(x, 5))                  // Ok(1)
    .Bind(x => Divide(10, x));                // Ok(10)

// Error propagates automatically
Result<int, string> error = Divide(20, 0)     // Err("Division by zero")
    .Bind(x => Divide(x, 5))                  // Skipped
    .Bind(x => Divide(10, x));                // Still Err("Division by zero")
```

### Map: Functor Operation (Bonus)

Result is also a **functor**, which means it supports `Map`:

```csharp
// Map: Transform the value inside the context
Result<int, string> result = new Ok<int, string>(5)
    .Map(x => x * 2)      // Ok(10)
    .Map(x => x + 3);     // Ok(13)
```

**Difference from Bind:**

- `Map`: Function returns `T`, Result wraps it → `T -> T`
- `Bind`: Function returns `Result<T, E>` → `T -> Result<T, E>`

## Benefits of Monads

### 1. Automatic Context Handling

The monad handles the context (error propagation) automatically:

```csharp
// Without monad (manual error checking)
var step1 = Divide(20, 4);
if (step1.IsErr) return step1;

var step2 = Divide(step1.Value, 5);
if (step2.IsErr) return step2;

var step3 = Divide(10, step2.Value);
return step3;

// With monad (automatic error propagation)
return Divide(20, 4)
    .Bind(x => Divide(x, 5))
    .Bind(x => Divide(10, x));
```

### 2. Composability

Operations compose naturally:

```csharp
// Define reusable operations
Result<int, string> ValidatePositive(int x) =>
    x > 0 ? new Ok<int, string>(x) : new Err<int, string>("Must be positive");

Result<int, string> ValidateEven(int x) =>
    x % 2 == 0 ? new Ok<int, string>(x) : new Err<int, string>("Must be even");

Result<int, string> Double(int x) =>
    new Ok<int, string>(x * 2);

// Compose them
var result = ParseInt("42")
    .Bind(ValidatePositive)
    .Bind(ValidateEven)
    .Bind(Double);
```

### 3. Type Safety

The compiler ensures all cases are handled:

```csharp
// Compiler ensures you handle both success and error
result.Match(
    ok: value => Console.WriteLine($"Success: {value}"),
    err: error => Console.WriteLine($"Error: {error}")
);
```

### 4. Separation of Concerns

Business logic and error handling are separated:

```csharp
// Business logic - clean and focused
Result<User, string> CreateUser(string name, int age)
{
    return ValidateName(name)
        .Bind(validName => ValidateAge(age))
        .Bind(validAge => SaveUser(new User(name, age)));
}

// Error handling - centralized
var result = CreateUser("Alice", 25);
result.Match(
    ok: user => Console.WriteLine($"Created: {user.Name}"),
    err: error => Logger.Error(error)  // All errors handled here
);
```

## Common Monads in Programming

### Maybe/Option Monad

Handles **null/missing values**:

```csharp
Option<User> FindUser(int id);

var userName = FindUser(42)
    .Map(user => user.Name)
    .Match(
        some: name => name,
        none: () => "Unknown"
    );
```

### Result/Either Monad (This Library!)

Handles **computations that can fail**:

```csharp
Result<User, Error> CreateUser(string name);

var result = CreateUser("Alice")
    .Map(user => user.Id)
    .Match(
        ok: id => $"Created user {id}",
        err: error => $"Failed: {error}"
    );
```

### List/Collection Monad

Handles **multiple values**:

```csharp
var result = GetUsers()
    .Select(user => user.Name)
    .Where(name => name.StartsWith("A"))
    .ToList();
```

### Task/Promise Monad

Handles **asynchronous computation**:

```csharp
var result = await FetchDataAsync()
    .ContinueWith(task => ProcessData(task.Result))
    .ContinueWith(task => SaveData(task.Result));
```

### Monads + Task = Async Result Monad

```csharp
var result = await FetchUserAsync()
    .Map(user => user.Name)
    .Bind(name => SaveNameAsync(name));
```

## Railway-Oriented Programming

Result monad enables **railway-oriented programming** - a metaphor where your code follows two tracks:

```text
Success Track:  ─────────────────────────────────────────────
                     ↓             ↓             ↓
                [Operation 1] [Operation 2] [Operation 3]
                     ↓             ↓             ↓
Error Track:    ─────┴─────────────┴─────────────┴────────────
```

### The Concept

- **Success track**: Operations continue flowing
- **Error track**: Once an error occurs, remaining operations are skipped
- **Switches**: Operations can switch from success to error track

### Example

```csharp
Result<Order, string> ProcessOrder(OrderRequest request)
{
    return ValidateRequest(request)     // ─→ Success or ↓ Error
        .Bind(ValidateInventory)        // ─→ Success or ↓ Error
        .Bind(CalculateTotal)           // ─→ Success or ↓ Error
        .Bind(ApplyDiscount)            // ─→ Success or ↓ Error
        .Bind(CreateOrder);             // ─→ Success or ↓ Error
}                                        // Final result
```

**Visualization:**

```text
ValidateRequest ─────→ ValidateInventory ─────→ CalculateTotal ─────→ ...
     │                       │                        │
     ↓ Error                 ↓ Error                  ↓ Error
─────┴───────────────────────┴────────────────────────┴──────────────→ Err
```

### Benefits of Railway-Oriented Programming

1. **Linear flow**: Code reads top-to-bottom
2. **Early exit**: Errors stop the pipeline automatically
3. **No nesting**: Avoid deeply nested if/try-catch blocks
4. **Explicit switches**: Clear where errors can occur

### Traditional vs Railway-Oriented

**Traditional (nested error handling):**

```csharp
public Order ProcessOrder(OrderRequest request)
{
    var validRequest = ValidateRequest(request);
    if (validRequest.IsError)
        throw new ValidationException(validRequest.Error);
    
    var inventory = ValidateInventory(validRequest.Value);
    if (inventory.IsError)
        throw new InventoryException(inventory.Error);
    
    var total = CalculateTotal(inventory.Value);
    if (total.IsError)
        throw new CalculationException(total.Error);
    
    // ... more nesting
}
```

**Railway-Oriented:**

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

## Practical Applications

### 1. Validation Pipeline

```csharp
Result<User, string> RegisterUser(UserRegistration reg)
{
    return ValidateEmail(reg.Email)
        .Bind(email => ValidatePassword(reg.Password))
        .Bind(password => ValidateAge(reg.Age))
        .Bind(age => CreateUser(reg));
}
```

### 2. Data Processing Pipeline

```csharp
Result<Report, string> GenerateReport(string filePath)
{
    return ReadFile(filePath)
        .Bind(ParseCsv)
        .Bind(ValidateData)
        .Map(TransformData)
        .Bind(GenerateCharts)
        .Bind(CreatePdf);
}
```

### 3. API Request Chain

```csharp
async Task<Result<UserProfile, string>> GetUserProfile(int userId)
{
    return await FetchUser(userId)
        .Bind(user => FetchUserSettings(user.Id))
        .Bind(settings => FetchUserPreferences(settings.UserId))
        .Map(prefs => new UserProfile(user, settings, prefs));
}
```

## Monad vs Other Patterns

| Pattern | Purpose | Error Handling |
|---------|---------|----------------|
| **Monad** | Chain operations with context | Explicit in type, automatic propagation |
| **Exception** | Signal exceptional conditions | Implicit, manual propagation |
| **Null** | Represent absence | Implicit, easy to forget checks |
| **Status Code** | Return success/failure | Explicit but separated from value |

## Further Reading

- 📖 [Result Type Guide](result-type.md) - Deep dive into Result<T, E>
- 📖 [Error Handling Patterns](error-handling.md) - Best practices
- 📖 [Async Patterns](async-patterns.md) - Monads with async/await
- 🔗 [Railway-Oriented Programming (Scott Wlaschin)](https://fsharpforfunandprofit.com/rop/) - Original concept
- 🔗 [Monads for Functional Programming (Philip Wadler)](https://homepages.inf.ed.ac.uk/wadler/papers/marktoberdorf/baastad.pdf) - Academic paper

## Summary

**Key Takeaways:**

✅ Monads provide a pattern for chaining operations with context

✅ Result monad handles error context automatically

✅ Three laws ensure predictable behavior (identity, associativity)

✅ Railway-oriented programming makes error handling linear and clear

✅ Composition and type safety are the main benefits

**Core Operations:**

- `Return/Unit`: Wrap value → `new Ok<T, E>(value)`
- `Bind/FlatMap`: Chain operations → `result.Bind(f)`
- `Map/Functor`: Transform value → `result.Map(f)`

Understanding monads helps you write more composable, maintainable, and correct code. While the theory is mathematical, the practical benefits are concrete and immediate.
