# Predicates

> Test an option's state and value in a single expression.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `PredicateExtension`

## Signatures

```csharp
public bool IsSomeAnd(Func<T, bool> predicate);
public bool IsNoneOr(Func<T, bool> predicate);
```

> **Note:** `IsSome` and `IsNone` are built-in properties on the `Option<T>` struct itself and do not require an import.

## Description

`IsSomeAnd` returns `true` only when the option is Some **and** the wrapped value satisfies `predicate`. If the option is None, the predicate is never invoked and `false` is returned.

`IsNoneOr` returns `true` when the option is None, **or** when it is Some and the wrapped value satisfies `predicate`. This is equivalent to `!IsSomeAnd(x => !predicate(x))`. Use it as a permissive guard that only rejects a present value that fails the test.

Both methods are useful in guard clauses and assertions where you need to inspect an option's content without extracting the value.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `predicate` | `Func<T, bool>` | Test applied to the Some value. Not invoked when the option is None. |

## Returns

| Method | Returns `true` when |
|--------|---------------------|
| `IsSomeAnd` | Option is Some and predicate returns `true`. |
| `IsNoneOr` | Option is None, or option is Some and predicate returns `true`. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `predicate` is null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

// IsSomeAnd
Some(42).IsSomeAnd(x => x > 10)    // true
Some(5).IsSomeAnd(x => x > 10)     // false
None<int>().IsSomeAnd(x => x > 10) // false

// IsNoneOr
None<int>().IsNoneOr(x => x > 10)  // true
Some(42).IsNoneOr(x => x > 10)     // true
Some(5).IsNoneOr(x => x > 10)      // false

// Guard clause: reject if a value is present but invalid
Option<string> input = Some("  ");
if (input.IsSomeAnd(s => string.IsNullOrWhiteSpace(s)))
{
    throw new ArgumentException("Input must not be blank.");
}
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
