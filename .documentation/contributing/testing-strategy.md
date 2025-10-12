# Testing Strategy Guide

This document outlines the comprehensive testing approach, coverage requirements, test organization, and best practices for the Monads library. A robust testing strategy ensures reliability, maintainability, and confidence in the codebase.

## Testing Philosophy

### Core Principles

1. **Test-Driven Development**: Write tests before or alongside implementation
2. **Comprehensive Coverage**: Aim for high test coverage across all scenarios
3. **Fast Feedback**: Tests should run quickly to enable rapid development
4. **Reliable Tests**: Tests should be deterministic and not flaky
5. **Maintainable Tests**: Tests should be easy to read, modify, and debug
6. **Documentation**: Tests serve as executable specifications

### Testing Pyramid

```Bash
    /\
   /UI\          <- Few, expensive, slow
  /____\
 /Integration\   <- Some, moderate cost
/__________\
/  Unit Tests  \  <- Many, cheap, fast
```

The Monads library focuses primarily on unit tests with targeted integration tests for complex scenarios.

## Test Categories

### 1. Unit Tests

**Purpose**: Test individual methods and components in isolation

**Characteristics**:

- Fast execution (< 1ms per test)
- No external dependencies
- Deterministic results
- High coverage of edge cases

**Example**:

```csharp
[Test]
public void Map_WithOkResult_TransformsValueCorrectly()
{
    // Arrange
    var result = new Ok<int, string>(42);
    
    // Act
    Result<string, string> mapped = result.Map(x => x.ToString());
    
    // Assert
    Assert.That(mapped.IsOk, Is.True);
    mapped.Match(
        onOk: value => Assert.That(value, Is.EqualTo("42")),
        onErr: error => Assert.Fail($"Expected success but got error: {error}")
    );
}
```

### 2. Property-Based Tests

**Purpose**: Test mathematical properties and invariants

**Characteristics**:

- Tests properties that should hold for all inputs
- Discovers edge cases automatically
- Validates mathematical laws (functor, monad laws)

**Example**:

```csharp
[Test]
public void Map_FunctorIdentityLaw_MapWithIdentityReturnsOriginal()
{
    // Property: result.Map(x => x) should equal result
    var result = new Ok<int, string>(42);
    
    Result<int, string> mapped = result.Map(x => x);
    
    // Both results should be equivalent
    Assert.That(
        mapped.Match(x => x, e => -999), 
        Is.EqualTo(result.Match(x => x, e => -999))
    );
}

[Test]
public void Map_FunctorCompositionLaw_MapOfCompositionEqualsCompositionOfMaps()
{
    // Property: result.Map(f).Map(g) == result.Map(x => g(f(x)))
    var result = new Ok<int, string>(5);
    Func<int, string> f = x => x.ToString();
    Func<string, int> g = x => x.Length;
    
    Result<int, string> left = result.Map(f).Map(g);
    Result<int, string> right = result.Map(x => g(f(x)));
    
    Assert.That(
        left.Match(x => x, e => -1),
        Is.EqualTo(right.Match(x => x, e => -1))
    );
}
```

### 3. Integration Tests

**Purpose**: Test interactions between components

**Characteristics**:

- Test realistic scenarios
- May involve multiple classes
- Test async composition chains
- Validate error propagation

**Example**:

```csharp
[Test]
public async Task ComplexAsyncChain_WithMixedResults_HandlesErrorsProperly()
{
    // Arrange
    var processor = new DataProcessor();
    var invalidInput = new InputData { Id = -1, Value = null };
    
    // Act
    Result<ProcessedOutput, ProcessingError> result = await processor
        .ValidateAsync(invalidInput)
        .BindAsync(async data => await processor.TransformAsync(data))
        .BindAsync(async data => await processor.SaveAsync(data));
    
    // Assert
    Assert.That(result.IsErr, Is.True);
    result.Match(
        onOk: output => Assert.Fail("Expected error but got success"),
        onErr: error => Assert.That(error.Type, Is.EqualTo(ErrorType.Validation))
    );
}
```

### 4. Performance Tests

**Purpose**: Validate performance characteristics

**Characteristics**:

- Benchmark critical paths
- Ensure no performance regressions
- Test memory allocation patterns
- Validate async performance

**Example**:

