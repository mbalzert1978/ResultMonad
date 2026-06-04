# Monads

Zero-dependency `Result<T, E>` and `Option<T>` for .NET 10 / C# 14, modeled on Rust's `Result` / `Option`.

## What it is

Monads provides two types: `Result<T, E>` for railway-oriented error handling and `Option<T>` for explicit null-free optionals. Both target .NET 10 / C# 14. The entire public API is delivered via C# 14 `extension` blocks; the core types use a sealed internal hierarchy with no publicly constructible subtypes.

## Status

Not published to NuGet. No CI pipeline. No LICENSE file. Use as a `<ProjectReference>` by cloning the repository.

## Installation

```bash
git clone https://github.com/mbalzert1978/ResultMonad.git
```

Then in your project file:

```xml
<ItemGroup>
  <ProjectReference Include="../path/to/ResultMonad/src/Monads/Monads.csproj" />
</ItemGroup>
```

## Quick Start — `Result<T, E>`

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

Result<int, string> ok = Ok<int, string>(42);
Result<int, string> err = Err<int, string>("something went wrong");

// Match is the single primitive
string message = ok.Match(
    onOk: v => $"got {v}",
    onErr: e => $"failed: {e}");

// Transform the success value
Result<int, string> doubled = ok.Map(x => x * 2);

// Chain fallible operations
static Result<int, string> Divide(int a, int b) =>
    b == 0 ? Err<int, string>("division by zero") : Ok<int, string>(a / b);

Result<int, string> chained = ok.Bind(x => Divide(x, 2));

// Recover from errors
Result<int, string> fallback = err.OrElse(e => Ok<int, string>(0));
```

## Quick Start — `Option<T>`

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using Monads.Results;
using static Monads.Options.Option;

var some = Some(42);
var none = None<int>();

string message = some.Match(
    onSome: v => $"got {v}",
    onNone: () => "nothing");

var doubled = some.Map(x => x * 2);
var filtered = some.Filter(x => x > 10);

// Bridge to Result
Result<int, string> asResult = some.OkOr("not found");
```

## Architecture

```
Result<T, E>           ← abstract record, T/E : notnull
├── Ok<T, E>           ← internal sealed record (success variant)
└── Err<T, E>          ← internal sealed record (failure variant)

Option<T>              ← readonly record struct, T : notnull
├── Some(value)        ← IsSome = true
└── None               ← default state (IsSome = false)

Unit                   ← readonly struct (void-equivalent for Result<Unit, E>)
```

`Result<T, E>` has a `private protected` constructor; external subclassing is impossible and a third variant is unrepresentable. The entire public API is delivered via C# 14 `extension` blocks. `Match` is the single dispatch primitive for `Result<T, E>` — every other extension is implemented in terms of it. Method names deliberately mirror Rust's `Result` / `Option` API.

## Documentation

### Core Types

| Type | Description | Reference |
|------|-------------|-----------|
| `Result<T, E>` | Discriminated union for railway-oriented programming | [→ docs/models/result.md](docs/models/result.md) |
| `Option<T>` | Explicit optional value, null-free | [→ docs/models/option.md](docs/models/option.md) |
| `Unit` | Void-equivalent functional type | [→ docs/models/unit.md](docs/models/unit.md) |

### Result&lt;T, E&gt; Extensions — Synchronous

