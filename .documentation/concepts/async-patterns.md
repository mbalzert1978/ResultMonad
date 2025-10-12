# Async Patterns

This guide covers asynchronous programming patterns with Monads, including Task and ValueTask variants.

## Table of Contents

- [Overview](#overview)
- [Task vs ValueTask](#task-vs-valuetask)
- [Async Extension Methods](#async-extension-methods)
- [Common Async Patterns](#common-async-patterns)
- [Error Handling in Async](#error-handling-in-async)
- [Performance Considerations](#performance-considerations)
- [Best Practices](#best-practices)

## Overview

Monads provides full support for asynchronous operations through extension methods that work with both `Task<Result<T, E>>` and `ValueTask<Result<T, E>>`.

### Why Async Support Matters

```csharp
// Without async support - awkward
async Task<Result<string, string>> ProcessDataAsync()
{
    var result = await FetchDataAsync();
    if (result.IsOk)
    {
        var transformed = result.Match(
            ok: data => data.ToUpper(),
            err: _ => ""
        );
        return new Ok<string, string>(transformed);
    }
    return result;
}

// With async support - clean
async Task<Result<string, string>> ProcessDataAsync()
{
    return await FetchDataAsync()
        .Map(data => data.ToUpper());
}
```

## Task vs ValueTask

### Task\<T\>

**Use Task when:**

- The operation is always asynchronous
- Result might be cached/shared
- Working with existing async APIs
- Performance is not critical

**Characteristics:**

- Always allocated on heap
- Can be awaited multiple times
- Thread-safe
- Standard for most async code

```csharp
async Task<Result<User, string>> GetUserAsync(int id)
{
    return await database.Users
        .FindAsync(id)
        .ContinueWith(task => task.Result != null
            ? new Ok<User, string>(task.Result)
            : new Err<User, string>("User not found"));
}
```

### ValueTask\<T\>

**Use ValueTask when:**

- Operation might complete synchronously
- Performance is critical (hot path)
- Result won't be cached
- High-frequency calls

**Characteristics:**

- Value type (no heap allocation if synchronous)
- Cannot be awaited multiple times
- Better performance for sync completion
- More complex to use correctly

```csharp
async ValueTask<Result<Config, string>> GetConfigAsync(string key)
{
    // Might complete synchronously if cached
    if (cache.TryGetValue(key, out var config))
        return new Ok<Config, string>(config);
    
    var loaded = await LoadConfigAsync(key);
    cache.Add(key, loaded);
    return new Ok<Config, string>(loaded);
}
```

### Decision Matrix

| Scenario | Use Task | Use ValueTask |
|----------|----------|---------------|
| Always async (I/O) | ✅ | ❌ |
| May complete sync (cache) | ❌ | ✅ |
| Result will be cached | ✅ | ❌ |
| Hot path / high frequency | ❌ | ✅ |
| Simple code | ✅ | ❌ |
| Maximum performance | ❌ | ✅ |

## Async Extension Methods

Monads provides async variants of all core operations:

### MapAsync (Task)

Transform success value asynchronously:

```csharp
// Sync mapper
Task<Result<int, string>> result = FetchNumberAsync()
    .Map(x => x * 2);

// Async mapper  
Task<Result<int, string>> result = FetchNumberAsync()
    .Map(async x => await CalculateAsync(x));
```

### MapAsync (ValueTask)

Same as MapAsync but for ValueTask:

```csharp
ValueTask<Result<string, string>> result = GetCachedDataAsync()
    .Map(data => data.ToUpper());
```

### BindAsync (Task)

Chain async operations:

```csharp
Task<Result<Order, string>> PlaceOrderAsync(Cart cart)
{
    return ValidateCartAsync(cart)
        .Bind(async validated => await ProcessPaymentAsync(validated))
        .Bind(async payment => await CreateOrderAsync(payment));
}
```

### BindAsync (ValueTask)

Chain ValueTask operations:

```csharp
ValueTask<Result<User, string>> GetUserWithPrefsAsync(int id)
{
    return GetUserAsync(id)
        .Bind(async user => await LoadPreferencesAsync(user));
}
```

### MatchAsync (Task)

Pattern match with async handlers:

```csharp
await FetchUserAsync(id)
    .Match(
        ok: async user => await WelcomeUserAsync(user),
        err: async error => await LogErrorAsync(error)
    );
```

### MatchAsync (ValueTask)

Pattern match for ValueTask:

```csharp
await GetConfigAsync("key")
    .Match(
        ok: config => ApplyConfig(config),
        err: error => UseDefault()
    );
```

### MapErrAsync

Transform errors asynchronously:

```csharp
Task<Result<Data, string>> FetchDataAsync()
{
    return FetchFromApiAsync()
        .MapErr(async error => await TranslateErrorAsync(error));
}
```

### OrElseAsync

Async fallback handling:

```csharp
Task<Result<Config, string>> LoadConfigAsync()
{
    return ReadConfigFileAsync()
        .OrElse(async _ => await FetchDefaultConfigAsync());
}
```

### FlattenAsync

Flatten nested async results:

```csharp
Task<Result<Result<int, string>, string>> nested = GetNestedAsync();
Task<Result<int, string>> flattened = nested.Flatten();
```

## Common Async Patterns

### Sequential Operations

Execute operations one after another:

```csharp
async Task<Result<Receipt, string>> CheckoutAsync(Cart cart)
{
    return await ValidateCartAsync(cart)
        .Bind(validated => CalculateTotalAsync(validated))
        .Bind(total => ChargeCustomerAsync(total))
        .Bind(payment => GenerateReceiptAsync(payment));
}
```

### Parallel Operations

Execute independent operations concurrently:

```csharp
async Task<Result<UserProfile, string>> LoadProfileAsync(int userId)
{
    // Start all operations concurrently
    var userTask = GetUserAsync(userId);
    var settingsTask = GetSettingsAsync(userId);
    var prefsTask = GetPreferencesAsync(userId);
    
    // Wait for all to complete
    await Task.WhenAll(userTask, settingsTask, prefsTask);
    
    // Combine results
    var user = await userTask;
    var settings = await settingsTask;
    var prefs = await prefsTask;
    
    // All succeeded?
    if (user.IsOk && settings.IsOk && prefs.IsOk)
    {
        return new Ok<UserProfile, string>(
            new UserProfile(
                user.Match(ok: u => u, err: _ => throw new UnreachableException()),
                settings.Match(ok: s => s, err: _ => throw new UnreachableException()),
                prefs.Match(ok: p => p, err: _ => throw new UnreachableException())
            )
        );
    }
    
    // Return first error
    if (user.IsErr)
        return user.MapErr(e => $"User: {e}");
    if (settings.IsErr)
        return settings.MapErr(e => $"Settings: {e}");
    return prefs.MapErr(e => $"Preferences: {e}");
}
```

### Conditional Async

Execute operations conditionally:

```csharp
async Task<Result<Order, string>> ProcessOrderAsync(OrderRequest request)
{
    var validation = await ValidateOrderAsync(request);
    
    if (validation.IsErr)
        return validation;
    
    // Only if validation succeeded
    var result = await validation
        .Bind(async order =>
        {
            if (order.RequiresApproval)
                return await RequestApprovalAsync(order);
            return new Ok<Order, string>(order);
        })
        .Bind(approved => SaveOrderAsync(approved));
    
    return result;
}
```

### Retry Pattern

Retry failed async operations:

```csharp
async Task<Result<T, string>> RetryAsync<T>(
    Func<Task<Result<T, string>>> operation,
    int maxAttempts = 3,
    TimeSpan delay = default)
{
    Result<T, string> result = new Err<T, string>("Not attempted");
    
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        result = await operation();
        
        if (result.IsOk)
            return result;
        
        if (attempt < maxAttempts)
            await Task.Delay(delay);
    }
    
    return result;
}

// Usage
var data = await RetryAsync(
    async () => await FetchDataAsync(url),
    maxAttempts: 3,
    delay: TimeSpan.FromSeconds(1)
);
```

### Timeout Pattern

Add timeout to async operations:

```csharp
async Task<Result<T, string>> WithTimeoutAsync<T>(
    Task<Result<T, string>> operation,
    TimeSpan timeout)
{
    var timeoutTask = Task.Delay(timeout);
    var completedTask = await Task.WhenAny(operation, timeoutTask);
    
    if (completedTask == timeoutTask)
        return new Err<T, string>("Operation timed out");
    
    return await operation;
}

// Usage
var result = await WithTimeoutAsync(
    FetchDataAsync(url),
    TimeSpan.FromSeconds(5)
);
```

### Caching Pattern

Cache async results:

```csharp
public class CachedResultService
{
    private readonly Dictionary<string, Result<Data, string>> cache = new();
    
    public async ValueTask<Result<Data, string>> GetDataAsync(string key)
    {
        // Check cache first (synchronous)
        if (cache.TryGetValue(key, out var cached))
            return cached;
        
        // Fetch if not cached (asynchronous)
        var result = await FetchDataAsync(key);
        
        // Cache successful results
        if (result.IsOk)
            cache[key] = result;
        
        return result;
    }
}
```

## Error Handling in Async

### Async Exception Wrapping

Convert exceptions to Result in async code:

```csharp
async Task<Result<string, string>> SafeFetchAsync(string url)
{
    try
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        return new Ok<string, string>(response);
    }
    catch (HttpRequestException ex)
    {
        return new Err<string, string>($"HTTP error: {ex.Message}");
    }
    catch (TaskCanceledException)
    {
        return new Err<string, string>("Request timed out");
    }
    catch (Exception ex)
    {
        return new Err<string, string>($"Unexpected error: {ex.Message}");
    }
}
```

### Async Error Recovery

Recover from async errors:

```csharp
async Task<Result<Config, string>> LoadConfigWithFallbackAsync()
{
    return await LoadConfigFromFileAsync()
        .OrElse(async error =>
        {
            await LogErrorAsync(error);
            return await LoadDefaultConfigAsync();
        })
        .OrElse(async error =>
        {
            await LogCriticalAsync(error);
            return GetHardcodedConfig();
        });
}
```

### Async Error Transformation

Transform errors at async boundaries:

```csharp
async Task<Result<Data, ApiError>> FetchFromApiAsync(string endpoint)
{
    return await FetchInternalAsync(endpoint) // Result<Data, string>
        .MapErr(error => new ApiError
        {
            Code = "FETCH_FAILED",
            Message = error,
            Timestamp = DateTime.UtcNow
        });
}
```

## Performance Considerations

### Avoid Unnecessary Allocations

```csharp
// ❌ Bad - allocates Task even if sync
async Task<Result<int, string>> GetValueAsync()
{
    if (cache.TryGetValue("key", out var value))
        return new Ok<int, string>(value); // Still allocates Task
    
    return await FetchValueAsync();
}

// ✅ Good - no allocation if cached
ValueTask<Result<int, string>> GetValueAsync()
{
    if (cache.TryGetValue("key", out var value))
        return new ValueTask<Result<int, string>>(
            new Ok<int, string>(value)
        );
    
    return new ValueTask<Result<int, string>>(FetchValueAsync());
}
```

### Avoid Await When Possible

```csharp
// ❌ Bad - unnecessary await
async Task<Result<User, string>> GetUserAsync(int id)
{
    return await database.Users.FindAsync(id);
}

// ✅ Good - return Task directly
Task<Result<User, string>> GetUserAsync(int id)
{
    return database.Users.FindAsync(id);
}
```

### Use ConfigureAwait(false) in Libraries

```csharp
async Task<Result<Data, string>> LibraryMethodAsync()
{
    var data = await FetchDataAsync().ConfigureAwait(false);
    return await ProcessDataAsync(data).ConfigureAwait(false);
}
```

### Batch Operations

Process multiple items efficiently:

```csharp
async Task<List<Result<User, string>>> ProcessUsersAsync(List<int> userIds)
{
    // Process in batches
    const int batchSize = 10;
    var results = new List<Result<User, string>>();
    
    for (int i = 0; i < userIds.Count; i += batchSize)
    {
        var batch = userIds.Skip(i).Take(batchSize);
        var batchTasks = batch.Select(id => GetUserAsync(id));
        var batchResults = await Task.WhenAll(batchTasks);
        results.AddRange(batchResults);
    }
    
    return results;
}
```

## Best Practices

### ✅ DO: Use Task for Most Cases

```csharp
// ✅ Simple and standard
async Task<Result<User, string>> GetUserAsync(int id)
{
    return await FetchUserAsync(id);
}
```

### ✅ DO: Use ValueTask for Hot Paths

```csharp
// ✅ Performance-critical code
ValueTask<Result<Config, string>> GetConfigAsync(string key)
{
    if (cache.TryGetValue(key, out var config))
        return new ValueTask<Result<Config, string>>(
            new Ok<Config, string>(config)
        );
    
    return new ValueTask<Result<Config, string>>(LoadConfigAsync(key));
}
```

### ✅ DO: Chain Async Operations

```csharp
// ✅ Clean async pipeline
return await FetchDataAsync()
    .Map(data => Parse(data))
    .Bind(parsed => ValidateAsync(parsed))
    .Map(valid => Transform(valid));
```

### ✅ DO: Handle Cancellation

```csharp
async Task<Result<Data, string>> FetchDataAsync(
    string url,
    CancellationToken ct = default)
{
    try
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(url, ct);
        return new Ok<Data, string>(Parse(response));
    }
    catch (OperationCanceledException)
    {
        return new Err<Data, string>("Operation was cancelled");
    }
}
```

### ❌ DON'T: Await ValueTask Multiple Times

```csharp
// ❌ Bad - ValueTask can only be awaited once
var task = GetConfigAsync("key");
var result1 = await task;
var result2 = await task; // Error!

// ✅ Good - await once
var result = await GetConfigAsync("key");
```

### ❌ DON'T: Mix Sync and Async

```csharp
// ❌ Bad - blocking async code
var result = FetchDataAsync().Result; // Deadlock risk!

// ✅ Good - all async or all sync
var result = await FetchDataAsync();
```

### ❌ DON'T: Forget ConfigureAwait in Libraries

```csharp
// ❌ Bad - captures context unnecessarily
await FetchDataAsync();

// ✅ Good - avoids context capture
await FetchDataAsync().ConfigureAwait(false);
```

## Summary

| Pattern | Task | ValueTask | When to Use |
|---------|------|-----------|-------------|
| Always async | ✅ | ❌ | I/O operations |
| May be sync | ❌ | ✅ | Cached data |
| Cache result | ✅ | ❌ | Shared computation |
| Hot path | ❌ | ✅ | Performance critical |
| Simple code | ✅ | ❌ | Default choice |

**Key Takeaways:**

✅ Monads supports both Task and ValueTask variants

✅ Use Task for most cases, ValueTask for hot paths

✅ Chain async operations with Bind, Map, etc.

✅ Handle exceptions by wrapping in Result

✅ Use ConfigureAwait(false) in library code

✅ Process items in parallel when independent

✅ Add timeouts and retry logic for reliability

## Next Steps

- 📖 [Error Handling](error-handling.md) - Error patterns
- 📖 [Basic Concepts](../getting-started/basic-concepts.md) - Core operations
- 📖 [API Reference](../api/) - Complete async method documentation
- 📖 [Examples](../examples/) - Real-world async scenarios

## Further Reading

- [Task vs ValueTask](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/) - Stephen Toub
- [Async Best Practices](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming) - Stephen Cleary
- [ConfigureAwait FAQ](https://devblogs.microsoft.com/dotnet/configureawait-faq/) - Stephen Toub
