# MapErrAsync

> Asynchronously transform the Err value of a result, leaving Ok unchanged.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `MapErrTaskExtension`, `MapErrValueTaskExtension`

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
// Task overloads (MapErrTaskExtension)
public async Task<Result<T, F>> MapErrAsync<F>(this Task<Result<T, E>> result, Func<E, F> operation);          // 1
public async Task<Result<T, F>> MapErrAsync<F>(this Result<T, E> result, Func<E, Task<F>> operation);          // 2
public async Task<Result<T, F>> MapErrAsync<F>(this Task<Result<T, E>> result, Func<E, Task<F>> operation);    // 3

// ValueTask overloads (MapErrValueTaskExtension)
public async ValueTask<Result<T, F>> MapErrAsync<F>(this ValueTask<Result<T, E>> result, Func<E, F> operation);              // 4
public async ValueTask<Result<T, F>> MapErrAsync<F>(this Result<T, E> result, Func<E, ValueTask<F>> operation);              // 5
public async ValueTask<Result<T, F>> MapErrAsync<F>(this ValueTask<Result<T, E>> result, Func<E, ValueTask<F>> operation);   // 6
```

## Description

`MapErrAsync` is the dual of `MapAsync` — it transforms the `Err` value while leaving an `Ok` result untouched and the operation uninvoked.

Common uses include translating a low-level error type into a domain-specific error, or enriching an error with additional context fetched asynchronously (e.g. from a lookup table or remote service).

- **Overloads 1 and 4** await the async receiver and apply a synchronous error mapping.
- **Overloads 2 and 5** work on an already-resolved `Result<T,E>` with an async error mapper.
- **Overloads 3 and 6** combine both an async receiver and an async error mapper.

All overloads call `.ConfigureAwait(false)` internally.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync error mapper
Task<Result<int, string>> taskResult = Task.FromResult(Err<int, string>("not found"));

Result<int, int> mapped = await taskResult.MapErrAsync(e => e.Length);
// mapped == Err(9)

// Ok is passed through without invoking the operation
Task<Result<int, string>> okTask = Task.FromResult(Ok<int, string>(1));
Result<int, int> passThrough = await okTask.MapErrAsync(e => e.Length);
// passThrough == Ok(1)

// Overload 3 — Task<Result> receiver, async error mapper (e.g. look up error details)
Result<int, ErrorDetail> mapped2 = await taskResult.MapErrAsync(
    async e =>
    {
        await Task.Delay(1).ConfigureAwait(false);
        return new ErrorDetail(Code: 404, Message: e);
    });

// Overload 5 — Result receiver, ValueTask-returning mapper (hot path)
Result<int, string> errResult = Err<int, string>("oops");
Result<int, int> mapped3 = await errResult.MapErrAsync(
    async e => { await SomeValueTaskAsync().ConfigureAwait(false); return e.Length; });
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
