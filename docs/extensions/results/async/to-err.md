# ToErrAsync

> Await a result and convert it to an `Option<E>`, discarding any Ok value.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `ToErrTaskExtension`, `ToErrValueTaskExtension`

## Overloads

| # | Receiver | Returns |
|---|----------|---------|
| 1 | `Task<Result<T,E>>` | `Task<Option<E>>` |
| 2 | `ValueTask<Result<T,E>>` | `ValueTask<Option<E>>` |

## Signatures

```csharp
// ToErrTaskExtension
public async Task<Option<E>> ToErrAsync<T, E>(this Task<Result<T, E>> result);          // 1

// ToErrValueTaskExtension
public async ValueTask<Option<E>> ToErrAsync<T, E>(this ValueTask<Result<T, E>> result); // 2
```

## Description

`ToErrAsync` awaits the incoming `Task`/`ValueTask<Result<T,E>>` and converts it to an `Option<E>`:

- `Err(e)` → `Some(e)`
- `Ok(v)` → `None` (the Ok value is silently discarded)

Use this when you want to switch to an `Option`-based error-handling flow after an async result step, and you are only interested in the error case.

If you need to preserve the Ok value, use `ToOkAsync` on the Ok path, or keep the `Result` and use `MatchAsync`.

All awaits use `.ConfigureAwait(false)`.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver
Task<Result<int, string>> okTask  = Task.FromResult(Ok<int, string>(42));
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("not found"));

Option<string> none = await okTask.ToErrAsync();       // None
Option<string> some = await errTask.ToErrAsync();      // Some("not found")

// Overload 2 — ValueTask<Result> receiver (hot path)
ValueTask<Result<int, string>> vtOk  = ValueTask.FromResult(Ok<int, string>(7));
ValueTask<Result<int, string>> vtErr = ValueTask.FromResult(Err<int, string>("gone"));

Option<string> none2 = await vtOk.ToErrAsync();   // None
Option<string> some2 = await vtErr.ToErrAsync();  // Some("gone")
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
