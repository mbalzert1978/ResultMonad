# Flatten

> Collapse a nested `Option<Option<T>>` into a single `Option<T>`.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `FlattenExtension`

## Signature

```csharp
// Receiver: Option<Option<T>>
public Option<T> Flatten();
```

## Description

`Flatten` removes one layer of `Option` nesting. It is the equivalent of calling `Bind(x => x)` and is useful when a composition of operations produces a doubly-wrapped option. The three cases collapse as follows: `Some(Some(x))` → `Some(x)`, `Some(None)` → `None`, and the outer `None` → `None`. Because `Option<T>` is a struct, `Flatten` never throws `ArgumentNullException` for the receiver.

## Returns

| Input | Output |
|-------|--------|
| `Some(Some(x))` | `Some(x)` |
| `Some(None<T>())` | `None<T>()` |
| `None<Option<T>>()` | `None<T>()` |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<Option<int>> someSome = Some(Some(42));
Option<Option<int>> someNone = Some(None<int>());
Option<Option<int>> none     = None<Option<int>>();

Option<int> a = someSome.Flatten();  // Some(42)
Option<int> b = someNone.Flatten();  // None
Option<int> c = none.Flatten();      // None

// Useful when Map produces a nested option
Option<Option<int>> nested = Some("42").Map(s =>
    int.TryParse(s, out var n) ? Some(n) : None<int>());

Option<int> flat = nested.Flatten();  // Some(42)
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
