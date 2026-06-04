# OrElse

> Recover from an Err by lazily computing an alternative result from the error value.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `OrElseExtension`

## Signature

```csharp
public Result<T, F> OrElse<F>(Func<E, Result<T, F>> operation) where F : notnull;
```

## Description

`OrElse` is the lazy counterpart to `Or`. When this result is Ok, it is returned unchanged (re-typed to `Result<T, F>`). When it is Err, `operation` is invoked with the error value and its result is returned. Because `operation` can inspect the error, `OrElse` is suited for error recovery strategies that depend on what went wrong — retrying with a different input, substituting a computed default, or converting one error type to another.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<E, Result<T, F>>` | Invoked with the Err value to produce an alternative result. Only called when this result is Err. |

## Returns

This result (re-wrapped as `Result<T, F>`) when Ok; otherwise the `Result<T, F>` returned by `operation`.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` or `operation` is null. |
| `InvalidOperationException` | When `operation` returns null. |
| `UnreachableException` | Defensive Roslyn-required guard — never reached in practice. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Err path — operation recovers with a default
Result<int, string> recovered =
    Err<int, string>("not found").OrElse(e => Ok<int, string>(0));
// recovered == Ok(0)

// Ok path — operation is not called, value passes through
Result<int, string> unchanged =
    Ok<int, string>(5).OrElse(e => Ok<int, string>(0));
// unchanged == Ok(5)

// Recovery that depends on the error
Result<int, string> result = Err<int, string>("cache miss");
Result<int, string> fetched = result.OrElse(e =>
    e == "cache miss" ? FetchFromDatabase() : Err<int, string>(e));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
