# Code Style Guide

This document defines the coding standards, formatting rules, and best practices for the Monads library. Consistent code style improves readability, maintainability, and collaboration across the project.

## General Principles

### Core Values

1. **Readability**: Code should be self-documenting and easy to understand
2. **Consistency**: Uniform style across the entire codebase
3. **Maintainability**: Code should be easy to modify and extend
4. **Type Safety**: Leverage C#'s type system for compile-time guarantees
5. **Performance**: Consider performance implications of style choices

### Style Philosophy

- **Explicit over Implicit**: Make intentions clear through explicit declarations
- **Functional Patterns**: Embrace functional programming concepts where appropriate
- **Immutability**: Prefer immutable data structures and readonly fields
- **Composition**: Favor composition over inheritance
- **Small Functions**: Keep methods focused and concise

## Formatting Rules

### Indentation and Spacing

```csharp
// Use 4 spaces for indentation (no tabs)
public class ExampleClass
{
    private readonly IValidator _validator;
    
    public Result<T, TError> Process<T, TError>(T value)
    {
        if (value is null)
        {
            return new Err<T, TError>(CreateError("Value cannot be null"));
        }
        
        return new Ok<T, TError>(value);
    }
}
```

### Line Length

- **Maximum line length**: 120 characters
- **Preferred line length**: 100 characters
- **Break long lines** at logical points (parameters, operators, etc.)

```csharp
// Good: Broken at logical points
public static Result<TResult, TError> Bind<T, TResult, TError>(
    this Result<T, TError> result,
    Func<T, Result<TResult, TError>> bindFunc)

// Good: Method chaining on new lines
return await GetUserAsync(id)
    .BindAsync(user => ValidateUserAsync(user))
    .MapAsync(user => EnrichUserDataAsync(user))
    .MatchAsync(
        onOk: user => ProcessUserAsync(user),
        onErr: error => HandleErrorAsync(error));
```

### Braces and Brackets

```csharp
// Always use braces, even for single statements
if (condition)
{
    DoSomething();
}

// Opening brace on new line for types and members
public class ResultExtensions
{
    public static Result<T, TError> Method<T, TError>()
    {
        // Implementation
    }
}

// Collection initializers
var items = new List<string>
{
    "item1",
    "item2",
    "item3"
};

// Object initializers
var config = new Configuration
{
    Timeout = TimeSpan.FromSeconds(30),
    RetryCount = 3,
    EnableLogging = true
};
```

### Blank Lines

```csharp
public class Example
{
    // One blank line between different member types
    private readonly IService _service;
    
    public string Property { get; set; }
    
    public Example(IService service)
    {
        _service = service;
    }
    
    public void Method()
    {
        // Blank line before return statement in complex methods
        var result = ProcessData();
        
        return result;
    }
}
```

## Naming Conventions

### Types

```csharp
// Classes, interfaces, structs, enums - PascalCase
public class ResultFactory
public interface IResultValidator  
public struct ValidationResult
public enum ErrorType

// Generic type parameters - Single uppercase letter or PascalCase with T prefix
public class Result<T, TError>
public interface IProcessor<TInput, TOutput>
public class Cache<TKey, TValue>
```

### Members

```csharp
public class Example
{
    // Constants - PascalCase
    public const string DefaultErrorMessage = "Operation failed";
    private const int MaxRetryCount = 3;
    
    // Fields - camelCase with underscore prefix for private
    private readonly IValidator _validator;
    public readonly string Value;
    
    // Properties - PascalCase
    public bool IsValid { get; }
    public string ErrorMessage { get; private set; }
    
    // Methods - PascalCase
    public Result<T, TError> ProcessItem<T, TError>(T item)
    {
        return ValidateAndProcess(item);
    }
    
    // Parameters and local variables - camelCase
    public void ProcessUser(User userData, ValidationSettings validationConfig)
    {
        string processedName = userData.Name?.Trim();
        bool isValidUser = _validator.Validate(userData);
    }
}
```

### Namespaces and Files

```csharp
// Namespaces follow directory structure
namespace Monads.Extensions.Results.Sync;
namespace Tests.Monads.Extensions.Results.Sync;

// File names match primary type (PascalCase)
Result.cs                    // Contains Result<T, TError>
BindExtension.cs            // Contains BindExtension static class
BindExtensionTests.cs       // Contains BindExtensionTests class
```

