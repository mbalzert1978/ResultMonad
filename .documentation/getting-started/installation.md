# Installation

This guide covers how to install and set up Monads in your C# project.

## Prerequisites

- **.NET 9.0 SDK** or later
- A C# project (console app, web app, library, etc.)
- Basic familiarity with NuGet package management

## Installation Methods

### Method 1: .NET CLI (Recommended)

The simplest way to add Monads to your project:

```bash
dotnet add package Monads
```

This command will:

- Download the latest version of Monads
- Add a package reference to your `.csproj` file
- Restore the package automatically

### Method 2: Package Manager Console (Visual Studio)

If you're using Visual Studio, open the Package Manager Console and run:

```powershell
Install-Package Monads
```

### Method 3: Manual .csproj Edit

Add the package reference directly to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Monads" Version="1.0.0" />
</ItemGroup>
```

Then restore packages:

```bash
dotnet restore
```

### Method 4: Visual Studio NuGet Package Manager

1. Right-click your project in Solution Explorer
2. Select **Manage NuGet Packages**
3. Search for **Monads**
4. Click **Install**

## Verifying Installation

After installation, verify that Monads is available:

```csharp
using Monads;

// This should compile without errors
Result<int, string> result = new Ok<int, string>(42);
Console.WriteLine(result.IsOk); // True
```

## Version Selection

### Latest Stable Version

```bash
dotnet add package Monads
```

### Specific Version

```bash
dotnet add package Monads --version 1.0.0
```

### Pre-release Versions

```bash
dotnet add package Monads --prerelease
```

## Project Setup

### Target Framework

Monads requires **.NET 9.0** or later. Ensure your `.csproj` file has:

```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
</PropertyGroup>
```

### Nullable Reference Types (Recommended)

For the best type safety, enable nullable reference types:

```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

This ensures the compiler helps you handle null values correctly when working with Result types.

### C# Language Version

Monads works with **C# 9.0** and later. Most projects targeting .NET 9.0 will use this by default.

## Multi-Project Solutions

If you have multiple projects in your solution that need Monads:

```bash
# Add to specific project
dotnet add src/MyProject/MyProject.csproj package Monads

# Or navigate to each project directory
cd src/MyProject
dotnet add package Monads
```

### Using Directory.Packages.props (Central Package Management)

For solutions with many projects, consider central package management:

1. Create `Directory.Packages.props` in your solution root:

   ```xml
   <Project>
     <PropertyGroup>
       <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
     </PropertyGroup>
     
     <ItemGroup>
       <PackageVersion Include="Monads" Version="1.0.0" />
     </ItemGroup>
   </Project>
   ```

1. Reference the package in each project without specifying version:

   ```xml
   <ItemGroup>
     <PackageReference Include="Monads" />
   </ItemGroup>
   ```

## Troubleshooting

### Package Not Found

If you get a "package not found" error:

1. Check your NuGet package sources:

   ```bash
   dotnet nuget list source
   ```

1. Ensure nuget.org is included:

   ```bash
   dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
   ```

### Version Conflicts

If you encounter version conflicts with other packages:

1. List all package references:

   ```bash
   dotnet list package
   ```

1. Update conflicting packages or use binding redirects

### Build Errors After Installation

1. Clean and rebuild:

   ```bash
   dotnet clean
   dotnet build
   ```

1. Restore packages explicitly:

   ```bash
   dotnet restore --force
   ```

## Next Steps

- 📖 [Quick Start Guide](quick-start.md) - Write your first Result-based code
- 📖 [Basic Concepts](basic-concepts.md) - Understand Result, Ok, and Err
- 📖 [API Reference](../api/) - Explore all available methods and types

## IDE Support

### Visual Studio

- **IntelliSense**: Full XML documentation support
- **Code Navigation**: Go to definition, find references
- **Debugging**: Full debugging support for Result types

### Visual Studio Code

- Install the **C# Dev Kit** extension for full IntelliSense support
- XML documentation will appear in hover tooltips

### JetBrains Rider

- Full IntelliSense and refactoring support out of the box
- Quick documentation (Ctrl+Q / Cmd+J)

## Additional Resources

- [NuGet Package Page](https://www.nuget.org/packages/Monads)
- [GitHub Repository](https://github.com/mbalzert1978/Monads)
- [Release Notes](../changelog/changelog.md)
