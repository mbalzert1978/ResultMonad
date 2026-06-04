# OrElse

> Return this option if it is Some; otherwise compute and return an alternative.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `OrElseExtension`

## Signature

```csharp
public Option<T> OrElse(Func<Option<T>> operation);
```

## Description

`OrElse` returns the current option unchanged when it is Some. When it is None, `operation` is invoked and its result is returned instead. The fallback is lazy — `operation` is only called when needed. Use `OrElse` to provide a fallback data source without breaking the pipeline, for example when the primary lookup returns None and a secondary lookup should be tried.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<Option<T>>` | Invoked when this option is None; must return a replacement `Option<T>`. |

## Returns

This option when it is Some; the result of `operation()` when it is None.

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

Option<int> some = Some(42);
Option<int> none = None<int>();

Option<int> fromSome = some.OrElse(() => Some(0));  // Some(42)
Option<int> fromNone = none.OrElse(() => Some(0));  // Some(0)

// Try a secondary lookup when the primary returns None
Option<string> config = GetEnvVar("MY_VAR")
    .OrElse(() => GetConfigFile("my_var"))
    .OrElse(() => Some("default"));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
