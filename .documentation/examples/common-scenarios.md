# Common Scenarios

This document provides real-world usage examples of the Monads library, demonstrating practical applications across various domains. Each scenario shows how to effectively use `Result<T, E>` types to handle success and error cases.

## 1. File I/O Operations

### Reading Configuration Files

```csharp
public static class ConfigurationLoader
{
    public static Result<AppConfig, string> LoadConfiguration(string filePath)
    {
        return ReadFileContent(filePath)
            .Bind(content => ParseJson<AppConfig>(content))
            .Bind(config => ValidateConfiguration(config))
            .Map(config => config with { LoadedAt = DateTime.UtcNow });
    }
    
    private static Result<string, string> ReadFileContent(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return new Err<string, string>($"Configuration file not found: {filePath}");
                
            string content = File.ReadAllText(filePath);
            return new Ok<string, string>(content);
        }
        catch (Exception ex)
        {
            return new Err<string, string>($"Failed to read file: {ex.Message}");
        }
    }
    
    private static Result<T, string> ParseJson<T>(string json) where T : notnull
    {
        try
        {
            T? result = JsonSerializer.Deserialize<T>(json);
            return result is not null
                ? new Ok<T, string>(result)
                : new Err<T, string>("Deserialization returned null");
        }
        catch (JsonException ex)
        {
            return new Err<T, string>($"Invalid JSON: {ex.Message}");
        }
    }
    
    private static Result<AppConfig, string> ValidateConfiguration(AppConfig config)
    {
        if (string.IsNullOrEmpty(config.DatabaseConnection))
            return new Err<AppConfig, string>("Database connection string is required");
            
        if (config.Port <= 0 || config.Port > 65535)
            return new Err<AppConfig, string>("Port must be between 1 and 65535");
            
        return new Ok<AppConfig, string>(config);
    }
}

// Usage
Result<AppConfig, string> configResult = ConfigurationLoader.LoadConfiguration("appsettings.json");

string message = configResult.Match(
    onOk: config => $"Configuration loaded successfully. Server will run on port {config.Port}",
    onErr: error => $"Failed to load configuration: {error}"
);

Console.WriteLine(message);
```

## 2. HTTP API Calls

### REST API Client with Retry Logic

```csharp
public class UserApiClient
{
    private readonly HttpClient _httpClient;
    
    public UserApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Result<User, ApiError>> GetUserAsync(int userId)
    {
        return await ExecuteWithRetry(() => FetchUserAsync(userId), maxRetries: 3);
    }
    
    private async Task<Result<User, ApiError>> FetchUserAsync(int userId)
    {
        return await ValidateUserId(userId)
            .BindAsync(async id => await MakeHttpRequest($"/api/users/{id}"))
            .BindAsync(response => ParseUserResponse(response))
            .MapErrAsync(error => EnrichErrorWithContext(error, userId));
    }
    
    private static Result<int, ApiError> ValidateUserId(int userId)
    {
        return userId > 0
            ? new Ok<int, ApiError>(userId)
            : new Err<int, ApiError>(new ApiError("Invalid user ID", 400));
    }
    
    private async Task<Result<HttpResponseMessage, ApiError>> MakeHttpRequest(string endpoint)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            
            return response.IsSuccessStatusCode
                ? new Ok<HttpResponseMessage, ApiError>(response)
                : new Err<HttpResponseMessage, ApiError>(
                    new ApiError($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}", 
                               (int)response.StatusCode));
        }
        catch (HttpRequestException ex)
        {
            return new Err<HttpResponseMessage, ApiError>(
                new ApiError($"Network error: {ex.Message}", 0));
        }
        catch (TaskCanceledException ex)
        {
            return new Err<HttpResponseMessage, ApiError>(
                new ApiError($"Request timeout: {ex.Message}", 408));
        }
    }
    
    private static async Task<Result<User, ApiError>> ParseUserResponse(HttpResponseMessage response)
    {
        try
        {
            string json = await response.Content.ReadAsStringAsync();
            User? user = JsonSerializer.Deserialize<User>(json);
            
            return user is not null
                ? new Ok<User, ApiError>(user)
                : new Err<User, ApiError>(new ApiError("Invalid user data received", 422));
        }
        catch (JsonException ex)
        {
            return new Err<User, ApiError>(new ApiError($"JSON parsing failed: {ex.Message}", 422));
        }
    }
    
    private static async Task<Result<T, ApiError>> ExecuteWithRetry<T>(
        Func<Task<Result<T, ApiError>>> operation,
        int maxRetries)
        where T : notnull
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            Result<T, ApiError> result = await operation();
            
            if (result.IsOk || !ShouldRetry(result, attempt, maxRetries))
                return result;
                
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt - 1))); // Exponential backoff
        }
        
        return await operation(); // Final attempt
    }
    
    private static bool ShouldRetry<T>(Result<T, ApiError> result, int attempt, int maxRetries)
        where T : notnull
    {
        return attempt < maxRetries && 
               result.IsErrAnd(error => error.StatusCode >= 500 || error.StatusCode == 408);
    }
}

// Usage
var client = new UserApiClient(httpClient);
Result<User, ApiError> userResult = await client.GetUserAsync(123);

await userResult.MatchAsync(
    onOk: async user => 
    {
        Console.WriteLine($"User: {user.Name} ({user.Email})");
        await ProcessUserAsync(user);
    },
    onErr: async error => 
    {
        Console.WriteLine($"Failed to get user: {error.Message}");
        await LogErrorAsync(error);
    }
);
```

