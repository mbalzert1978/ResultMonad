# UnwrapOrElseAsync

> Asynchronously extract the Some value or lazily compute a fallback for None.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `UnwrapOrElseTaskExtension`, `UnwrapOrElseValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync fallback
public Task<T> UnwrapOrElseAsync(
    this Task<Option<T>> self,
    Func<T> fallback);

// Task<Option<T>> receiver — async fallback
public Task<T> UnwrapOrElseAsync(
    this Task<Option<T>> self,
    Func<Task<T>> fallback);

// Option<T> receiver — async fallback
public Task<T> UnwrapOrElseAsync(
    this Option<T> self,
    Func<Task<T>> fallback);

// ValueTask variants (same three shapes, returning ValueTask<T>)
public ValueTask<T> UnwrapOrElseAsync(
    this ValueTask<Option<T>> self,
    Func<T> fallback);

public ValueTask<T> UnwrapOrElseAsync(
    this ValueTask<Option<T>> self,
    Func<ValueTask<T>> fallback);

public ValueTask<T> UnwrapOrElseAsync(
    this Option<T> self,
    Func<ValueTask<T>> fallback);
```

## Description

`UnwrapOrElseAsync` is the asynchronous, lazy counterpart to `UnwrapOrAsync`. When the option is Some, the wrapped value is returned without invoking `fallback`. When the option is None, `fallback` is invoked (and awaited if async) and its result is returned. The fallback takes no arguments (`Func<T>` or `Func<Task<T>>`). All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `Func<T>` or async variant | Invoked lazily when this option is None; provides the default value. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `fallback` is null. |
| `InvalidOperationException` | When `fallback` returns null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task<Option<T>> with sync fallback
Task<Option<int>> taskOpt = Task.FromResult(None<int>());
int result = await taskOpt.UnwrapOrElseAsync(() => DateTime.Now.Second);
// e.g. 37

// Option<T> with async fallback
Option<string> cached = None<string>();
string token = await cached.UnwrapOrElseAsync(
    async () => await GenerateTokenAsync().ConfigureAwait(false));

// ValueTask variant — Some passes through without invoking fallback
ValueTask<Option<int>> vtOpt = ValueTask.FromResult(Some(42));
int val = await vtOpt.UnwrapOrElseAsync(
    async () => await ExpensiveComputeAsync().ConfigureAwait(false));
// 42 — fallback never called
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
