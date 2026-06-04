# InspectAsync

> Asynchronously run a side-effect on the Ok value without altering the result.

**Namespace:** `Monads.Results.Extensions.Async`  
**Class:** `InspectTaskExtension`

## Extension blocks

`InspectTaskExtension` has two extension blocks:

1. **On `Task<Result<T,E>>`** — awaits the task, then delegates to the synchronous `Inspect` extension.
2. **On `Result<T,E>`** — accepts an async side-effect (`Func<T, Task>`) and awaits it when the result is `Ok`.

## Overloads

| # | Receiver | Action | Returns |
|---|----------|--------|---------|
| 1 | `Task<Result<T,E>>` | `Action<T>` (synchronous) | `Task<Result<T,E>>` |
| 2 | `Result<T,E>` | `Func<T, Task>` (async) | `Task<Result<T,E>>` |

## Signatures

```csharp
// InspectTaskExtension — block on Task<Result<T,E>>
public async Task<Result<T, E>> InspectAsync(this Task<Result<T, E>> result, Action<T> action);     // 1

// InspectTaskExtension — block on Result<T,E>
public async Task<Result<T, E>> InspectAsync(this Result<T, E> result, Func<T, Task> action);       // 2
```

## Description

`InspectAsync` lets you observe the `Ok` value for side-effects (logging, metrics, auditing) without breaking the pipeline. The original result is always returned unchanged regardless of the variant.

- **Overload 1** awaits the incoming `Task<Result<T,E>>` and then runs a synchronous action if the result is `Ok`. The action receives the unwrapped value.
- **Overload 2** accepts an already-resolved `Result<T,E>` and an async action. If the result is `Ok`, the action is awaited. The resolved result is then returned as a `Task<Result<T,E>>` for further chaining.

If the result is `Err`, neither action is ever invoked.

All awaits use `.ConfigureAwait(false)`.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — await the task, run sync side-effect
Task<Result<int, string>> taskResult = Task.FromResult(Ok<int, string>(42));

Result<int, string> same = await taskResult.InspectAsync(
    v => Console.WriteLine($"Ok value: {v}"));
// Prints "Ok value: 42"; same == Ok(42)

// Overload 2 — resolved result, async side-effect (e.g. async logging)
Result<int, string> result = Ok<int, string>(7);

Result<int, string> same2 = await result.InspectAsync(
    async v =>
    {
        await LogAsync($"processing {v}").ConfigureAwait(false);
    });
// same2 == Ok(7)

// Err — action is never called
Result<int, string> errResult = Err<int, string>("oops");
Result<int, string> same3 = await errResult.InspectAsync(
    async v => await LogAsync($"ok: {v}").ConfigureAwait(false));
// same3 == Err("oops"), LogAsync was not called

// Pipeline usage
Result<int, string> final = await Task.FromResult(Ok<int, string>(1))
    .InspectAsync(v => Console.WriteLine($"step 1: {v}"))
    .MapAsync(v => v + 1);
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
