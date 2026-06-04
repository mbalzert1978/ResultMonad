# ToErr

> Convert a `Result<T, E>` to an `Option<E>` over the Err value, discarding any Ok value.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `ToErrExtension`

## Signature

```csharp
public Option<E> ToErr();
```

## Description

`ToErr` projects a `Result<T, E>` into `Option<E>` by keeping the Err value and discarding the Ok value. `Err(e)` becomes `Some(e)` and `Ok(x)` becomes `None`. Use this when you want to extract and inspect an error as an optional value — for example, when collecting errors from multiple results or when an API expects `Option<E>`.

## Parameters

_None._

## Returns

`Some(error)` when this result is Err; `None` when this result is Ok.

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

// Err → Some(error)
Option<string> some = Err<int, string>("not found").ToErr();
// some == Some("not found")

// Ok → None (value is discarded)
Option<string> none = Ok<int, string>(42).ToErr();
// none == None

// Collecting errors from multiple results
IEnumerable<Result<int, string>> results = GetAllResults();
IEnumerable<string> errors = results
    .Select(r => r.ToErr())
    .Where(o => o.IsSome)
    .Select(o => o.Unwrap());
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
