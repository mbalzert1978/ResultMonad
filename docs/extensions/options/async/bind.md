# BindAsync

> Asynchronously chain option-returning operations without manual unwrapping.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `BindTaskExtension`, `BindValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver — sync operation
public Task<Option<U>> BindAsync<U>(
    this Task<Option<T>> self,
    Func<T, Option<U>> operation)
    where U : notnull;

// Task<Option<T>> receiver — async operation
public Task<Option<U>> BindAsync<U>(
    this Task<Option<T>> self,
    Func<T, Task<Option<U>>> operation)
    where U : notnull;

// Option<T> receiver — async operation
public Task<Option<U>> BindAsync<U>(
    this Option<T> self,
    Func<T, Task<Option<U>>> operation)
    where U : notnull;

// ValueTask variants (same three shapes, returning ValueTask<Option<U>>)
public ValueTask<Option<U>> BindAsync<U>(
    this ValueTask<Option<T>> self,
    Func<T, Option<U>> operation)
    where U : notnull;

public ValueTask<Option<U>> BindAsync<U>(
    this ValueTask<Option<T>> self,
    Func<T, ValueTask<Option<U>>> operation)
    where U : notnull;

public ValueTask<Option<U>> BindAsync<U>(
    this Option<T> self,
    Func<T, ValueTask<Option<U>>> operation)
    where U : notnull;
```

## Description

`BindAsync` is the asynchronous counterpart to `Bind`. It awaits the option when the receiver is a task type, then applies `operation` to the wrapped value if Some. When the option is None, `operation` is never invoked. The operation itself returns an `Option<U>`, so it can independently produce Some or None. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<T, Option<U>>` or async variant | Option-returning transformation applied to the Some value. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `operation` is null. |
| `InvalidOperationException` | When `operation` returns a null option reference. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

static async Task<Option<User>> FetchUserAsync(int id)
{
    var user = await db.FindAsync(id).ConfigureAwait(false);
    return user is null ? None<User>() : Some(user);
}

// Task<Option<T>> with sync operation
static Option<int> Positive(int n) => n > 0 ? Some(n) : None<int>();

Task<Option<int>> taskOpt = Task.FromResult(Some(5));
Option<int> result = await taskOpt.BindAsync(Positive);
// Some(5)

// Option<T> with async operation
Option<int> userId = Some(42);
Option<User> user = await userId.BindAsync(
    async id => await FetchUserAsync(id).ConfigureAwait(false));

// ValueTask variant
ValueTask<Option<int>> vtOpt = ValueTask.FromResult(Some(-3));
Option<int> neg = await vtOpt.BindAsync(Positive);
// None
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
