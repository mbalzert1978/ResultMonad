# UnwrapOrElseAsync

> Await a result and extract the Ok value, or compute a fallback from the Err value.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `UnwrapOrElseTaskExtension`, `UnwrapOrElseValueTaskExtension`

## Extension blocks

Each class has two extension blocks:

1. **On `Task<Result<T,E>>`** (or `ValueTask<Result<T,E>>`) — sync fallback: awaits the task, then applies a synchronous `Func<E, T>`.
2. **On `Result<T,E>`** — async fallback: accepts an already-resolved result and a `Func<E, Task<T>>` (or `Func<E, ValueTask<T>>`), awaiting it on the `Err` path.

## Overloads

| # | Receiver | Fallback | Returns |
|---|----------|----------|---------|
| 1 | `Task<Result<T,E>>` | `Func<E, T>` (synchronous) | `Task<T>` |
| 2 | `Result<T,E>` | `Func<E, Task<T>>` (async) | `Task<T>` |
| 3 | `ValueTask<Result<T,E>>` | `Func<E, T>` (synchronous) | `ValueTask<T>` |
| 4 | `Result<T,E>` | `Func<E, ValueTask<T>>` (async) | `ValueTask<T>` |

## Signatures

```csharp
// UnwrapOrElseTaskExtension — block on Task<Result<T,E>>
public async Task<T> UnwrapOrElseAsync(this Task<Result<T, E>> result, Func<E, T> fallback);          // 1

// UnwrapOrElseTaskExtension — block on Result<T,E>
public async Task<T> UnwrapOrElseAsync(this Result<T, E> result, Func<E, Task<T>> fallback);          // 2

// UnwrapOrElseValueTaskExtension — block on ValueTask<Result<T,E>>
public async ValueTask<T> UnwrapOrElseAsync(this ValueTask<Result<T, E>> result, Func<E, T> fallback); // 3

// UnwrapOrElseValueTaskExtension — block on Result<T,E>
public async ValueTask<T> UnwrapOrElseAsync(this Result<T, E> result, Func<E, ValueTask<T>> fallback); // 4
```

## Description

`UnwrapOrElseAsync` is like `UnwrapOrAsync` but the fallback is a lazily-evaluated function that receives the `Err` value. The fallback is only invoked on `Err` — on `Ok`, the contained value is returned directly.

Use this over `UnwrapOrAsync` when:
- The fallback value depends on the error (e.g. a default derived from the error message).
- Computing the fallback is expensive or has side effects.
- The fallback involves an async operation such as a cache lookup or a remote call.

This method collapses the `Result` wrapper and exits the railway — no further result-chaining is possible after this call.

All awaits use `.ConfigureAwait(false)`.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync fallback
Task<Result<int, string>> okTask  = Task.FromResult(Ok<int, string>(42));
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("not found"));

int value    = await okTask.UnwrapOrElseAsync(e => -1);    // 42
int computed = await errTask.UnwrapOrElseAsync(e => e.Length); // 9

// Overload 2 — resolved result, async fallback (e.g. async cache read on error)
Result<int, string> errResult = Err<int, string>("missing");
int fromCache = await errResult.UnwrapOrElseAsync(
    async e =>
    {
        await Task.Delay(1).ConfigureAwait(false);
        return 0; // returned from cache
    });

// Overload 3 — ValueTask<Result> receiver, sync fallback (hot path)
ValueTask<Result<int, string>> vtErr = ValueTask.FromResult(Err<int, string>("oops"));
int fallback = await vtErr.UnwrapOrElseAsync(e => e.Length); // 4

// Overload 4 — resolved result, ValueTask-returning fallback
int fromCache2 = await errResult.UnwrapOrElseAsync(
    async e => { await SomeValueTaskAsync().ConfigureAwait(false); return 0; });
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