### Special Naming Rules

```csharp
// Extension classes end with "Extension"
public static class MapExtension
public static class BindExtension

// Test classes end with "Tests"  
public class MapExtensionTests
public class ResultTests

// Async methods end with "Async"
public async Task<Result<T, E>> ProcessAsync<T, E>()
public async ValueTask<Result<T, E>> GetDataAsync<T, E>()

// Boolean properties start with "Is", "Has", "Can", "Should"
public bool IsOk { get; }
public bool HasValue { get; }
public bool CanProcess { get; }
public bool ShouldRetry { get; }
```

## Type Declarations

### Class Structure Order

```csharp
public class ExampleClass
{
    // 1. Constants
    private const string DefaultMessage = "Default";
    
    // 2. Static fields
    private static readonly ILogger Logger = LogManager.GetLogger();
    
    // 3. Instance fields
    private readonly IService _service;
    private string _cachedValue;
    
    // 4. Constructors
    public ExampleClass(IService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
    
    // 5. Properties
    public string Value { get; private set; }
    
    // 6. Static methods
    public static ExampleClass Create(IService service)
    {
        return new ExampleClass(service);
    }
    
    // 7. Instance methods
    public Result<string, Error> Process()
    {
        // Implementation
    }
    
    // 8. Private methods
    private bool ValidateInput(string input)
    {
        return !string.IsNullOrEmpty(input);
    }
}
```

### Interface Design

```csharp
// Interfaces start with 'I' and use PascalCase
public interface IResultProcessor<T, TError>
{
    // Properties first
    bool CanProcess { get; }
    
    // Methods second, grouped by functionality
    Result<T, TError> Process(T input);
    Task<Result<T, TError>> ProcessAsync(T input);
    
    // Events last (if any)
    event EventHandler<ProcessingEventArgs> Processing;
}
```

### Generic Constraints

```csharp
// Constraints on separate lines for readability
public static Result<T, TError> Process<T, TError>(T value)
    where T : class, IComparable<T>
    where TError : Exception
{
    // Implementation
}

// Multiple constraints formatted consistently
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : struct, IEquatable<TKey>
{
    // Implementation
}
```

## Expression and Statement Style

### Variable Declarations

```csharp
// Use 'var' when type is obvious from right side
var result = new Ok<string, Error>("success");
var items = GetItems().ToList();
var config = CreateConfiguration();

// Use explicit types when clarity is important
Result<User, ValidationError> userResult = ValidateUser(input);
IEnumerable<string> filteredItems = items.Where(x => x.Length > 5);
string explicitString = GetStringFromComplexMethod();

// Prefer target-typed new expressions (C# 9+)
Result<string, Error> result = new("success");
List<User> users = new();
Dictionary<string, int> counts = new();
```

### Method Calls and Chaining

```csharp
// Method chaining - align dots
return await GetUserAsync(id)
    .BindAsync(user => ValidateUserAsync(user))
    .MapAsync(user => TransformUserAsync(user))
    .MatchAsync(
        onOk: user => ProcessSuccessAsync(user),
        onErr: error => ProcessErrorAsync(error));

// Long parameter lists - one per line
var result = ProcessComplexOperation(
    parameter1: "value1",
    parameter2: 42,
    parameter3: true,
    parameter4: TimeSpan.FromMinutes(5));

// Short method calls on single line
var result = GetUser(id).Map(u => u.Name);
```

### Conditional Expressions

```csharp
// Simple ternary expressions on single line
string message = result.IsOk ? "Success" : "Failed";

// Complex conditional expressions with line breaks
string status = result.IsOk 
    ? $"Operation completed successfully with result: {result.Value}"
    : $"Operation failed with error: {result.Error}";

// Pattern matching expressions
string description = result switch
{
    { IsOk: true } => "Operation succeeded",
    { IsErr: true } => "Operation failed",
    _ => "Unknown state"
};

// Guard clauses for early returns
public Result<User, Error> GetUser(int id)
{
    if (id <= 0)
        return Error.InvalidId();
        
    if (!_cache.ContainsKey(id))
        return Error.NotFound();
        
    return _cache[id];
}
```

### Exception Handling

