# Option&lt;T&gt;

`Option<T>` represents an explicit optional value. It is either `Some(value)` — carrying a non-null `T` — or `None`, indicating absence. It replaces nullable references and `null` checks with a type-safe, composable API modeled on Rust's `Option`.

## Type definition

```
Option<T>              ← readonly record struct, T : notnull
├── Some(value)        ← IsSome = true,  Value = value
└── None               ← IsSome = false, Value = default (null for ref types)
```

`Option<T>` is a `readonly record struct`. Its default value (`default(Option<T>)`) is `None`.

`T` is constrained to `notnull` — the contained value cannot be `null`. This constraint is what makes `IsSome` a reliable signal.

## Namespaces

```csharp
using Monads.Options;                       // Option<T>
using Monads.Options.Extensions.Sync;       // sync extension methods
using Monads.Options.Extensions.Async;      // async extension methods
using static Monads.Options.Option;         // Some<T>(...) and None<T>() shorthands
```

## Construction

Values are created through the static `Option` factory class. The `Option<T>` constructor is internal.

```csharp
using static Monads.Options.Option;

// Some
var some = Some(42);           // type inferred as Option<int>
var some2 = Some("hello");     // Option<string>

// None
var none = None<int>();        // Option<int> in None state
```

Both factory methods validate their argument with `ArgumentNullException.ThrowIfNull` (for `Some`). `None<T>()` returns the default `Option<T>`.

## readonly record struct Option&lt;T&gt;

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsSome` | `bool` | `true` when the option holds a value |
| `IsNone` | `bool` | `true` when the option is empty (`!IsSome`) |
| `Value` | `T?` | The contained value, or `default` (i.e. `null`) when `IsNone` |

`Value` is intentionally nullable because the struct's default state is `None`. Prefer `Match` or `UnwrapOr` over reading `Value` directly.

### Instance method: Match

`Match` is the primary dispatch mechanism. It accepts two delegates and invokes the appropriate one.

```csharp
Option<T> Match<U>(Func<T, U> onSome, Func<U> onNone)
```

```csharp
var some = Some(42);
var none = None<int>();

string msg1 = some.Match(
    onSome: v => $"got {v}",
    onNone: () => "nothing");
// msg1 == "got 42"

string msg2 = none.Match(
    onSome: v => $"got {v}",
    onNone: () => "nothing");
// msg2 == "nothing"
```

Both delegates must return `U`. `Match` returns `null` when either delegate returns `null` (the return type is `U?` internally).

## static class Option — factory

| Method | Signature | Description |
|--------|-----------|-------------|
| `Some` | `Some<T>(T value) → Option<T>` | Creates a Some option; throws `ArgumentNullException` if `value` is null |
| `None` | `None<T>() → Option<T>` | Returns the None state (`default(Option<T>)`) |

## Common patterns

### Safe dictionary lookup

```csharp
using static Monads.Options.Option;

static Option<TValue> TryGet<TKey, TValue>(
    Dictionary<TKey, TValue> dict, TKey key)
    where TKey   : notnull
    where TValue : notnull =>
    dict.TryGetValue(key, out var value) ? Some(value) : None<TValue>();

var prices = new Dictionary<string, decimal>
{
    ["apple"] = 1.20m,
    ["pear"]  = 0.80m,
};

decimal total = TryGet(prices, "apple")
    .Bind(a => TryGet(prices, "pear").Map(p => a + p))
    .UnwrapOr(0m);
// total == 2.00m
```

### Chaining with Map and Bind

```csharp
Option<string> raw = Some("  42  ");

Option<int> parsed = raw
    .Map(s => s.Trim())
    .Bind(s => int.TryParse(s, out int n) ? Some(n) : None<int>())
    .Filter(n => n > 0);

int result = parsed.UnwrapOr(-1);
// result == 42
```

### Bridge to Result

```csharp
using Monads.Options.Extensions.Sync;
using Monads.Results;

Option<int> opt = Some(7);

Result<int, string> result = opt.OkOr("value was missing");
// result is Ok(7)

Result<int, string> result2 = None<int>().OkOr("value was missing");
// result2 is Err("value was missing")
```

### Async pipeline

```csharp
using Monads.Options.Extensions.Async;

Option<string> option = Some("hello");

Option<int> result = await option
    .MapAsync(async s =>
    {
        await Task.Delay(10).ConfigureAwait(false);
        return s.Length;
    })
    .FilterAsync(async n =>
    {
        await Task.Delay(10).ConfigureAwait(false);
        return n > 3;
    });
```

Each async extension has three overloads: `Task<Option>` + sync op, `Option` + async op, `Task<Option>` + async op. A `ValueTask` variant exists for every `Task` variant.

## See also

- [Extension reference — sync](../extensions/options/sync/match.md)
- [Extension reference — async](../extensions/options/async/match.md)
- [Result&lt;T, E&gt;](./result.md) — bridge via `OkOr`, `OkOrElse`
- [Unit](./unit.md) — void-equivalent

---

**Navigation:** [← README](../../README.md) | [↑ Models](./) | [← Result](./result.md) | [Unit →](./unit.md)
