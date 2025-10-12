# Async Workflows

This document demonstrates how to effectively compose asynchronous operations using the Result monad pattern. Learn how to build complex async workflows while maintaining clean error handling and avoiding common async pitfalls.

## Basic Async Composition

### Sequential Async Operations

```csharp
public class UserProfileService
{
    public async Task<Result<UserProfile, string>> GetCompleteUserProfileAsync(int userId)
    {
        return await GetUserAsync(userId)
            .BindAsync(user => GetUserPreferencesAsync(user.Id))
            .BindAsync(async preferences => 
            {
                var avatarResult = await GetUserAvatarAsync(userId);
                return avatarResult.Map(avatar => 
                    new UserProfile(preferences.User, preferences, avatar));
            })
            .MapAsync(profile => EnrichProfileWithStatsAsync(profile));
    }
    
    private async Task<Result<User, string>> GetUserAsync(int userId)
    {
        try
        {
            User? user = await _userRepository.GetByIdAsync(userId);
            return user is not null
                ? new Ok<User, string>(user)
                : new Err<User, string>($"User {userId} not found");
        }
        catch (Exception ex)
        {
            return new Err<User, string>($"Database error: {ex.Message}");
        }
    }
    
    private async Task<Result<UserWithPreferences, string>> GetUserPreferencesAsync(int userId)
    {
        try
        {
            var user = await GetUserAsync(userId);
            if (user.IsErr)
                return user.MapErr(e => e);
                
            var preferences = await _preferencesRepository.GetByUserIdAsync(userId);
            return new Ok<UserWithPreferences, string>(
                new UserWithPreferences(user.Match(u => u, _ => throw new InvalidOperationException()), preferences));
        }
        catch (Exception ex)
        {
            return new Err<UserWithPreferences, string>($"Preferences error: {ex.Message}");
        }
    }
}
```

### Parallel Async Operations

```csharp
public class DashboardService
{
    public async Task<Result<Dashboard, string>> BuildDashboardAsync(int userId)
    {
        // Start all async operations in parallel
        Task<Result<User, string>> userTask = GetUserAsync(userId);
        Task<Result<List<Notification>, string>> notificationsTask = GetNotificationsAsync(userId);
        Task<Result<List<Activity>, string>> activitiesTask = GetRecentActivitiesAsync(userId);
        Task<Result<UserStats, string>> statsTask = GetUserStatsAsync(userId);
        
        // Wait for all to complete
        await Task.WhenAll(userTask, notificationsTask, activitiesTask, statsTask);
        
        // Combine results
        Result<User, string> userResult = await userTask;
        Result<List<Notification>, string> notificationsResult = await notificationsTask;
        Result<List<Activity>, string> activitiesResult = await activitiesTask;
        Result<UserStats, string> statsResult = await statsTask;
        
        // Use applicative pattern to combine multiple Results
        return CombineResults(userResult, notificationsResult, activitiesResult, statsResult)
            .Map(data => new Dashboard(
                User: data.User,
                Notifications: data.Notifications,
                Activities: data.Activities,
                Stats: data.Stats
            ));
    }
    
    private static Result<(User User, List<Notification> Notifications, List<Activity> Activities, UserStats Stats), string> 
        CombineResults(
            Result<User, string> userResult,
            Result<List<Notification>, string> notificationsResult,
            Result<List<Activity>, string> activitiesResult,
            Result<UserStats, string> statsResult)
    {
        return userResult.Bind(user =>
            notificationsResult.Bind(notifications =>
                activitiesResult.Bind(activities =>
                    statsResult.Map(stats =>
                        (user, notifications, activities, stats)))));
    }
    
    // Alternative: Allow partial failures with graceful degradation
    public async Task<DashboardWithStatus> BuildDashboardWithFallbacksAsync(int userId)
    {
        // Start all operations in parallel
        Task<Result<User, string>> userTask = GetUserAsync(userId);
        Task<Result<List<Notification>, string>> notificationsTask = GetNotificationsAsync(userId);
        Task<Result<List<Activity>, string>> activitiesTask = GetRecentActivitiesAsync(userId);
        Task<Result<UserStats, string>> statsTask = GetUserStatsAsync(userId);
        
        await Task.WhenAll(userTask, notificationsTask, activitiesTask, statsTask);
        
        // Extract results with defaults for failures
        User? user = (await userTask).Match(u => u, _ => null);
        List<Notification> notifications = (await notificationsTask).Match(n => n, _ => new List<Notification>());
        List<Activity> activities = (await activitiesTask).Match(a => a, _ => new List<Activity>());
        UserStats stats = (await statsTask).Match(s => s, _ => UserStats.Empty);
        
        return new DashboardWithStatus(
            User: user,
            Notifications: notifications,
            Activities: activities,
            Stats: stats,
            HasErrors: user is null,
            ErrorMessages: GetErrorMessages(userTask, notificationsTask, activitiesTask, statsTask)
        );
    }
}
```

