# TransposeAsync

> Await a result containing an option and transpose the two wrappers.

**Namespace:** `Monads.Results.Extensions.Async`  
**Class:** `TransposeAsyncExtension`

## Extension blocks

`TransposeAsyncExtension` has two extension blocks:

1. **On `Task<Result<Option<T>,E>>`** — awaits the task and transposes.
2. **On `ValueTask<Result<Option<T>,E>>`** — awaits the value-task and transposes.

## Overloads

| # | Receiver | Returns |
|---|----------|---------|
| 1 | `Task<Result<Option<T>,E>>` | `Task<Option<Result<T,E>>>` |
| 2 | `ValueTask<Result<Option<T>,E>>` | `ValueTask<Option<Result<T,E>>>` |

## Signatures

```csharp
// TransposeAsyncExtension — block on Task<Result<Option<T>,E>>
public async Task<Option<Result<T, E>>> TransposeAsync<T, E>(
    this Task<Result<Option<T>, E>> result);       // 1

// TransposeAsyncExtension — block on ValueTask<Result<Option<T>,E>>
public async ValueTask<Option<Result<T, E>>> TransposeAsync<T, E>(
    this ValueTask<Result<Option<T>, E>> result);  // 2
```

## Description

`TransposeAsync` swaps the `Result` and `Option` wrappers according to these rules:

| Input | Output |
|-------|--------|
| `Ok(Some(v))` | `Some(Ok(v))` |
| `Ok(None)` | `None` |
| `Err(e)` | `Some(Err(e))` |

This is useful when you have a pipeline that produces a `Result<Option<T>,E>` and the next step expects an `Option<Result<T,E>>`, or when you want to collapse "Ok but no value found" into the `None` case of an outer Option.

All awaits use `.ConfigureAwait(false)`.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;
using static Monads.Options.Option;

// Ok(Some(v)) → Some(Ok(v))
Task<Result<Option<int>, string>> t1 =
    Task.FromResult(Ok<Option<int>, string>(Some(42)));

Option<Result<int, string>> r1 = await t1.TransposeAsync();
// r1 == Some(Ok(42))

// Ok(None) → None
Task<Result<Option<int>, string>> t2 =
    Task.FromResult(Ok<Option<int>, string>(None<int>()));

Option<Result<int, string>> r2 = await t2.TransposeAsync();
// r2 == None

// Err(e) → Some(Err(e))
Task<Result<Option<int>, string>> t3 =
    Task.FromResult(Err<Option<int>, string>("lookup failed"));

Option<Result<int, string>> r3 = await t3.TransposeAsync();
// r3 == Some(Err("lookup failed"))

// ValueTask overload (hot path)
ValueTask<Result<Option<int>, string>> vt =
    ValueTask.FromResult(Ok<Option<int>, string>(Some(7)));

Option<Result<int, string>> r4 = await vt.TransposeAsync();
// r4 == Some(Ok(7))
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