| Method | Description | Reference |
|--------|-------------|-----------|
| `Match` | Dispatch on Ok/Err — the primitive all others use | [→ sync/match.md](docs/extensions/results/sync/match.md) |
| `Map` | Transform the Ok value | [→ sync/map.md](docs/extensions/results/sync/map.md) |
| `MapErr` | Transform the Err value | [→ sync/map-err.md](docs/extensions/results/sync/map-err.md) |
| `MapOr` | Map Ok or return an eager fallback | [→ sync/map-or.md](docs/extensions/results/sync/map-or.md) |
| `MapOrElse` | Map Ok or lazily compute a fallback from Err | [→ sync/map-or-else.md](docs/extensions/results/sync/map-or-else.md) |
| `Bind` | Chain a fallible operation (flatMap) | [→ sync/bind.md](docs/extensions/results/sync/bind.md) |
| `Or` | Return self if Ok, otherwise `other` | [→ sync/or.md](docs/extensions/results/sync/or.md) |
| `OrElse` | Return self if Ok, otherwise compute fallback | [→ sync/or-else.md](docs/extensions/results/sync/or-else.md) |
| `Inspect` | Side-effect on Ok value, return self | [→ sync/inspect.md](docs/extensions/results/sync/inspect.md) |
| `InspectErr` | Side-effect on Err value, return self | [→ sync/inspect-err.md](docs/extensions/results/sync/inspect-err.md) |
| `UnwrapOr` | Extract Ok value or return eager fallback | [→ sync/unwrap-or.md](docs/extensions/results/sync/unwrap-or.md) |
| `UnwrapOrElse` | Extract Ok value or compute fallback from Err | [→ sync/unwrap-or-else.md](docs/extensions/results/sync/unwrap-or-else.md) |
| `Flatten` | Collapse `Result<Result<T,E>,E>` → `Result<T,E>` | [→ sync/flatten.md](docs/extensions/results/sync/flatten.md) |
| `Transpose` | Convert `Result<Option<T>,E>` ↔ `Option<Result<T,E>>` | [→ sync/transpose.md](docs/extensions/results/sync/transpose.md) |
| `ToOk` | Convert to `Option<T>` (Ok→Some, Err→None) | [→ sync/to-ok.md](docs/extensions/results/sync/to-ok.md) |
| `ToErr` | Convert to `Option<E>` (Err→Some, Ok→None) | [→ sync/to-err.md](docs/extensions/results/sync/to-err.md) |
| `IsOk`, `IsErr`, `IsOkAnd`, `IsErrAnd` | Boolean predicates | [→ sync/predicates.md](docs/extensions/results/sync/predicates.md) |

### Result&lt;T, E&gt; Extensions — Asynchronous (Task + ValueTask)

| Method | Description | Reference |
|--------|-------------|-----------|
| `MatchAsync` | Async dispatch on Ok/Err | [→ async/match.md](docs/extensions/results/async/match.md) |
| `MapAsync` | Async transform Ok value | [→ async/map.md](docs/extensions/results/async/map.md) |
| `MapErrAsync` | Async transform Err value | [→ async/map-err.md](docs/extensions/results/async/map-err.md) |
| `MapOrAsync` | Async map Ok or eager fallback | [→ async/map-or.md](docs/extensions/results/async/map-or.md) |
| `MapOrElseAsync` | Async map Ok or lazy fallback | [→ async/map-or-else.md](docs/extensions/results/async/map-or-else.md) |
| `BindAsync` | Async chain fallible operation | [→ async/bind.md](docs/extensions/results/async/bind.md) |
| `OrElseAsync` | Async compute fallback on Err | [→ async/or-else.md](docs/extensions/results/async/or-else.md) |
| `InspectAsync` | Async side-effect on Ok, return self | [→ async/inspect.md](docs/extensions/results/async/inspect.md) |
| `InspectErrAsync` | Async side-effect on Err, return self | [→ async/inspect-err.md](docs/extensions/results/async/inspect-err.md) |
| `UnwrapOrAsync` | Async extract Ok or eager fallback | [→ async/unwrap-or.md](docs/extensions/results/async/unwrap-or.md) |
| `UnwrapOrElseAsync` | Async extract Ok or compute from Err | [→ async/unwrap-or-else.md](docs/extensions/results/async/unwrap-or-else.md) |
| `FlattenAsync` | Async collapse nested Result | [→ async/flatten.md](docs/extensions/results/async/flatten.md) |
| `TransposeAsync` | Async transpose Result/Option | [→ async/transpose.md](docs/extensions/results/async/transpose.md) |
| `ToOkAsync` | Async convert to Option over Ok | [→ async/to-ok.md](docs/extensions/results/async/to-ok.md) |
| `ToErrAsync` | Async convert to Option over Err | [→ async/to-err.md](docs/extensions/results/async/to-err.md) |

> Each async method has three overloads: `Task<Result>` + sync op, `Result` + async op, `Task<Result>` + async op. A `ValueTask` variant exists for every `Task` variant.

### Option&lt;T&gt; Extensions — Synchronous