## 3. Database Operations

### Repository Pattern with Transactions

```csharp
public class OrderRepository
{
    private readonly IDbConnection _connection;
    
    public OrderRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    
    public async Task<Result<Order, DatabaseError>> CreateOrderAsync(CreateOrderRequest request)
    {
        return await ExecuteInTransaction(async transaction =>
        {
            return await ValidateOrderRequest(request)
                .BindAsync(async validRequest => await InsertOrderAsync(validRequest, transaction))
                .BindAsync(async order => await InsertOrderItemsAsync(order, request.Items, transaction))
                .BindAsync(async order => await UpdateInventoryAsync(order, transaction))
                .MapAsync(async order => await EnrichOrderWithDetailsAsync(order));
        });
    }
    
    private async Task<Result<T, DatabaseError>> ExecuteInTransaction<T>(
        Func<IDbTransaction, Task<Result<T, DatabaseError>>> operation)
        where T : notnull
    {
        using IDbTransaction transaction = _connection.BeginTransaction();
        
        try
        {
            Result<T, DatabaseError> result = await operation(transaction);
            
            if (result.IsOk)
            {
                transaction.Commit();
                return result;
            }
            else
            {
                transaction.Rollback();
                return result;
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new Err<T, DatabaseError>(
                new DatabaseError($"Transaction failed: {ex.Message}", ex));
        }
    }
    
    private static Result<CreateOrderRequest, DatabaseError> ValidateOrderRequest(
        CreateOrderRequest request)
    {
        if (request.CustomerId <= 0)
            return new Err<CreateOrderRequest, DatabaseError>(
                new DatabaseError("Invalid customer ID"));
                
        if (!request.Items.Any())
            return new Err<CreateOrderRequest, DatabaseError>(
                new DatabaseError("Order must contain at least one item"));
                
        if (request.Items.Any(item => item.Quantity <= 0))
            return new Err<CreateOrderRequest, DatabaseError>(
                new DatabaseError("All items must have positive quantity"));
                
        return new Ok<CreateOrderRequest, DatabaseError>(request);
    }
    
    private async Task<Result<Order, DatabaseError>> InsertOrderAsync(
        CreateOrderRequest request, 
        IDbTransaction transaction)
    {
        try
        {
            const string sql = @"
                INSERT INTO Orders (CustomerId, OrderDate, Status, TotalAmount)
                VALUES (@CustomerId, @OrderDate, @Status, @TotalAmount);
                SELECT LAST_INSERT_ID();";
                
            int orderId = await _connection.QuerySingleAsync<int>(sql, new
            {
                CustomerId = request.CustomerId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = request.Items.Sum(i => i.Price * i.Quantity)
            }, transaction);
            
            var order = new Order(orderId, request.CustomerId, DateTime.UtcNow, "Pending");
            return new Ok<Order, DatabaseError>(order);
        }
        catch (Exception ex)
        {
            return new Err<Order, DatabaseError>(
                new DatabaseError($"Failed to insert order: {ex.Message}", ex));
        }
    }
}

// Usage
var repository = new OrderRepository(connection);
var request = new CreateOrderRequest(customerId: 123, items: orderItems);

Result<Order, DatabaseError> orderResult = await repository.CreateOrderAsync(request);

string outcome = orderResult.Match(
    onOk: order => $"Order {order.Id} created successfully",
    onErr: error => $"Failed to create order: {error.Message}"
);

Console.WriteLine(outcome);
```

