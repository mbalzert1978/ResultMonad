# UnwrapOrAsync

> Asynchronously extract the Some value or return an eagerly-evaluated fallback for None.

**Namespace:** `Monads.Options.Extensions.Async`
**Classes:** `UnwrapOrTaskExtension`, `UnwrapOrValueTaskExtension`

## Signatures

```csharp
// Task<Option<T>> receiver
public Task<T> UnwrapOrAsync(
    this Task<Option<T>> self,
    T fallback);

// ValueTask<Option<T>> receiver
public ValueTask<T> UnwrapOrAsync(
    this ValueTask<Option<T>> self,
    T fallback);
```

## Description

`UnwrapOrAsync` is the asynchronous counterpart to `UnwrapOr`. It awaits the option and returns the wrapped value when it is Some. When it is None, it returns `fallback` directly. Because `fallback` is a value argument it is always evaluated; use `UnwrapOrElseAsync` when the fallback is expensive and should only be computed on demand. All awaits use `.ConfigureAwait(false)`.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `fallback` | `T` | Value returned when the option is None. Eagerly evaluated. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `fallback` is null. |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task<Option<T>> receiver
Task<Option<int>> taskSome = Task.FromResult(Some(42));
Task<Option<int>> taskNone = Task.FromResult(None<int>());

int fromSome = await taskSome.UnwrapOrAsync(0);  // 42
int fromNone = await taskNone.UnwrapOrAsync(0);  // 0

// ValueTask variant — collapse at the end of an async pipeline
ValueTask<Option<string>> vtOpt = LookupUserNameAsync(id);
string name = await vtOpt.UnwrapOrAsync("Anonymous");
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
