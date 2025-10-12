# Quick Start Guide

This guide will help you write your first Monads code in just a few minutes.

## Prerequisites

- Monads installed ([Installation Guide](installation.md))
- Basic C# knowledge
- A C# project ready to use

## Your First Result

Let's create a simple function that might fail and handle it using Result.

### Step 1: Import the Namespace

```csharp
using Monads;
```

### Step 2: Create a Function That Returns Result

Instead of throwing exceptions, return a `Result<T, E>`:

```csharp
public Result<int, string> Divide(int numerator, int denominator)
{
    if (denominator == 0)
    {
        return new Err<int, string>("Cannot divide by zero");
    }
    
    return new Ok<int, string>(numerator / denominator);
}
```

### Step 3: Handle the Result

Use pattern matching with `Match` to handle both success and error cases:

```csharp
var result = Divide(10, 2);

var message = result.Match(
    ok: value => $"Result: {value}",
    err: error => $"Error: {error}"
);

Console.WriteLine(message); // Output: Result: 5
```

### Step 4: Try an Error Case

```csharp
var errorResult = Divide(10, 0);

var errorMessage = errorResult.Match(
    ok: value => $"Result: {value}",
    err: error => $"Error: {error}"
);

Console.WriteLine(errorMessage); // Output: Error: Cannot divide by zero
```

## Complete Example

Here's a complete console application:

```csharp
using Monads;

class Program
{
    static void Main()
    {
        // Success case
        var result1 = Divide(10, 2);
        PrintResult(result1);
        
        // Error case
        var result2 = Divide(10, 0);
        PrintResult(result2);
        
        // Chaining operations
        var result3 = Divide(20, 4)
            .Map(x => x * 2)        // Transform success value
            .Map(x => x + 10);      // Chain another transformation
        PrintResult(result3);
    }
    
    static Result<int, string> Divide(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            return new Err<int, string>("Cannot divide by zero");
        }
        
        return new Ok<int, string>(numerator / denominator);
    }
    
    static void PrintResult(Result<int, string> result)
    {
        var message = result.Match(
            ok: value => $"✓ Success: {value}",
            err: error => $"✗ Error: {error}"
        );
        Console.WriteLine(message);
    }
}
```

**Output:**

```text
✓ Success: 5
✗ Error: Cannot divide by zero
✓ Success: 20
```

## Common Patterns

### Checking Result State

```csharp
var result = Divide(10, 2);

if (result.IsOk)
{
    Console.WriteLine("Operation succeeded!");
}

if (result.IsErr)
{
    Console.WriteLine("Operation failed!");
}
```

### Transforming Success Values with Map

```csharp
var result = Divide(10, 2)
    .Map(x => x * 2)           // Double the result
    .Map(x => x.ToString())    // Convert to string
    .Map(s => $"Value: {s}");  // Format as message
```

### Chaining Operations with Bind

Use `Bind` when your transformation also returns a `Result`:

```csharp
var result = Divide(20, 4)
    .Bind(x => Divide(x, 2))   // Chain another division
    .Bind(x => Divide(x, 2));  // And another

result.Match(
    ok: value => Console.WriteLine($"Final result: {value}"),
    err: error => Console.WriteLine($"Failed: {error}")
);
```

### Providing Fallback Values with OrElse

```csharp
var result = Divide(10, 0)
    .OrElse(error => 
    {
        Console.WriteLine($"Recovered from: {error}");
        return new Ok<int, string>(0); // Fallback value
    });

Console.WriteLine($"Value: {result.Match(ok: v => v, err: _ => -1)}");
```

### Transforming Errors with MapErr

```csharp
var result = Divide(10, 0)
    .MapErr(error => $"[ERROR] {error}");

result.Match(
    ok: value => Console.WriteLine($"Success: {value}"),
    err: error => Console.WriteLine(error) // Output: [ERROR] Cannot divide by zero
);
```

## Real-World Example: File Reading

