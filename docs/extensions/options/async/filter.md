# FilterAsync

> Asynchronously keep the Some value only when it satisfies a predicate; turn it into None otherwise.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `FilterTaskExtension`, `FilterValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync predicate
public Task<Option<T>> FilterAsync(
    this Task<Option<T>> self,
    Func<T, bool> predicate);

// Task<Option<T>> receiver — async predicate
public Task<Option<T>> FilterAsync(
    this Task<Option<T>> self,
    Func<T, Task<bool>> predicate);

// Option<T> receiver — async predicate
public Task<Option<T>> FilterAsync(
    this Option<T> self,
    Func<T, Task<bool>> predicate);

// ValueTask variants (same three shapes, returning ValueTask<Option<T>>)
public ValueTask<Option<T>> FilterAsync(
    this ValueTask<Option<T>> self,
    Func<T, bool> predicate);

public ValueTask<Option<T>> FilterAsync(
    this ValueTask<Option<T>> self,
    Func<T, ValueTask<bool>> predicate);

public ValueTask<Option<T>> FilterAsync(
    this Option<T> self,
    Func<T, ValueTask<bool>> predicate);
```

## Description

`FilterAsync` is the asynchronous counterpart to `Filter`. It awaits the option when the receiver is a task type, then tests the wrapped value against `predicate` if Some. When the predicate returns `true`, the original Some option is returned. When the predicate returns `false`, or when the option is None, `None` is returned. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `predicate` | `Func<T, bool>` or async variant | Test applied to the Some value. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `predicate` is null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task<Option<T>> with sync predicate
Task<Option<int>> taskOpt = Task.FromResult(Some(42));
Option<int> kept = await taskOpt.FilterAsync(x => x > 10);
// Some(42)

// Option<T> with async predicate (e.g., database existence check)
Option<int> userId = Some(7);
Option<int> active = await userId.FilterAsync(
    async id => await IsUserActiveAsync(id).ConfigureAwait(false));

// ValueTask variant
ValueTask<Option<int>> vtOpt = ValueTask.FromResult(Some(3));
Option<int> result = await vtOpt.FilterAsync(x => x > 10);
// None
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
