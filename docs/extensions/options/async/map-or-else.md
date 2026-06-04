# MapOrElseAsync

> Asynchronously transform the Some value or lazily compute a fallback for None.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `MapOrElseTaskExtension`, `MapOrElseValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync callbacks
public Task<U> MapOrElseAsync<U>(
    this Task<Option<T>> self,
    Func<U> fallback,
    Func<T, U> operation)
    where U : notnull;

// Task<Option<T>> receiver — async callbacks
public Task<U> MapOrElseAsync<U>(
    this Task<Option<T>> self,
    Func<Task<U>> fallback,
    Func<T, Task<U>> operation)
    where U : notnull;

// Option<T> receiver — async callbacks
public Task<U> MapOrElseAsync<U>(
    this Option<T> self,
    Func<Task<U>> fallback,
    Func<T, Task<U>> operation)
    where U : notnull;

// ValueTask variants (same three shapes, returning ValueTask<U>)
public ValueTask<U> MapOrElseAsync<U>(
    this ValueTask<Option<T>> self,
    Func<U> fallback,
    Func<T, U> operation)
    where U : notnull;

public ValueTask<U> MapOrElseAsync<U>(
    this ValueTask<Option<T>> self,
    Func<ValueTask<U>> fallback,
    Func<T, ValueTask<U>> operation)
    where U : notnull;

public ValueTask<U> MapOrElseAsync<U>(
    this Option<T> self,
    Func<ValueTask<U>> fallback,
    Func<T, ValueTask<U>> operation)
    where U : notnull;
```

## Description

`MapOrElseAsync` is the asynchronous, lazy counterpart to `MapOrAsync`. When the option is Some, `operation` is invoked and awaited. When the option is None, `fallback` is invoked and awaited instead. The fallback takes no arguments (`Func<U>` or `Func<Task<U>>`), since there is no value to pass on the None path. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `Func<U>` or async variant | Invoked lazily when the option is None; no arguments. |
| `operation` | `Func<T, U>` or async variant | Transformation applied to the Some value. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `fallback` or `operation` is null. |
| `InvalidOperationException` | When `fallback` or `operation` returns null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task<Option<T>> with sync callbacks
Task<Option<int>> taskOpt = Task.FromResult(None<int>());
int result = await taskOpt.MapOrElseAsync(
    () => -1,
    x  => x * 2);
// -1

// Option<T> with async callbacks
Option<string> opt = Some("alice");
string display = await opt.MapOrElseAsync(
    async () => { await Task.Delay(1).ConfigureAwait(false); return "unknown"; },
    async name => { await Task.Delay(1).ConfigureAwait(false); return name.ToUpper(); });
// "ALICE"

// ValueTask variant
ValueTask<Option<int>> vtOpt = ValueTask.FromResult(Some(5));
int val = await vtOpt.MapOrElseAsync(
    () => 0,
    x  => x + 10);
// 15
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
