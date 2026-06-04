# UnwrapOrElse

> Extract the Some value or lazily compute a fallback for None.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `UnwrapOrElseExtension`

## Signature

```csharp
public T UnwrapOrElse(Func<T> fallback);
```

## Description

`UnwrapOrElse` returns the wrapped value when this option is Some. When this option is None, it invokes `fallback()` and returns its result. Because the fallback is a delegate it is only evaluated when needed, making `UnwrapOrElse` the lazy counterpart to `UnwrapOr`. Prefer `UnwrapOrElse` when the default value has a cost or side effect — for example, reading from a database or generating a timestamp.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `Func<T>` | Invoked when this option is None; provides the default value lazily. |

## Returns

The wrapped value when this is Some; the value returned by `fallback()` when this is None.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `fallback` is null. |
| `InvalidOperationException` | When `fallback` returns null (violates the `T : notnull` constraint). |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int> some = Some(42);
Option<int> none = None<int>();

int fromSome = some.UnwrapOrElse(() => DateTime.Now.Second);  // 42
int fromNone = none.UnwrapOrElse(() => DateTime.Now.Second);  // e.g. 37

// Lazy fallback: only computed when needed
string token = GetCachedToken()
    .UnwrapOrElse(() => GenerateNewToken());
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
