# MatchAsync

> Dispatch on Some or None asynchronously and transform the option into a single value.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `MatchTaskExtension`, `MatchValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync callbacks
public Task<U> MatchAsync<U>(
    this Task<Option<T>> self,
    Func<T, U> onSome,
    Func<U> onNone)
    where U : notnull;

// Task<Option<T>> receiver — async callbacks
public Task<U> MatchAsync<U>(
    this Task<Option<T>> self,
    Func<T, Task<U>> onSome,
    Func<Task<U>> onNone)
    where U : notnull;

// Option<T> receiver — async callbacks
public Task<U> MatchAsync<U>(
    this Option<T> self,
    Func<T, Task<U>> onSome,
    Func<Task<U>> onNone)
    where U : notnull;

// ValueTask variants (same three shapes, returning ValueTask<U>)
public ValueTask<U> MatchAsync<U>(
    this ValueTask<Option<T>> self,
    Func<T, U> onSome,
    Func<U> onNone)
    where U : notnull;

public ValueTask<U> MatchAsync<U>(
    this ValueTask<Option<T>> self,
    Func<T, ValueTask<U>> onSome,
    Func<ValueTask<U>> onNone)
    where U : notnull;

public ValueTask<U> MatchAsync<U>(
    this Option<T> self,
    Func<T, ValueTask<U>> onSome,
    Func<ValueTask<U>> onNone)
    where U : notnull;
```

## Description

`MatchAsync` is the asynchronous counterpart to `Match`. It awaits the option (when the receiver is `Task<Option<T>>` or `ValueTask<Option<T>>`), then dispatches to `onSome` or `onNone`. When the callbacks are async, the selected branch is awaited before the result is returned. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `onSome` | `Func<T, U>` or `Func<T, Task<U>>` / `Func<T, ValueTask<U>>` | Invoked with the wrapped value when this option is Some. |
| `onNone` | `Func<U>` or `Func<Task<U>>` / `Func<ValueTask<U>>` | Invoked when this option is None. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `onSome` or `onNone` is null. |
| `InvalidOperationException` | When the selected branch returns null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task<Option<T>> receiver with sync callbacks
Task<Option<int>> taskOpt = Task.FromResult(Some(42));
string msg = await taskOpt.MatchAsync(
    v  => $"has {v}",
    () => "empty");
// "has 42"

// Option<T> receiver with async callbacks
Option<int> opt = Some(7);
string result = await opt.MatchAsync(
    async v  => { await Task.Delay(1).ConfigureAwait(false); return $"got {v}"; },
    async () => { await Task.Delay(1).ConfigureAwait(false); return "nothing"; });
// "got 7"

// ValueTask variant
ValueTask<Option<int>> vtOpt = ValueTask.FromResult(None<int>());
string empty = await vtOpt.MatchAsync(
    v  => $"has {v}",
    () => "empty");
// "empty"
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
