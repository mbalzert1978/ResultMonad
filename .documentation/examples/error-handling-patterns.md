# Error Handling Patterns

This document covers best practices, common patterns, and anti-patterns when working with error handling using the `Result<T, E>` monad pattern. Learn how to effectively manage errors in your applications while maintaining clean and maintainable code.

## Core Principles

### 1. Make Errors Explicit

**✅ Good: Use Result types to make potential failures explicit**

```csharp
// Good: Caller knows this operation can fail
public Result<User, string> GetUser(int id)
{
    if (id <= 0)
        return new Err<User, string>("Invalid user ID");
        
    User? user = database.FindUser(id);
    return user is not null
        ? new Ok<User, string>(user)
        : new Err<User, string>("User not found");
}
```

**❌ Bad: Hidden exceptions**

```csharp
// Bad: Caller doesn't know this can throw exceptions
public User GetUser(int id)
{
    if (id <= 0)
        throw new ArgumentException("Invalid user ID");
        
    User? user = database.FindUser(id);
    return user ?? throw new InvalidOperationException("User not found");
}
```

### 2. Fail Fast, Propagate Cleanly

**✅ Good: Early validation with clean error propagation**

```csharp
public Result<ProcessedOrder, OrderError> ProcessOrder(OrderRequest request)
{
    return ValidateOrderRequest(request)
        .Bind(validRequest => CalculatePricing(validRequest))
        .Bind(pricedOrder => CheckInventory(pricedOrder))
        .Bind(confirmedOrder => ChargePayment(confirmedOrder))
        .Map(paidOrder => FinalizeOrder(paidOrder));
}

// Each step can fail, but errors propagate automatically
private Result<OrderRequest, OrderError> ValidateOrderRequest(OrderRequest request)
{
    if (request.Items.Count == 0)
        return new Err<OrderRequest, OrderError>(
            OrderError.EmptyOrder("Order must contain at least one item"));
            
    if (request.CustomerId <= 0)
        return new Err<OrderRequest, OrderError>(
            OrderError.InvalidCustomer("Invalid customer ID"));
            
    return new Ok<OrderRequest, OrderError>(request);
}
```

### 3. Use Meaningful Error Types

**✅ Good: Domain-specific error types**

```csharp
public abstract record ValidationError(string Field, string Message);
public record RequiredFieldError(string Field) : ValidationError(Field, $"{Field} is required");
public record InvalidFormatError(string Field, string Format) : ValidationError(Field, $"{Field} must be in {Format} format");
public record OutOfRangeError(string Field, int Min, int Max) : ValidationError(Field, $"{Field} must be between {Min} and {Max}");

public Result<ValidatedInput, ValidationError> ValidateEmail(string email)
{
    if (string.IsNullOrEmpty(email))
        return new Err<ValidatedInput, ValidationError>(new RequiredFieldError("Email"));
        
    if (!email.Contains('@'))
        return new Err<ValidatedInput, ValidationError>(
            new InvalidFormatError("Email", "user@domain.com"));
            
    return new Ok<ValidatedInput, ValidationError>(new ValidatedInput(email));
}
```

**❌ Bad: Generic string errors**

```csharp
// Bad: Loses error context and makes handling difficult
public Result<ValidatedInput, string> ValidateEmail(string email)
{
    if (string.IsNullOrEmpty(email))
        return new Err<ValidatedInput, string>("Email error");
        
    if (!email.Contains('@'))
        return new Err<ValidatedInput, string>("Email error");
        
    return new Ok<ValidatedInput, string>(new ValidatedInput(email));
}
```

## Common Patterns

### 1. Validation Pipeline

**Pattern: Chain multiple validations, stopping at the first failure**

```csharp
public static class UserValidator
{
    public static Result<ValidatedUser, ValidationError> ValidateUser(UserInput input)
    {
        return ValidateEmail(input.Email)
            .Bind(_ => ValidatePassword(input.Password))
            .Bind(_ => ValidateAge(input.Age))
            .Bind(_ => ValidatePhoneNumber(input.PhoneNumber))
            .Map(_ => new ValidatedUser(input));
    }
    
    // Alternative: Collect all validation errors
    public static Result<ValidatedUser, List<ValidationError>> ValidateUserCollectErrors(UserInput input)
    {
        var errors = new List<ValidationError>();
        
        if (ValidateEmail(input.Email).IsErr)
            errors.Add(new InvalidFormatError("Email", "user@domain.com"));
            
        if (ValidatePassword(input.Password).IsErr)
            errors.Add(new RequiredFieldError("Password"));
            
        if (ValidateAge(input.Age).IsErr)
            errors.Add(new OutOfRangeError("Age", 13, 120));
            
        return errors.Count == 0
            ? new Ok<ValidatedUser, List<ValidationError>>(new ValidatedUser(input))
            : new Err<ValidatedUser, List<ValidationError>>(errors);
    }
}
```

