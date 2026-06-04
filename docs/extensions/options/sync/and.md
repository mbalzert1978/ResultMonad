# And

> Return a second option when this one is Some; return None when this one is None.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `AndExtension`

## Signature

```csharp
public Option<U> And<U>(Option<U> other) where U : notnull;
```

## Description

`And` sequences two options: when this option is Some, it discards the wrapped value and returns `other`. When this option is None, it ignores `other` entirely and returns `None<U>()`. Note that `other` is always evaluated because it is passed by value — use `Bind` when the second option should only be computed on demand. `And` is useful when you care whether both options are Some but only need the value from the second.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `other` | `Option<U>` | The option returned when this option is Some. Always evaluated. |

## Returns

`other` when this is Some; `None<U>()` when this is None.

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int>    some = Some(42);
Option<int>    none = None<int>();
Option<string> next = Some("hello");

Option<string> fromSome = some.And(next);          // Some("hello")
Option<string> fromNone = none.And(next);          // None
Option<string> bothNone = none.And(None<string>()); // None

// Gate a second lookup on the success of a first
Option<Config> config = HasFeatureFlag("my-feature")
    .And(LoadConfig("my-feature.json"));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