```csharp
[Test]
public void Map_Performance_HandlesManyOperationsEfficiently()
{
    // Arrange
    const int iterations = 100_000;
    var result = new Ok<int, string>(42);
    
    // Act
    var stopwatch = Stopwatch.StartNew();
    
    for (int i = 0; i < iterations; i++)
    {
        result.Map(x => x + 1);
    }
    
    stopwatch.Stop();
    
    // Assert
    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(50), 
        "Map operations should be very fast");
}

[Test]
public async Task AsyncChain_Performance_CompletesWithinReasonableTime()
{
    // Arrange
    var tasks = Enumerable.Range(0, 1000)
        .Select(i => ProcessDataAsync(i))
        .ToArray();
    
    // Act
    var stopwatch = Stopwatch.StartNew();
    Result<int, string>[] results = await Task.WhenAll(tasks);
    stopwatch.Stop();
    
    // Assert
    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000),
        "Async operations should complete efficiently");
        
    Assert.That(results.Count(r => r.IsOk), Is.EqualTo(1000),
        "All operations should succeed");
}
```

## Test Organization

### Project Structure

```bash
tests/
└── Tests.Monads.Result/
    ├── Tests.Monads.Result.csproj
    ├── xunit.runner.json
    ├── Extensions/
    │   ├── Async/
    │   │   ├── BindTaskExtensionTests.cs
    │   │   ├── BindValueTaskExtensionTests.cs
    │   │   ├── MapTaskExtensionTests.cs
    │   │   └── ...
    │   └── Sync/
    │       ├── BindTests.cs
    │       ├── MapTests.cs
    │       ├── MapErrTests.cs
    │       └── ...
    ├── Models/
    │   ├── ErrTests.cs
    │   ├── OkTests.cs
    │   ├── ResultPredicateTests.cs
    │   └── UnitTests.cs
    ├── Integration/
    │   ├── AsyncWorkflowTests.cs
    │   └── ErrorPropagationTests.cs
    └── Performance/
        ├── ExtensionBenchmarks.cs
        └── AsyncBenchmarks.cs
```

### Naming Conventions

#### Test Classes

```csharp
// Source: BindExtension.cs -> Test: BindTests.cs or BindExtensionTests.cs
public class BindTests
public class BindExtensionTests

// Source: Result.cs -> Test: ResultTests.cs  
public class ResultTests

// Integration tests
public class AsyncWorkflowTests
public class ErrorPropagationTests

// Performance tests
public class ExtensionBenchmarks
```

#### Test Methods

```csharp
// Pattern: MethodName_Scenario_ExpectedResult
[Test]
public void Map_WithOkResult_TransformsValue() { }

[Test] 
public void Map_WithErrResult_PreservesError() { }

[Test]
public void Map_WithNullFunction_ThrowsArgumentNullException() { }

// Async tests
[Test]
public async Task MapAsync_WithValidInput_CompletesSuccessfully() { }

// Property tests
[Test]
public void Map_FunctorLaws_SatisfyIdentityAndComposition() { }
```

### Test Categories and Tags

```csharp
// Use categories to organize test execution
[Test, Category("Unit")]
public void BasicFunctionality() { }

[Test, Category("Integration")]
public void ComplexScenario() { }

[Test, Category("Performance")]
public void BenchmarkOperation() { }

// Use explicit markers for long-running tests
[Test, Category("Slow")]
public async Task LongRunningAsyncOperation() { }

// Mark tests that require specific conditions
[Test, Category("Flaky"), Ignore("Needs investigation")]
public void UnstableTest() { }
```

## Coverage Requirements

### Minimum Coverage Targets

- **Line Coverage**: 95% for all public APIs
- **Branch Coverage**: 90% for conditional logic
- **Method Coverage**: 100% for all public methods
- **Type Coverage**: 100% for all public types

### Coverage Exclusions

```csharp
// Exclude generated code
[ExcludeFromCodeCoverage]
public class GeneratedClass { }

// Exclude trivial properties (with justification)
[ExcludeFromCodeCoverage]
public string SimpleProperty { get; set; }

// Exclude defensive code paths
public void Method(string input)
{
    ArgumentNullException.ThrowIfNull(input); // May be excluded if trivial
    
    // Main logic must be covered
    ProcessInput(input);
}
```

### Measuring Coverage

```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage reports
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report

# CI/CD integration
dotnet test --collect:"XPlat Code Coverage" --results-directory:./coverage
```

## Test Implementation Patterns

### Arrange-Act-Assert Pattern

```csharp
[Test]
public void Bind_WithSuccessfulChaining_ReturnsTransformedResult()
{
    // Arrange - Set up test data and expectations
    var initialResult = new Ok<int, string>(5);
    Func<int, Result<string, string>> bindFunction = x => new Ok<string, string>(x.ToString());
    
    // Act - Execute the operation under test
    Result<string, string> result = initialResult.Bind(bindFunction);
    
    // Assert - Verify the outcome
    Assert.That(result.IsOk, Is.True);
    result.Match(
        onOk: value => Assert.That(value, Is.EqualTo("5")),
        onErr: error => Assert.Fail($"Expected success but got error: {error}")
    );
}
```

### Parameterized Tests

