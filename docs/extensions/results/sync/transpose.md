# Transpose

> Swap the `Result` and `Option` layers — convert `Result<Option<T>, E>` to `Option<Result<T, E>>`.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `TransposeExtension`

## Signature

```csharp
// Receiver: Result<Option<T>, E>
public Option<Result<T, E>> Transpose();
```

## Description

`Transpose` converts between `Result<Option<T>, E>` and `Option<Result<T, E>>`. This is useful when combining results from APIs that return optional values inside a result, and you need to reason about the optionality at the outer level instead. The mapping rules are:

| Input | Output |
|-------|--------|
| `Ok(None)` | `None` |
| `Ok(Some(x))` | `Some(Ok(x))` |
| `Err(e)` | `Some(Err(e))` |

## Parameters

_None._ The receiver is `Result<Option<T>, E>`.

## Returns

`None` when the result is `Ok(None)`; `Some(Ok(x))` when the result is `Ok(Some(x))`; `Some(Err(e))` when the result is `Err(e)`.

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
using static Monads.Options.Option;

// Ok(Some(x)) → Some(Ok(x))
Result<Option<int>, string> okSome =
    Ok<Option<int>, string>(Some(42));
Option<Result<int, string>> result = okSome.Transpose();
// result == Some(Ok(42))

// Ok(None) → None
Result<Option<int>, string> okNone =
    Ok<Option<int>, string>(None<int>());
Option<Result<int, string>> none = okNone.Transpose();
// none == None

// Err(e) → Some(Err(e))
Result<Option<int>, string> err =
    Err<Option<int>, string>("lookup failed");
Option<Result<int, string>> someErr = err.Transpose();
// someErr == Some(Err("lookup failed"))
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
