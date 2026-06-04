# Map

> Transform the value inside a Some option; pass None through unchanged.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `MapExtension`

## Signature

```csharp
public Option<U> Map<U>(Func<T, U> operation) where U : notnull;
```

## Description

`Map` applies `operation` to the wrapped value when this option is Some, returning a new `Option<U>` containing the result. When this option is None, `operation` is never invoked and `None` is returned as-is. Use `Map` when the transformation itself cannot fail; reach for `Bind` when the transformation may return another `Option`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<T, U>` | Transformation applied to the Some value. |

## Returns

`Some(operation(value))` when this is Some; `None<U>()` when this is None.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `operation` is null. |
| `InvalidOperationException` | When `operation` returns null (violates the `U : notnull` constraint). |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int> some = Some(5);
Option<int> none = None<int>();

Option<int>    doubled  = some.Map(x => x * 2);         // Some(10)
Option<string> asString = some.Map(x => x.ToString());  // Some("5")
Option<int>    fromNone = none.Map(x => x * 2);         // None

// Chain multiple transformations
Option<string> result = Some(42)
    .Map(x => x + 8)          // Some(50)
    .Map(x => x.ToString());  // Some("50")
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
