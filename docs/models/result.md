# Result&lt;T, E&gt;

`Result<T, E>` is the core discriminated union for railway-oriented error handling. It holds either a success value (`Ok`) or a failure value (`Err`), never both, and never neither.

## Type hierarchy

```
Result<T, E>           ← abstract record, T/E : notnull
├── Ok<T, E>           ← internal sealed record (success variant)
└── Err<T, E>          ← internal sealed record (failure variant)
```

Both `T` and `E` are constrained to `notnull` — neither the success value nor the error value can be `null`. This makes every `Result<T, E>` unambiguous without null checks.

`Result<T, E>` has a `private protected` constructor. External subclassing is impossible and a third variant is unrepresentable at compile time.

## Namespaces

```csharp
using Monads.Results;                       // Result<T,E>, Ok<T,E>, Err<T,E>
using Monads.Results.Extensions.Sync;       // sync extension methods
using Monads.Results.Extensions.Async;      // async extension methods
using static Monads.Results.Result;         // Ok<T,E>(...) and Err<T,E>(...) factory shorthands
```

## Construction

Values are created through the static `Result` factory class. Direct instantiation of `Ok<T, E>` or `Err<T, E>` is not possible from outside the library.

```csharp
using static Monads.Results.Result;

// Success
Result<int, string> ok = Ok<int, string>(42);

// Failure
Result<int, string> err = Err<int, string>("something went wrong");
```

The factory methods validate that the provided value is not null and throw `ArgumentNullException` if it is.

## abstract record Result&lt;T, E&gt;

The base type. Carries no public properties — its entire API surface is provided by extension methods, except for equality (inherited from `record`).

| Member | Kind | Description |
|--------|------|-------------|
| (equality) | from `record` | Two results are equal when they are the same variant and their contained values are equal |

## internal sealed record Ok&lt;T, E&gt;

Holds the success value. Not directly constructible; use `Ok<T,E>(value)` from the factory.

```csharp
Result<string, int> result = Ok<string, int>("hello");

// Access the value via Match or any convenience extension
string value = result.Match(
    onOk:  v => v,
    onErr: _ => "default");
// value == "hello"
```

## internal sealed record Err&lt;T, E&gt;

Holds the failure value. Not directly constructible; use `Err<T,E>(error)` from the factory.

```csharp
Result<string, int> result = Err<string, int>(404);

string value = result.Match(
    onOk:  v => v,
    onErr: code => $"error {code}");
// value == "error 404"
```

## static class Result — factory

| Method | Signature | Description |
|--------|-----------|-------------|
| `Ok` | `Ok<T, E>(T value) → Result<T, E>` | Creates a success result |
| `Err` | `Err<T, E>(E error) → Result<T, E>` | Creates a failure result |

Both validate their argument with `ArgumentNullException.ThrowIfNull`.

## Match — the dispatch primitive

`Match` is the only way to examine which variant a result holds. Every other extension method is implemented in terms of it.

```csharp
Result<int, string> result = Ok<int, string>(7);

int outcome = result.Match(
    onOk:  v => v * 2,
    onErr: _ => -1);
// outcome == 14
```

## Common patterns

### Railway chaining

```csharp
using static Monads.Results.Result;

static Result<int, string> ParsePositive(string s) =>
    int.TryParse(s, out int n)
        ? Ok<int, string>(n)
        : Err<int, string>($"'{s}' is not a number");

static Result<int, string> EnsurePositive(int n) =>
    n > 0
        ? Ok<int, string>(n)
        : Err<int, string>($"{n} is not positive");

Result<int, string> result =
    ParsePositive("42")
        .Bind(EnsurePositive)
        .Map(n => n * 100);

string message = result.Match(
    onOk:  v => $"success: {v}",
    onErr: e => $"failure: {e}");
// message == "success: 4200"
```

### Void-returning operations

Use `Result<Unit, E>` when the success case carries no meaningful value:

```csharp
using Monads.Common;

static Result<Unit, string> SaveFile(string path, string content)
{
    try
    {
        File.WriteAllText(path, content);
        return Ok<Unit, string>(Unit.Default);
    }
    catch (IOException ex)
    {
        return Err<Unit, string>(ex.Message);
    }
}
```

### Async pipelines

```csharp
using Monads.Results.Extensions.Async;

Result<int, string> seed = Ok<int, string>(1);

Result<string, string> final = await seed
    .MapAsync(async n =>
    {
        await Task.Delay(10).ConfigureAwait(false);
        return n + 41;
    })
    .BindAsync(async n =>
    {
        await Task.Delay(10).ConfigureAwait(false);
        return n > 0
            ? Ok<string, string>(n.ToString())
            : Err<string, string>("non-positive");
    });
```

Each async extension has three overloads: `Task<Result>` + sync op, `Result` + async op, `Task<Result>` + async op. A `ValueTask` variant exists for every `Task` variant.

## See also

- [Extension reference — sync](../extensions/results/sync/match.md)
- [Extension reference — async](../extensions/results/async/match.md)
- [Option&lt;T&gt;](./option.md) — bridge via `ToOk`, `ToErr`, `Transpose`
- [Unit](./unit.md) — void-equivalent for `Result<Unit, E>`

---

**Navigation:** [← README](../../README.md) | [↑ Models](./) | [Option →](./option.md)