```csharp
// Use ArgumentNullException.ThrowIfNull for parameter validation
public static Result<T, TError> Map<T, TError>(
    this Result<T, TError> result,
    Func<T, TError> mapFunc)
{
    ArgumentNullException.ThrowIfNull(mapFunc);
    // Implementation
}

// Specific exception handling
public Result<Data, Error> LoadData(string path)
{
    try
    {
        var data = File.ReadAllText(path);
        return ProcessData(data);
    }
    catch (FileNotFoundException)
    {
        return Error.FileNotFound(path);
    }
    catch (UnauthorizedAccessException)
    {
        return Error.AccessDenied(path);
    }
    catch (IOException ex)
    {
        return Error.IoError(ex.Message);
    }
}
```

## Documentation Style

### XML Documentation

```csharp
/// <summary>
/// Transforms the success value of a Result using the specified function.
/// If the result represents an error, the error is preserved unchanged.
/// </summary>
/// <typeparam name="T">The type of the success value in the input result.</typeparam>
/// <typeparam name="TResult">The type of the success value in the output result.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
/// <param name="result">The result to transform.</param>
/// <param name="mapFunc">The function to apply to the success value.</param>
/// <returns>
/// A new <see cref="Result{T, TError}"/> with the transformed success value if the input 
/// represents a success, or the original error if the input represents a failure.
/// </returns>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="mapFunc"/> is <see langword="null"/>.
/// </exception>
/// <example>
/// <para>Transform a successful result:</para>
/// <code>
/// Result&lt;int, string&gt; result = ResultFactory.Success&lt;string&gt;(42);
/// Result&lt;string, string&gt; mapped = result.Map(x => x.ToString());
/// // mapped.IsOk == true, mapped.Value == "42"
/// </code>
/// <para>Error results are preserved:</para>
/// <code>
/// Result&lt;int, string&gt; result = ResultFactory.Failure&lt;int&gt;("error");
/// Result&lt;string, string&gt; mapped = result.Map(x => x.ToString());
/// // mapped.IsErr == true, mapped.Error == "error"
/// </code>
/// </example>
public static Result<TResult, TError> Map<T, TResult, TError>(
    this Result<T, TError> result,
    Func<T, TResult> mapFunc)
```

### Code Comments

```csharp
public class ResultProcessor
{
    // Use comments for complex business logic
    public Result<ProcessedData, Error> Process(RawData input)
    {
        // Validate input according to business rules specified in REQ-001
        if (!IsValidBusinessData(input))
            return Error.InvalidBusinessData();
            
        // Apply transformation pipeline based on data type
        // See: https://internal-wiki/data-processing-pipeline
        var transformed = input.Type switch
        {
            DataType.Customer => ProcessCustomerData(input),
            DataType.Product => ProcessProductData(input),
            _ => Error.UnsupportedDataType(input.Type)
        };
        
        return transformed;
    }
    
    // Avoid obvious comments
    // Bad: 
    // var count = 0;  // Initialize count to zero
    
    // Good: Explain why, not what
    // Use exponential backoff to avoid overwhelming the external service
    var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100);
}
```

## Testing Style

### Test Method Naming

```csharp
[TestFixture]
public class BindExtensionTests
{
    // Pattern: MethodName_Scenario_ExpectedBehavior
    [Test]
    public void Bind_WithOkResult_AppliesFunctionAndReturnsNewResult()
    {
        // Arrange
        var result = new Ok<int, string>(42);
        
        // Act  
        var bound = result.Bind(x => new Ok<string, string>(x.ToString()));
        
        // Assert
        Assert.That(bound.IsOk, Is.True);
        bound.Match(
            onOk: value => Assert.That(value, Is.EqualTo("42")),
            onErr: error => Assert.Fail($"Expected success but got: {error}")
        );
    }
    
    [Test]
    public void Bind_WithErrResult_ReturnsOriginalErrorWithoutCallingFunction()
    {
        // Test implementation
    }
    
    [Test]
    public void Bind_WithNullFunction_ThrowsArgumentNullException()
    {
        // Test implementation
    }
}
```

### Test Structure

```csharp
public class ExampleTests
{
    // Test setup and teardown
    [SetUp]
    public void SetUp()
    {
        // Common setup for each test
    }
    
    [TearDown] 
    public void TearDown()
    {
        // Cleanup after each test
    }
    
    // Group related tests with nested classes
    [TestFixture]
    public class WhenResultIsOk
    {
        [Test]
        public void Map_TransformsValue() { }
        
        [Test]
        public void Bind_CallsFunction() { }
    }
    
    [TestFixture]
    public class WhenResultIsErr
    {
        [Test]
        public void Map_PreservesError() { }
        
        [Test]
        public void Bind_DoesNotCallFunction() { }
    }
}
```