```csharp
using System.IO;
using Monads;

public class FileReader
{
    public Result<string, string> ReadFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return new Err<string, string>($"File not found: {path}");
            }
            
            var content = File.ReadAllText(path);
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return new Err<string, string>("File is empty");
            }
            
            return new Ok<string, string>(content);
        }
        catch (Exception ex)
        {
            return new Err<string, string>($"Failed to read file: {ex.Message}");
        }
    }
    
    public Result<int, string> CountWords(string path)
    {
        return ReadFile(path)
            .Map(content => content.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Map(words => words.Length);
    }
}

// Usage
var reader = new FileReader();
var wordCount = reader.CountWords("document.txt");

wordCount.Match(
    ok: count => Console.WriteLine($"Word count: {count}"),
    err: error => Console.WriteLine($"Error: {error}")
);
```

## Working with Async Operations

Monads fully supports async/await:

```csharp
using System.Net.Http;
using Monads;

public async Task<Result<string, string>> FetchDataAsync(string url)
{
    try
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            return new Err<string, string>(
                $"HTTP {response.StatusCode}: {response.ReasonPhrase}"
            );
        }
        
        var content = await response.Content.ReadAsStringAsync();
        return new Ok<string, string>(content);
    }
    catch (Exception ex)
    {
        return new Err<string, string>($"Request failed: {ex.Message}");
    }
}

// Usage with async chaining
var result = await FetchDataAsync("https://api.example.com/data")
    .Map(data => data.ToUpper())
    .Map(data => data.Length);

result.Match(
    ok: length => Console.WriteLine($"Response length: {length}"),
    err: error => Console.WriteLine($"Error: {error}")
);
```

## Key Takeaways

✅ **Use `Result<T, E>` instead of throwing exceptions** for expected errors

✅ **Always handle both cases** with `Match` or check `IsOk`/`IsErr`

✅ **Use `Map` for transformations** that don't return Result

✅ **Use `Bind` for chaining** operations that return Result

✅ **Use `OrElse` for fallback** values or error recovery

✅ **Use `MapErr` to transform** error messages

## Next Steps

- 📖 [Basic Concepts](basic-concepts.md) - Deep dive into Result, Ok, and Err
- 📖 [Error Handling Patterns](../concepts/error-handling.md) - Best practices
- 📖 [Async Patterns](../concepts/async-patterns.md) - Advanced async scenarios
- 📖 [API Reference](../api/) - Complete method documentation
- 📖 [Examples](../examples/) - More real-world use cases

## Common Questions

### When should I use Result instead of exceptions?

Use `Result` for:

- ✅ Expected errors (validation, business logic failures)
- ✅ Control flow with errors
- ✅ Composable error handling

Use exceptions for:

- ❌ Truly exceptional situations (out of memory, system failures)
- ❌ Framework/library requirements
- ❌ Unrecoverable errors

### Do I need to catch exceptions when using Result?

No! That's the point. Result makes errors explicit in your type signatures. However, you might still need try-catch when interfacing with code that throws exceptions (like I/O operations).

### Can I convert between Result and exceptions?

Yes! You can wrap exception-throwing code:

```csharp
public Result<T, string> TryExecute<T>(Func<T> operation)
{
    try
    {
        return new Ok<T, string>(operation());
    }
    catch (Exception ex)
    {
        return new Err<T, string>(ex.Message);
    }
}
```

## Troubleshooting

### "The type 'Result' could not be found"

- Ensure you have `using Monads;` at the top of your file
- Verify the package is installed: `dotnet list package`

### "Cannot convert from 'Ok' to 'Result'"

Make sure you're specifying both type parameters:

```csharp
// ❌ Wrong
Result<int, string> result = new Ok(42);

// ✅ Correct
Result<int, string> result = new Ok<int, string>(42);
```

### Pattern matching not working

Ensure you're using `Match` correctly:

```csharp
// ✅ Correct - both parameters required
result.Match(
    ok: value => DoSomething(value),
    err: error => HandleError(error)
);
```
