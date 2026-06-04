# ToOk

> Convert a `Result<T, E>` to an `Option<T>` over the Ok value, discarding any error.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `ToOkExtension`

## Signature

```csharp
public Option<T> ToOk();
```

## Description

`ToOk` projects a `Result<T, E>` into `Option<T>` by keeping the Ok value and discarding the error. `Ok(x)` becomes `Some(x)` and `Err(e)` becomes `None`. Use this when you need to interface with APIs that consume `Option<T>` and you only care about the success case — the error information is intentionally dropped.

## Parameters

_None._

## Returns

`Some(value)` when this result is Ok; `None` when this result is Err.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` is null. |

## Examples

```csharp
using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Ok → Some
Option<int> some = Ok<int, string>(42).ToOk();
// some == Some(42)

// Err → None (error is discarded)
Option<int> none = Err<int, string>("not found").ToOk();
// none == None

// Useful when combining with Option-based APIs
Option<string> displayName = FindUser(id)
    .ToOk()
    .Map(u => u.DisplayName);
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