## Complex Async Patterns

### Async Pipeline with Conditional Logic

```csharp
public class OrderProcessingService
{
    public async Task<Result<ProcessedOrder, OrderError>> ProcessOrderAsync(OrderRequest request)
    {
        return await ValidateOrderRequestAsync(request)
            .BindAsync(async validRequest => await CheckInventoryAsync(validRequest))
            .BindAsync(async checkedOrder => await ProcessPaymentAsync(checkedOrder))
            .BindAsync(async paidOrder => 
            {
                // Conditional logic: Only process shipping for physical items
                if (paidOrder.HasPhysicalItems)
                {
                    return await ProcessShippingAsync(paidOrder);
                }
                else
                {
                    return await ProcessDigitalDeliveryAsync(paidOrder);
                }
            })
            .BindAsync(async deliveredOrder => await SendConfirmationAsync(deliveredOrder))
            .MapAsync(async finalOrder => await LogOrderProcessingAsync(finalOrder));
    }
    
    private async Task<Result<ValidatedOrderRequest, OrderError>> ValidateOrderRequestAsync(OrderRequest request)
    {
        // Async validation that might call external services
        return await ValidateCustomerAsync(request.CustomerId)
            .BindAsync(async customer => await ValidateItemsAsync(request.Items))
            .BindAsync(async items => await ValidateShippingAddressAsync(request.ShippingAddress))
            .MapAsync(async address => 
            {
                await LogValidationAsync(request.Id);
                return new ValidatedOrderRequest(request, DateTime.UtcNow);
            });
    }
    
    private async Task<Result<Customer, OrderError>> ValidateCustomerAsync(int customerId)
    {
        try
        {
            Customer? customer = await _customerService.GetCustomerAsync(customerId);
            
            if (customer is null)
                return new Err<Customer, OrderError>(OrderError.InvalidCustomer("Customer not found"));
                
            if (!customer.IsActive)
                return new Err<Customer, OrderError>(OrderError.InvalidCustomer("Customer account is inactive"));
                
            // Check customer credit limit asynchronously
            bool hasCredit = await _creditService.CheckCreditLimitAsync(customer.Id);
            if (!hasCredit)
                return new Err<Customer, OrderError>(OrderError.InsufficientCredit("Credit limit exceeded"));
                
            return new Ok<Customer, OrderError>(customer);
        }
        catch (Exception ex)
        {
            return new Err<Customer, OrderError>(OrderError.ValidationFailed($"Customer validation failed: {ex.Message}"));
        }
    }
}
```

### Async Retry with Circuit Breaker

