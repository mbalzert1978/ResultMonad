# Map

> Transform the Ok value with a function, leaving Err results untouched.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `MapExtension`

## Signature

```csharp
public Result<U, E> Map<U>(Func<T, U> operation) where U : notnull;
```

## Description

`Map` applies `operation` to the Ok value and wraps the result in a new `Ok<U, E>`. If this result is Err, the error propagates unchanged and `operation` is never called. This is the standard functor map over the success channel — use it to transform data while keeping errors flowing through the railway.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<T, U>` | The transformation to apply to the Ok value. |

## Returns

`Ok<U, E>` containing the transformed value when this result is Ok; otherwise `Err<U, E>` carrying the original error.

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

// Ok path — operation is applied
Result<int, string> doubled = Ok<int, string>(5).Map(x => x * 2);
// doubled == Ok(10)

// Err path — operation is skipped, error propagates
Result<int, string> errResult = Err<int, string>("not found").Map(x => x * 2);
// errResult == Err("not found")

// Chaining multiple maps
Result<string, string> result = Ok<int, string>(3)
    .Map(x => x * x)      // Ok(9)
    .Map(x => $"#{x}");   // Ok("#9")
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
