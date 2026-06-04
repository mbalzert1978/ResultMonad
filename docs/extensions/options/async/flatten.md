# FlattenAsync

> Asynchronously collapse a nested `Task<Option<Option<T>>>` or `ValueTask<Option<Option<T>>>` into a single `Option<T>`.

**Namespace:** `Monads.Options.Extensions.Async`
**Class:** `FlattenAsyncExtension`

## Signatures

```csharp
// Task<Option<Option<T>>> receiver
public Task<Option<T>> FlattenAsync(
    this Task<Option<Option<T>>> self);

// ValueTask<Option<Option<T>>> receiver
public ValueTask<Option<T>> FlattenAsync(
    this ValueTask<Option<Option<T>>> self);
```

## Description

`FlattenAsync` is the asynchronous counterpart to `Flatten`. It awaits the nested option and then removes one layer of `Option` nesting, following the same three-case rule as the sync version: `Some(Some(x))` → `Some(x)`, `Some(None)` → `None`, outer `None` → `None`. Use it when an async pipeline step produces a doubly-wrapped option. All awaits use `.ConfigureAwait(false)`.

## Returns

| Input (after await) | Output |
|---------------------|--------|
| `Some(Some(x))` | `Some(x)` |
| `Some(None<T>())` | `None<T>()` |
| `None<Option<T>>()` | `None<T>()` |

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Async;
using static Monads.Options.Option;

// Task variant
Task<Option<Option<int>>> nested =
    Task.FromResult(Some(Some(42)));

Option<int> flat = await nested.FlattenAsync();
// Some(42)

// Useful when MapAsync produces a nested option
Task<Option<Option<User>>> doubleWrapped =
    Task.FromResult(Some(None<User>()));

Option<User> result = await doubleWrapped.FlattenAsync();
// None

// ValueTask variant
ValueTask<Option<Option<int>>> vtNested =
    ValueTask.FromResult(Some(Some(7)));

Option<int> vtFlat = await vtNested.FlattenAsync();
// Some(7)
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Async](./) | [Options Sync](../sync/) | [Results](../../results/)