```csharp
public class ResilientApiClient
{
    private readonly HttpClient _httpClient;
    private readonly CircuitBreaker _circuitBreaker;
    
    public async Task<Result<ApiResponse<T>, ApiError>> GetWithRetryAsync<T>(
        string endpoint,
        int maxRetries = 3,
        CancellationToken cancellationToken = default) where T : notnull
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            return await RetryWithBackoffAsync(
                () => MakeRequestAsync<T>(endpoint, cancellationToken),
                maxRetries,
                cancellationToken
            );
        });
    }
    
    private async Task<Result<ApiResponse<T>, ApiError>> RetryWithBackoffAsync<T>(
        Func<Task<Result<ApiResponse<T>, ApiError>>> operation,
        int maxRetries,
        CancellationToken cancellationToken) where T : notnull
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Result<ApiResponse<T>, ApiError> result = await operation();
                
                if (result.IsOk)
                    return result;
                    
                // Check if we should retry based on error type
                bool shouldRetry = result.IsErrAnd(error => 
                    error.IsRetryable && attempt < maxRetries);
                    
                if (!shouldRetry)
                    return result;
                    
                // Exponential backoff with jitter
                TimeSpan delay = CalculateDelay(attempt);
                await Task.Delay(delay, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return new Err<ApiResponse<T>, ApiError>(
                    ApiError.Timeout("Request was cancelled"));
            }
        }
        
        // This should never be reached, but satisfy compiler
        return await operation();
    }
    
    private async Task<Result<ApiResponse<T>, ApiError>> MakeRequestAsync<T>(
        string endpoint, 
        CancellationToken cancellationToken) where T : notnull
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                T? data = JsonSerializer.Deserialize<T>(content);
                
                return data is not null
                    ? new Ok<ApiResponse<T>, ApiError>(new ApiResponse<T>(data, response.StatusCode))
                    : new Err<ApiResponse<T>, ApiError>(ApiError.InvalidResponse("Deserialization returned null"));
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return new Err<ApiResponse<T>, ApiError>(
                    ApiError.HttpError(response.StatusCode, errorContent));
            }
        }
        catch (HttpRequestException ex)
        {
            return new Err<ApiResponse<T>, ApiError>(
                ApiError.NetworkError($"Network error: {ex.Message}"));
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return new Err<ApiResponse<T>, ApiError>(
                ApiError.Timeout("Request timeout"));
        }
        catch (JsonException ex)
        {
            return new Err<ApiResponse<T>, ApiError>(
                ApiError.InvalidResponse($"JSON parsing failed: {ex.Message}"));
        }
    }
    
    private static TimeSpan CalculateDelay(int attempt)
    {
        // Exponential backoff with jitter
        double baseDelay = Math.Pow(2, attempt - 1) * 1000; // Base delay in milliseconds
        double jitter = Random.Shared.NextDouble() * 0.1 * baseDelay; // 10% jitter
        return TimeSpan.FromMilliseconds(baseDelay + jitter);
    }
}
```

### Streaming Data Processing

```csharp
public class DataStreamProcessor
{
    public async Task<Result<ProcessingSummary, ProcessingError>> ProcessDataStreamAsync(
        IAsyncEnumerable<DataChunk> dataStream,
        CancellationToken cancellationToken = default)
    {
        var summary = new ProcessingSummary();
        var errors = new List<ProcessingError>();
        
        try
        {
            await foreach (DataChunk chunk in dataStream.WithCancellation(cancellationToken))
            {
                Result<ProcessedChunk, ProcessingError> result = await ProcessChunkAsync(chunk);
                
                result.Match(
                    onOk: processedChunk => 
                    {
                        summary = summary.AddSuccess(processedChunk);
                    },
                    onErr: error => 
                    {
                        errors.Add(error);
                        summary = summary.AddError();
                    }
                );
                
                // Optional: Fail fast on critical errors
                if (result.IsErrAnd(error => error.IsCritical))
                {
                    return new Err<ProcessingSummary, ProcessingError>(
                        ProcessingError.Critical($"Critical error in chunk {chunk.Id}: {result.Match(_ => "", e => e.Message)}"));
                }
                
                // Optional: Stop processing if too many errors
                if (errors.Count > 10)
                {
                    return new Err<ProcessingSummary, ProcessingError>(
                        ProcessingError.TooManyErrors($"Stopped processing after {errors.Count} errors"));
                }
            }
            
            return new Ok<ProcessingSummary, ProcessingError>(summary.WithErrors(errors));
        }
        catch (OperationCanceledException)
        {
            return new Err<ProcessingSummary, ProcessingError>(
                ProcessingError.Cancelled("Stream processing was cancelled"));
        }
        catch (Exception ex)
        {
            return new Err<ProcessingSummary, ProcessingError>(
                ProcessingError.Unexpected($"Unexpected error: {ex.Message}"));
        }
    }
    
    private async Task<Result<ProcessedChunk, ProcessingError>> ProcessChunkAsync(DataChunk chunk)
    {
        return await ValidateChunkAsync(chunk)
            .BindAsync(async validChunk => await TransformChunkAsync(validChunk))
            .BindAsync(async transformedChunk => await SaveChunkAsync(transformedChunk))
            .MapAsync(async savedChunk => await NotifyChunkProcessedAsync(savedChunk));
    }
    
    // Batch processing with controlled concurrency
    public async Task<Result<BatchProcessingSummary, ProcessingError>> ProcessBatchAsync(
        IEnumerable<DataItem> items,
        int maxConcurrency = 5,
        CancellationToken cancellationToken = default)
    {
        using var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        
        Task<Result<ProcessedItem, ProcessingError>>[] tasks = items
            .Select(item => ProcessItemWithSemaphoreAsync(item, semaphore, cancellationToken))
            .ToArray();
        
        Result<ProcessedItem, ProcessingError>[] results = await Task.WhenAll(tasks);
        
        var successes = new List<ProcessedItem>();
        var errors = new List<ProcessingError>();
        
        foreach (Result<ProcessedItem, ProcessingError> result in results)
        {
            result.Match(
                onOk: success => successes.Add(success),
                onErr: error => errors.Add(error)
            );
        }
        
        var summary = new BatchProcessingSummary(
            TotalItems: items.Count(),
            SuccessfulItems: successes.Count,
            FailedItems: errors.Count,
            Successes: successes,
            Errors: errors
        );
        
        return new Ok<BatchProcessingSummary, ProcessingError>(summary);
    }
    
    private async Task<Result<ProcessedItem, ProcessingError>> ProcessItemWithSemaphoreAsync(
        DataItem item,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        
        try
        {
            return await ProcessSingleItemAsync(item, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
```

