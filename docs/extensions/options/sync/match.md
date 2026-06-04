# Match

> Dispatch on Some or None and transform the option into a single value — the primitive all other extensions build on.

**Namespace:** `Monads.Options`
**Class:** `Option<T>` (instance method, not an extension)

## Signature

```csharp
public U Match<U>(Func<T, U> onSome, Func<U> onNone) where U : notnull;
```

## Description

`Match` is the single dispatch primitive for `Option<T>`. It invokes `onSome` with the wrapped value when the option is Some, and `onNone` when it is None, returning the value produced by whichever branch ran. Every other sync extension in this library is implemented exclusively in terms of `Match`. Use it whenever you need to fold an option down to a concrete value without losing exhaustiveness checking.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `onSome` | `Func<T, U>` | Invoked with the wrapped value when this option is Some. |
| `onNone` | `Func<U>` | Invoked with no arguments when this option is None. |

## Returns

The value returned by `onSome(value)` or `onNone()`, depending on which variant this option is.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `onSome` or `onNone` is null. |
| `InvalidOperationException` | When `onSome` or `onNone` returns null (violates the `U : notnull` constraint). |

## Examples

```csharp
using Monads.Options;
using static Monads.Options.Option;

var some = Some(42);
var none = None<int>();

string msg  = some.Match(v => $"has {v}", () => "empty");  // "has 42"
string msg2 = none.Match(v => $"has {v}", () => "empty");  // "empty"

// Use as a pipeline step — Match collapses the Option into an HTTP response body
Option<string> user = Some("alice");
string body = user.Match(name => $"Hello, {name}!", () => "User not found.");
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
