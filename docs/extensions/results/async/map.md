# MapAsync

> Asynchronously transform the Ok value of a result, leaving Err unchanged.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `MapTaskExtension`, `MapValueTaskExtension`

## Overloads

| # | Receiver | Operation | Returns |
|---|----------|-----------|---------|
| 1 | `Task<Result<T,E>>` | synchronous | `Task<Result<U,E>>` |
| 2 | `Result<T,E>` | `Task`-returning | `Task<Result<U,E>>` |
| 3 | `Task<Result<T,E>>` | `Task`-returning | `Task<Result<U,E>>` |
| 4 | `ValueTask<Result<T,E>>` | synchronous | `ValueTask<Result<U,E>>` |
| 5 | `Result<T,E>` | `ValueTask`-returning | `ValueTask<Result<U,E>>` |
| 6 | `ValueTask<Result<T,E>>` | `ValueTask`-returning | `ValueTask<Result<U,E>>` |

## Signatures

```csharp
// Task overloads (MapTaskExtension)
public async Task<Result<U, E>> MapAsync<U>(this Task<Result<T, E>> result, Func<T, U> operation);          // 1
public async Task<Result<U, E>> MapAsync<U>(this Result<T, E> result, Func<T, Task<U>> operation);          // 2
public async Task<Result<U, E>> MapAsync<U>(this Task<Result<T, E>> result, Func<T, Task<U>> operation);    // 3

// ValueTask overloads (MapValueTaskExtension)
public async ValueTask<Result<U, E>> MapAsync<U>(this ValueTask<Result<T, E>> result, Func<T, U> operation);              // 4
public async ValueTask<Result<U, E>> MapAsync<U>(this Result<T, E> result, Func<T, ValueTask<U>> operation);              // 5
public async ValueTask<Result<U, E>> MapAsync<U>(this ValueTask<Result<T, E>> result, Func<T, ValueTask<U>> operation);   // 6
```

## Description

`MapAsync` applies a projection to the `Ok` value of a result. If the result is `Err`, the error is propagated unchanged and the operation is never invoked.

- **Overloads 1 and 4** await the incoming `Task`/`ValueTask<Result>` and apply a synchronous projection. Use these when the transformation is CPU-bound.
- **Overloads 2 and 5** take a synchronous `Result<T,E>` but use an async projection. Useful when you have an already-resolved result and need to call an async API (e.g. enriching a value from a database) only on the Ok path.
- **Overloads 3 and 6** combine an async receiver with an async projection for end-to-end async pipelines.

All overloads call `.ConfigureAwait(false)` internally.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync projection
Task<Result<int, string>> taskResult = Task.FromResult(Ok<int, string>(10));

Result<string, string> mapped = await taskResult.MapAsync(v => v.ToString());
// mapped == Ok("10")

// Overload 3 — Task<Result> receiver, async projection (e.g. remote call)
Result<string, string> mapped2 = await taskResult.MapAsync(
    async v =>
    {
        await Task.Delay(1).ConfigureAwait(false);
        return v.ToString();
    });

// Err is passed through without invoking the operation
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("oops"));
Result<string, string> passThrough = await errTask.MapAsync(v => v.ToString());
// passThrough == Err("oops")

// Overload 5 — Result receiver, ValueTask-returning projection (hot path)
Result<int, string> result = Ok<int, string>(5);
Result<string, string> mapped3 = await result.MapAsync(
    async v => { await SomeValueTaskAsync().ConfigureAwait(false); return v.ToString(); });
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