### Event-Driven Async Workflows

```csharp
public class OrderWorkflowOrchestrator
{
    private readonly IEventBus _eventBus;
    private readonly Dictionary<string, TaskCompletionSource<Result<WorkflowEvent, WorkflowError>>> _pendingWorkflows = new();
    
    public async Task<Result<CompletedWorkflow, WorkflowError>> ExecuteOrderWorkflowAsync(
        OrderCreatedEvent orderEvent,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        if (timeout == default)
            timeout = TimeSpan.FromMinutes(5);
            
        string workflowId = Guid.NewGuid().ToString();
        var workflowState = new OrderWorkflowState(orderEvent.OrderId, workflowId);
        
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        
        try
        {
            // Step 1: Process payment
            Result<PaymentProcessedEvent, WorkflowError> paymentResult = 
                await ProcessPaymentStepAsync(workflowState, combinedCts.Token);
                
            if (paymentResult.IsErr)
                return paymentResult.MapErr(e => e);
                
            // Step 2: Reserve inventory
            Result<InventoryReservedEvent, WorkflowError> inventoryResult = 
                await ProcessInventoryStepAsync(workflowState, combinedCts.Token);
                
            if (inventoryResult.IsErr)
            {
                // Compensate: Refund payment
                await CompensatePaymentAsync(workflowState);
                return inventoryResult.MapErr(e => e);
            }
            
            // Step 3: Schedule shipping
            Result<ShippingScheduledEvent, WorkflowError> shippingResult = 
                await ProcessShippingStepAsync(workflowState, combinedCts.Token);
                
            if (shippingResult.IsErr)
            {
                // Compensate: Release inventory and refund payment
                await CompensateInventoryAsync(workflowState);
                await CompensatePaymentAsync(workflowState);
                return shippingResult.MapErr(e => e);
            }
            
            // Success: Create completed workflow
            var completedWorkflow = new CompletedWorkflow(
                WorkflowId: workflowId,
                OrderId: orderEvent.OrderId,
                PaymentId: paymentResult.Match(p => p.PaymentId, _ => ""),
                ReservationId: inventoryResult.Match(i => i.ReservationId, _ => ""),
                ShippingId: shippingResult.Match(s => s.ShippingId, _ => ""),
                CompletedAt: DateTime.UtcNow
            );
            
            return new Ok<CompletedWorkflow, WorkflowError>(completedWorkflow);
        }
        catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
        {
            return new Err<CompletedWorkflow, WorkflowError>(
                WorkflowError.Timeout($"Workflow {workflowId} timed out after {timeout}"));
        }
        catch (OperationCanceledException)
        {
            return new Err<CompletedWorkflow, WorkflowError>(
                WorkflowError.Cancelled($"Workflow {workflowId} was cancelled"));
        }
    }
    
    private async Task<Result<PaymentProcessedEvent, WorkflowError>> ProcessPaymentStepAsync(
        OrderWorkflowState state,
        CancellationToken cancellationToken)
    {
        // Register for payment events
        var tcs = new TaskCompletionSource<Result<WorkflowEvent, WorkflowError>>();
        _pendingWorkflows[state.WorkflowId] = tcs;
        
        try
        {
            // Send payment command
            await _eventBus.PublishAsync(new ProcessPaymentCommand(state.OrderId, state.WorkflowId), cancellationToken);
            
            // Wait for payment event
            Result<WorkflowEvent, WorkflowError> result = await tcs.Task.WaitAsync(cancellationToken);
            
            return result.Bind(eventData =>
            {
                if (eventData is PaymentProcessedEvent paymentEvent)
                {
                    return new Ok<PaymentProcessedEvent, WorkflowError>(paymentEvent);
                }
                else if (eventData is PaymentFailedEvent failedEvent)
                {
                    return new Err<PaymentProcessedEvent, WorkflowError>(
                        WorkflowError.PaymentFailed(failedEvent.Reason));
                }
                else
                {
                    return new Err<PaymentProcessedEvent, WorkflowError>(
                        WorkflowError.UnexpectedEvent($"Unexpected event type: {eventData.GetType().Name}"));
                }
            });
        }
        finally
        {
            _pendingWorkflows.Remove(state.WorkflowId);
        }
    }
    
    // Event handler for workflow events
    public async Task HandleWorkflowEventAsync(WorkflowEvent workflowEvent)
    {
        if (_pendingWorkflows.TryGetValue(workflowEvent.WorkflowId, out TaskCompletionSource<Result<WorkflowEvent, WorkflowError>>? tcs))
        {
            tcs.SetResult(new Ok<WorkflowEvent, WorkflowError>(workflowEvent));
        }
    }
}
```

