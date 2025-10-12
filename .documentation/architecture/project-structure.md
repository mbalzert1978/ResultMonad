# Project Structure

This document explains the organization and structure of the Monads library project, including the rationale behind the folder layout, file naming conventions, and how different components relate to each other.

## Overview

The Monads library follows a clean, modular architecture that separates concerns and promotes maintainability. The project structure is organized around the following principles:

- **Separation of Concerns**: Core models, extensions, and utilities are clearly separated
- **Discoverability**: Related functionality is grouped together logically
- **Scalability**: Structure supports adding new monads and functionality
- **Testing**: Tests mirror the source structure for easy navigation
- **Documentation**: Comprehensive documentation structure for different audiences

## Root Directory Structure

```bash
Monads/
├── .documentation/          # Comprehensive documentation
├── src/                     # Source code
├── tests/                   # Test projects
├── tasks/                   # Project management and planning
├── Directory.Build.props    # MSBuild properties shared across projects
├── Directory.Packages.props # Central package management
├── global.json             # .NET SDK version specification
├── Monads.slnx            # Solution file
├── nuget.config           # NuGet package sources configuration
└── README.md              # Project overview and quick start
```

### Root-Level Configuration Files

#### MSBuild Configuration

- **`Directory.Build.props`**: Shared MSBuild properties and settings for all projects
  - Compiler settings and warnings
  - Package metadata (authors, license, etc.)
  - Common NuGet package references
  - Code analysis rules

- **`Directory.Packages.props`**: Central Package Management (CPM) configuration
  - Centralized version management for all NuGet packages
  - Ensures consistent package versions across projects
  - Simplifies dependency updates

#### .NET Configuration

- **`global.json`**: Specifies the .NET SDK version and rollForward policy
  - Ensures consistent build environment across different machines
  - Controls which .NET SDK versions are acceptable

#### Package Management

- **`nuget.config`**: NuGet configuration for package sources and settings
  - Specifies package sources (NuGet.org, private feeds, etc.)
  - Package source mappings and security settings

## Source Code Structure (`src/`)

```bash
src/
└── Monads/
    ├── Monads.csproj           # Main library project file
    ├── Extensions/             # Extension methods for Result<T,E>
    │   ├── Results/           
    │   │   ├── Async/         # Async extension methods
    │   │   └── Sync/          # Synchronous extension methods
    ├── Models/                # Core model types
    │   ├── Unit.cs            # Unit type for void operations
    │   └── Results/           # Result monad implementations
    │       ├── Result{T,E}.cs # Abstract base class
    │       ├── Ok{T,E}.cs     # Success result implementation
    │       └── Err{T,E}.cs    # Error result implementation
    └── Strings/               # String constants and resources
        └── Constants.cs       # Error messages and constants
```

### Core Models (`Models/`)

The `Models` directory contains the fundamental types that define the monad implementations:

#### `Results/` Subdirectory

- **`Result{T,E}.cs`**: Abstract base class that defines the contract for all Result types
  - Contains shared logic and abstract methods
  - Defines factory methods for creating results
  - Implements common properties like `IsOk` and `IsErr`

- **`Ok{T,E}.cs`**: Represents successful operation results
  - Sealed class inheriting from `Result<T,E>`
  - Contains the success value
  - Implements success-specific behavior

- **`Err{T,E}.cs`**: Represents failed operation results
  - Sealed class inheriting from `Result<T,E>`
  - Contains the error information
  - Implements error-specific behavior

#### `Unit.cs`

- Special type representing the absence of a meaningful value
- Used for operations that don't return data (equivalent to `void`)
- Provides operators and implicit conversions for convenience

### Extensions (`Extensions/`)

The `Extensions` directory contains all extension methods that provide the fluent API:

#### Sync Extensions (`Extensions/Results/Sync/`)

- **`BindExtension.cs`**: Monadic bind operations for chaining
- **`MapExtension.cs`**: Functor map operations for transformations
- **`MapErrExtension.cs`**: Error transformation operations
- **`MatchExtension.cs`**: Pattern matching and exhaustive handling
- **`FlattenExtension.cs`**: Flattening nested Result structures
- **`OrElseExtension.cs`**: Fallback and alternative value operations

#### Async Extensions (`Extensions/Results/Async/`)

