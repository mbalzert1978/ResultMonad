# MapOrElse

> Transform the Ok value, or lazily compute a fallback from the Err value.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `MapOrElseExtension`

## Signature

```csharp
public U MapOrElse<U>(Func<E, U> fallback, Func<T, U> operation) where U : notnull;
```

## Description

`MapOrElse` applies `operation` to the Ok value when this result is Ok, and invokes `fallback` with the error when it is Err. Both branches produce a `U`, so the result is always unwrapped. The fallback is lazy — it is only invoked when needed, which makes it appropriate when constructing the default is expensive or when the error value itself should influence the fallback.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `Func<E, U>` | Invoked with the Err value when this result is Err. |
| `operation` | `Func<T, U>` | Invoked with the Ok value when this result is Ok. |

## Returns

`operation(value)` when this result is Ok; otherwise `fallback(error)`.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self`, `fallback`, or `operation` is null. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Ok path — operation runs, fallback is not called
int fromOk = Ok<int, string>(5).MapOrElse(e => -1, x => x * 2);
// fromOk == 10

// Err path — fallback receives the error
int fromErr = Err<int, string>("fail").MapOrElse(e => -1, x => x * 2);
// fromErr == -1

// Fallback uses the error to produce a meaningful default
Result<int, string> result = Err<int, string>("not a number");
int parsed = result.MapOrElse(e => e.Length, v => v);
// parsed == 12  (length of "not a number")
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
