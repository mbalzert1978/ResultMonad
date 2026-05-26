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

## API Surface

### `Result<T, E>`

| Operation | Description | Async |
|-----------|-------------|:-----:|
| `Match` | Dispatch on Ok/Err; all other operations are implemented in terms of this | Y |
| `Map` | Transform the Ok value | Y |
| `MapErr` | Transform the Err value | Y |
| `MapOr` | Map Ok or return a default | Y |
| `MapOrElse` | Map Ok or compute a default from Err | Y |
| `Bind` | Chain a fallible operation | Y |
| `And` | Return `other` if Ok, otherwise propagate Err | N |
| `Or` | Return self if Ok, otherwise `other` | N |
| `OrElse` | Return self if Ok, otherwise compute a fallback | Y |
| `Flatten` | Collapse `Result<Result<T,E>,E>` | Y |
| `Inspect` | Run a side-effect on Ok value, return self | Y |
| `InspectErr` | Run a side-effect on Err value, return self | Y |
| `ToOk` | Convert to `Option<T>` (Ok→Some, Err→None) | Y |
| `ToErr` | Convert to `Option<E>` (Err→Some, Ok→None) | Y |
| `Transpose` | Convert `Result<Option<T>,E>` ↔ `Option<Result<T,E>>` | Y |
| `UnwrapOr` | Return Ok value or a default | Y |
| `UnwrapOrElse` | Return Ok value or compute a default from Err | Y |
| `IsOk` | `true` if Ok | N |
| `IsErr` | `true` if Err | N |
| `IsOkAnd` | `true` if Ok and predicate holds | N |
| `IsErrAnd` | `true` if Err and predicate holds | N |

Async (Y): three overloads exist per operation — `Task<Result>` + sync op, `Result` + async op, `Task<Result>` + async op. A `ValueTask` variant is provided for every `Task` variant.

### `Option<T>`

| Operation | Description | Async |
|-----------|-------------|:-----:|
| `IsSome` | `true` if Some (built-in property) | — |
| `IsNone` | `true` if None (built-in property) | — |
| `Match` | Dispatch on Some/None (built-in instance method) | Y |
| `Map` | Transform the Some value | Y |
| `MapOr` | Map Some or return a default | Y |
| `MapOrElse` | Map Some or compute a default | Y |
| `Bind` | Chain a fallible operation | Y |
| `And` | Return `other` if Some, otherwise None | N |
| `Or` | Return self if Some, otherwise `other` | N |
| `OrElse` | Return self if Some, otherwise compute a fallback | Y |
| `Filter` | Keep Some if predicate holds, otherwise None | Y |
| `Flatten` | Collapse `Option<Option<T>>` | Y |
| `OkOr` | Convert to `Result<T,E>` with an eager error | Y |
| `OkOrElse` | Convert to `Result<T,E>` with a lazy error | Y |
| `UnwrapOr` | Return Some value or a default | Y |
| `UnwrapOrElse` | Return Some value or compute a default | Y |
| `IsSomeAnd` | `true` if Some and predicate holds | N |
| `IsNoneOr` | `true` if None or predicate holds | N |
| `Xor` | Some if exactly one of self/other is Some, otherwise None | N |
| `Zip` | Pair two Some values into `Option<(T, U)>`, otherwise None | N |

Async (Y): same three-overload pattern as `Result<T, E>` above.

## Architecture

`Result<T, E>` is an `abstract record` with a `private protected` constructor; `Ok<T, E>` and `Err<T, E>` are `internal sealed` variants — a third variant is unrepresentable and external subclassing is impossible. The entire public API is delivered via C# 14 `extension` blocks; `Match` is the only primitive for `Result<T, E>` and all other operations are implemented in terms of it. Method names and signatures deliberately mirror Rust's `Result` / `Option` to give Rust developers a familiar surface (see `CONTEXT.md` for the full glossary).

## Further Reading

- [`CONTEXT.md`](CONTEXT.md) — domain glossary: Result, Ok, Err, Match, Extension API, Factory Methods, Unreachable Branch
- [`AGENTS.md`](AGENTS.md) — conventions for contributors and AI agents
- [`docs/superpowers/specs/`](docs/superpowers/specs/) — implementation specs

## License

No license file is present in this repository. All rights reserved by the copyright holder (see file headers) until a license is added.
