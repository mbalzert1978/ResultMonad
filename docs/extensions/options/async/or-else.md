# OrElseAsync

> Asynchronously return this option if it is Some; otherwise compute and return an alternative.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `OrElseTaskExtension`, `OrElseValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync operation
public Task<Option<T>> OrElseAsync(
    this Task<Option<T>> self,
    Func<Option<T>> operation);

// Task<Option<T>> receiver — async operation
public Task<Option<T>> OrElseAsync(
    this Task<Option<T>> self,
    Func<Task<Option<T>>> operation);

// Option<T> receiver — async operation
public Task<Option<T>> OrElseAsync(
    this Option<T> self,
    Func<Task<Option<T>>> operation);

// ValueTask variants (same three shapes, returning ValueTask<Option<T>>)
public ValueTask<Option<T>> OrElseAsync(
    this ValueTask<Option<T>> self,
    Func<Option<T>> operation);

public ValueTask<Option<T>> OrElseAsync(
    this ValueTask<Option<T>> self,
    Func<ValueTask<Option<T>>> operation);

public ValueTask<Option<T>> OrElseAsync(
    this Option<T> self,
    Func<ValueTask<Option<T>>> operation);
```

## Description

`OrElseAsync` is the asynchronous counterpart to `OrElse`. When the option is Some, it is returned immediately without invoking `operation`. When the option is None, `operation` is invoked (and awaited if async) and its result is returned. The operation is always lazy — it is never called on the Some path. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<Option<T>>` or async variant | Invoked lazily when this option is None; returns the fallback option. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `operation` is null. |
| `InvalidOperationException` | When `operation` returns a null option reference. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task<Option<T>> with sync fallback
Task<Option<int>> taskOpt = Task.FromResult(None<int>());
Option<int> result = await taskOpt.OrElseAsync(() => Some(0));
// Some(0)

// Option<T> with async fallback (secondary lookup)
Option<string> cached = GetFromCache("key");
Option<string> value = await cached.OrElseAsync(
    async () => await FetchFromDatabaseAsync("key").ConfigureAwait(false));

// ValueTask variant — Some passes through
ValueTask<Option<int>> vtOpt = ValueTask.FromResult(Some(42));
Option<int> kept = await vtOpt.OrElseAsync(() => Some(0));
// Some(42)
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
