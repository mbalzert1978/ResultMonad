# InspectErrAsync

> Asynchronously run a side-effect on the Err value without altering the result.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `InspectErrTaskExtension`, `InspectErrValueTaskExtension`

## Extension blocks

Each class has two extension blocks mirroring the `InspectAsync` pattern:

1. **On `Task<Result<T,E>>`** (or `ValueTask<Result<T,E>>`) — awaits the task, then delegates to the synchronous `InspectErr` extension.
2. **On `Result<T,E>`** — accepts an async side-effect (`Func<E, Task>` or `Func<E, ValueTask>`) and awaits it when the result is `Err`.

## Overloads

| # | Receiver | Action | Returns |
|---|----------|--------|---------|
| 1 | `Task<Result<T,E>>` | `Action<E>` (synchronous) | `Task<Result<T,E>>` |
| 2 | `Result<T,E>` | `Func<E, Task>` (async) | `Task<Result<T,E>>` |
| 3 | `ValueTask<Result<T,E>>` | `Action<E>` (synchronous) | `ValueTask<Result<T,E>>` |
| 4 | `Result<T,E>` | `Func<E, ValueTask>` (async) | `ValueTask<Result<T,E>>` |

## Signatures

```csharp
// InspectErrTaskExtension — block on Task<Result<T,E>>
public async Task<Result<T, E>> InspectErrAsync(this Task<Result<T, E>> result, Action<E> action);      // 1

// InspectErrTaskExtension — block on Result<T,E>
public async Task<Result<T, E>> InspectErrAsync(this Result<T, E> result, Func<E, Task> action);        // 2

// InspectErrValueTaskExtension — block on ValueTask<Result<T,E>>
public async ValueTask<Result<T, E>> InspectErrAsync(this ValueTask<Result<T, E>> result, Action<E> action);    // 3

// InspectErrValueTaskExtension — block on Result<T,E>
public async ValueTask<Result<T, E>> InspectErrAsync(this Result<T, E> result, Func<E, ValueTask> action);     // 4
```

## Description

`InspectErrAsync` is the dual of `InspectAsync` — it triggers a side-effect only when the result is `Err`, and always returns the original result unchanged.

Typical uses: error logging, metrics on failure paths, alerting, or recording diagnostics. The `Ok` path is unaffected and the action is never invoked on `Ok`.

All awaits use `.ConfigureAwait(false)`.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync side-effect
Task<Result<int, string>> errTask = Task.FromResult(Err<int, string>("not found"));

Result<int, string> same = await errTask.InspectErrAsync(
    e => Console.WriteLine($"Error: {e}"));
// Prints "Error: not found"; same == Err("not found")

// Overload 2 — resolved result, async side-effect (e.g. async error logging)
Result<int, string> errResult = Err<int, string>("timeout");
Result<int, string> same2 = await errResult.InspectErrAsync(
    async e =>
    {
        await LogErrorAsync(e).ConfigureAwait(false);
    });
// same2 == Err("timeout")

// Ok — action is never called
Result<int, string> okResult = Ok<int, string>(1);
Result<int, string> same3 = await okResult.InspectErrAsync(
    async e => await LogErrorAsync(e).ConfigureAwait(false));
// same3 == Ok(1), LogErrorAsync was not called

// Overload 3 — ValueTask receiver, sync side-effect (hot path)
ValueTask<Result<int, string>> vtErr = ValueTask.FromResult(Err<int, string>("oops"));
Result<int, string> same4 = await vtErr.InspectErrAsync(
    e => Metrics.Increment("errors"));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
