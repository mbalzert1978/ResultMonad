# MatchAsync

> Asynchronously dispatch on the Ok or Err variant and project to a value of type `U`.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `MatchTaskExtension`, `MatchValueTaskExtension`

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
// Task overloads (MatchTaskExtension)
public async Task<U> MatchAsync<U>(this Task<Result<T, E>> result, Func<T, U> onOk, Func<E, U> onErr);          // 1
public async Task<U> MatchAsync<U>(this Result<T, E> result, Func<T, Task<U>> onOk, Func<E, Task<U>> onErr);    // 2
public async Task<U> MatchAsync<U>(this Task<Result<T, E>> result, Func<T, Task<U>> onOk, Func<E, Task<U>> onErr); // 3

// ValueTask overloads (MatchValueTaskExtension)
public async ValueTask<U> MatchAsync<U>(this ValueTask<Result<T, E>> result, Func<T, U> onOk, Func<E, U> onErr);              // 4
public async ValueTask<U> MatchAsync<U>(this Result<T, E> result, Func<T, ValueTask<U>> onOk, Func<E, ValueTask<U>> onErr);   // 5
public async ValueTask<U> MatchAsync<U>(this ValueTask<Result<T, E>> result, Func<T, ValueTask<U>> onOk, Func<E, ValueTask<U>> onErr); // 6
```

## Description

`MatchAsync` is the async dispatch primitive — it exhaustively handles both `Ok` and `Err` variants and produces a value. All other async extensions are implemented on top of it.

- **Overloads 1 and 4** await the incoming `Task`/`ValueTask` result and then invoke synchronous projection functions. Use these when your `onOk`/`onErr` logic is CPU-bound or returns immediately.
- **Overloads 2 and 5** accept a synchronous `Result<T,E>` but asynchronous projection functions. Use these when you already have an unwrapped result and need to call an async API (e.g. a database query) based on its variant.
- **Overloads 3 and 6** combine an async receiver with async projection functions. This is the most general form and handles end-to-end async pipelines.

Choose `Task` overloads when integrating with `Task`-based APIs; prefer `ValueTask` overloads in hot paths or when the calling API already returns `ValueTask` to avoid unnecessary heap allocation.

All overloads call `.ConfigureAwait(false)` internally.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync handlers
Task<Result<int, string>> taskResult = Task.FromResult(Ok<int, string>(42));

string message = await taskResult.MatchAsync(
    onOk:  v => $"got {v}",
    onErr: e => $"error: {e}");
// message == "got 42"

// Overload 3 — Task<Result> receiver, async handlers
string message2 = await taskResult.MatchAsync(
    onOk:  async v => { await Task.Delay(1).ConfigureAwait(false); return $"async got {v}"; },
    onErr: async e => { await Task.Delay(1).ConfigureAwait(false); return $"async error: {e}"; });

// Overload 4 — ValueTask<Result> receiver, sync handlers
ValueTask<Result<int, string>> vtResult = ValueTask.FromResult(Ok<int, string>(99));

string message3 = await vtResult.MatchAsync(
    onOk:  v => $"vt got {v}",
    onErr: e => $"vt error: {e}");

// Overload 6 — ValueTask<Result> receiver, async handlers (hot-path usage)
string message4 = await vtResult.MatchAsync(
    onOk:  async v => { await SomeValueTaskAsync().ConfigureAwait(false); return $"hot {v}"; },
    onErr: async e => { await SomeValueTaskAsync().ConfigureAwait(false); return $"hot err {e}"; });
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
