# UnwrapOrElse

> Extract the Ok value, or lazily compute a fallback from the Err value.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `UnwrapOrElseExtension`

## Signature

```csharp
public T UnwrapOrElse(Func<E, T> fallback);
```

## Description

`UnwrapOrElse` exits the `Result` railway by extracting the Ok value directly. When this result is Err, `fallback` is invoked with the error value and its return is used. The fallback is lazy — it is only called when needed. Use this over `UnwrapOr` when the fallback is expensive to construct or when the error value should inform what the fallback is.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `Func<E, T>` | Invoked with the Err value when this result is Err. |

## Returns

The Ok value when this result is Ok; otherwise `fallback(error)`.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` or `fallback` is null. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Ok path — fallback not called
int fromOk = Ok<int, string>(99).UnwrapOrElse(e => e.Length);
// fromOk == 99

// Err path — fallback receives error, computes default
int fromErr = Err<int, string>("fail").UnwrapOrElse(e => e.Length);
// fromErr == 4

// Error-dependent recovery: cache lookup with DB fallback
int price = GetCachedPrice(sku).UnwrapOrElse(_ => QueryDatabase(sku));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