- **`BindTaskExtension.cs`**: Async bind operations for Task<Result<T,E>>
- **`BindValueTaskExtension.cs`**: Async bind operations for ValueTask<Result<T,E>>
- **`MapTaskExtension.cs`**: Async map operations for Task<Result<T,E>>
- **`MapValueTaskExtension.cs`**: Async map operations for ValueTask<Result<T,E>>
- **`MapErrTaskExtension.cs`**: Async error transformation for Task<Result<T,E>>
- **`MapErrValueTaskExtension.cs`**: Async error transformation for ValueTask<Result<T,E>>
- **`MatchTaskExtension.cs`**: Async pattern matching for Task<Result<T,E>>
- **`MatchValueTaskExtension.cs`**: Async pattern matching for ValueTask<Result<T,E>>
- **`OrElseTaskExtension.cs`**: Async fallback operations for Task<Result<T,E>>
- **`OrElseValueTaskExtension.cs`**: Async fallback operations for ValueTask<Result<T,E>>
- **`FlattenAsyncExtension.cs`**: Async flattening operations

### Utilities (`Strings/`)

The `Strings` directory contains shared constants and resources:

#### `Constants.cs`

- Error message templates
- Exception messages
- Shared string resources
- Localization keys (future expansion)

## Test Structure (`tests/`)

```bash
tests/
└── Tests.Monads.Result/
    ├── Tests.Monads.Result.csproj  # Test project file
    ├── xunit.runner.json           # xUnit configuration
    ├── Extensions/                 # Tests for extension methods
    │   ├── Async/                 # Async extension method tests
    │   │   ├── BindTaskExtensionTests.cs
    │   │   ├── BindValueTaskExtensionTests.cs
    │   │   ├── MapTaskExtensionTests.cs
    │   │   └── ...
    │   └── Sync/                  # Sync extension method tests
    │       ├── BindTests.cs
    │       ├── MapTests.cs
    │       ├── MapErrTests.cs
    │       └── ...
    └── Models/                    # Tests for core models
        ├── ErrTests.cs           # Tests for Err<T,E>
        ├── OkTests.cs            # Tests for Ok<T,E>
        ├── ResultPredicateTests.cs # Tests for Result<T,E> predicates
        └── UnitTests.cs          # Tests for Unit type
```

### Test Organization Principles

#### Mirrored Structure

- Test project structure mirrors the source code structure
- Each source file has a corresponding test file with `Tests` suffix
- Same namespace hierarchy maintained in test projects

#### Test Categories

- **Unit Tests**: Test individual methods and properties in isolation
- **Integration Tests**: Test interaction between components
- **Property Tests**: Test mathematical properties (associativity, etc.)
- **Performance Tests**: Benchmark critical paths

#### Test File Naming

- Source file: `BindExtension.cs` → Test file: `BindTests.cs`
- Class name: `BindExtensionTests` or `BindTests`
- Clear, descriptive test method names following pattern: `Method_Scenario_ExpectedResult`

## Documentation Structure (`.documentation/`)

```bash
.documentation/
├── api-reference/              # API documentation
│   ├── models/                # Core type documentation
│   │   ├── result.md         # Result<T,E> API reference
│   │   ├── ok.md             # Ok<T,E> API reference
│   │   ├── err.md            # Err<T,E> API reference
│   │   └── unit.md           # Unit type API reference
│   ├── extensions/           # Extension method documentation
│   │   ├── sync-extensions.md    # Synchronous extensions
│   │   └── async-extensions.md   # Asynchronous extensions
│   └── exceptions.md         # Exception reference
├── examples/                 # Usage examples and patterns
│   ├── common-scenarios.md   # Real-world usage examples
│   ├── error-handling-patterns.md # Best practices
│   └── async-workflows.md    # Async composition patterns
├── architecture/             # Architecture documentation
│   ├── design-decisions.md   # Design rationale and trade-offs
│   ├── project-structure.md  # This document
│   └── extension-architecture.md # Extension method patterns
└── contributing/             # Contributor documentation
    ├── guidelines.md         # Development workflow
    ├── code-style.md         # Coding standards
    └── testing-strategy.md   # Testing approach
```

### Documentation Categories

#### API Reference

- **Comprehensive**: Every public type and member documented
- **Examples**: Code examples for all major functionality
- **Cross-References**: Links between related types and methods
- **XML Integration**: Synced with XML documentation in source code

#### Examples and Patterns

- **Practical**: Real-world scenarios and use cases
- **Progressive**: From simple to complex examples
- **Best Practices**: Recommended patterns and anti-patterns
- **Performance**: Guidance on efficient usage

#### Architecture

- **Design Rationale**: Why decisions were made
- **Trade-offs**: Alternatives considered and rejected
- **Extension Points**: How to extend the library
- **Future Plans**: Roadmap and planned improvements

#### Contributing

- **Developer Setup**: Environment and tooling requirements
- **Standards**: Code style, naming, and documentation standards
- **Process**: How to contribute, review process, CI/CD
- **Testing**: Requirements and best practices

## Project Management (`tasks/`)

```bash
tasks/
├── 0001-prd-comprehensive-documentation.md  # Product Requirements Document
└── tasks-0001-prd-comprehensive-documentation.md  # Task breakdown
```

