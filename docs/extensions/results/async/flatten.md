# FlattenAsync

> Await a nested result and collapse it into a single `Result<T,E>`.

**Namespace:** `Monads.Results.Extensions.Async`  
**Class:** `FlattenAsyncExtension`

## Overloads

| # | Receiver | Returns |
|---|----------|---------|
| 1 | `Task<Result<Result<T,E>,E>>` | `Task<Result<T,E>>` |

## Signatures

```csharp
// FlattenAsyncExtension
public async Task<Result<T, E>> FlattenAsync<T, E>(this Task<Result<Result<T, E>, E>> result);  // 1
```

## Description

`FlattenAsync` awaits a `Task` that wraps a doubly-nested `Result<Result<T,E>,E>` and collapses it one level:

- `Ok(Ok(v))` → `Ok(v)`
- `Ok(Err(e))` → `Err(e)`
- `Err(e)` → `Err(e)`

Nested results arise naturally when chaining `MapAsync` with an operation that itself returns a `Result` — use `BindAsync` to avoid nesting in the first place, or `FlattenAsync` to collapse an already-nested result.

All awaits use `.ConfigureAwait(false)`.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Ok(Ok(v)) → Ok(v)
Task<Result<Result<int, string>, string>> nested1 =
    Task.FromResult(Ok<Result<int, string>, string>(Ok<int, string>(42)));

Result<int, string> flat1 = await nested1.FlattenAsync();
// flat1 == Ok(42)

// Ok(Err(e)) → Err(e)
Task<Result<Result<int, string>, string>> nested2 =
    Task.FromResult(Ok<Result<int, string>, string>(Err<int, string>("inner error")));

Result<int, string> flat2 = await nested2.FlattenAsync();
// flat2 == Err("inner error")

// Err(e) → Err(e)
Task<Result<Result<int, string>, string>> nested3 =
    Task.FromResult(Err<Result<int, string>, string>("outer error"));

Result<int, string> flat3 = await nested3.FlattenAsync();
// flat3 == Err("outer error")

// Practical: MapAsync that returns a Result produces a nested result, FlattenAsync collapses it
Task<Result<int, string>> source = Task.FromResult(Ok<string, string>("hello"));

Result<int, string> result = await source
    .MapAsync(s => s.Length > 0
        ? Ok<int, string>(s.Length)
        : Err<int, string>("empty"))
    // MapAsync here produces Task<Result<Result<int,string>,string>> — but BindAsync avoids that.
    // FlattenAsync would be used if MapAsync was already called and produced a nested result.
    ;
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
