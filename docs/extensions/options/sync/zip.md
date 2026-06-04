# Zip

> Pair two Some values into a tuple option; return None if either option is None.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `ZipExtension`

## Signature

```csharp
public Option<(T, U)> Zip<U>(Option<U> other) where U : notnull;
```

## Description

`Zip` combines two options into a single option containing a value tuple. When both options are Some, the result is `Some((thisValue, otherValue))`. When either option is None, the result is `None`. No values are discarded if both are present — `Zip` preserves both wrapped values, which distinguishes it from `And` (which discards the first). Use `Zip` when you need to pair two independently computed optional values before processing them together.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `other` | `Option<U>` | The option whose value is paired as the second element of the tuple. |

## Returns

`Some((value, otherValue))` when both are Some; `None<(T, U)>()` when either is None.

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int>    num  = Some(1);
Option<string> str  = Some("a");
Option<string> none = None<string>();

Option<(int, string)> paired   = num.Zip(str);   // Some((1, "a"))
Option<(int, string)> missing  = num.Zip(none);  // None

// Deconstruct the tuple after zipping
Option<string> display = num.Zip(str)
    .Map(((int n, string s) t) => $"{t.n}:{t.s}");  // Some("1:a")

// Combine two independent lookups
Option<(User, Profile)> userWithProfile =
    FindUser(id).Zip(FindProfile(id));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
