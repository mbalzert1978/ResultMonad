# Filter

> Keep the Some value only when it satisfies a predicate; turn it into None otherwise.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `FilterExtension`

## Signature

```csharp
public Option<T> Filter(Func<T, bool> predicate);
```

## Description

`Filter` tests the wrapped value against `predicate` when this option is Some. If the predicate returns `true`, the original Some option is returned unchanged. If the predicate returns `false`, `None` is returned. When this option is already None, the predicate is never invoked and `None` is returned directly. Use `Filter` to conditionally discard values without breaking a pipeline chain.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `predicate` | `Func<T, bool>` | Test applied to the Some value. |

## Returns

The original `Option<T>` when this is Some and the predicate returns `true`; `None<T>()` otherwise.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `predicate` is null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int> some = Some(42);
Option<int> none = None<int>();

Option<int> kept    = some.Filter(x => x > 10);  // Some(42)
Option<int> dropped = some.Filter(x => x > 100); // None
Option<int> fromNone = none.Filter(x => x > 10); // None

// Use in a pipeline to reject invalid values early
Option<string> name = Some("  ")
    .Filter(s => !string.IsNullOrWhiteSpace(s));  // None
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
