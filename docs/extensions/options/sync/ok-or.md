# OkOr

> Convert an `Option<T>` to a `Result<T, E>`, providing an eagerly-evaluated error for the None case.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `OkOrExtension`

## Signature

```csharp
public Result<T, E> OkOr<E>(E error) where E : notnull;
```

## Description

`OkOr` bridges the `Option` and `Result` worlds. When this option is Some, it returns `Ok(value)`. When this option is None, it returns `Err(error)`. Because `error` is passed as a value argument it is always evaluated, even when the option is Some. Use `OkOrElse` when constructing the error value is expensive and should only happen on the None path.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `error` | `E` | The error value placed into `Err` when this option is None. Eagerly evaluated. |

## Returns

`Ok<T, E>(value)` when this is Some; `Err<T, E>(error)` when this is None.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `error` is null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using Monads.Results;
using static Monads.Options.Option;

Option<int> some = Some(42);
Option<int> none = None<int>();

Result<int, string> ok  = some.OkOr("not found");  // Ok(42)
Result<int, string> err = none.OkOr("not found");  // Err("not found")

// Use at the boundary between Option-oriented lookup and Result-oriented pipeline
Result<User, string> result = FindUserById(id)
    .OkOr("User not found");
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