### 2. Retry Pattern

**Pattern: Retry operations with exponential backoff**

```csharp
public static class RetryPattern
{
    public static async Task<Result<T, string>> WithRetryAsync<T>(
        Func<Task<Result<T, string>>> operation,
        int maxAttempts = 3,
        TimeSpan baseDelay = default) where T : notnull
    {
        if (baseDelay == default)
            baseDelay = TimeSpan.FromSeconds(1);
            
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            Result<T, string> result = await operation();
            
            if (result.IsOk)
                return result;
                
            // Don't retry on the last attempt
            if (attempt == maxAttempts)
                return result;
                
            // Only retry on certain types of errors
            bool shouldRetry = result.IsErrAnd(error => 
                error.Contains("timeout") || 
                error.Contains("network") || 
                error.Contains("503") || 
                error.Contains("504"));
                
            if (!shouldRetry)
                return result;
                
            // Exponential backoff
            TimeSpan delay = TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
            await Task.Delay(delay);
        }
        
        // This should never be reached, but satisfy the compiler
        return await operation();
    }
}

// Usage
Result<ApiResponse, string> response = await RetryPattern.WithRetryAsync(
    () => apiClient.GetDataAsync(),
    maxAttempts: 3,
    baseDelay: TimeSpan.FromSeconds(2)
);
```

### 3. Circuit Breaker Pattern

**Pattern: Prevent cascading failures by temporarily stopping calls to failing services**

```csharp
public class CircuitBreaker<T, E> where T : notnull where E : notnull
{
    private readonly int _failureThreshold;
    private readonly TimeSpan _timeout;
    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitState _state = CircuitState.Closed;
    
    public CircuitBreaker(int failureThreshold = 5, TimeSpan timeout = default)
    {
        _failureThreshold = failureThreshold;
        _timeout = timeout == default ? TimeSpan.FromMinutes(1) : timeout;
    }
    
    public async Task<Result<T, E>> ExecuteAsync(Func<Task<Result<T, E>>> operation)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime < _timeout)
            {
                return new Err<T, E>((E)(object)"Circuit breaker is open");
            }
            else
            {
                _state = CircuitState.HalfOpen;
            }
        }
        
        try
        {
            Result<T, E> result = await operation();
            
            if (result.IsOk)
            {
                Reset();
                return result;
            }
            else
            {
                RecordFailure();
                return result;
            }
        }
        catch (Exception ex)
        {
            RecordFailure();
            return new Err<T, E>((E)(object)ex.Message);
        }
    }
    
    private void RecordFailure()
    {
        _failureCount++;
        _lastFailureTime = DateTime.UtcNow;
        
        if (_failureCount >= _failureThreshold)
        {
            _state = CircuitState.Open;
        }
    }
    
    private void Reset()
    {
        _failureCount = 0;
        _state = CircuitState.Closed;
    }
}

private enum CircuitState { Closed, Open, HalfOpen }

// Usage
var circuitBreaker = new CircuitBreaker<ApiData, string>(failureThreshold: 3, timeout: TimeSpan.FromMinutes(5));

Result<ApiData, string> result = await circuitBreaker.ExecuteAsync(() => apiClient.GetDataAsync());
```

### 4. Fallback Chain Pattern

**Pattern: Try multiple alternatives in order**

```csharp
public class DataService
{
    public async Task<Result<Data, string>> GetDataAsync(string key)
    {
        return await GetFromPrimaryCache(key)
            .OrElseAsync(() => GetFromSecondaryCache(key))
            .OrElseAsync(() => GetFromDatabase(key))
            .OrElseAsync(() => GetFromExternalApi(key))
            .OrElseAsync(() => Task.FromResult(GetDefaultData(key)));
    }
    
    private async Task<Result<Data, string>> GetFromPrimaryCache(string key)
    {
        try
        {
            Data? data = await _primaryCache.GetAsync(key);
            return data is not null
                ? new Ok<Data, string>(data)
                : new Err<Data, string>("Not found in primary cache");
        }
        catch (Exception ex)
        {
            return new Err<Data, string>($"Primary cache error: {ex.Message}");
        }
    }
    
    private Result<Data, string> GetDefaultData(string key)
    {
        // Always succeeds with fallback data
        return new Ok<Data, string>(new Data { Id = key, Value = "Default" });
    }
}
```

