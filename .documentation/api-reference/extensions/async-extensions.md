# Asynchronous Extensions

The asynchronous extension methods provide essential operations for working with `Result<T, E>` types in async/await scenarios. These methods come in two variants: `Task<T>` and `ValueTask<T>`, enabling efficient async composition without blocking threads.

## Namespace

```csharp
Monads.Results.Extensions.Async
```

## Overview

All asynchronous extension methods follow these principles:

- **Task/ValueTask Support**: Methods are available for both `Task<Result<T, E>>` and `ValueTask<Result<T, E>>`
- **Non-blocking**: All operations are truly asynchronous and don't block threads
- **Error Propagation**: Errors automatically propagate through async operation chains
- **ConfigureAwait(false)**: All internal awaits use `ConfigureAwait(false)` for better performance
- **Exception Safety**: Async exceptions are properly propagated through the Task/ValueTask

## Method Categories

### Bind Operations

#### BindAsync (Task Variants)

Chains asynchronous operations that return `Task<Result<T, E>>`.

```csharp
// Task<Result<T,E>> -> (T -> Task<Result<U,E>>) -> Task<Result<U,E>>
public static Task<Result<U, E>> BindAsync<T, E, U>(
    this Task<Result<T, E>> self,
    Func<T, Task<Result<U, E>>> operation
)

// Task<Result<T,E>> -> (T -> Result<U,E>) -> Task<Result<U,E>>
public static Task<Result<U, E>> BindAsync<T, E, U>(
    this Task<Result<T, E>> self,
    Func<T, Result<U, E>> operation
)
```

**Purpose:** Enables sequential composition of async operations that can fail.

**Example:**

```csharp
async Task<Result<User, string>> GetUserDataAsync(int userId)
{
    Task<Result<int, string>> userIdTask = ValidateUserIdAsync(userId);
    
    return await userIdTask.BindAsync(async id =>
    {
        var userData = await FetchUserFromDatabaseAsync(id);
        return userData;
    });
}

// Chaining multiple async operations
async Task<Result<string, string>> ProcessUserAsync(int userId)
{
    return await ValidateUserIdAsync(userId)
        .BindAsync(id => FetchUserAsync(id))
        .BindAsync(user => FormatUserDataAsync(user));
}
```

#### BindAsync (ValueTask Variants)

Similar to Task variants but using `ValueTask<T>` for better performance when results are often synchronous.

```csharp
// ValueTask<Result<T,E>> -> (T -> ValueTask<Result<U,E>>) -> ValueTask<Result<U,E>>
public static ValueTask<Result<U, E>> BindAsync<T, E, U>(
    this ValueTask<Result<T, E>> self,
    Func<T, ValueTask<Result<U, E>>> operation
)
```

**Example:**

```csharp
async ValueTask<Result<Data, string>> ProcessDataAsync(string input)
{
    ValueTask<Result<string, string>> validatedTask = ValidateInputAsync(input);
    
    return await validatedTask.BindAsync(async validInput =>
    {
        // Might complete synchronously from cache
        return await GetDataFromCacheOrDbAsync(validInput);
    });
}
```

### Map Operations

#### MapAsync (Task Variants)

Transforms success values asynchronously without the possibility of failure.

```csharp
// Task<Result<T,E>> -> (T -> Task<U>) -> Task<Result<U,E>>
public static Task<Result<U, E>> MapAsync<T, E, U>(
    this Task<Result<T, E>> self,
    Func<T, Task<U>> operation
)

// Task<Result<T,E>> -> (T -> U) -> Task<Result<U,E>>
public static Task<Result<U, E>> MapAsync<T, E, U>(
    this Task<Result<T, E>> self,
    Func<T, U> operation
)
```

**Purpose:** Applies async transformations to success values.

**Example:**

```csharp
async Task<Result<string, string>> FormatUserDisplayAsync(int userId)
{
    Task<Result<User, string>> userTask = GetUserAsync(userId);
    
    return await userTask.MapAsync(async user =>
    {
        // Async formatting operation
        string formatted = await FormatUserNameAsync(user.Name);
        return $"{formatted} ({user.Email})";
    });
}

// Simple synchronous transformation
Task<Result<int, string>> userAgeTask = GetUserAsync(userId)
    .MapAsync(user => user.Age);
```

#### MapAsync (ValueTask Variants)

```csharp
public static ValueTask<Result<U, E>> MapAsync<T, E, U>(
    this ValueTask<Result<T, E>> self,
    Func<T, ValueTask<U>> operation
)
```

### MapErr Operations

#### MapErrAsync

Transforms error values asynchronously.

