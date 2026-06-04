# InspectErr

> Observe the Err value via a side-effecting action, then pass the result through unchanged.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `InspectErrExtension`

## Signature

```csharp
public Result<T, E> InspectErr(Action<E> action);
```

## Description

`InspectErr` is the error-channel mirror of `Inspect`. It invokes `action` with the Err value when this result is Err, then returns this result unchanged. When this result is Ok, `action` is never called. Use `InspectErr` to log errors, record metrics, or trigger alerts inside a pipeline without consuming or transforming the error.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `action` | `Action<E>` | The side-effecting action to invoke with the Err value. |

## Returns

This result unchanged, regardless of whether it is Ok or Err.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` or `action` is null. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Err path — action fires, result continues as Err
Result<int, string> result = Err<int, string>("oops")
    .InspectErr(e => Console.WriteLine($"Error: {e}"))  // prints "Error: oops"
    .OrElse(_ => Ok<int, string>(0));                   // Ok(0)

// Ok path — action is skipped, Ok propagates
Result<int, string> ok = Ok<int, string>(5)
    .InspectErr(e => Console.WriteLine($"Error: {e}"))  // nothing printed
    .Map(v => v + 1);                                   // Ok(6)

// Structured error logging in a pipeline
Result<Config, string> config = LoadConfig(path)
    .InspectErr(e => logger.LogError("Config load failed: {Error}", e))
    .Bind(ValidateConfig)
    .InspectErr(e => logger.LogError("Config validation failed: {Error}", e));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