### Task Management

- **PRD Documents**: Product requirements and specifications
- **Task Breakdowns**: Detailed task lists and acceptance criteria
- **Progress Tracking**: Status updates and completion tracking
- **Planning**: Future feature planning and roadmap

## Build and Output Structure

### Build Artifacts (`bin/` and `obj/`)

```bash
src/Monads/
├── bin/
│   └── Debug/
│       └── net9.0/
│           ├── Monads.dll        # Compiled library
│           ├── Monads.xml        # XML documentation
│           └── Monads.deps.json  # Dependency information
└── obj/
    ├── *.nuget.*                # NuGet restore files
    ├── project.assets.json      # Project dependencies
    └── Debug/
        └── net9.0/              # Intermediate build files
```

### Package Structure

When packaged as NuGet package, the library follows standard conventions:

```bash
lib/
├── net9.0/
│   ├── Monads.dll
│   └── Monads.xml
├── netstandard2.1/
│   ├── Monads.dll
│   └── Monads.xml
└── ...
```

## Dependency Management

### Central Package Management (CPM)

The project uses Central Package Management to ensure consistent package versions:

- **`Directory.Packages.props`**: Defines package versions centrally
- **Project files**: Reference packages without versions
- **Benefits**:
  - Consistent versions across all projects
  - Easier dependency updates
  - Better dependency conflict resolution
  - Simplified project files

### Target Frameworks

The library supports multiple .NET target frameworks:

- **.NET 9.0**: Latest features and performance improvements
- **.NET Standard 2.1**: Broad compatibility with .NET Core 3.0+
- **.NET Standard 2.0**: Maximum compatibility (future consideration)

## Naming Conventions

### File Naming

- **PascalCase**: All C# source files use PascalCase (`Result.cs`, `BindExtension.cs`)
- **Descriptive**: File names clearly indicate their purpose
- **Consistent Suffixes**:
  - `Extension.cs` for extension method files
  - `Tests.cs` for test files
  - `{TypeName}.cs` for type definition files

### Namespace Organization

```csharp
// Core models
namespace Monads.Models;
namespace Monads.Models.Results;

// Extensions
namespace Monads.Extensions.Results.Sync;
namespace Monads.Extensions.Results.Async;

// Tests mirror source namespaces
namespace Tests.Monads.Models;
namespace Tests.Monads.Extensions.Results.Sync;
```

### Type Naming

- **Generic Parameters**: `T` for value type, `TError` or `E` for error type
- **Result Types**: `Result<T, TError>`, `Ok<T, TError>`, `Err<T, TError>`
- **Extension Classes**: Static classes with `Extension` suffix
- **Test Classes**: Class name + `Tests` suffix

## Integration Points

### IDE Integration

- **IntelliSense**: XML documentation provides rich IDE tooltips
- **Debugging**: Source stepping and symbol support
- **Code Analysis**: Analyzers provide compile-time guidance
- **Refactoring**: Proper namespace organization supports IDE refactoring tools

### CI/CD Integration

- **Build Scripts**: MSBuild integration for automated builds
- **Testing**: xUnit integration for automated test execution
- **Documentation**: Automated documentation generation from XML comments
- **Packaging**: NuGet package generation with proper metadata

### Extensibility Points

- **Extension Methods**: Primary mechanism for adding functionality
- **Namespace Organization**: Allows selective imports
- **Interface Segregation**: Small, focused interfaces for different concerns
- **Generic Constraints**: Proper constraints enable safe extensions

## Best Practices

### File Organization

1. **One Type Per File**: Each class, interface, or enum in its own file
2. **Logical Grouping**: Related functionality grouped in same directories
3. **Clear Hierarchy**: Namespace hierarchy matches folder structure
4. **Consistent Naming**: Follow established naming patterns

### Dependency Management BP

1. **Minimal Dependencies**: Keep external dependencies to minimum
2. **Version Pinning**: Use Central Package Management for consistency
3. **Framework Targeting**: Support appropriate target frameworks
4. **Transitive Dependencies**: Monitor and manage transitive dependencies

### Documentation

1. **Comprehensive Coverage**: Document all public APIs
2. **Examples**: Provide code examples for complex functionality
3. **Cross-References**: Link related types and methods
4. **Consistency**: Maintain consistent documentation style

### Testing

1. **Mirror Structure**: Test structure matches source structure
2. **Comprehensive Coverage**: High test coverage for all scenarios
3. **Clear Names**: Descriptive test method names
4. **Fast Execution**: Keep tests fast and independent

This structure provides a solid foundation for maintaining and extending the Monads library while ensuring code quality, discoverability, and developer productivity.