```csharp
// Task<Result<T,E>> -> (E -> Task<U>) -> Task<Result<T,U>>
public static Task<Result<T, U>> MapErrAsync<T, E, U>(
    this Task<Result<T, E>> self,
    Func<E, Task<U>> operation
)

// ValueTask variant
public static ValueTask<Result<T, U>> MapErrAsync<T, E, U>(
    this ValueTask<Result<T, E>> self,
    Func<E, ValueTask<U>> operation
)
```

**Purpose:** Transforms or enriches error information asynchronously.

**Example:**

```csharp
async Task<Result<Data, ErrorInfo>> ProcessWithDetailedErrorsAsync(string input)
{
    Task<Result<Data, string>> processTask = ProcessDataAsync(input);
    
    return await processTask.MapErrAsync(async errorMsg =>
    {
        // Enrich error with additional async context
        var context = await GetErrorContextAsync();
        return new ErrorInfo(errorMsg, context, DateTime.UtcNow);
    });
}
```

### Match Operations

#### MatchAsync

Pattern matches on async results, applying different async functions for Ok and Err cases.

```csharp
// Task<Result<T,E>> -> (T -> Task<U>) -> (E -> Task<U>) -> Task<U>
public static Task<U> MatchAsync<T, E, U>(
    this Task<Result<T, E>> self,
    Func<T, Task<U>> onOk,
    Func<E, Task<U>> onErr
)

// ValueTask variant
public static ValueTask<U> MatchAsync<T, E, U>(
    this ValueTask<Result<T, E>> self,
    Func<T, ValueTask<U>> onOk,
    Func<E, ValueTask<U>> onErr
)
```

**Purpose:** Extracts values from async results by handling both success and error cases asynchronously.

**Example:**

```csharp
async Task<string> HandleUserRequestAsync(int userId)
{
    Task<Result<User, string>> userTask = GetUserAsync(userId);
    
    return await userTask.MatchAsync(
        onOk: async user =>
        {
            await LogSuccessAsync($"User {user.Id} processed");
            return $"Welcome, {user.Name}!";
        },
        onErr: async error =>
        {
            await LogErrorAsync($"Failed to get user: {error}");
            return "User not found";
        }
    );
}
```

### OrElse Operations

#### OrElseAsync

Provides async fallback results when the current result is an error.

```csharp
// Task<Result<T,E>> -> (() -> Task<Result<T,E>>) -> Task<Result<T,E>>
public static Task<Result<T, E>> OrElseAsync<T, E>(
    this Task<Result<T, E>> self,
    Func<Task<Result<T, E>>> fallback
)

// ValueTask variant
public static ValueTask<Result<T, E>> OrElseAsync<T, E>(
    this ValueTask<Result<T, E>> self,
    Func<ValueTask<Result<T, E>>> fallback
)
```

**Purpose:** Implements async fallback logic for error recovery.

**Example:**

```csharp
async Task<Result<Data, string>> GetDataWithFallbacksAsync(string id)
{
    Task<Result<Data, string>> primaryTask = GetFromPrimarySourceAsync(id);
    
    return await primaryTask
        .OrElseAsync(() => GetFromSecondarySourceAsync(id))
        .OrElseAsync(() => GetFromCacheAsync(id))
        .OrElseAsync(() => Task.FromResult(GetDefaultData(id)));
}
```

### Flatten Operations

#### FlattenAsync

Flattens nested async Result types.

```csharp
// Task<Result<Result<T,E>,E>> -> Task<Result<T,E>>
public static Task<Result<T, E>> FlattenAsync<T, E>(
    this Task<Result<Result<T, E>, E>> self
)

// ValueTask variant  
public static ValueTask<Result<T, E>> FlattenAsync<T, E>(
    this ValueTask<Result<Result<T, E>, E>> self
)
```

**Example:**

```csharp
async Task<Result<Data, string>> ProcessNestedAsync()
{
    // Method that returns nested results
    Task<Result<Result<Data, string>, string>> nestedTask = GetNestedResultAsync();
    
    return await nestedTask.FlattenAsync();
}
```

## Async Composition Patterns

### Sequential Processing

```csharp
async Task<Result<ProcessedData, string>> ProcessSequentiallyAsync(string input)
{
    return await ValidateInputAsync(input)
        .BindAsync(valid => ParseDataAsync(valid))
        .BindAsync(parsed => EnrichDataAsync(parsed))
        .BindAsync(enriched => FinalizeDataAsync(enriched))
        .MapAsync(final => final.ToProcessedData());
}
```

### Parallel Processing with Combination

