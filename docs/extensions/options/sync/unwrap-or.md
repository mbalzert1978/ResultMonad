# UnwrapOr

> Extract the Some value or return an eagerly-evaluated fallback for None.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `UnwrapOrExtension`

## Signature

```csharp
public T UnwrapOr(T fallback);
```

## Description

`UnwrapOr` returns the wrapped value when this option is Some. When this option is None, it returns `fallback` directly. Because `fallback` is passed as a value argument it is always evaluated, even when the option is Some. Use `UnwrapOrElse` when computing the fallback is expensive and should only happen on the None path. This method is typically used at the end of a pipeline to collapse an `Option<T>` into a plain `T`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `T` | Value returned when this option is None. Eagerly evaluated. |

## Returns

The wrapped value when this is Some; `fallback` when this is None.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `fallback` is null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int> some = Some(42);
Option<int> none = None<int>();

int fromSome = some.UnwrapOr(0);  // 42
int fromNone = none.UnwrapOr(0);  // 0

// Collapse at the end of a pipeline
string userName = GetUser(id)
    .Map(u => u.DisplayName)
    .UnwrapOr("Anonymous");
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
