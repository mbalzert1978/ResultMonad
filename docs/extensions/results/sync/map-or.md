# MapOr

> Transform the Ok value or return an eagerly-evaluated fallback when Err.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `MapOrExtension`

## Signature

```csharp
public U MapOr<U>(U fallback, Func<T, U> operation) where U : notnull;
```

## Description

`MapOr` applies `operation` to the Ok value when this result is Ok, and returns `fallback` when it is Err. Unlike `MapOrElse`, the fallback is an eagerly-evaluated value — it is computed before `MapOr` is called. Use this when the fallback is cheap to construct (a constant, a cached value) and lazy evaluation is not needed.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `U` | The value to return when this result is Err. Always evaluated. |
| `operation` | `Func<T, U>` | The transformation to apply to the Ok value when this result is Ok. |

## Returns

`operation(value)` when this result is Ok; otherwise `fallback`.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` or `operation` is null. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Ok path — operation applied, fallback ignored
int fromOk = Ok<int, string>(5).MapOr(0, x => x * 2);
// fromOk == 10

// Err path — fallback returned, operation skipped
int fromErr = Err<int, string>("fail").MapOr(0, x => x * 2);
// fromErr == 0

// Practical: extract a field with a safe default
Result<User, string> userResult = FindUser(id);
string displayName = userResult.MapOr("Anonymous", u => u.Name);
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