## 4. Form Validation

### Multi-Step Form Validation

```csharp
public static class UserRegistrationValidator
{
    public static Result<ValidatedUser, ValidationError> ValidateRegistration(
        UserRegistrationForm form)
    {
        return ValidateEmail(form.Email)
            .Bind(_ => ValidatePassword(form.Password))
            .Bind(_ => ValidateConfirmPassword(form.Password, form.ConfirmPassword))
            .Bind(_ => ValidateAge(form.Age))
            .Bind(_ => ValidateTermsAcceptance(form.AcceptedTerms))
            .Map(_ => new ValidatedUser(form.Email, form.Password, form.Age));
    }
    
    private static Result<string, ValidationError> ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new Err<string, ValidationError>(
                new ValidationError("Email", "Email is required"));
                
        if (!IsValidEmail(email))
            return new Err<string, ValidationError>(
                new ValidationError("Email", "Invalid email format"));
                
        return new Ok<string, ValidationError>(email);
    }
    
    private static Result<string, ValidationError> ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return new Err<string, ValidationError>(
                new ValidationError("Password", "Password is required"));
                
        if (password.Length < 8)
            return new Err<string, ValidationError>(
                new ValidationError("Password", "Password must be at least 8 characters"));
                
        if (!password.Any(char.IsUpper))
            return new Err<string, ValidationError>(
                new ValidationError("Password", "Password must contain an uppercase letter"));
                
        if (!password.Any(char.IsDigit))
            return new Err<string, ValidationError>(
                new ValidationError("Password", "Password must contain a number"));
                
        return new Ok<string, ValidationError>(password);
    }
    
    private static Result<Unit, ValidationError> ValidateConfirmPassword(
        string password, 
        string confirmPassword)
    {
        if (password != confirmPassword)
            return new Err<Unit, ValidationError>(
                new ValidationError("ConfirmPassword", "Passwords do not match"));
                
        return new Ok<Unit, ValidationError>(Unit.Default);
    }
    
    private static Result<int, ValidationError> ValidateAge(int age)
    {
        if (age < 13)
            return new Err<int, ValidationError>(
                new ValidationError("Age", "Must be at least 13 years old"));
                
        if (age > 120)
            return new Err<int, ValidationError>(
                new ValidationError("Age", "Invalid age"));
                
        return new Ok<int, ValidationError>(age);
    }
    
    private static Result<Unit, ValidationError> ValidateTermsAcceptance(bool accepted)
    {
        return accepted
            ? new Ok<Unit, ValidationError>(Unit.Default)
            : new Err<Unit, ValidationError>(
                new ValidationError("Terms", "You must accept the terms and conditions"));
    }
    
    private static bool IsValidEmail(string email) =>
        email.Contains('@') && email.Contains('.') && !email.StartsWith('@');
}

// Usage
var form = new UserRegistrationForm("user@example.com", "Password123", "Password123", 25, true);
Result<ValidatedUser, ValidationError> validationResult = 
    UserRegistrationValidator.ValidateRegistration(form);

string message = validationResult.Match(
    onOk: user => $"Registration valid for {user.Email}",
    onErr: error => $"Validation failed: {error.Field} - {error.Message}"
);

Console.WriteLine(message);
```

