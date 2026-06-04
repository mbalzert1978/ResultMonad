# OkOrElseAsync

> Asynchronously convert a `Task<Option<T>>` or `ValueTask<Option<T>>` to a `Result<T, E>`, lazily computing the error for None.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `OkOrElseTaskExtension`, `OkOrElseValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync error factory
public Task<Result<T, E>> OkOrElseAsync<E>(
    this Task<Option<T>> self,
    Func<E> error)
    where E : notnull;

// Task<Option<T>> receiver — async error factory
public Task<Result<T, E>> OkOrElseAsync<E>(
    this Task<Option<T>> self,
    Func<Task<E>> error)
    where E : notnull;

// Option<T> receiver — async error factory
public Task<Result<T, E>> OkOrElseAsync<E>(
    this Option<T> self,
    Func<Task<E>> error)
    where E : notnull;

// ValueTask variants (same three shapes, returning ValueTask<Result<T, E>>)
public ValueTask<Result<T, E>> OkOrElseAsync<E>(
    this ValueTask<Option<T>> self,
    Func<E> error)
    where E : notnull;

public ValueTask<Result<T, E>> OkOrElseAsync<E>(
    this ValueTask<Option<T>> self,
    Func<ValueTask<E>> error)
    where E : notnull;

public ValueTask<Result<T, E>> OkOrElseAsync<E>(
    this Option<T> self,
    Func<ValueTask<E>> error)
    where E : notnull;
```

## Description

`OkOrElseAsync` is the asynchronous, lazy counterpart to `OkOrAsync`. When the option is Some, it returns `Ok(value)` without invoking `error`. When the option is None, it invokes `error()` (and awaits it if async) and returns `Err` with the produced value. The error factory takes no arguments (`Func<E>` or `Func<Task<E>>`). All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `error` | `Func<E>` or async variant | Invoked lazily when the option is None; provides the error value. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `error` is null. |
| `InvalidOperationException` | When `error` returns null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using Monads.Results;
using static Monads.Options.Option;

// Task<Option<T>> with sync error factory
Task<Option<int>> taskSome = Task.FromResult(Some(42));
Task<Option<int>> taskNone = Task.FromResult(None<int>());

Result<int, string> ok  = await taskSome.OkOrElseAsync(() => "not found");  // Ok(42)
Result<int, string> err = await taskNone.OkOrElseAsync(() => "not found");  // Err("not found")

// Option<T> with async error factory — build rich error only on None path
Option<User> user = None<User>();
Result<User, AppError> result = await user.OkOrElseAsync(
    async () =>
    {
        var msg = await BuildErrorMessageAsync().ConfigureAwait(false);
        return new AppError(404, msg);
    });

// ValueTask variant
ValueTask<Option<string>> vtOpt = ValueTask.FromResult(Some("payload"));
Result<string, string> vtResult = await vtOpt.OkOrElseAsync(() => "missing");
// Ok("payload")
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
