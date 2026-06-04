# MapErr

> Transform the Err value with a function, leaving Ok results untouched.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `MapErrExtension`

## Signature

```csharp
public Result<T, F> MapErr<F>(Func<E, F> operation) where F : notnull;
```

## Description

`MapErr` is the mirror of `Map` for the error channel. It applies `operation` to the Err value when this result is Err, producing a new `Err<T, F>` with a converted error. If this result is Ok, the value propagates unchanged and `operation` is never called. Use `MapErr` to convert between error representations — for example, turning an integer error code into a domain error record.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<E, F>` | The transformation to apply to the Err value. |

## Returns

`Err<T, F>` containing the transformed error when this result is Err; otherwise `Ok<T, F>` carrying the original Ok value.

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

// Err path — HTTP status code converted to a readable message
Result<string, string> result = Err<string, int>(404).MapErr(code => $"HTTP {code}");
// result == Err("HTTP 404")

// Ok path — operation is skipped, value propagates
Result<string, string> ok = Ok<string, int>("data").MapErr(code => $"HTTP {code}");
// ok == Ok("data")

// Adapting error types between layers
record DomainError(string Message);

Result<int, DomainError> adapted = Err<int, string>("record not found")
    .MapErr(msg => new DomainError(msg));
// adapted == Err(DomainError { Message = "record not found" })
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
