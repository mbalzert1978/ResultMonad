# MapOrElseAsync

> Asynchronously map the Ok value, or compute a fallback from the Err value.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `MapOrElseTaskExtension`, `MapOrElseValueTaskExtension`

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
// Task overloads (MapOrElseTaskExtension)
public async Task<U> MapOrElseAsync<U>(this Task<Result<T, E>> result, Func<E, U> fallback, Func<T, U> operation);          // 1
public async Task<U> MapOrElseAsync<U>(this Result<T, E> result, Func<E, Task<U>> fallback, Func<T, Task<U>> operation);    // 2
public async Task<U> MapOrElseAsync<U>(this Task<Result<T, E>> result, Func<E, Task<U>> fallback, Func<T, Task<U>> operation); // 3

// ValueTask overloads (MapOrElseValueTaskExtension)
public async ValueTask<U> MapOrElseAsync<U>(this ValueTask<Result<T, E>> result, Func<E, U> fallback, Func<T, U> operation);              // 4
public async ValueTask<U> MapOrElseAsync<U>(this Result<T, E> result, Func<E, ValueTask<U>> fallback, Func<T, ValueTask<U>> operation);   // 5
public async ValueTask<U> MapOrElseAsync<U>(this ValueTask<Result<T, E>> result, Func<E, ValueTask<U>> fallback, Func<T, ValueTask<U>> operation); // 6
```

## Description

`MapOrElseAsync` is like `MapOrAsync` but the fallback is a lazily-evaluated function rather than an eager value. Only one of `fallback` or `operation` is ever invoked:

- `Ok(v)` — calls `operation(v)` and returns the result.
- `Err(e)` — calls `fallback(e)` and returns the result.

Because both branches are functions, this is suitable when computing the fallback value has side effects or is expensive. Use `MapOrAsync` when the fallback is a cheap constant.

All overloads call `.ConfigureAwait(false)` internally.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync branches
Task<Result<int, string>> okTask  = Task.FromResult(Ok<int, string>(42));
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("not found"));

string ok  = await okTask.MapOrElseAsync(e => $"default({e})", v => v.ToString());
// ok == "42"

string err = await errTask.MapOrElseAsync(e => $"default({e})", v => v.ToString());
// err == "default(not found)"

// Overload 3 — Task<Result> receiver, async branches (e.g. async logging on error)
string result = await errTask.MapOrElseAsync(
    fallback:  async e => { await LogErrorAsync(e).ConfigureAwait(false); return "logged"; },
    operation: async v => { await Task.Delay(1).ConfigureAwait(false); return v.ToString(); });

// Overload 4 — ValueTask<Result> receiver, sync branches (hot path)
ValueTask<Result<int, string>> vtResult = ValueTask.FromResult(Ok<int, string>(5));
string hot = await vtResult.MapOrElseAsync(e => e, v => v.ToString());
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