### 5. Error Transformation Pattern

**Pattern: Convert between different error types as data flows through layers**

```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    
    // Domain layer: Rich domain errors
    public async Task<Result<User, DomainError>> GetUserAsync(int id)
    {
        return await _repository.GetUserAsync(id)
            .MapErrAsync(dbError => TransformDatabaseError(dbError));
    }
    
    private static DomainError TransformDatabaseError(DatabaseError dbError)
    {
        return dbError.Type switch
        {
            DatabaseErrorType.NotFound => DomainError.UserNotFound(dbError.Message),
            DatabaseErrorType.Timeout => DomainError.ServiceUnavailable("Database timeout"),
            DatabaseErrorType.ConnectionFailed => DomainError.ServiceUnavailable("Database unavailable"),
            _ => DomainError.UnexpectedError($"Database error: {dbError.Message}")
        };
    }
}

public class UserController
{
    private readonly UserService _userService;
    
    // API layer: HTTP-specific responses
    public async Task<IActionResult> GetUser(int id)
    {
        Result<User, DomainError> result = await _userService.GetUserAsync(id);
        
        return result.Match<IActionResult>(
            onOk: user => Ok(UserDto.FromDomain(user)),
            onErr: error => error switch
            {
                DomainError.UserNotFound => NotFound(error.Message),
                DomainError.ServiceUnavailable => StatusCode(503, error.Message),
                DomainError.ValidationFailed => BadRequest(error.Message),
                _ => StatusCode(500, "Internal server error")
            }
        );
    }
}
```

### 6. Partial Success Pattern

**Pattern: Handle scenarios where some operations succeed and others fail**

```csharp
public class BatchProcessor
{
    public async Task<BatchResult<ProcessedItem, ProcessingError>> ProcessBatchAsync(
        IEnumerable<InputItem> items)
    {
        var successes = new List<ProcessedItem>();
        var failures = new List<(InputItem Item, ProcessingError Error)>();
        
        foreach (InputItem item in items)
        {
            Result<ProcessedItem, ProcessingError> result = await ProcessSingleItemAsync(item);
            
            result.Match(
                onOk: success => successes.Add(success),
                onErr: error => failures.Add((item, error))
            );
        }
        
        return new BatchResult<ProcessedItem, ProcessingError>(
            Successes: successes,
            Failures: failures,
            TotalCount: items.Count()
        );
    }
    
    // Alternative: Fail fast on first error
    public async Task<Result<List<ProcessedItem>, ProcessingError>> ProcessBatchFailFastAsync(
        IEnumerable<InputItem> items)
    {
        var results = new List<ProcessedItem>();
        
        foreach (InputItem item in items)
        {
            Result<ProcessedItem, ProcessingError> result = await ProcessSingleItemAsync(item);
            
            if (result.IsErr)
                return result.MapErr(error => error); // Propagate first error
                
            results.Add(result.Match(success => success, _ => throw new InvalidOperationException()));
        }
        
        return new Ok<List<ProcessedItem>, ProcessingError>(results);
    }
}

public record BatchResult<TSuccess, TError>(
    IReadOnlyList<TSuccess> Successes,
    IReadOnlyList<(InputItem Item, TError Error)> Failures,
    int TotalCount
)
{
    public bool HasFailures => Failures.Count > 0;
    public double SuccessRate => TotalCount > 0 ? (double)Successes.Count / TotalCount : 0;
}
```

## Anti-Patterns to Avoid

### 1. ❌ Ignoring Errors

```csharp
// Bad: Ignoring potential errors
Result<Data, string> result = GetData();
// Using result without checking if it's Ok or Err
Data data = result.Match(d => d, _ => null!); // Can return null!

// Good: Always handle both cases
Result<Data, string> result = GetData();
string message = result.Match(
    onOk: data => $"Got data: {data.Value}",
    onErr: error => $"Failed to get data: {error}"
);
```

### 2. ❌ Converting Results Back to Exceptions

```csharp
// Bad: Defeating the purpose of Result types
public Data GetDataOrThrow(int id)
{
    Result<Data, string> result = GetData(id);
    return result.Match(
        onOk: data => data,
        onErr: error => throw new Exception(error) // Don't do this!
    );
}

// Good: Keep working with Result types
public Result<Data, string> GetData(int id)
{
    return ValidateId(id)
        .Bind(validId => FetchFromDatabase(validId))
        .Map(data => EnrichData(data));
}
```