```csharp
[TestCaseSource(nameof(ValidInputCases))]
public void Map_WithVariousValidInputs_TransformsCorrectly(int input, string expected)
{
    // Arrange
    var result = new Ok<int, string>(input);
    
    // Act
    Result<string, string> mapped = result.Map(x => x.ToString());
    
    // Assert
    Assert.That(mapped.IsOk, Is.True);
    mapped.Match(
        onOk: value => Assert.That(value, Is.EqualTo(expected)),
        onErr: error => Assert.Fail($"Unexpected error: {error}")
    );
}

private static IEnumerable<TestCaseData> ValidInputCases()
{
    yield return new TestCaseData(0, "0");
    yield return new TestCaseData(42, "42");
    yield return new TestCaseData(-1, "-1");
    yield return new TestCaseData(int.MaxValue, int.MaxValue.ToString());
}
```

### Exception Testing

```csharp
[Test]
public void Map_WithNullFunction_ThrowsArgumentNullException()
{
    // Arrange
    var result = new Ok<int, string>(42);
    
    // Act & Assert
    var exception = Assert.Throws<ArgumentNullException>(() => 
        result.Map<string>(null!));
        
    Assert.That(exception.ParamName, Is.EqualTo("mapFunc"));
}

[Test]
public void Constructor_WithNullError_ThrowsArgumentNullException()
{
    // Arrange & Act & Assert
    Assert.Throws<ArgumentNullException>(() => 
        new Err<int, string>(null!));
}
```

### Async Testing Patterns

```csharp
[Test]
public async Task BindAsync_WithAsyncOperations_ChainsCorrectly()
{
    // Arrange
    var initialResult = Task.FromResult(new Ok<int, string>(5));
    
    // Act
    Result<string, string> result = await initialResult
        .BindAsync(async x => 
        {
            await Task.Delay(1); // Simulate async work
            return new Ok<string, string>(x.ToString());
        });
    
    // Assert
    Assert.That(result.IsOk, Is.True);
    result.Match(
        onOk: value => Assert.That(value, Is.EqualTo("5")),
        onErr: error => Assert.Fail($"Expected success: {error}")
    );
}

[Test]
public async Task AsyncChain_WithCancellation_PropagatesCancellation()
{
    // Arrange
    using var cts = new CancellationTokenSource();
    var initialResult = Task.FromResult(new Ok<int, string>(5));
    
    // Act
    cts.CancelAfter(10); // Cancel quickly
    
    // Assert
    await Assert.ThrowsAsync<OperationCanceledException>(async () =>
    {
        await initialResult.BindAsync(async x =>
        {
            await Task.Delay(1000, cts.Token); // Will be cancelled
            return new Ok<string, string>(x.ToString());
        });
    });
}
```

### Mock and Stub Usage

```csharp
public class ServiceTests
{
    [Test]
    public async Task ProcessData_WithFailingDependency_ReturnsError()
    {
        // Arrange
        var mockRepository = new Mock<IDataRepository>();
        mockRepository
            .Setup(x => x.GetDataAsync(It.IsAny<int>()))
            .ReturnsAsync(new Err<Data, RepositoryError>(new RepositoryError("Database unavailable")));
            
        var service = new DataService(mockRepository.Object);
        
        // Act
        Result<ProcessedData, ServiceError> result = await service.ProcessDataAsync(123);
        
        // Assert
        Assert.That(result.IsErr, Is.True);
        result.Match(
            onOk: data => Assert.Fail("Expected error but got success"),
            onErr: error => Assert.That(error.Message, Does.Contain("Database unavailable"))
        );
    }
}
```

## Test Utilities and Helpers

### Custom Assertions

```csharp
public static class ResultAssertions
{
    public static void ShouldBeOk<T, TError>(this Result<T, TError> result)
    {
        Assert.That(result.IsOk, Is.True, "Expected result to be Ok but was Err");
    }
    
    public static void ShouldBeErr<T, TError>(this Result<T, TError> result)
    {
        Assert.That(result.IsErr, Is.True, "Expected result to be Err but was Ok");
    }
    
    public static void ShouldHaveValue<T, TError>(this Result<T, TError> result, T expected)
    {
        result.ShouldBeOk();
        result.Match(
            onOk: value => Assert.That(value, Is.EqualTo(expected)),
            onErr: error => Assert.Fail($"Expected value {expected} but result was error: {error}")
        );
    }
    
    public static void ShouldHaveError<T, TError>(this Result<T, TError> result, TError expected)
    {
        result.ShouldBeErr();
        result.Match(
            onOk: value => Assert.Fail($"Expected error {expected} but result was success: {value}"),
            onErr: error => Assert.That(error, Is.EqualTo(expected))
        );
    }
}

// Usage
[Test]
public void ExampleTest()
{
    var result = new Ok<int, string>(42);
    
    result.ShouldBeOk();
    result.ShouldHaveValue(42);
}
```