## 5. Parsing and Data Transformation

### CSV Processing Pipeline

```csharp
public class CsvProcessor
{
    public static Result<List<Customer>, ProcessingError> ProcessCustomerCsv(string csvContent)
    {
        return ParseCsvLines(csvContent)
            .Bind(lines => ValidateHeaders(lines))
            .Bind(dataLines => ParseCustomers(dataLines))
            .Bind(customers => ValidateCustomers(customers))
            .Map(customers => customers.ToList());
    }
    
    private static Result<string[], ProcessingError> ParseCsvLines(string csvContent)
    {
        if (string.IsNullOrWhiteSpace(csvContent))
            return new Err<string[], ProcessingError>(
                new ProcessingError("Empty CSV content"));
                
        string[] lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(line => line.Trim())
                                  .Where(line => !string.IsNullOrEmpty(line))
                                  .ToArray();
                                  
        return lines.Length > 0
            ? new Ok<string[], ProcessingError>(lines)
            : new Err<string[], ProcessingError>(
                new ProcessingError("No valid lines found in CSV"));
    }
    
    private static Result<string[], ProcessingError> ValidateHeaders(string[] lines)
    {
        if (lines.Length < 2)
            return new Err<string[], ProcessingError>(
                new ProcessingError("CSV must contain header and at least one data row"));
                
        string[] expectedHeaders = { "Name", "Email", "Age", "City" };
        string[] actualHeaders = lines[0].Split(',').Select(h => h.Trim()).ToArray();
        
        if (!expectedHeaders.SequenceEqual(actualHeaders))
            return new Err<string[], ProcessingError>(
                new ProcessingError($"Invalid headers. Expected: {string.Join(", ", expectedHeaders)}"));
                
        return new Ok<string[], ProcessingError>(lines[1..]);
    }
    
    private static Result<Customer[], ProcessingError> ParseCustomers(string[] dataLines)
    {
        var customers = new List<Customer>();
        var errors = new List<string>();
        
        for (int i = 0; i < dataLines.Length; i++)
        {
            Result<Customer, string> customerResult = ParseCustomerLine(dataLines[i], i + 2);
            
            customerResult.Match(
                onOk: customer => customers.Add(customer),
                onErr: error => errors.Add(error)
            );
        }
        
        return errors.Count == 0
            ? new Ok<Customer[], ProcessingError>(customers.ToArray())
            : new Err<Customer[], ProcessingError>(
                new ProcessingError($"Parsing errors: {string.Join("; ", errors)}"));
    }
    
    private static Result<Customer, string> ParseCustomerLine(string line, int lineNumber)
    {
        string[] fields = line.Split(',').Select(f => f.Trim()).ToArray();
        
        if (fields.Length != 4)
            return new Err<Customer, string>(
                $"Line {lineNumber}: Expected 4 fields, got {fields.Length}");
                
        return ParseName(fields[0])
            .Bind(name => ParseEmail(fields[1]).Map(email => (name, email)))
            .Bind(data => ParseAge(fields[2]).Map(age => (data.name, data.email, age)))
            .Bind(data => ParseCity(fields[3]).Map(city => 
                new Customer(data.name, data.email, data.age, city)))
            .MapErr(error => $"Line {lineNumber}: {error}");
    }
    
    private static Result<string, string> ParseName(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? new Err<string, string>("Name cannot be empty")
            : new Ok<string, string>(value);
    }
    
    private static Result<string, string> ParseEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new Err<string, string>("Email cannot be empty");
            
        if (!value.Contains('@'))
            return new Err<string, string>("Invalid email format");
            
        return new Ok<string, string>(value);
    }
    
    private static Result<int, string> ParseAge(string value)
    {
        if (!int.TryParse(value, out int age))
            return new Err<int, string>("Age must be a number");
            
        if (age < 0 || age > 150)
            return new Err<int, string>("Age must be between 0 and 150");
            
        return new Ok<int, string>(age);
    }
    
    private static Result<string, string> ParseCity(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? new Err<string, string>("City cannot be empty")
            : new Ok<string, string>(value);
    }
}

// Usage
string csvData = File.ReadAllText("customers.csv");
Result<List<Customer>, ProcessingError> result = CsvProcessor.ProcessCustomerCsv(csvData);

result.Match(
    onOk: customers => 
    {
        Console.WriteLine($"Successfully processed {customers.Count} customers");
        customers.ForEach(c => Console.WriteLine($"- {c.Name} ({c.Email})"));
    },
    onErr: error => 
    {
        Console.WriteLine($"Processing failed: {error.Message}");
    }
);
```