### 3. ❌ Overusing String Error Types

```csharp
// Bad: Generic strings lose context
public Result<User, string> CreateUser(CreateUserRequest request)
{
    if (request.Email == null)
        return new Err<User, string>("Error");
        
    if (request.Age < 0)
        return new Err<User, string>("Error");
        
    // Caller can't distinguish between different error types
}

// Good: Specific error types
public Result<User, UserCreationError> CreateUser(CreateUserRequest request)
{
    if (request.Email == null)
        return new Err<User, UserCreationError>(UserCreationError.InvalidEmail("Email is required"));
        
    if (request.Age < 0)
        return new Err<User, UserCreationError>(UserCreationError.InvalidAge("Age must be non-negative"));
}
```

### 4. ❌ Deep Nesting Instead of Early Returns

```csharp
// Bad: Nested if statements
public Result<ProcessedData, string> ProcessData(InputData input)
{
    Result<ValidatedData, string> validated = ValidateData(input);
    if (validated.IsOk)
    {
        Result<EnrichedData, string> enriched = EnrichData(validated.Match(d => d, _ => null!));
        if (enriched.IsOk)
        {
            Result<ProcessedData, string> processed = FinalizeData(enriched.Match(d => d, _ => null!));
            if (processed.IsOk)
            {
                return processed;
            }
            else
            {
                return processed;
            }
        }
        else
        {
            return enriched.MapErr(e => e);
        }
    }
    else
    {
        return validated.MapErr(e => e);
    }
}

// Good: Use Bind for flat composition
public Result<ProcessedData, string> ProcessData(InputData input)
{
    return ValidateData(input)
        .Bind(validated => EnrichData(validated))
        .Bind(enriched => FinalizeData(enriched));
}
```

### 5. ❌ Not Using Appropriate Extension Methods

```csharp
// Bad: Manual pattern matching for simple transformations
public Result<string, string> GetUserDisplayName(int userId)
{
    Result<User, string> userResult = GetUser(userId);
    
    return userResult.Match(
        onOk: user => new Ok<string, string>(user.DisplayName),
        onErr: error => new Err<string, string>(error)
    );
}

// Good: Use Map for simple transformations
public Result<string, string> GetUserDisplayName(int userId)
{
    return GetUser(userId).Map(user => user.DisplayName);
}

// Bad: Manual chaining
public Result<ProcessedUser, string> ProcessUser(int userId)
{
    Result<User, string> userResult = GetUser(userId);
    
    if (userResult.IsOk)
    {
        User user = userResult.Match(u => u, _ => null!);
        return ProcessUserData(user);
    }
    else
    {
        return userResult.MapErr(error => error);
    }
}

// Good: Use Bind for chaining
public Result<ProcessedUser, string> ProcessUser(int userId)
{
    return GetUser(userId).Bind(user => ProcessUserData(user));
}
```

## Error Recovery Strategies

### 1. Graceful Degradation

```csharp
public async Task<Result<UserProfile, string>> GetUserProfileAsync(int userId)
{
    Result<User, string> userResult = await GetUserAsync(userId);
    
    if (userResult.IsErr)
        return userResult.MapErr(error => error);
        
    User user = userResult.Match(u => u, _ => throw new InvalidOperationException());
    
    // Try to get optional data, but don't fail if it's unavailable
    Result<UserPreferences, string> preferencesResult = await GetUserPreferencesAsync(userId);
    Result<UserStats, string> statsResult = await GetUserStatsAsync(userId);
    
    UserPreferences? preferences = preferencesResult.Match(p => p, _ => null);
    UserStats? stats = statsResult.Match(s => s, _ => null);
    
    var profile = new UserProfile(
        User: user,
        Preferences: preferences ?? UserPreferences.Default,
        Stats: stats ?? UserStats.Empty
    );
    
    return new Ok<UserProfile, string>(profile);
}
```

### 2. Compensation Actions

