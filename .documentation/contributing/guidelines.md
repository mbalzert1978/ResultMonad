# Contributing Guidelines

Welcome to the Monads library! We appreciate your interest in contributing. This document provides comprehensive guidelines for setting up your development environment, understanding our workflow, and making meaningful contributions to the project.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Development Environment Setup](#development-environment-setup)
3. [Project Structure](#project-structure)
4. [Coding Standards](#coding-standards)
5. [Testing Requirements](#testing-requirements)
6. [Contribution Workflow](#contribution-workflow)
7. [Pull Request Process](#pull-request-process)
8. [Code Review Guidelines](#code-review-guidelines)
9. [Documentation Standards](#documentation-standards)
10. [Release Process](#release-process)

## Getting Started

### Prerequisites

Before contributing, ensure you have the following installed:

- **.NET SDK 9.0 or later**: Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Git**: Version control system
- **IDE**: Visual Studio 2022, Visual Studio Code, or JetBrains Rider
- **Optional**: Docker (for containerized development)

### Quick Start

1. **Fork the Repository**

   ```bash
   # Fork on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/Monads.git
   cd Monads
   ```

2. **Verify Development Environment**

   ```bash
   # Check .NET SDK version
   dotnet --version  # Should be 9.0 or later
   
   # Restore dependencies
   dotnet restore
   
   # Build the solution
   dotnet build
   
   # Run tests
   dotnet test
   ```

3. **Create a Feature Branch**

   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Environment Setup

### IDE Configuration

#### Visual Studio 2022

1. **Required Components**:
   - .NET 9.0 SDK
   - Git tools for Visual Studio
   - Code analysis tools

2. **Recommended Extensions**:
   - EditorConfig Language Service
   - Markdown Editor
   - GitLens (if using VS Code extensions)

3. **Settings**:
   - Enable "Format document on save"
   - Configure line endings to LF (Unix-style)
   - Set tab size to 4 spaces

#### Visual Studio Code

1. **Required Extensions**:
   - C# for Visual Studio Code
   - .NET Install Tool
   - EditorConfig for VS Code

2. **Recommended Extensions**:
   - GitLens
   - Markdown All in One
   - Code Spell Checker
   - SonarLint

3. **Workspace Settings** (`.vscode/settings.json`):

   ```json
   {
     "editor.formatOnSave": true,
     "editor.codeActionsOnSave": {
       "source.fixAll": true
     },
     "files.eol": "\n",
     "editor.insertSpaces": true,
     "editor.tabSize": 4
   }
   ```

### Build Configuration

The project uses several configuration files:

- **`Directory.Build.props`**: Shared MSBuild properties
- **`Directory.Packages.props`**: Central Package Management
- **`global.json`**: .NET SDK version specification
- **`nuget.config`**: Package source configuration
- **`.editorconfig`**: Code formatting rules

### Environment Variables

Set these environment variables for optimal development experience:

```bash
# Enable detailed build logging
export MSBUILDVERBOSITY=detailed

# Enable source link for debugging
export ContinuousIntegrationBuild=true

# Disable telemetry (optional)
export DOTNET_CLI_TELEMETRY_OPTOUT=true
```

## Project Structure

Understanding the project structure is crucial for effective contributions:

```bash
Monads/
├── .documentation/          # Comprehensive documentation
│   ├── api-reference/      # API documentation
│   ├── examples/           # Usage examples
│   ├── architecture/       # Architecture documentation
│   └── contributing/       # Contributor documentation
├── src/                    # Source code
│   └── Monads/            # Main library
│       ├── Extensions/     # Extension methods
│       ├── Models/         # Core types
│       └── Strings/        # Constants and resources
├── tests/                  # Test projects
│   └── Tests.Monads.Result/ # Result monad tests
└── tasks/                  # Project management
```

### Key Principles

- **Separation of Concerns**: Core models, extensions, and utilities are clearly separated
- **Test Structure**: Tests mirror the source code structure
- **Documentation**: Comprehensive documentation for all audiences
- **Modularity**: Components can be developed and tested independently

## Coding Standards

### General Principles

1. **Clarity Over Cleverness**: Write code that is easy to read and understand
2. **Consistency**: Follow established patterns throughout the codebase
3. **Type Safety**: Leverage C#'s type system for compile-time safety
4. **Performance**: Consider performance implications, especially in hot paths
5. **Immutability**: Prefer immutable data structures where possible

### Naming Conventions

#### Types and Members

```csharp
// Classes, interfaces, structs - PascalCase
public class ResultFactory
public interface IResultValidator
public struct ValidationResult

// Methods and Properties - PascalCase
public bool IsOk { get; }
public Result<T, TError> Map<T, TError>(...)

// Fields - camelCase with underscore prefix for private
private readonly IValidator _validator;
public readonly string Value;

// Parameters and local variables - camelCase
public void ProcessResult(Result<User, string> userResult)
{
    string errorMessage = "default";
}

// Constants - PascalCase
public const string DefaultErrorMessage = "Operation failed";

// Generic type parameters
T      // Value type
TError // Error type  
TResult // Result type
```

#### Files and Namespaces

```csharp
// Namespace follows folder structure
namespace Monads.Extensions.Results.Sync;
namespace Tests.Monads.Extensions.Results.Sync;

// File names match primary type
Result.cs               // Contains Result<T, TError>
BindExtension.cs        // Contains BindExtension
BindTests.cs           // Contains BindExtensionTests
```

### Code Structure

#### Extension Methods

```csharp
namespace Monads.Extensions.Results.Sync;

/// <summary>
/// Extension methods for binding operations on Result types.
/// </summary>
public static class BindExtension
{
    /// <summary>
    /// Chains a function that returns a Result, enabling composition of fallible operations.
    /// </summary>
    /// <typeparam name="T">The type of the success value in the input result.</typeparam>
    /// <typeparam name="TResult">The type of the success value in the output result.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="bindFunc">The function to apply to the success value.</param>
    /// <returns>The result of applying the bind function, or the original error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="bindFunc"/> is null.</exception>
    public static Result<TResult, TError> Bind<T, TResult, TError>(
        this Result<T, TError> result,
        Func<T, Result<TResult, TError>> bindFunc)
    {
        ArgumentNullException.ThrowIfNull(bindFunc);
        
        return result.IsOk
            ? bindFunc(result.Match(x => x, _ => throw new UnreachableException()))
            : result.MapErr(e => e);
    }
}
```

#### Model Types

```csharp
namespace Monads.Models.Results;

/// <summary>
/// Represents a successful result containing a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
public sealed class Ok<T, TError> : Result<T, TError>
{
    /// <summary>
    /// Gets the success value.
    /// </summary>
    public T Value { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Ok{T, TError}"/> class.
    /// </summary>
    /// <param name="value">The success value.</param>
    public Ok(T value)
    {
        Value = value;
    }
    
    /// <inheritdoc />
    public override bool IsOk => true;
    
    /// <inheritdoc />
    public override bool IsErr => false;
}
```

### Error Handling

#### Argument Validation

```csharp
public static Result<T, TError> Map<T, TResult, TError>(
    this Result<T, TError> result,
    Func<T, TResult> mapFunc)
{
    // Always validate function parameters
    ArgumentNullException.ThrowIfNull(mapFunc);
    
    // Implementation...
}
```

#### Exception Usage

- **Use exceptions only for exceptional circumstances**
- **ArgumentNullException**: For null parameter validation
- **InvalidOperationException**: For invalid state conditions
- **UnreachableException**: For code paths that should never execute

```csharp
// Good: Using Result for expected failures
public Result<User, ValidationError> ValidateUser(User user) { ... }

// Good: Using exceptions for programming errors
public void ProcessUser(User user)
{
    ArgumentNullException.ThrowIfNull(user);
    // ...
}

// Avoid: Using exceptions for business logic failures
public User GetUser(int id)
{
    // Don't do this - user not found is expected
    if (userNotFound) throw new UserNotFoundException();
}
```

## Testing Requirements

### Test Coverage

- **Minimum Coverage**: 95% line coverage for all public APIs
- **Branch Coverage**: 90% branch coverage for conditional logic
- **Test Categories**: Unit tests, integration tests, property-based tests

### Test Structure

#### Test Organization

```csharp
namespace Tests.Monads.Extensions.Results.Sync;

[TestFixture]
public class BindTests
{
    [Test]
    public void Bind_WithOkResult_AppliesFunctionAndReturnsResult()
    {
        // Arrange
        var result = new Ok<int, string>(42);
        
        // Act
        Result<string, string> bound = result.Bind(x => new Ok<string, string>(x.ToString()));
        
        // Assert
        Assert.That(bound.IsOk, Is.True);
        bound.Match(
            onOk: value => Assert.That(value, Is.EqualTo("42")),
            onErr: error => Assert.Fail($"Expected success but got error: {error}")
        );
    }
    
    [Test]
    public void Bind_WithErrResult_ReturnsOriginalError()
    {
        // Arrange
        var result = new Err<int, string>("original error");
        
        // Act
        Result<string, string> bound = result.Bind(x => new Ok<string, string>(x.ToString()));
        
        // Assert
        Assert.That(bound.IsErr, Is.True);
        bound.Match(
            onOk: value => Assert.Fail($"Expected error but got success: {value}"),
            onErr: error => Assert.That(error, Is.EqualTo("original error"))
        );
    }
    
    [Test]
    public void Bind_WithNullFunction_ThrowsArgumentNullException()
    {
        // Arrange
        var result = new Ok<int, string>(42);
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => result.Bind<string>(null!));
    }
}
```

#### Test Categories

1. **Happy Path Tests**: Test successful scenarios
2. **Error Path Tests**: Test error scenarios and edge cases
3. **Null Parameter Tests**: Test argument validation
4. **Property Tests**: Test mathematical properties (when applicable)

### Property-Based Testing

For mathematical properties, use property-based testing:

```csharp
[Test]
public void Map_IdentityLaw_MapWithIdentityReturnsOriginal()
{
    // Property: result.Map(x => x) == result
    var result = new Ok<int, string>(42);
    
    Result<int, string> mapped = result.Map(x => x);
    
    Assert.That(mapped.Match(x => x, e => -1), Is.EqualTo(result.Match(x => x, e => -1)));
}
```

### Performance Testing

Critical paths should have performance benchmarks:

```csharp
[Test]
public void Map_Performance_HandlesLargeNumbersEfficiently()
{
    const int iterations = 1_000_000;
    var result = new Ok<int, string>(42);
    
    Stopwatch sw = Stopwatch.StartNew();
    
    for (int i = 0; i < iterations; i++)
    {
        result.Map(x => x + 1);
    }
    
    sw.Stop();
    
    // Should complete in reasonable time
    Assert.That(sw.ElapsedMilliseconds, Is.LessThan(100));
}
```

## Contribution Workflow

### Issue Management

1. **Check Existing Issues**: Before creating new issues, search existing ones
2. **Use Issue Templates**: Follow provided templates for bug reports and feature requests
3. **Label Appropriately**: Use labels to categorize issues (bug, enhancement, documentation, etc.)
4. **Provide Details**: Include reproduction steps for bugs, use cases for features

### Branch Strategy

```bash
# Feature branches
feature/add-validation-extensions
feature/improve-error-messages

# Bug fix branches  
bugfix/fix-null-handling
bugfix/correct-async-behavior

# Documentation branches
docs/update-api-reference
docs/add-contributing-guide
```

### Commit Messages

Use conventional commit format:

```bash
# Feature commits
feat: add Filter extension method for conditional results
feat(async): implement ValueTask variants for better performance

# Bug fixes
fix: handle null values in Map extension correctly
fix(tests): resolve flaky async test timing issues

# Documentation
docs: update README with new extension methods
docs(api): add XML documentation for new Filter method

# Refactoring
refactor: simplify error handling in Bind operations
refactor(tests): extract common test utilities

# Performance improvements
perf: optimize hot path in Map extension
perf(async): reduce allocations in async operations
```

### Development Workflow

1. **Create Feature Branch**

   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make Changes**
   - Write code following coding standards
   - Add comprehensive tests
   - Update documentation
   - Run tests locally

3. **Pre-commit Validation**

   ```bash
   # Build and test
   dotnet build
   dotnet test
   
   # Format code
   dotnet format
   
   # Check for issues
   dotnet build --verbosity normal
   ```

4. **Commit Changes**

   ```bash
   git add .
   git commit -m "feat: add new extension method with tests and docs"
   ```

5. **Push and Create Pull Request**

   ```bash
   git push origin feature/your-feature-name
   # Create PR on GitHub
   ```

## Pull Request Process

### PR Requirements

Before submitting a pull request, ensure:

- [ ] All tests pass locally
- [ ] Code follows established coding standards
- [ ] New features have comprehensive tests (95%+ coverage)
- [ ] Breaking changes are documented
- [ ] XML documentation is provided for new public APIs
- [ ] Relevant documentation is updated
- [ ] Commit messages follow conventional format

### PR Template

```markdown
## Description
Brief description of the changes.

## Type of Change
- [ ] Bug fix (non-breaking change that fixes an issue)
- [ ] New feature (non-breaking change that adds functionality)
- [ ] Breaking change (fix or feature that causes existing functionality to change)
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] New tests added for new functionality
- [ ] Performance benchmarks (if applicable)

## Documentation
- [ ] XML documentation added/updated
- [ ] README updated (if necessary)
- [ ] Architecture documentation updated (if necessary)

## Checklist
- [ ] My code follows the coding standards
- [ ] I have performed a self-review of my code
- [ ] I have commented my code, particularly in hard-to-understand areas
- [ ] My changes generate no new warnings
- [ ] New and existing unit tests pass locally
```

### Review Process

1. **Automated Checks**: CI/CD pipeline runs automated tests and checks
2. **Code Review**: Maintainers review code quality, design, and testing
3. **Feedback Integration**: Address review feedback and update PR
4. **Approval**: PR approved by maintainers
5. **Merge**: PR merged to main branch

## Code Review Guidelines

### For Contributors

- **Self-Review**: Review your own PR before requesting review
- **Small PRs**: Keep PRs focused and reasonably sized
- **Clear Description**: Explain what changes were made and why
- **Responsive**: Address feedback promptly and professionally

### For Reviewers

#### What to Look For

1. **Correctness**: Does the code do what it's supposed to do?
2. **Testing**: Are there adequate tests for the changes?
3. **Performance**: Are there any performance implications?
4. **Maintainability**: Is the code readable and maintainable?
5. **Standards**: Does the code follow established conventions?

#### Review Checklist

- [ ] Code follows naming conventions and formatting standards
- [ ] All new public APIs have XML documentation
- [ ] Tests cover both happy path and error scenarios
- [ ] No breaking changes without proper documentation
- [ ] Performance-critical paths are optimized
- [ ] Error handling follows established patterns
- [ ] Async code uses proper patterns (ConfigureAwait, etc.)

## Documentation Standards

All contributions must include appropriate documentation and pass automated validation:

### XML Documentation Requirements

All public APIs must have comprehensive XML documentation:

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
/// <returns>A new result with the transformed success value, or the original error.</returns>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="mapFunc"/> is null.</exception>
/// <example>
/// <code>
/// Result&lt;int, string&gt; result = ResultFactory.Success&lt;string&gt;(5);
/// Result&lt;string, string&gt; mapped = result.Map(x => x.ToString());
/// // mapped contains "5"
/// </code>
/// </example>
public static Result<TResult, TError> Map<T, TResult, TError>(...) { ... }
```

**Important**: CS1591 warnings for missing XML documentation are treated as build failures in CI/CD.

### Markdown Documentation Requirements

- Use clear headings and structure
- Include code examples for complex concepts
- Update relevant documentation in the `.documentation/` folder
- Follow the established documentation structure
- Maintain consistency with existing documentation style
- **All markdown files must pass markdownlint validation**
- **All internal links must be valid and working**

### Automated Documentation Validation

Our CI/CD pipeline automatically validates:

1. **XML Documentation**: Builds fail if CS1591 warnings are present
2. **Markdown Formatting**: Uses markdownlint to ensure consistent formatting  
3. **Link Validation**: Verifies all internal documentation links work
4. **Documentation Structure**: Ensures required documentation directories exist
5. **Test Suite**: All tests must pass before documentation changes are accepted

#### Pre-commit Validation

Before submitting a PR, ensure your changes pass local validation:

```bash
# Build and check for XML documentation warnings
dotnet build --verbosity normal

# Install and run markdownlint (if you have npm/node)
npm install -g markdownlint-cli
markdownlint .documentation/**/*.md README.md

# Run full test suite  
dotnet test

- Cross-reference related functionality
- Keep examples practical and relevant

## Release Process

### Versioning Strategy

The project follows Semantic Versioning (SemVer):

- **Major**: Breaking changes to public API
- **Minor**: New functionality, backward compatible
- **Patch**: Bug fixes, backward compatible

### Release Checklist

1. **Update Version Numbers**: Update project files with new version
2. **Update CHANGELOG**: Document all changes since last release
3. **Run Full Test Suite**: Ensure all tests pass
4. **Update Documentation**: Ensure documentation reflects changes
5. **Create Release Tag**: Tag the release commit
6. **Publish Package**: Publish NuGet package
7. **Update Examples**: Ensure examples work with new version

Thank you for contributing to the Monads library! Your contributions help make functional programming in C# more accessible and enjoyable for everyone.
