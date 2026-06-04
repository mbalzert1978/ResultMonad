# MapOrAsync

> Asynchronously transform the Some value or return an eagerly-evaluated fallback for None.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `MapOrTaskExtension`, `MapOrValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync operation
public Task<U> MapOrAsync<U>(
    this Task<Option<T>> self,
    U fallback,
    Func<T, U> operation)
    where U : notnull;

// Task<Option<T>> receiver — async operation
public Task<U> MapOrAsync<U>(
    this Task<Option<T>> self,
    U fallback,
    Func<T, Task<U>> operation)
    where U : notnull;

// Option<T> receiver — async operation
public Task<U> MapOrAsync<U>(
    this Option<T> self,
    U fallback,
    Func<T, Task<U>> operation)
    where U : notnull;

// ValueTask variants (same three shapes, returning ValueTask<U>)
public ValueTask<U> MapOrAsync<U>(
    this ValueTask<Option<T>> self,
    U fallback,
    Func<T, U> operation)
    where U : notnull;

public ValueTask<U> MapOrAsync<U>(
    this ValueTask<Option<T>> self,
    U fallback,
    Func<T, ValueTask<U>> operation)
    where U : notnull;

public ValueTask<U> MapOrAsync<U>(
    this Option<T> self,
    U fallback,
    Func<T, ValueTask<U>> operation)
    where U : notnull;
```

## Description

`MapOrAsync` is the asynchronous counterpart to `MapOr`. It awaits the option when the receiver is a task type, applies `operation` to the Some value, and returns the result. When the option is None it returns `fallback` without invoking `operation`. Because `fallback` is passed by value it is always evaluated; use `MapOrElseAsync` for a lazy fallback. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `U` | Value returned when the option is None. Eagerly evaluated. |
| `operation` | `Func<T, U>` or async variant | Transformation applied to the Some value. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `fallback` or `operation` is null. |
| `InvalidOperationException` | When `operation` returns null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task<Option<T>> with sync operation
Task<Option<int>> taskOpt = Task.FromResult(Some(5));
int result = await taskOpt.MapOrAsync(0, x => x * 2);
// 10

// Option<T> with async operation
Option<string> opt = None<string>();
string display = await opt.MapOrAsync(
    "unknown",
    async name => await FormatNameAsync(name).ConfigureAwait(false));
// "unknown"

// ValueTask variant
ValueTask<Option<int>> vtOpt = ValueTask.FromResult(Some(3));
int val = await vtOpt.MapOrAsync(0, x => x + 1);
// 4
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