## 6. Caching with Fallback

### Multi-Level Cache Strategy

```csharp
public class ProductService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly IProductRepository _repository;
    
    public ProductService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IProductRepository repository)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _repository = repository;
    }
    
    public async Task<Result<Product, string>> GetProductAsync(int productId)
    {
        return await GetFromMemoryCache(productId)
            .OrElseAsync(() => GetFromDistributedCache(productId))
            .OrElseAsync(() => GetFromDatabase(productId));
    }
    
    private async Task<Result<Product, string>> GetFromMemoryCache(int productId)
    {
        string cacheKey = $"product_{productId}";
        
        if (_memoryCache.TryGetValue(cacheKey, out Product? cachedProduct) && 
            cachedProduct is not null)
        {
            return new Ok<Product, string>(cachedProduct);
        }
        
        return new Err<Product, string>("Not found in memory cache");
    }
    
    private async Task<Result<Product, string>> GetFromDistributedCache(int productId)
    {
        try
        {
            string cacheKey = $"product_{productId}";
            string? cachedJson = await _distributedCache.GetStringAsync(cacheKey);
            
            if (string.IsNullOrEmpty(cachedJson))
                return new Err<Product, string>("Not found in distributed cache");
                
            Product? product = JsonSerializer.Deserialize<Product>(cachedJson);
            
            if (product is null)
                return new Err<Product, string>("Invalid data in distributed cache");
                
            // Cache in memory for faster subsequent access
            _memoryCache.Set($"product_{productId}", product, TimeSpan.FromMinutes(5));
            
            return new Ok<Product, string>(product);
        }
        catch (Exception ex)
        {
            return new Err<Product, string>($"Distributed cache error: {ex.Message}");
        }
    }
    
    private async Task<Result<Product, string>> GetFromDatabase(int productId)
    {
        try
        {
            Product? product = await _repository.GetByIdAsync(productId);
            
            if (product is null)
                return new Err<Product, string>($"Product {productId} not found");
                
            // Cache at both levels
            await CacheProduct(product);
            
            return new Ok<Product, string>(product);
        }
        catch (Exception ex)
        {
            return new Err<Product, string>($"Database error: {ex.Message}");
        }
    }
    
    private async Task CacheProduct(Product product)
    {
        // Memory cache (5 minutes)
        _memoryCache.Set($"product_{product.Id}", product, TimeSpan.FromMinutes(5));
        
        // Distributed cache (1 hour)
        try
        {
            string json = JsonSerializer.Serialize(product);
            await _distributedCache.SetStringAsync(
                $"product_{product.Id}", 
                json, 
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });
        }
        catch (Exception ex)
        {
            // Log but don't fail the operation
            Console.WriteLine($"Failed to cache product in distributed cache: {ex.Message}");
        }
    }
}

// Usage
var productService = new ProductService(memoryCache, distributedCache, repository);

Result<Product, string> productResult = await productService.GetProductAsync(123);

await productResult.MatchAsync(
    onOk: async product =>
    {
        Console.WriteLine($"Product: {product.Name} - ${product.Price:F2}");
        await DisplayProductDetailsAsync(product);
    },
    onErr: async error =>
    {
        Console.WriteLine($"Failed to get product: {error}");
        await ShowProductNotFoundMessageAsync();
    }
);
```

