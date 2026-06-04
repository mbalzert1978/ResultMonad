# UnwrapOr

> Extract the Ok value, or return an eagerly-provided fallback when Err.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `UnwrapOrExtension`

## Signature

```csharp
public T UnwrapOr(T fallback);
```

## Description

`UnwrapOr` exits the `Result` railway by extracting the Ok value directly. When this result is Err, `fallback` is returned instead. The fallback is eager — it is evaluated before `UnwrapOr` is called. Use this at the end of a pipeline when you need a plain `T` and have a cheap, pre-computed default. For expensive defaults or defaults that depend on the error value, use `UnwrapOrElse`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `T` | The value to return when this result is Err. Always evaluated. |

## Returns

The Ok value when this result is Ok; otherwise `fallback`.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` is null. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Ok path — Ok value returned
int fromOk = Ok<int, string>(42).UnwrapOr(0);
// fromOk == 42

// Err path — fallback returned
int fromErr = Err<int, string>("fail").UnwrapOr(0);
// fromErr == 0

// Terminating a pipeline with a safe default
string username = FindUser(id)
    .Map(u => u.Username)
    .UnwrapOr("Guest");
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
