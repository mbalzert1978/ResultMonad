# MapOr

> Transform the Some value or return an eagerly-evaluated fallback for None.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `MapOrExtension`

## Signature

```csharp
public U MapOr<U>(U fallback, Func<T, U> operation) where U : notnull;
```

## Description

`MapOr` applies `operation` to the wrapped value when this option is Some, returning the transformed result. When this option is None, it returns `fallback` directly. Because `fallback` is passed as a value argument it is always evaluated, even when the option is Some. Use `MapOrElse` when the fallback is expensive to compute and should only be evaluated on the None path.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `U` | Value returned when this option is None. Eagerly evaluated. |
| `operation` | `Func<T, U>` | Transformation applied to the Some value. |

## Returns

`operation(value)` when this is Some; `fallback` when this is None.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `fallback` or `operation` is null. |
| `InvalidOperationException` | When `operation` returns null (violates the `U : notnull` constraint). |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int> some = Some(5);
Option<int> none = None<int>();

int fromSome = some.MapOr(0, x => x * 2);  // 10
int fromNone = none.MapOr(0, x => x * 2);  // 0

// Useful for collapsing an option into a display value
string display = Some("alice").MapOr("anonymous", name => name.ToUpper());  // "ALICE"
string guest   = None<string>().MapOr("anonymous", name => name.ToUpper()); // "anonymous"
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
