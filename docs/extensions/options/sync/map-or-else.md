# MapOrElse

> Transform the Some value or lazily compute a fallback for None.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `MapOrElseExtension`

## Signature

```csharp
public U MapOrElse<U>(Func<U> fallback, Func<T, U> operation) where U : notnull;
```

## Description

`MapOrElse` applies `operation` to the wrapped value when this option is Some, returning the transformed result. When this option is None, it invokes `fallback()` and returns its value. Because the fallback is a delegate it is only evaluated when needed, making `MapOrElse` the lazy counterpart to `MapOr`. Prefer `MapOrElse` when computing the default value has a cost or side effect.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `Func<U>` | Invoked when this option is None; provides the default value lazily. |
| `operation` | `Func<T, U>` | Transformation applied to the Some value. |

## Returns

`operation(value)` when this is Some; `fallback()` when this is None.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `fallback` or `operation` is null. |
| `InvalidOperationException` | When `fallback` or `operation` returns null (violates the `U : notnull` constraint). |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int> some = Some(5);
Option<int> none = None<int>();

int fromSome = some.MapOrElse(() => -1, x => x * 2);  // 10
int fromNone = none.MapOrElse(() => -1, x => x * 2);  // -1

// Lazy fallback: only computed when needed
string label = None<string>().MapOrElse(
    () => $"default-{DateTime.Now.Ticks}",
    name => name.ToUpper());
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