| Method | Description | Reference |
|--------|-------------|-----------|
| `Match` | Dispatch on Some/None (built-in instance method) | [→ sync/match.md](docs/extensions/options/sync/match.md) |
| `Map` | Transform Some value | [→ sync/map.md](docs/extensions/options/sync/map.md) |
| `MapOr` | Map Some or return eager fallback | [→ sync/map-or.md](docs/extensions/options/sync/map-or.md) |
| `MapOrElse` | Map Some or lazily compute fallback | [→ sync/map-or-else.md](docs/extensions/options/sync/map-or-else.md) |
| `Bind` | Chain option-returning operation | [→ sync/bind.md](docs/extensions/options/sync/bind.md) |
| `Filter` | Keep Some if predicate holds, else None | [→ sync/filter.md](docs/extensions/options/sync/filter.md) |
| `OrElse` | Return self if Some, else invoke fallback | [→ sync/or-else.md](docs/extensions/options/sync/or-else.md) |
| `UnwrapOr` | Extract value or eager fallback | [→ sync/unwrap-or.md](docs/extensions/options/sync/unwrap-or.md) |
| `UnwrapOrElse` | Extract value or compute fallback | [→ sync/unwrap-or-else.md](docs/extensions/options/sync/unwrap-or-else.md) |
| `Flatten` | Collapse `Option<Option<T>>` | [→ sync/flatten.md](docs/extensions/options/sync/flatten.md) |
| `OkOr` | Convert to `Result<T,E>` with eager error | [→ sync/ok-or.md](docs/extensions/options/sync/ok-or.md) |
| `OkOrElse` | Convert to `Result<T,E>` with lazy error | [→ sync/ok-or-else.md](docs/extensions/options/sync/ok-or-else.md) |
| `And` | Return `other` if Some, else None | [→ sync/and.md](docs/extensions/options/sync/and.md) |
| `Xor` | Some if exactly one is Some, else None | [→ sync/xor.md](docs/extensions/options/sync/xor.md) |
| `Zip` | Pair two Some values, else None | [→ sync/zip.md](docs/extensions/options/sync/zip.md) |
| `IsSome`, `IsNone`, `IsSomeAnd`, `IsNoneOr` | Boolean predicates | [→ sync/predicates.md](docs/extensions/options/sync/predicates.md) |

### Option&lt;T&gt; Extensions — Asynchronous (Task + ValueTask)

| Method | Description | Reference |
|--------|-------------|-----------|
| `MatchAsync` | Async dispatch on Some/None | [→ async/match.md](docs/extensions/options/async/match.md) |
| `MapAsync` | Async transform Some value | [→ async/map.md](docs/extensions/options/async/map.md) |
| `MapOrAsync` | Async map Some or eager fallback | [→ async/map-or.md](docs/extensions/options/async/map-or.md) |
| `MapOrElseAsync` | Async map Some or lazy fallback | [→ async/map-or-else.md](docs/extensions/options/async/map-or-else.md) |
| `BindAsync` | Async chain option-returning op | [→ async/bind.md](docs/extensions/options/async/bind.md) |
| `FilterAsync` | Async predicate filter | [→ async/filter.md](docs/extensions/options/async/filter.md) |
| `OrElseAsync` | Async compute fallback on None | [→ async/or-else.md](docs/extensions/options/async/or-else.md) |
| `UnwrapOrAsync` | Async extract or eager fallback | [→ async/unwrap-or.md](docs/extensions/options/async/unwrap-or.md) |
| `UnwrapOrElseAsync` | Async extract or compute fallback | [→ async/unwrap-or-else.md](docs/extensions/options/async/unwrap-or-else.md) |
| `FlattenAsync` | Async collapse nested Option | [→ async/flatten.md](docs/extensions/options/async/flatten.md) |
| `OkOrAsync` | Async convert to Result with eager error | [→ async/ok-or.md](docs/extensions/options/async/ok-or.md) |
| `OkOrElseAsync` | Async convert to Result with lazy error | [→ async/ok-or-else.md](docs/extensions/options/async/ok-or-else.md) |

## Further Reading

- [`CONTEXT.md`](CONTEXT.md) — domain glossary: Result, Ok, Err, Match, Extension API, Factory Methods, Unreachable Branch
- [`AGENTS.md`](AGENTS.md) — conventions for contributors and AI agents
- [`docs/superpowers/specs/`](docs/superpowers/specs/) — implementation specs

## License

No license file is present in this repository. All rights reserved by the copyright holder (see file headers) until a license is added.
