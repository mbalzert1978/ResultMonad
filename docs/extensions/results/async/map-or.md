# MapOrAsync

> Asynchronously map the Ok value, or return a provided fallback for Err.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `MapOrTaskExtension`, `MapOrValueTaskExtension`

## Overloads

| # | Receiver | Operation | Returns |
|---|----------|-----------|---------|
| 1 | `Task<Result<T,E>>` | synchronous | `Task<U>` |
| 2 | `Result<T,E>` | `Task`-returning | `Task<U>` |
| 3 | `Task<Result<T,E>>` | `Task`-returning | `Task<U>` |
| 4 | `ValueTask<Result<T,E>>` | synchronous | `ValueTask<U>` |
| 5 | `Result<T,E>` | `ValueTask`-returning | `ValueTask<U>` |
| 6 | `ValueTask<Result<T,E>>` | `ValueTask`-returning | `ValueTask<U>` |

## Signatures

```csharp
// Task overloads (MapOrTaskExtension)
public async Task<U> MapOrAsync<U>(this Task<Result<T, E>> result, U fallback, Func<T, U> operation);          // 1
public async Task<U> MapOrAsync<U>(this Result<T, E> result, U fallback, Func<T, Task<U>> operation);          // 2
public async Task<U> MapOrAsync<U>(this Task<Result<T, E>> result, U fallback, Func<T, Task<U>> operation);    // 3

// ValueTask overloads (MapOrValueTaskExtension)
public async ValueTask<U> MapOrAsync<U>(this ValueTask<Result<T, E>> result, U fallback, Func<T, U> operation);              // 4
public async ValueTask<U> MapOrAsync<U>(this Result<T, E> result, U fallback, Func<T, ValueTask<U>> operation);              // 5
public async ValueTask<U> MapOrAsync<U>(this ValueTask<Result<T, E>> result, U fallback, Func<T, ValueTask<U>> operation);   // 6
```

## Description

`MapOrAsync` unwraps the result and either projects the `Ok` value through `operation` or returns the eagerly-evaluated `fallback` constant for `Err`.

Because `fallback` is always evaluated (it is a regular parameter, not a lambda), prefer `MapOrElseAsync` when computing the fallback is expensive.

- **Overloads 1 and 4** await the async receiver and apply a synchronous projection on `Ok`.
- **Overloads 2 and 5** accept a resolved `Result<T,E>` and an async projection.
- **Overloads 3 and 6** combine an async receiver with an async projection.

All overloads call `.ConfigureAwait(false)` internally.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync projection
Task<Result<int, string>> okTask = Task.FromResult(Ok<int, string>(7));

string value = await okTask.MapOrAsync("default", v => v.ToString());
// value == "7"

// Err returns the fallback
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("oops"));
string fallback = await errTask.MapOrAsync("default", v => v.ToString());
// fallback == "default"

// Overload 3 — Task<Result> receiver, async projection
string value2 = await okTask.MapOrAsync(
    "default",
    async v => { await Task.Delay(1).ConfigureAwait(false); return v.ToString(); });

// Overload 4 — ValueTask<Result> receiver, sync projection (hot path)
ValueTask<Result<int, string>> vtResult = ValueTask.FromResult(Ok<int, string>(3));
string value3 = await vtResult.MapOrAsync("default", v => v.ToString());
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