## Async Testing Patterns

### Testing Async Workflows

```csharp
[Test]
public async Task ProcessOrderAsync_ValidOrder_CompletesSuccessfully()
{
    // Arrange
    var orderRequest = CreateValidOrderRequest();
    var mockPaymentService = new Mock<IPaymentService>();
    var mockInventoryService = new Mock<IInventoryService>();
    var mockShippingService = new Mock<IShippingService>();
    
    mockPaymentService
        .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
        .ReturnsAsync(new Ok<PaymentResult, PaymentError>(new PaymentResult("payment-123")));
        
    mockInventoryService
        .Setup(x => x.ReserveInventoryAsync(It.IsAny<InventoryRequest>()))
        .ReturnsAsync(new Ok<InventoryReservation, InventoryError>(new InventoryReservation("reservation-456")));
        
    mockShippingService
        .Setup(x => x.ScheduleShippingAsync(It.IsAny<ShippingRequest>()))
        .ReturnsAsync(new Ok<ShippingSchedule, ShippingError>(new ShippingSchedule("shipping-789")));
    
    var service = new OrderProcessingService(
        mockPaymentService.Object,
        mockInventoryService.Object, 
        mockShippingService.Object);
    
    // Act
    Result<ProcessedOrder, OrderError> result = await service.ProcessOrderAsync(orderRequest);
    
    // Assert
    Assert.That(result.IsOk, Is.True);
    
    result.Match(
        onOk: order => 
        {
            Assert.That(order.PaymentId, Is.EqualTo("payment-123"));
            Assert.That(order.ReservationId, Is.EqualTo("reservation-456"));
            Assert.That(order.ShippingId, Is.EqualTo("shipping-789"));
        },
        onErr: error => Assert.Fail($"Expected success but got error: {error}")
    );
}

[Test]
public async Task ProcessOrderAsync_PaymentFails_ReturnsPaymentError()
{
    // Arrange
    var orderRequest = CreateValidOrderRequest();
    var mockPaymentService = new Mock<IPaymentService>();
    
    mockPaymentService
        .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
        .ReturnsAsync(new Err<PaymentResult, PaymentError>(
            PaymentError.InsufficientFunds("Card has insufficient funds")));
    
    var service = new OrderProcessingService(mockPaymentService.Object, null!, null!);
    
    // Act
    Result<ProcessedOrder, OrderError> result = await service.ProcessOrderAsync(orderRequest);
    
    // Assert
    Assert.That(result.IsErr, Is.True);
    
    result.Match(
        onOk: order => Assert.Fail("Expected error but got success"),
        onErr: error => Assert.That(error.Message, Does.Contain("insufficient funds"))
    );
}
```