### Test Data Builders

```csharp
public class ResultBuilder<T, TError>
{
    public static ResultBuilder<T, TError> Ok(T value) => new(new Ok<T, TError>(value));
    public static ResultBuilder<T, TError> Err(TError error) => new(new Err<T, TError>(error));
    
    private readonly Result<T, TError> _result;
    
    private ResultBuilder(Result<T, TError> result)
    {
        _result = result;
    }
    
    public Result<T, TError> Build() => _result;
    
    public static implicit operator Result<T, TError>(ResultBuilder<T, TError> builder) => builder.Build();
}

// Usage in tests
[Test]
public void TestWithBuilder()
{
    Result<int, string> result = ResultBuilder<int, string>.Ok(42);
    result.ShouldHaveValue(42);
}
```

### Fake Implementations

```csharp
public class FakeDataRepository : IDataRepository
{
    private readonly Dictionary<int, Data> _data = new();
    private bool _shouldFail;
    
    public FakeDataRepository SetupFailure() 
    { 
        _shouldFail = true; 
        return this; 
    }
    
    public FakeDataRepository WithData(int id, Data data) 
    { 
        _data[id] = data; 
        return this; 
    }
    
    public Task<Result<Data, RepositoryError>> GetDataAsync(int id)
    {
        if (_shouldFail)
            return Task.FromResult<Result<Data, RepositoryError>>(
                new Err<Data, RepositoryError>(new RepositoryError("Simulated failure")));
                
        return Task.FromResult(_data.TryGetValue(id, out var data)
            ? new Ok<Data, RepositoryError>(data)
            : new Err<Data, RepositoryError>(new RepositoryError("Not found")));
    }
}
```

## Continuous Integration

### Test Configuration

```xml
<!-- Tests.Monads.Result.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="NUnit" Version="3.14.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="Moq" Version="4.20.69" />
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
  </ItemGroup>
</Project>
```

### Test Runner Configuration

```json
// xunit.runner.json
{
  "methodDisplay": "method",
  "methodDisplayOptions": "all",
  "parallel": {
    "maxParallelThreads": -1,
    "parallelizeAssembly": true,
    "parallelizeTestCollections": true
  }
}
```

### CI Pipeline Integration

```yaml
# GitHub Actions example
name: Test

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      
    - name: Generate coverage report
      run: |
        dotnet tool install -g dotnet-reportgenerator-globaltool
        reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
        
    - name: Upload coverage reports
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage-report/cobertura.xml
```

## Quality Gates

### Definition of Done for Tests

A feature is considered "done" when:

- [ ] All new public APIs have unit tests
- [ ] All edge cases and error paths are tested
- [ ] Integration tests cover realistic scenarios
- [ ] Property tests validate mathematical laws (where applicable)
- [ ] Performance tests ensure no regressions
- [ ] All tests pass consistently
- [ ] Code coverage meets minimum thresholds
- [ ] Tests are well-documented and maintainable

### Test Review Checklist

When reviewing test code, ensure:

- [ ] Tests are focused and test one thing at a time
- [ ] Test names clearly describe the scenario and expected outcome
- [ ] Arrange-Act-Assert pattern is followed
- [ ] Tests are deterministic and not flaky
- [ ] Edge cases and error conditions are covered
- [ ] Async tests properly handle cancellation and timeouts
- [ ] Mock usage is appropriate and not over-engineered
- [ ] Performance tests have realistic expectations
- [ ] Tests serve as good documentation of the API

## Best Practices Summary

### Do's

- ✅ Write tests for all public APIs and edge cases
- ✅ Use descriptive test names that explain the scenario
- ✅ Follow the Arrange-Act-Assert pattern consistently
- ✅ Test both success and failure paths
- ✅ Use property-based testing for mathematical properties
- ✅ Keep tests simple, focused, and maintainable
- ✅ Use appropriate test doubles (mocks, stubs, fakes)
- ✅ Measure and maintain high test coverage
- ✅ Run tests frequently during development
- ✅ Treat test code with the same quality standards as production code

### Don'ts

- ❌ Don't write tests that depend on external resources
- ❌ Don't write flaky tests that pass/fail randomly
- ❌ Don't test implementation details (test behavior, not internals)
- ❌ Don't write overly complex test setups
- ❌ Don't ignore failing tests or mark them as "Ignore" permanently
- ❌ Don't write tests without clear assertions
- ❌ Don't copy-paste test code without understanding it
- ❌ Don't mix unit tests with integration tests in the same class
- ❌ Don't write tests that take too long to execute
- ❌ Don't forget to test async cancellation scenarios

This comprehensive testing strategy ensures that the Monads library maintains high quality, reliability, and confidence in its functionality across all supported scenarios.
