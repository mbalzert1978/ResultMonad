# UnwrapOrAsync

> Await a result and extract the Ok value, or return a provided fallback for Err.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `UnwrapOrTaskExtension`, `UnwrapOrValueTaskExtension`

## Overloads

| # | Receiver | Returns |
|---|----------|---------|
| 1 | `Task<Result<T,E>>` | `Task<T>` |
| 2 | `ValueTask<Result<T,E>>` | `ValueTask<T>` |

## Signatures

```csharp
// UnwrapOrTaskExtension
public async Task<T> UnwrapOrAsync(this Task<Result<T, E>> result, T fallback);          // 1

// UnwrapOrValueTaskExtension
public async ValueTask<T> UnwrapOrAsync(this ValueTask<Result<T, E>> result, T fallback); // 2
```

## Description

`UnwrapOrAsync` awaits the incoming `Task`/`ValueTask<Result<T,E>>` and returns the contained `Ok` value, or `fallback` if the result is `Err`. The fallback is always eagerly evaluated (it is a regular parameter, not a lambda). Use `UnwrapOrElseAsync` when computing the fallback from the error value is needed or when evaluation is expensive.

This method collapses the `Result` wrapper and exits the railway — no further result-chaining is possible after this call.

All awaits use `.ConfigureAwait(false)`.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver
Task<Result<int, string>> okTask  = Task.FromResult(Ok<int, string>(42));
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("oops"));

int value   = await okTask.UnwrapOrAsync(0);   // 42
int fallback = await errTask.UnwrapOrAsync(0); // 0

// Overload 2 — ValueTask<Result> receiver (hot path)
ValueTask<Result<int, string>> vtOk  = ValueTask.FromResult(Ok<int, string>(7));
ValueTask<Result<int, string>> vtErr = ValueTask.FromResult(Err<int, string>("gone"));

int value2   = await vtOk.UnwrapOrAsync(-1);  // 7
int fallback2 = await vtErr.UnwrapOrAsync(-1); // -1
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