### Testing Async Error Scenarios

```csharp
[Test]
public async Task GetDataAsync_NetworkTimeout_ReturnsTimeoutError()
{
    // Arrange
    var mockHttpClient = new Mock<HttpClient>();
    mockHttpClient
        .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ThrowsAsync(new TaskCanceledException("Timeout", new TimeoutException()));
    
    var apiClient = new ResilientApiClient(mockHttpClient.Object);
    
    // Act
    Result<ApiResponse<TestData>, ApiError> result = await apiClient.GetWithRetryAsync<TestData>("/test");
    
    // Assert
    Assert.That(result.IsErr, Is.True);
    
    result.Match(
        onOk: response => Assert.Fail("Expected timeout error"),
        onErr: error => 
        {
            Assert.That(error.ErrorType, Is.EqualTo(ApiErrorType.Timeout));
            Assert.That(error.Message, Does.Contain("timeout"));
        }
    );
}

[Test]
public async Task ProcessBatchAsync_CancellationRequested_ReturnsCancelledError()
{
    // Arrange
    var items = Enumerable.Range(1, 100).Select(i => new DataItem(i)).ToList();
    var processor = new DataStreamProcessor();
    
    using var cts = new CancellationTokenSource();
    
    // Act
    Task<Result<BatchProcessingSummary, ProcessingError>> task = processor.ProcessBatchAsync(items, 5, cts.Token);
    
    // Cancel after a short delay
    _ = Task.Delay(100).ContinueWith(_ => cts.Cancel());
    
    Result<BatchProcessingSummary, ProcessingError> result = await task;
    
    // Assert
    Assert.That(result.IsErr, Is.True);
    
    result.Match(
        onOk: summary => Assert.Fail("Expected cancellation error"),
        onErr: error => Assert.That(error.ErrorType, Is.EqualTo(ProcessingErrorType.Cancelled))
    );
}
```

## Performance Considerations

### ConfigureAwait Usage

```csharp
// Good: Use ConfigureAwait(false) in library code
public async Task<Result<Data, string>> GetDataAsync()
{
    try
    {
        HttpResponseMessage response = await _httpClient.GetAsync("/api/data").ConfigureAwait(false);
        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        Data? data = JsonSerializer.Deserialize<Data>(content);
        return data is not null
            ? new Ok<Data, string>(data)
            : new Err<Data, string>("Deserialization failed");
    }
    catch (Exception ex)
    {
        return new Err<Data, string>(ex.Message);
    }
}
```

### Memory Efficiency with ValueTask

```csharp
public class CachedDataService
{
    private readonly ConcurrentDictionary<string, Data> _cache = new();
    
    // Use ValueTask when result might be available synchronously
    public async ValueTask<Result<Data, string>> GetDataAsync(string key)
    {
        // Check cache first (synchronous)
        if (_cache.TryGetValue(key, out Data? cachedData))
        {
            return new Ok<Data, string>(cachedData);
        }
        
        // Fetch from database (asynchronous)
        try
        {
            Data data = await _database.GetDataAsync(key).ConfigureAwait(false);
            _cache[key] = data;
            return new Ok<Data, string>(data);
        }
        catch (Exception ex)
        {
            return new Err<Data, string>(ex.Message);
        }
    }
}
```

## Best Practices Summary

1. **Use appropriate async patterns** - ValueTask for hot paths, Task for always-async operations
2. **Handle cancellation properly** - Always pass and respect CancellationTokens
3. **Use ConfigureAwait(false)** in library code to avoid deadlocks
4. **Implement timeout handling** for external calls
5. **Use circuit breakers** for resilience against failing services
6. **Control concurrency** with SemaphoreSlim for resource-intensive operations  
7. **Implement proper retry logic** with exponential backoff
8. **Test both success and failure paths** including cancellation scenarios
9. **Use parallel execution** where operations are independent
10. **Implement compensation patterns** for complex workflows that can fail partway through

These patterns will help you build robust, scalable async applications with proper error handling and resource management.
