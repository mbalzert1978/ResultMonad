# OrElseAsync

> Asynchronously recover from an Err by applying a fallible operation to the error value.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `OrElseTaskExtension`, `OrElseValueTaskExtension`

## Overloads

| # | Receiver | Operation | Returns |
|---|----------|-----------|---------|
| 1 | `Task<Result<T,E>>` | synchronous | `Task<Result<T,F>>` |
| 2 | `Result<T,E>` | `Task`-returning | `Task<Result<T,F>>` |
| 3 | `Task<Result<T,E>>` | `Task`-returning | `Task<Result<T,F>>` |
| 4 | `ValueTask<Result<T,E>>` | synchronous | `ValueTask<Result<T,F>>` |
| 5 | `Result<T,E>` | `ValueTask`-returning | `ValueTask<Result<T,F>>` |
| 6 | `ValueTask<Result<T,E>>` | `ValueTask`-returning | `ValueTask<Result<T,F>>` |

## Signatures

```csharp
// Task overloads (OrElseTaskExtension)
public async Task<Result<T, F>> OrElseAsync<F>(this Task<Result<T, E>> result, Func<E, Result<T, F>> operation);          // 1
public async Task<Result<T, F>> OrElseAsync<F>(this Result<T, E> result, Func<E, Task<Result<T, F>>> operation);          // 2
public async Task<Result<T, F>> OrElseAsync<F>(this Task<Result<T, E>> result, Func<E, Task<Result<T, F>>> operation);    // 3

// ValueTask overloads (OrElseValueTaskExtension)
public async ValueTask<Result<T, F>> OrElseAsync<F>(this ValueTask<Result<T, E>> result, Func<E, Result<T, F>> operation);              // 4
public async ValueTask<Result<T, F>> OrElseAsync<F>(this Result<T, E> result, Func<E, ValueTask<Result<T, F>>> operation);              // 5
public async ValueTask<Result<T, F>> OrElseAsync<F>(this ValueTask<Result<T, E>> result, Func<E, ValueTask<Result<T, F>>> operation);   // 6
```

## Description

`OrElseAsync` is the dual of `BindAsync` — it operates on the `Err` path instead of the `Ok` path.

- If the result is `Ok(v)`, it is returned unchanged and `operation` is never called.
- If the result is `Err(e)`, `operation(e)` is invoked and its `Result<T,F>` is returned — which may itself be an `Ok` (recovery) or a new `Err` (re-mapped error type).

Note that the error type changes from `E` to `F`. This lets you convert between error representations across recovery boundaries.

All overloads call `.ConfigureAwait(false)` internally.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync recovery
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("not found"));

Result<int, int> recovered = await errTask.OrElseAsync(
    e => e == "not found"
        ? Ok<int, int>(0)          // recover with default
        : Err<int, int>(e.Length)); // re-map error
// recovered == Ok(0)

// Ok passes through untouched
Task<Result<int, string>> okTask = Task.FromResult(Ok<int, string>(42));
Result<int, int> passThrough = await okTask.OrElseAsync(e => Ok<int, int>(0));
// passThrough == Ok(42)

// Overload 3 — Task<Result> receiver, async recovery (e.g. retry from another source)
Result<int, string> recovered2 = await errTask.OrElseAsync(
    async e =>
    {
        await Task.Delay(1).ConfigureAwait(false);
        return Ok<int, string>(0); // recovered from cache
    });

// Overload 5 — Result receiver, ValueTask-returning recovery (hot path)
Result<int, string> errResult = Err<int, string>("oops");
Result<int, int> recovered3 = await errResult.OrElseAsync(
    async e => { await SomeValueTaskAsync().ConfigureAwait(false); return Ok<int, int>(0); });
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
