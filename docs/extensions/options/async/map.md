# MapAsync

> Asynchronously transform the value inside a Some option; pass None through unchanged.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `MapTaskExtension`, `MapValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync operation
public Task<Option<U>> MapAsync<U>(
    this Task<Option<T>> self,
    Func<T, U> operation)
    where U : notnull;

// Task<Option<T>> receiver — async operation
public Task<Option<U>> MapAsync<U>(
    this Task<Option<T>> self,
    Func<T, Task<U>> operation)
    where U : notnull;

// Option<T> receiver — async operation
public Task<Option<U>> MapAsync<U>(
    this Option<T> self,
    Func<T, Task<U>> operation)
    where U : notnull;

// ValueTask variants (same three shapes, returning ValueTask<Option<U>>)
public ValueTask<Option<U>> MapAsync<U>(
    this ValueTask<Option<T>> self,
    Func<T, U> operation)
    where U : notnull;

public ValueTask<Option<U>> MapAsync<U>(
    this ValueTask<Option<T>> self,
    Func<T, ValueTask<U>> operation)
    where U : notnull;

public ValueTask<Option<U>> MapAsync<U>(
    this Option<T> self,
    Func<T, ValueTask<U>> operation)
    where U : notnull;
```

## Description

`MapAsync` is the asynchronous counterpart to `Map`. It awaits the option when the receiver is a task type, then applies `operation` to the wrapped value if Some. When the option is None, `operation` is never invoked. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<T, U>` or async variant | Transformation applied to the Some value. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `operation` is null. |
| `InvalidOperationException` | When `operation` returns null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task<Option<T>> with sync operation
Task<Option<int>> taskOpt = Task.FromResult(Some(5));
Option<int> doubled = await taskOpt.MapAsync(x => x * 2);
// Some(10)

// Option<T> with async operation
Option<string> opt = Some("user-42");
Option<User> user = await opt.MapAsync(
    async id => await FetchUserAsync(id).ConfigureAwait(false));

// ValueTask variant
ValueTask<Option<int>> vtOpt = ValueTask.FromResult(None<int>());
Option<int> result = await vtOpt.MapAsync(x => x * 2);
// None
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