## Performance Considerations

### Hot Path Optimizations

```csharp
// Prefer early returns to reduce nesting
public Result<T, TError> Process<T, TError>(T value)
{
    if (value is null)
        return Error.NullValue();
        
    if (!IsValid(value))
        return Error.InvalidValue();
        
    return ProcessValidValue(value);
}

// Use readonly structs for performance-critical value types
public readonly struct ProcessingOptions
{
    public readonly bool EnableCaching;
    public readonly TimeSpan Timeout;
    
    public ProcessingOptions(bool enableCaching, TimeSpan timeout)
    {
        EnableCaching = enableCaching;
        Timeout = timeout;
    }
}

// Prefer ValueTask for frequently synchronous operations
public async ValueTask<Result<T, TError>> GetCachedDataAsync<T, TError>(string key)
{
    if (_cache.TryGetValue(key, out T? value))
        return new Ok<T, TError>(value); // No async state machine
        
    return await LoadFromDatabaseAsync<T, TError>(key); // Actual async call
}
```

### Memory Efficiency

```csharp
// Use object pooling for frequently created objects (if applicable)
private static readonly ObjectPool<StringBuilder> StringBuilderPool = 
    new DefaultObjectPool<StringBuilder>();

// Prefer spans for string manipulation
public bool IsValidFormat(ReadOnlySpan<char> input)
{
    return input.Length > 0 && char.IsLetter(input[0]);
}

// Use string interpolation efficiently
public string CreateMessage(string operation, TimeSpan duration)
{
    // Good: Simple interpolation
    return $"Operation '{operation}' completed in {duration.TotalMilliseconds}ms";
    
    // Better for complex formatting: Use StringBuilder or string.Create
}
```

## Code Analysis and Tools

### EditorConfig Settings

```ini
# .editorconfig
root = true

[*]
charset = utf-8
end_of_line = lf
insert_final_newline = true
trim_trailing_whitespace = true

[*.cs]
indent_style = space
indent_size = 4
max_line_length = 120

# C# formatting rules
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
csharp_indent_case_contents = true
csharp_indent_switch_labels = true
```

### Recommended Analyzers

```xml
<!-- In Directory.Build.props -->
<PropertyGroup>
  <WarningsAsErrors />
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <WarningsNotAsErrors>CS1591</WarningsNotAsErrors>
  <CodeAnalysisRuleSet>$(MSBuildThisFileDirectory)analyzers.ruleset</CodeAnalysisRuleSet>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
  <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="7.0.1" PrivateAssets="all" />
  <PackageReference Include="SonarAnalyzer.CSharp" Version="8.56.0.67649" PrivateAssets="all" />
</ItemGroup>
```

### Code Formatting Commands

```bash
# Format all files in the solution
dotnet format

# Format specific files
dotnet format --include src/Monads/Extensions/

# Check formatting without making changes
dotnet format --verify-no-changes
```

## Best Practices Summary

### Do's

- ✅ Use consistent naming conventions throughout the codebase
- ✅ Write comprehensive XML documentation for all public APIs
- ✅ Prefer explicit type declarations when clarity is important
- ✅ Use early returns to reduce nesting and complexity
- ✅ Group related functionality in namespaces and classes
- ✅ Follow the established project structure and organization
- ✅ Use appropriate access modifiers (prefer most restrictive)
- ✅ Validate parameters using ArgumentNullException.ThrowIfNull
- ✅ Use readonly fields and properties where possible
- ✅ Prefer composition over inheritance

### Don'ts

- ❌ Don't use single-letter variable names (except for loop counters)
- ❌ Don't write methods longer than 30-40 lines
- ❌ Don't use magic numbers or strings (use named constants)
- ❌ Don't ignore compiler warnings
- ❌ Don't use exceptions for control flow
- ❌ Don't create public mutable state
- ❌ Don't use nested using statements (prefer using declarations)
- ❌ Don't mix async and sync code unnecessarily
- ❌ Don't create overly complex inheritance hierarchies
- ❌ Don't write code without corresponding tests

Following these style guidelines ensures that the Monads library maintains high code quality, readability, and consistency across all contributors.
