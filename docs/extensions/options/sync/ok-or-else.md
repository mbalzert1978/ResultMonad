# OkOrElse

> Convert an `Option<T>` to a `Result<T, E>`, lazily computing the error for the None case.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `OkOrElseExtension`

## Signature

```csharp
public Result<T, E> OkOrElse<E>(Func<E> error) where E : notnull;
```

## Description

`OkOrElse` bridges the `Option` and `Result` worlds. When this option is Some, it returns `Ok(value)`. When this option is None, it invokes `error()` and returns `Err` with the produced value. Because the error factory is a delegate it is only evaluated when needed, making `OkOrElse` the lazy counterpart to `OkOr`. Prefer `OkOrElse` when constructing the error value involves allocation, a database call, or any other non-trivial work.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `error` | `Func<E>` | Invoked when this option is None; provides the error value lazily. |

## Returns

`Ok<T, E>(value)` when this is Some; `Err<T, E>(error())` when this is None.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `error` is null. |
| `InvalidOperationException` | When `error` returns null (violates the `E : notnull` constraint). |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using Monads.Results;
using static Monads.Options.Option;

Option<int> some = Some(42);
Option<int> none = None<int>();

Result<int, string> ok  = some.OkOrElse(() => "not found");  // Ok(42)
Result<int, string> err = none.OkOrElse(() => "not found");  // Err("not found")

// Lazy error: build a rich error object only on the None path
Result<User, AppError> result = FindUserById(id)
    .OkOrElse(() => new AppError(404, $"User {id} does not exist"));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