```csharp
public class OrderService
{
    public async Task<Result<CompletedOrder, OrderError>> ProcessOrderAsync(OrderRequest request)
    {
        var compensationActions = new Stack<Func<Task>>();
        
        try
        {
            // Step 1: Reserve inventory
            Result<ReservationId, OrderError> reservationResult = await ReserveInventoryAsync(request.Items);
            if (reservationResult.IsErr)
                return reservationResult.MapErr(e => e);
                
            ReservationId reservationId = reservationResult.Match(r => r, _ => throw new InvalidOperationException());
            compensationActions.Push(() => ReleaseInventoryAsync(reservationId));
            
            // Step 2: Charge payment
            Result<PaymentId, OrderError> paymentResult = await ChargePaymentAsync(request.PaymentInfo, request.TotalAmount);
            if (paymentResult.IsErr)
            {
                await ExecuteCompensationActionsAsync(compensationActions);
                return paymentResult.MapErr(e => e);
            }
            
            PaymentId paymentId = paymentResult.Match(p => p, _ => throw new InvalidOperationException());
            compensationActions.Push(() => RefundPaymentAsync(paymentId));
            
            // Step 3: Create order
            Result<CompletedOrder, OrderError> orderResult = await CreateOrderAsync(request, reservationId, paymentId);
            if (orderResult.IsErr)
            {
                await ExecuteCompensationActionsAsync(compensationActions);
                return orderResult;
            }
            
            // Success - clear compensation actions
            compensationActions.Clear();
            return orderResult;
        }
        catch (Exception ex)
        {
            await ExecuteCompensationActionsAsync(compensationActions);
            return new Err<CompletedOrder, OrderError>(OrderError.UnexpectedError(ex.Message));
        }
    }
    
    private static async Task ExecuteCompensationActionsAsync(Stack<Func<Task>> actions)
    {
        while (actions.Count > 0)
        {
            try
            {
                Func<Task> action = actions.Pop();
                await action();
            }
            catch (Exception ex)
            {
                // Log compensation failures but continue
                Console.WriteLine($"Compensation action failed: {ex.Message}");
            }
        }
    }
}
```

## Testing Error Scenarios

### 1. Testing Success and Error Paths

```csharp
[Test]
public async Task ProcessOrder_ValidRequest_ReturnsSuccess()
{
    // Arrange
    var request = new OrderRequest(customerId: 123, items: validItems);
    
    // Act
    Result<CompletedOrder, OrderError> result = await orderService.ProcessOrderAsync(request);
    
    // Assert
    Assert.That(result.IsOk, Is.True);
    result.Match(
        onOk: order => Assert.That(order.Id, Is.GreaterThan(0)),
        onErr: error => Assert.Fail($"Expected success but got error: {error}")
    );
}

[Test]
public async Task ProcessOrder_InvalidCustomerId_ReturnsValidationError()
{
    // Arrange
    var request = new OrderRequest(customerId: -1, items: validItems);
    
    // Act
    Result<CompletedOrder, OrderError> result = await orderService.ProcessOrderAsync(request);
    
    // Assert
    Assert.That(result.IsErr, Is.True);
    result.Match(
        onOk: order => Assert.Fail("Expected error but got success"),
        onErr: error => Assert.That(error, Is.TypeOf<OrderError.ValidationFailed>())
    );
}
```

### 2. Property-Based Testing

```csharp
[Test]
public void EmailValidation_Properties()
{
    // Property: Valid emails should always pass validation
    Prop.ForAll<string>(email =>
    {
        if (IsValidEmailFormat(email))
        {
            Result<string, ValidationError> result = EmailValidator.Validate(email);
            return result.IsOk;
        }
        return true; // Skip invalid formats
    }).QuickCheckThrowOnFailure();
    
    // Property: Invalid emails should always fail validation
    Prop.ForAll<string>(invalidEmail =>
    {
        if (!IsValidEmailFormat(invalidEmail))
        {
            Result<string, ValidationError> result = EmailValidator.Validate(invalidEmail);
            return result.IsErr;
        }
        return true; // Skip valid formats
    }).QuickCheckThrowOnFailure();
}
```

## Best Practices Summary

1. **Make errors explicit** with Result types instead of exceptions
2. **Use meaningful error types** that provide context for handling
3. **Compose operations** with Bind, Map, and other extension methods
4. **Handle errors at appropriate boundaries** (don't let them bubble up indefinitely)
5. **Provide fallback strategies** for graceful degradation
6. **Test both success and error paths** thoroughly
7. **Document error conditions** in your API
8. **Use circuit breakers and retries** for resilient systems
9. **Implement compensation actions** for complex workflows
10. **Keep error types serializable** for distributed systems

By following these patterns and avoiding the anti-patterns, you'll build more robust and maintainable applications with clear error handling strategies.