## 7. Event Processing Pipeline

### Message Queue Processing

```csharp
public class OrderEventProcessor
{
    private readonly IEventStore _eventStore;
    private readonly INotificationService _notificationService;
    private readonly IInventoryService _inventoryService;
    
    public async Task<Result<ProcessedEvent, EventError>> ProcessOrderEventAsync(OrderEvent orderEvent)
    {
        return await ValidateEvent(orderEvent)
            .BindAsync(async validEvent => await StoreEvent(validEvent))
            .BindAsync(async storedEvent => await ProcessBusinessLogic(storedEvent))
            .BindAsync(async processedEvent => await SendNotifications(processedEvent))
            .MapAsync(async finalEvent => await EnrichWithMetadata(finalEvent));
    }
    
    private static Result<OrderEvent, EventError> ValidateEvent(OrderEvent orderEvent)
    {
        if (orderEvent.OrderId <= 0)
            return new Err<OrderEvent, EventError>(
                new EventError("Invalid order ID", "INVALID_ORDER_ID"));
                
        if (string.IsNullOrEmpty(orderEvent.EventType))
            return new Err<OrderEvent, EventError>(
                new EventError("Event type is required", "MISSING_EVENT_TYPE"));
                
        if (orderEvent.Timestamp == default)
            return new Err<OrderEvent, EventError>(
                new EventError("Event timestamp is required", "MISSING_TIMESTAMP"));
                
        return new Ok<OrderEvent, EventError>(orderEvent);
    }
    
    private async Task<Result<StoredEvent, EventError>> StoreEvent(OrderEvent orderEvent)
    {
        try
        {
            var storedEvent = await _eventStore.StoreAsync(orderEvent);
            return new Ok<StoredEvent, EventError>(storedEvent);
        }
        catch (Exception ex)
        {
            return new Err<StoredEvent, EventError>(
                new EventError($"Failed to store event: {ex.Message}", "STORE_FAILED"));
        }
    }
    
    private async Task<Result<ProcessedEvent, EventError>> ProcessBusinessLogic(StoredEvent storedEvent)
    {
        return storedEvent.EventType switch
        {
            "OrderCreated" => await ProcessOrderCreated(storedEvent),
            "OrderShipped" => await ProcessOrderShipped(storedEvent),
            "OrderCancelled" => await ProcessOrderCancelled(storedEvent),
            _ => new Err<ProcessedEvent, EventError>(
                new EventError($"Unknown event type: {storedEvent.EventType}", "UNKNOWN_EVENT_TYPE"))
        };
    }
    
    private async Task<Result<ProcessedEvent, EventError>> ProcessOrderCreated(StoredEvent storedEvent)
    {
        return await UpdateInventory(storedEvent.OrderId, InventoryAction.Reserve)
            .BindAsync(async _ => await CreateShippingLabel(storedEvent.OrderId))
            .MapAsync(async _ => new ProcessedEvent(storedEvent, "OrderCreated processing completed"));
    }
    
    private async Task<Result<ProcessedEvent, EventError>> ProcessOrderShipped(StoredEvent storedEvent)
    {
        return await UpdateInventory(storedEvent.OrderId, InventoryAction.Commit)
            .BindAsync(async _ => await UpdateTrackingInfo(storedEvent.OrderId))
            .MapAsync(async _ => new ProcessedEvent(storedEvent, "OrderShipped processing completed"));
    }
    
    private async Task<Result<ProcessedEvent, EventError>> ProcessOrderCancelled(StoredEvent storedEvent)
    {
        return await UpdateInventory(storedEvent.OrderId, InventoryAction.Release)
            .BindAsync(async _ => await ProcessRefund(storedEvent.OrderId))
            .MapAsync(async _ => new ProcessedEvent(storedEvent, "OrderCancelled processing completed"));
    }
    
    private async Task<Result<Unit, EventError>> UpdateInventory(int orderId, InventoryAction action)
    {
        try
        {
            await _inventoryService.UpdateInventoryAsync(orderId, action);
            return new Ok<Unit, EventError>(Unit.Default);
        }
        catch (Exception ex)
        {
            return new Err<Unit, EventError>(
                new EventError($"Inventory update failed: {ex.Message}", "INVENTORY_UPDATE_FAILED"));
        }
    }
    
    private async Task<Result<ProcessedEvent, EventError>> SendNotifications(ProcessedEvent processedEvent)
    {
        try
        {
            await _notificationService.SendEventNotificationAsync(processedEvent);
            return new Ok<ProcessedEvent, EventError>(processedEvent);
        }
        catch (Exception ex)
        {
            // Notification failures shouldn't fail the entire pipeline
            Console.WriteLine($"Notification failed: {ex.Message}");
            return new Ok<ProcessedEvent, EventError>(processedEvent);
        }
    }
}

// Usage
var processor = new OrderEventProcessor(eventStore, notificationService, inventoryService);

var orderEvent = new OrderEvent(
    orderId: 12345,
    eventType: "OrderCreated",
    customerId: 789,
    timestamp: DateTime.UtcNow
);

Result<ProcessedEvent, EventError> result = await processor.ProcessOrderEventAsync(orderEvent);

string outcome = result.Match(
    onOk: processedEvent => $"Event processed successfully: {processedEvent.Message}",
    onErr: error => $"Event processing failed: {error.Message} (Code: {error.ErrorCode})"
);

Console.WriteLine(outcome);
```