```csharp
async Task<Result<CombinedData, string>> ProcessInParallelAsync(string input)
{
    Task<Result<DataA, string>> taskA = ProcessPartAAsync(input);
    Task<Result<DataB, string>> taskB = ProcessPartBAsync(input);
    
    // Wait for both to complete
    Result<DataA, string> resultA = await taskA;
    Result<DataB, string> resultB = await taskB;
    
    // Combine results
    return resultA.Bind(a => 
        resultB.Map(b => 
            new CombinedData(a, b)));
}
```

### Async Retry Pattern

```csharp
async Task<Result<Data, string>> WithRetryAsync<T>(
    Func<Task<Result<Data, string>>> operation,
    int maxRetries = 3)
{
    Task<Result<Data, string>> currentAttempt = operation();
    
    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        Result<Data, string> result = await currentAttempt;
        if (result.IsOk)
            return result;
            
        if (attempt < maxRetries - 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
            currentAttempt = operation();
        }
        else
        {
            return result; // Return the final failure
        }
    }
    
    throw new InvalidOperationException("Should not reach here");
}
```

## Performance Considerations

### Task vs ValueTask

**Use `Task<T>` when:**

- Result is always asynchronous (e.g., network calls, file I/O)
- Working with existing APIs that return `Task<T>`
- Long-running operations

**Use `ValueTask<T>` when:**

- Result might be available synchronously (e.g., cache hits)
- Hot paths where allocation matters
- High-frequency operations

```csharp
// Cache scenario - ValueTask is better
async ValueTask<Result<Data, string>> GetDataAsync(string key)
{
    if (cache.TryGetValue(key, out var cachedData))
        return new Ok<Data, string>(cachedData); // Completes synchronously
        
    var data = await FetchFromDatabaseAsync(key); // Async when cache miss
    cache.Set(key, data);
    return new Ok<Data, string>(data);
}
```

### ConfigureAwait Usage

All async extension methods internally use `ConfigureAwait(false)` to avoid deadlocks:

```csharp
public static async Task<Result<U, E>> MapAsync<T, E, U>(
    this Task<Result<T, E>> self,
    Func<T, Task<U>> operation)
{
    var result = await self.ConfigureAwait(false);
    return await result.Match(
        onOk: async value => 
        {
            var mapped = await operation(value).ConfigureAwait(false);
            return Success<U, E>(mapped);
        },
        onErr: error => Task.FromResult(Failure<U, E>(error))
    ).ConfigureAwait(false);
}
```

## Exception Handling

Async extensions properly handle exceptions:

```csharp
async Task<Result<Data, string>> SafeProcessAsync(string input)
{
    try
    {
        return await ProcessDataAsync(input)
            .MapAsync(async data => 
            {
                // If this throws, it's caught and wrapped
                return await TransformDataAsync(data);
            });
    }
    catch (Exception ex)
    {
        // Convert exceptions to error results
        return new Err<Data, string>(ex.Message);
    }
}
```

## Thread Safety

All async extension methods are thread-safe as they:

- Don't modify existing results (immutability)
- Don't share mutable state between operations
- Create new Task/ValueTask instances for return values
- Use proper async/await patterns

## Common Patterns

### API Gateway Pattern

```csharp
async Task<Result<ApiResponse, string>> HandleApiRequestAsync(ApiRequest request)
{
    return await ValidateRequestAsync(request)
        .BindAsync(valid => AuthenticateUserAsync(valid.UserId))
        .BindAsync(user => AuthorizeActionAsync(user, request.Action))
        .BindAsync(authorized => ProcessRequestAsync(request))
        .MapAsync(response => EnrichResponseAsync(response))
        .MapErrAsync(error => LogAndFormatErrorAsync(error));
}
```

### Database Transaction Pattern

```csharp
async Task<Result<User, string>> CreateUserTransactionAsync(UserData userData)
{
    return await BeginTransactionAsync()
        .BindAsync(tx => ValidateUserDataAsync(userData, tx))
        .BindAsync(validData => InsertUserAsync(validData, tx))
        .BindAsync(user => InsertUserProfileAsync(user, tx))
        .BindAsync(profile => CommitTransactionAsync(tx))
        .OrElseAsync(() => RollbackTransactionAsync());
}
```

## See Also

- [Synchronous Extensions](./sync-extensions.md) - Sync versions of these operations
- [Result&lt;T, E&gt; Type](../models/result.md) - Base result type
- [Async Workflows](../../examples/async-workflows.md) - Detailed async usage examples
- [Common Scenarios](../../examples/common-scenarios.md) - Real-world usage patterns
- [Error Handling Patterns](../../examples/error-handling-patterns.md) - Async error handling best practices
