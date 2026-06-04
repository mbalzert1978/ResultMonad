# Bind

> Chain option-returning operations without manual unwrapping — FlatMap for `Option<T>`.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `BindExtension`

## Signature

```csharp
public Option<U> Bind<U>(Func<T, Option<U>> operation) where U : notnull;
```

## Description

`Bind` applies `operation` to the wrapped value when this option is Some, returning whatever `Option<U>` the operation produces. When this option is None, `operation` is never invoked and `None<U>()` is returned directly. Unlike `Map`, the operation itself decides whether to return Some or None, making `Bind` the right choice when a step in a pipeline can legitimately produce no value.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<T, Option<U>>` | Option-returning transformation applied to the Some value. |

## Returns

The `Option<U>` returned by `operation(value)` when this is Some; `None<U>()` when this is None.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `operation` is null. |
| `InvalidOperationException` | When `operation` returns null (the returned option itself must not be null). |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

static Option<int> ParseInt(string s) =>
    int.TryParse(s, out var n) ? Some(n) : None<int>();

static Option<int> Positive(int n) =>
    n > 0 ? Some(n) : None<int>();

Some("42").Bind(ParseInt)     // Some(42)
Some("abc").Bind(ParseInt)    // None
None<string>().Bind(ParseInt) // None

// Chain multiple fallible steps
Option<int> result = Some("7")
    .Bind(ParseInt)    // Some(7)
    .Bind(Positive);   // Some(7)

Option<int> neg = Some("-3")
    .Bind(ParseInt)    // Some(-3)
    .Bind(Positive);   // None
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
