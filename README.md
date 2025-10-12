# ResultMonad

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/yourusername/ResultMonad)
[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-blue)](https://www.nuget.org/packages/ResultMonad)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![Coverage](https://img.shields.io/badge/coverage-100%25-brightgreen)](https://github.com/yourusername/ResultMonad)

A functional programming library for C# that provides a robust Result type for railway-oriented programming and explicit error handling.

## Overview

ResultMonad is a lightweight, type-safe implementation of the Result monad pattern in C#. It enables you to write cleaner, more maintainable code by making error handling explicit and composable, without relying on exceptions for control flow.

## What is ResultMonad?

ResultMonad provides a `Result<T, E>` type that represents either a successful value (`Ok<T, E>`) or an error (`Err<T, E>`). This approach, known as railway-oriented programming, allows you to:

- **Make errors explicit**: No more hidden exceptions in your method signatures
- **Compose operations**: Chain multiple fallible operations together cleanly
- **Handle errors gracefully**: Process errors where it makes sense, not where exceptions are thrown
- **Write safer code**: The type system ensures you handle both success and failure cases

### Key Benefits

- ✅ **Type-safe error handling**: Compiler enforces error handling at compile time
- ✅ **No exception overhead**: Errors are values, not exceptions
- ✅ **Functional composition**: Rich set of extension methods (`Map`, `Bind`, `Match`, etc.)
- ✅ **Async/await support**: Full support for `Task` and `ValueTask` with async extension methods
- ✅ **Railway-oriented programming**: Build robust data processing pipelines
- ✅ **Zero dependencies**: Lightweight library with no external dependencies

## Quick Start

### Installation

Add the package to your project:

```bash
dotnet add package ResultMonad
```

### Basic Usage

```csharp
using ResultMonad;

// Create success and error results
Result<int, string> success = new Ok<int, string>(42);
Result<int, string> failure = new Err<int, string>("Something went wrong");

// Pattern matching
var message = success.Match(
    ok: value => $"Success: {value}",
    err: error => $"Error: {error}"
);

// Transform success values with Map
var doubled = success.Map(x => x * 2);

// Chain operations with Bind
Result<int, string> Divide(int a, int b) =>
    b == 0 
        ? new Err<int, string>("Division by zero") 
        : new Ok<int, string>(a / b);

var result = success
    .Bind(x => Divide(x, 2))
    .Map(x => x + 10);

// Handle errors with OrElse
var fallback = failure.OrElse(err => new Ok<int, string>(0));
```

### Async Support

```csharp
// All extension methods support Task and ValueTask
async Task<Result<string, string>> FetchDataAsync()
{
    // Your async operation
    return new Ok<string, string>("data");
}

var result = await FetchDataAsync()
    .Map(data => data.ToUpper())
    .Bind(async data => await ProcessAsync(data));
```

## Features

### Core Result Type

- **`Result<T, E>`**: Abstract base type representing either success or failure
- **`Ok<T, E>`**: Success variant containing a value of type `T`
- **`Err<T, E>`**: Error variant containing an error of type `E`
- **Type predicates**: `IsOk`, `IsErr`, `IsOkAnd()`, `IsErrAnd()` for result inspection

### Functional Extensions

#### Synchronous Operations

- **`Map`**: Transform the success value
- **`MapErr`**: Transform the error value
- **`Bind`**: Chain operations that return `Result` (flatMap/andThen)
- **`Match`**: Pattern match on success or error
- **`OrElse`**: Provide fallback for error cases
- **`Flatten`**: Flatten nested results

#### Asynchronous Operations

Full support for both `Task<Result<T, E>>` and `ValueTask<Result<T, E>>`:

- **`MapTask`** / **`MapValueTask`**: Transform async success values
- **`MapErrTask`** / **`MapErrValueTask`**: Transform async error values
- **`BindTask`** / **`BindValueTask`**: Chain async operations
- **`MatchTask`** / **`MatchValueTask`**: Async pattern matching
- **`OrElseTask`** / **`OrElseValueTask`**: Async fallback handling
- **`FlattenAsync`**: Flatten nested async results

### Additional Types

- **`Unit`**: Represents absence of a meaningful value (similar to `void` but as a value)

## Documentation

Comprehensive documentation is available in the [`.documentation`](.documentation/) folder:

### 📚 Core Documentation

| Document | Description |
|----------|-------------|
| [Concepts](.documentation/concepts/) | Core concepts: Result type, railway-oriented programming, error handling patterns |
| [Getting Started](.documentation/getting-started/) | Installation, first steps, basic examples, migration guides |
| [API Reference](.documentation/api/) | Complete API documentation for all types and methods |

### 🔧 Development

| Document | Description |
|----------|-------------|
| [Architecture](.documentation/architecture/) | Project structure, design decisions, extension patterns |
| [Contributing](.documentation/contributing/) | Contribution guidelines, coding standards, documentation standards |
| [Testing](.documentation/testing/) | Testing philosophy, running tests, writing new tests |

### 📋 Additional Resources

| Document | Description |
|----------|-------------|
| [Examples](.documentation/examples/) | Real-world usage examples and patterns |
| [FAQ](.documentation/faq/) | Frequently asked questions and troubleshooting |
| [Changelog](.documentation/changelog/) | Version history and release notes |

## Contributing

We welcome contributions! Please see our [Contributing Guide](.documentation/contributing/contributing.md) for details on:

- Code of conduct
- Development setup
- Coding standards and guidelines
- Documentation standards
- Pull request process
- Testing requirements

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Made with ❤️ for functional programming in C#
