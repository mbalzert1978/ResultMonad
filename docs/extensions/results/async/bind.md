# BindAsync

> Asynchronously chain a fallible operation onto the Ok value of a result.

**Namespace:** `Monads.Results.Extensions.Async`  
**Classes:** `BindTaskExtension`, `BindValueTaskExtension`

## Overloads

| # | Receiver | Operation | Returns |
|---|----------|-----------|---------|
| 1 | `Task<Result<T,E>>` | synchronous | `Task<Result<U,E>>` |
| 2 | `Result<T,E>` | `Task`-returning | `Task<Result<U,E>>` |
| 3 | `Task<Result<T,E>>` | `Task`-returning | `Task<Result<U,E>>` |
| 4 | `ValueTask<Result<T,E>>` | synchronous | `ValueTask<Result<U,E>>` |
| 5 | `Result<T,E>` | `ValueTask`-returning | `ValueTask<Result<U,E>>` |
| 6 | `ValueTask<Result<T,E>>` | `ValueTask`-returning | `ValueTask<Result<U,E>>` |

## Signatures

```csharp
// Task overloads (BindTaskExtension)
public async Task<Result<U, E>> BindAsync<U>(this Task<Result<T, E>> result, Func<T, Result<U, E>> operation);          // 1
public async Task<Result<U, E>> BindAsync<U>(this Result<T, E> result, Func<T, Task<Result<U, E>>> operation);          // 2
public async Task<Result<U, E>> BindAsync<U>(this Task<Result<T, E>> result, Func<T, Task<Result<U, E>>> operation);    // 3

// ValueTask overloads (BindValueTaskExtension)
public async ValueTask<Result<U, E>> BindAsync<U>(this ValueTask<Result<T, E>> result, Func<T, Result<U, E>> operation);              // 4
public async ValueTask<Result<U, E>> BindAsync<U>(this Result<T, E> result, Func<T, ValueTask<Result<U, E>>> operation);              // 5
public async ValueTask<Result<U, E>> BindAsync<U>(this ValueTask<Result<T, E>> result, Func<T, ValueTask<Result<U, E>>> operation);   // 6
```

## Description

`BindAsync` (also known as `flatMap` or monadic bind) sequences a fallible async operation after a result. If the incoming result is `Ok(v)`, the operation is called with `v` and its `Result<U,E>` is returned. If the incoming result is `Err(e)`, the error is propagated and the operation is never called.

This allows railway-oriented pipelines where each step can independently fail:

```
Ok(v) → operation(v) → Ok(u) or Err(e)
Err(e) → Err(e)  (short-circuit, operation skipped)
```

Unlike `MapAsync`, `BindAsync` avoids nested `Result<Result<U,E>,E>` — use it when the operation itself returns a `Result`.

All overloads call `.ConfigureAwait(false)` internally.

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

// Overload 1 — Task<Result> receiver, sync operation
Task<Result<string, string>> taskResult = Task.FromResult(Ok<string, string>("hello"));

Result<int, string> bound = await taskResult.BindAsync(
    s => s.Length > 0
        ? Ok<int, string>(s.Length)
        : Err<int, string>("empty string"));
// bound == Ok(5)

// Chaining multiple async steps (overload 3)
Result<string, string> final = await taskResult
    .BindAsync(async s =>
    {
        await Task.Delay(1).ConfigureAwait(false);
        return Ok<string, string>(s.ToUpper());
    })
    .BindAsync(async s =>
    {
        await Task.Delay(1).ConfigureAwait(false);
        return s.Length < 100
            ? Ok<string, string>(s)
            : Err<string, string>("too long");
    });

// Err short-circuits the chain
Task<Result<string, string>> errTask = Task.FromResult(Err<string, string>("oops"));
Result<int, string> skipped = await errTask.BindAsync(s => Ok<int, string>(s.Length));
// skipped == Err("oops"), operation was never called

// Overload 5 — Result receiver, ValueTask-returning operation (hot path)
Result<string, string> resolved = Ok<string, string>("world");
Result<int, string> bound2 = await resolved.BindAsync(
    async s => { await SomeValueTaskAsync().ConfigureAwait(false); return Ok<int, string>(s.Length); });
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Async](./) | [Results Sync](../sync/) | [Options](../../options/)