## 8. Authentication and Authorization

### JWT Token Validation Pipeline

```csharp
public class AuthenticationService
{
    private readonly ITokenValidator _tokenValidator;
    private readonly IUserRepository _userRepository;
    private readonly IPermissionService _permissionService;
    
    public async Task<Result<AuthenticatedUser, AuthError>> AuthenticateAsync(
        string token, 
        string requiredPermission)
    {
        return await ValidateTokenFormat(token)
            .BindAsync(async validToken => await ValidateTokenSignature(validToken))
            .BindAsync(async claims => await ExtractUserInfo(claims))
            .BindAsync(async userInfo => await LoadUserDetails(userInfo))
            .BindAsync(async user => await ValidatePermissions(user, requiredPermission))
            .MapAsync(async user => await EnrichWithSessionInfo(user));
    }
    
    private static Result<string, AuthError> ValidateTokenFormat(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return new Err<string, AuthError>(
                new AuthError("Token is required", AuthErrorCode.MissingToken));
                
        if (!token.StartsWith("Bearer "))
            return new Err<string, AuthError>(
                new AuthError("Invalid token format", AuthErrorCode.InvalidTokenFormat));
                
        string actualToken = token[7..]; // Remove "Bearer "
        
        if (actualToken.Split('.').Length != 3)
            return new Err<string, AuthError>(
                new AuthError("Invalid JWT format", AuthErrorCode.InvalidTokenFormat));
                
        return new Ok<string, AuthError>(actualToken);
    }
    
    private async Task<Result<TokenClaims, AuthError>> ValidateTokenSignature(string token)
    {
        try
        {
            TokenClaims claims = await _tokenValidator.ValidateAsync(token);
            
            if (claims.ExpiryTime < DateTime.UtcNow)
                return new Err<TokenClaims, AuthError>(
                    new AuthError("Token has expired", AuthErrorCode.TokenExpired));
                    
            return new Ok<TokenClaims, AuthError>(claims);
        }
        catch (SecurityTokenException ex)
        {
            return new Err<TokenClaims, AuthError>(
                new AuthError($"Token validation failed: {ex.Message}", AuthErrorCode.InvalidToken));
        }
        catch (Exception ex)
        {
            return new Err<TokenClaims, AuthError>(
                new AuthError($"Authentication error: {ex.Message}", AuthErrorCode.AuthenticationFailed));
        }
    }
    
    private static Result<UserInfo, AuthError> ExtractUserInfo(TokenClaims claims)
    {
        if (string.IsNullOrEmpty(claims.Subject))
            return new Err<UserInfo, AuthError>(
                new AuthError("Invalid user ID in token", AuthErrorCode.InvalidToken));
                
        if (!int.TryParse(claims.Subject, out int userId))
            return new Err<UserInfo, AuthError>(
                new AuthError("Invalid user ID format", AuthErrorCode.InvalidToken));
                
        return new Ok<UserInfo, AuthError>(new UserInfo(userId, claims.Email, claims.Roles));
    }
    
    private async Task<Result<User, AuthError>> LoadUserDetails(UserInfo userInfo)
    {
        try
        {
            User? user = await _userRepository.GetByIdAsync(userInfo.UserId);
            
            if (user is null)
                return new Err<User, AuthError>(
                    new AuthError("User not found", AuthErrorCode.UserNotFound));
                    
            if (!user.IsActive)
                return new Err<User, AuthError>(
                    new AuthError("User account is disabled", AuthErrorCode.AccountDisabled));
                    
            return new Ok<User, AuthError>(user);
        }
        catch (Exception ex)
        {
            return new Err<User, AuthError>(
                new AuthError($"Failed to load user: {ex.Message}", AuthErrorCode.DatabaseError));
        }
    }
    
    private async Task<Result<User, AuthError>> ValidatePermissions(User user, string requiredPermission)
    {
        try
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(user.Id, requiredPermission);
            
            return hasPermission
                ? new Ok<User, AuthError>(user)
                : new Err<User, AuthError>(
                    new AuthError($"Insufficient permissions: {requiredPermission}", 
                                AuthErrorCode.InsufficientPermissions));
        }
        catch (Exception ex)
        {
            return new Err<User, AuthError>(
                new AuthError($"Permission check failed: {ex.Message}", AuthErrorCode.AuthorizationFailed));
        }
    }
}

// Usage in a web API controller
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(int id, [FromHeader] string authorization)
{
    var authService = new AuthenticationService(tokenValidator, userRepository, permissionService);
    
    Result<AuthenticatedUser, AuthError> authResult = 
        await authService.AuthenticateAsync(authorization, "users:read");
    
    return authResult.Match<IActionResult>(
        onOk: authenticatedUser =>
        {
            // Proceed with the actual operation
            var user = GetUserById(id);
            return Ok(user);
        },
        onErr: authError => authError.ErrorCode switch
        {
            AuthErrorCode.MissingToken => Unauthorized("Authentication required"),
            AuthErrorCode.TokenExpired => Unauthorized("Token has expired"),
            AuthErrorCode.InsufficientPermissions => Forbid("Insufficient permissions"),
            AuthErrorCode.UserNotFound => Unauthorized("Invalid user"),
            _ => StatusCode(500, "Authentication error")
        }
    );
}
```

## Key Patterns Demonstrated

### 1. **Chaining Operations**

All examples show how to chain multiple operations using `Bind`, `Map`, and other extensions, allowing complex workflows to be expressed clearly.

### 2. **Error Propagation**

Errors are automatically propagated through the chain, eliminating the need for manual error checking at each step.

### 3. **Fallback Strategies**

Using `OrElse` to implement retry logic, cache hierarchies, and alternative data sources.

### 4. **Pattern Matching**

Using `Match` to handle both success and error cases in a functional style.

### 5. **Async Composition**

Demonstrating how async operations compose naturally with the async extension methods.

### 6. **Domain-Specific Errors**

Creating custom error types that provide meaningful context for different domains.

### 7. **Validation Pipelines**

Building complex validation logic by composing simple validation functions.

### 8. **Resource Management**

Safe resource handling with transactions and proper cleanup in error scenarios.

These patterns can be adapted and combined to handle a wide variety of real-world scenarios while maintaining clean, readable, and maintainable code.
