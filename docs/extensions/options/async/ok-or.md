# OkOrAsync

> Asynchronously convert a `Task<Option<T>>` or `ValueTask<Option<T>>` to a `Result<T, E>` with an eagerly-evaluated error for None.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `OkOrTaskExtension`, `OkOrValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver
public Task<Result<T, E>> OkOrAsync<E>(
    this Task<Option<T>> self,
    E error)
    where E : notnull;

// ValueTask<Option<T>> receiver
public ValueTask<Result<T, E>> OkOrAsync<E>(
    this ValueTask<Option<T>> self,
    E error)
    where E : notnull;
```

## Description

`OkOrAsync` is the asynchronous counterpart to `OkOr`. It awaits the option and then converts it to a `Result<T, E>`: `Some(x)` becomes `Ok(x)` and `None` becomes `Err(error)`. Because `error` is passed by value it is always evaluated; use `OkOrElseAsync` when constructing the error should only happen on the None path. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `error` | `E` | The error value placed into `Err` when the option is None. Eagerly evaluated. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `error` is null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using Monads.Results;
using static Monads.Options.Option;

// Task<Option<T>> receiver
Task<Option<int>> taskSome = Task.FromResult(Some(42));
Task<Option<int>> taskNone = Task.FromResult(None<int>());

Result<int, string> ok  = await taskSome.OkOrAsync("not found");  // Ok(42)
Result<int, string> err = await taskNone.OkOrAsync("not found");  // Err("not found")

// ValueTask variant — bridge async option lookup to a Result pipeline
ValueTask<Option<User>> vtOpt = FetchUserAsync(id);
Result<User, string> result = await vtOpt.OkOrAsync("User not found");
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
