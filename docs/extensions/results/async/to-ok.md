# ToOkAsync

> Await a result and convert it to an `Option<T>`, discarding any error.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `ToOkTaskExtension`, `ToOkValueTaskExtension`

## Overloads

| # | Receiver | Returns |
|---|----------|---------|
| 1 | `Task<Result<T,E>>` | `Task<Option<T>>` |
| 2 | `ValueTask<Result<T,E>>` | `ValueTask<Option<T>>` |

## Signatures

```csharp
// ToOkTaskExtension
public async Task<Option<T>> ToOkAsync<T, E>(this Task<Result<T, E>> result);          // 1

// ToOkValueTaskExtension
public async ValueTask<Option<T>> ToOkAsync<T, E>(this ValueTask<Result<T, E>> result); // 2
```

## Description

`ToOkAsync` awaits the incoming `Task`/`ValueTask<Result<T,E>>` and converts it to an `Option<T>`:

- `Ok(v)` → `Some(v)`
- `Err(e)` → `None` (the error is silently discarded)

Use this when you want to continue working with an `Option`-based API after an async result-producing step, and you do not need to preserve the error.

If you need to preserve the error, use `ToErrAsync` on the `Err` path, or keep the `Result` and use `MatchAsync`.

All awaits use `.ConfigureAwait(false)`.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver
Task<Result<int, string>> okTask  = Task.FromResult(Ok<int, string>(42));
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("oops"));

Option<int> some = await okTask.ToOkAsync();   // Some(42)
Option<int> none = await errTask.ToOkAsync();  // None

// Overload 2 — ValueTask<Result> receiver (hot path)
ValueTask<Result<int, string>> vtOk  = ValueTask.FromResult(Ok<int, string>(7));
ValueTask<Result<int, string>> vtErr = ValueTask.FromResult(Err<int, string>("gone"));

Option<int> some2 = await vtOk.ToOkAsync();   // Some(7)
Option<int> none2 = await vtErr.ToOkAsync();  // None
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
