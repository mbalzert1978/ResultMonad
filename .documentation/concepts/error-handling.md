# Error Handling Patterns

This guide covers best practices and patterns for error handling with ResultMonad.

## Table of Contents

- [Error Handling Philosophy](#error-handling-philosophy)
- [Choosing Error Types](#choosing-error-types)
- [Error Composition Patterns](#error-composition-patterns)
- [Error Recovery Strategies](#error-recovery-strategies)
- [Error Propagation](#error-propagation)
- [Validation Patterns](#validation-patterns)
- [Error Reporting](#error-reporting)
- [Common Anti-Patterns](#common-anti-patterns)

## Error Handling Philosophy

### Expected vs Exceptional Errors

**Use Result for expected errors:**

```csharp
// ✅ Expected failure - use Result
Result<User, string> FindUser(int id)
{
    var user = database.Users.Find(id);
    if (user == null)
        return new Err<User, string>($"User {id} not found");
    return new Ok<User, string>(user);
}

// ✅ Expected validation failure - use Result
Result<Email, string> ValidateEmail(string input)
{
    if (!EmailRegex.IsMatch(input))
        return new Err<Email, string>("Invalid email format");
    return new Ok<Email, string>(new Email(input));
}
```

**Use exceptions for exceptional situations:**

```csharp
// ❌ Don't use Result for truly exceptional cases
public void ConnectToDatabase()
{
    if (!CanConnectToDatabase())
        throw new DatabaseConnectionException(); // This is exceptional
}

// ❌ Don't use Result for programming errors
public int GetAt(int index)
{
    if (index < 0 || index >= Count)
        throw new ArgumentOutOfRangeException(); // This is a bug
    return items[index];
}
```

### The Result vs Exception Decision Tree

```text
Can this error be reasonably expected?
├─ Yes → Use Result<T, E>
│  ├─ Examples: Validation, parsing, not found, unauthorized
│  └─ Benefits: Explicit, type-safe, composable
│
└─ No → Use Exceptions
   ├─ Examples: Out of memory, network failure, system errors
   └─ Benefits: Propagates automatically, standard .NET handling
```

## Choosing Error Types

### String Errors (Simple)

**Best for:**

- Prototyping
- Small applications
- Human-readable messages

```csharp
Result<User, string> ValidateAge(int age)
{
    if (age < 0)
        return new Err<User, string>("Age cannot be negative");
    if (age < 18)
        return new Err<User, string>("User must be 18 or older");
    return new Ok<User, string>(new User(age));
}
```

**Pros:**

- ✅ Simple and readable
- ✅ Easy to create
- ✅ Good for logging

**Cons:**

- ❌ Hard to handle programmatically
- ❌ No type safety for error cases
- ❌ Difficult to localize

### Error Enums (Structured)

**Best for:**

- Finite set of errors
- Type-safe error handling
- Pattern matching

```csharp
public enum ValidationError
{
    EmailInvalid,
    PasswordTooShort,
    UsernameTaken,
    AgeTooYoung
}

Result<User, ValidationError> ValidateUser(UserInput input)
{
    if (!IsValidEmail(input.Email))
        return new Err<User, ValidationError>(ValidationError.EmailInvalid);
    
    if (input.Password.Length < 8)
        return new Err<User, ValidationError>(ValidationError.PasswordTooShort);
    
    return new Ok<User, ValidationError>(new User(input));
}

// Handle errors with pattern matching
var result = ValidateUser(input);
result.Match(
    ok: user => CreateUser(user),
    err: error => error switch
    {
        ValidationError.EmailInvalid => "Please enter a valid email",
        ValidationError.PasswordTooShort => "Password must be at least 8 characters",
        ValidationError.UsernameTaken => "Username is already taken",
        ValidationError.AgeTooYoung => "You must be 18 or older",
        _ => "Validation failed"
    }
);
```

**Pros:**

- ✅ Type-safe
- ✅ Exhaustive pattern matching
- ✅ Easy to test

**Cons:**

- ❌ No additional context
- ❌ Must add new enum values for new errors

### Error Records (Rich Context)

**Best for:**

- Complex applications
- Rich error information
- API responses

```csharp
public record AppError(
    string Code,
    string Message,
    Dictionary<string, string> Details,
    DateTime Timestamp
);

Result<User, AppError> ValidateUser(UserInput input)
{
    if (!IsValidEmail(input.Email))
    {
        return new Err<User, AppError>(new AppError(
            Code: "VALIDATION_001",
            Message: "Invalid email address",
            Details: new() { ["field"] = "email", ["value"] = input.Email },
            Timestamp: DateTime.UtcNow
        ));
    }
    
    return new Ok<User, AppError>(new User(input));
}
```

**Pros:**

- ✅ Rich contextual information
- ✅ Structured for logging/monitoring
- ✅ Easy to serialize (API responses)

**Cons:**

- ❌ More verbose
- ❌ Requires more boilerplate

### Exception Types (When Interfacing)

**Best for:**

- Wrapping exception-throwing code
- Maintaining exception details

```csharp
Result<string, Exception> ReadFile(string path)
{
    try
    {
        var content = File.ReadAllText(path);
        return new Ok<string, Exception>(content);
    }
    catch (Exception ex)
    {
        return new Err<string, Exception>(ex);
    }
}

// Later, can re-throw if needed
result.Match(
    ok: content => Process(content),
    err: ex => throw ex // Or log and handle
);
```

## Error Composition Patterns

### Multiple Validation Errors

#### Pattern 1: Fail Fast (First Error)

```csharp
Result<User, string> ValidateUser(UserInput input)
{
    return ValidateEmail(input.Email)
        .Bind(_ => ValidatePassword(input.Password))
        .Bind(_ => ValidateAge(input.Age))
        .Map(_ => new User(input));
}
// Returns first error encountered
```

#### Pattern 2: Collect All Errors

```csharp
public record ValidationErrors(List<string> Errors);

Result<User, ValidationErrors> ValidateUser(UserInput input)
{
    var errors = new List<string>();
    
    var emailResult = ValidateEmail(input.Email);
    if (emailResult.IsErr)
        errors.Add(emailResult.Match(ok: _ => "", err: e => e));
    
    var passwordResult = ValidatePassword(input.Password);
    if (passwordResult.IsErr)
        errors.Add(passwordResult.Match(ok: _ => "", err: e => e));
    
    var ageResult = ValidateAge(input.Age);
    if (ageResult.IsErr)
        errors.Add(ageResult.Match(ok: _ => "", err: e => e));
    
    if (errors.Any())
        return new Err<User, ValidationErrors>(new ValidationErrors(errors));
    
    return new Ok<User, ValidationErrors>(new User(input));
}
```

### Error Transformation

Transform errors at boundaries:

```csharp
// Internal domain error
public enum DomainError { NotFound, Unauthorized, Invalid }

// External API error
public record ApiError(int StatusCode, string Message);

Result<User, ApiError> GetUserApi(int id)
{
    return GetUser(id) // Returns Result<User, DomainError>
        .MapErr(domainError => domainError switch
        {
            DomainError.NotFound => new ApiError(404, "User not found"),
            DomainError.Unauthorized => new ApiError(401, "Unauthorized"),
            DomainError.Invalid => new ApiError(400, "Invalid request"),
            _ => new ApiError(500, "Internal error")
        });
}
```

### Error Context Enrichment

Add context as errors propagate:

```csharp
Result<Order, string> ProcessOrder(OrderRequest request)
{
    return ValidateOrder(request)
        .MapErr(e => $"Validation failed: {e}")
        .Bind(order => SaveOrder(order))
        .MapErr(e => $"Failed to save order: {e}")
        .Bind(saved => NotifyCustomer(saved))
        .MapErr(e => $"Failed to notify customer: {e}");
}
```

## Error Recovery Strategies

### Fallback Values

Provide default values for errors:

```csharp
// Simple fallback
var config = LoadConfig("config.json")
    .OrElse(_ => GetDefaultConfig());

// Conditional fallback
var user = GetPremiumUser(id)
    .OrElse(err => GetBasicUser(id));

// Fallback chain
var data = LoadFromCache()
    .OrElse(_ => LoadFromDatabase())
    .OrElse(_ => LoadFromArchive())
    .OrElse(_ => GetDefaultData());
```

### Retry Logic

Retry operations that might succeed:

```csharp
async Task<Result<T, string>> RetryAsync<T>(
    Func<Task<Result<T, string>>> operation,
    int maxAttempts = 3,
    TimeSpan delay = default)
{
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        var result = await operation();
        if (result.IsOk || attempt == maxAttempts)
            return result;
        
        await Task.Delay(delay);
    }
    
    return new Err<T, string>("Max retry attempts exceeded");
}

// Usage
var result = await RetryAsync(
    async () => await FetchDataAsync(url),
    maxAttempts: 3,
    delay: TimeSpan.FromSeconds(1)
);
```

### Partial Success

Handle partial failures in batch operations:

```csharp
public record BatchResult<T, E>(
    List<T> Successes,
    List<E> Failures
);

BatchResult<User, string> ProcessBatch(List<UserInput> inputs)
{
    var successes = new List<User>();
    var failures = new List<string>();
    
    foreach (var input in inputs)
    {
        var result = CreateUser(input);
        result.Match(
            ok: user => successes.Add(user),
            err: error => failures.Add($"Failed to create {input.Email}: {error}")
        );
    }
    
    return new BatchResult<User, string>(successes, failures);
}
```

## Error Propagation

### Automatic Propagation with Bind

```csharp
Result<Receipt, string> CompleteCheckout(Cart cart)
{
    return ValidateCart(cart)
        .Bind(validated => CalculateTotal(validated))
        .Bind(total => ProcessPayment(total))
        .Bind(payment => GenerateReceipt(payment));
    // Any error stops the chain and propagates
}
```

### Manual Error Checking

Sometimes you need custom logic:

```csharp
Result<Order, string> CreateOrder(OrderRequest request)
{
    var validationResult = ValidateRequest(request);
    if (validationResult.IsErr)
    {
        // Log validation failure
        Logger.Warning($"Order validation failed: {validationResult}");
        return validationResult;
    }
    
    var inventoryResult = CheckInventory(request);
    if (inventoryResult.IsErr)
    {
        // Send notification
        NotifyOutOfStock(request.Items);
        return inventoryResult;
    }
    
    return CreateOrderInternal(request);
}
```

## Validation Patterns

### Single Field Validation

```csharp
Result<Email, string> ValidateEmail(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return new Err<Email, string>("Email is required");
    
    if (!EmailRegex.IsMatch(input))
        return new Err<Email, string>("Invalid email format");
    
    if (input.Length > 255)
        return new Err<Email, string>("Email too long");
    
    return new Ok<Email, string>(new Email(input));
}
```

### Multi-Field Validation

```csharp
Result<Password, string> ValidatePassword(string password, string confirm)
{
    if (string.IsNullOrWhiteSpace(password))
        return new Err<Password, string>("Password is required");
    
    if (password.Length < 8)
        return new Err<Password, string>("Password must be at least 8 characters");
    
    if (password != confirm)
        return new Err<Password, string>("Passwords do not match");
    
    if (!password.Any(char.IsUpper))
        return new Err<Password, string>("Password must contain uppercase letter");
    
    return new Ok<Password, string>(new Password(password));
}
```

### Business Rule Validation

```csharp
Result<Transfer, string> ValidateTransfer(Account from, Account to, decimal amount)
{
    if (from.Id == to.Id)
        return new Err<Transfer, string>("Cannot transfer to same account");
    
    if (amount <= 0)
        return new Err<Transfer, string>("Amount must be positive");
    
    if (from.Balance < amount)
        return new Err<Transfer, string>("Insufficient funds");
    
    if (amount > from.DailyLimit)
        return new Err<Transfer, string>("Exceeds daily limit");
    
    return new Ok<Transfer, string>(new Transfer(from, to, amount));
}
```

## Error Reporting

### Logging Errors

```csharp
Result<T, E> LogError<T, E>(Result<T, E> result, string context)
    where T : notnull
    where E : notnull
{
    return result.Match(
        ok: value => result,
        err: error =>
        {
            Logger.Error($"Error in {context}: {error}");
            return result;
        }
    );
}

// Usage
var result = ProcessOrder(order)
    .MapErr(e => LogError(e, "ProcessOrder"));
```

### User-Friendly Messages

```csharp
string GetUserMessage(Result<User, ValidationError> result)
{
    return result.Match(
        ok: user => $"Welcome, {user.Name}!",
        err: error => error switch
        {
            ValidationError.EmailInvalid => 
                "Please check your email address and try again.",
            ValidationError.PasswordTooShort => 
                "Your password should be at least 8 characters long.",
            ValidationError.UsernameTaken => 
                "That username is already taken. Please choose another.",
            _ => "Something went wrong. Please try again."
        }
    );
}
```

### Structured Logging

```csharp
public record ErrorContext(
    string Operation,
    string ErrorCode,
    string Message,
    Dictionary<string, object> Metadata
);

void LogStructuredError<T, E>(Result<T, E> result, string operation)
    where T : notnull
    where E : notnull
{
    result.Match(
        ok: _ => { },
        err: error =>
        {
            var context = new ErrorContext(
                Operation: operation,
                ErrorCode: error.GetType().Name,
                Message: error.ToString(),
                Metadata: new()
                {
                    ["timestamp"] = DateTime.UtcNow,
                    ["userId"] = CurrentUser.Id
                }
            );
            Logger.LogError(JsonSerializer.Serialize(context));
        }
    );
}
```

## Common Anti-Patterns

### ❌ Ignoring Errors

```csharp
// ❌ Bad - ignoring potential errors
var result = DivideNumbers(10, 0);
// Continues without checking if result is Ok or Err
```

### ❌ Throwing from Match

```csharp
// ❌ Bad - defeats purpose of Result
result.Match(
    ok: value => Process(value),
    err: error => throw new Exception(error) // Don't do this!
);
```

### ❌ Overusing Result

```csharp
// ❌ Bad - using Result for everything
Result<void, string> PrintMessage(string msg)
{
    Console.WriteLine(msg);
    return new Ok<void, string>(void); // Unnecessary
}

// ✅ Good - just return void
void PrintMessage(string msg)
{
    Console.WriteLine(msg);
}
```

### ❌ Result in Result

```csharp
// ❌ Bad - nested Results
Result<Result<int, string>, string> DoubleNested()
{
    return new Ok<Result<int, string>, string>(
        new Ok<int, string>(42)
    );
}

// ✅ Good - use Bind to flatten
Result<int, string> Flattened()
{
    return GetResult()
        .Bind(x => ProcessResult(x));
}
```

### ❌ Null in Result

```csharp
// ❌ Bad - Result with null (violates notnull constraint)
Result<string?, string> GetValue()
{
    return new Ok<string?, string>(null); // Compilation error
}

// ✅ Good - use Option<T> for nullable scenarios
Option<string> GetValue()
{
    return None; // Or Some(value)
}
```

## Best Practices Summary

✅ **Use Result for expected errors** (validation, not found, parsing)

✅ **Use exceptions for exceptional cases** (system failures, bugs)

✅ **Choose appropriate error types** (string for simple, enums for structured, records for rich)

✅ **Compose errors with Bind** for clean pipelines

✅ **Provide fallbacks with OrElse** for recovery

✅ **Transform errors at boundaries** (domain → API)

✅ **Log errors appropriately** without losing type safety

✅ **Always handle both cases** with Match

✅ **Avoid nesting Results** - use Bind to flatten

✅ **Keep error types non-null** - respect the constraints

## Next Steps

- 📖 [Result Type](result-type.md) - Understand the implementation
- 📖 [Async Patterns](async-patterns.md) - Error handling in async code
- 📖 [Basic Concepts](../getting-started/basic-concepts.md) - Learn the fundamentals
- 📖 [API Reference](../api/) - Complete method documentation

## Further Reading

- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) - Scott Wlaschin
- [Error Handling in Functional Languages](https://blog.janestreet.com/effective-ml-revisited/) - Yaron Minsky
- [Against Railway-Oriented Programming](https://fsharpforfunandprofit.com/posts/against-railway-oriented-programming/) - When not to use it
