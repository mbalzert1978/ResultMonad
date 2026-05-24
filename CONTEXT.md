# Context — Monads

## Glossary

### Result<T, E>

The central type of this library. A discriminated union representing either a successful outcome (`Ok`) or a failed outcome (`Err`). Both type parameters are constrained to `notnull`. The only valid states are `Ok<T, E>` and `Err<T, E>` — no third variant is possible by design.

### Ok<T, E>

The success variant of `Result<T, E>`. Carries a value of type `T`. `internal sealed` — not part of the public API surface. Consumers never reference this type directly; they receive the value only through callbacks passed to `Match`, `Map`, `Bind`, etc.

### Err<T, E>

The error variant of `Result<T, E>`. Carries a value of type `E`. `internal sealed` — not part of the public API surface. Same access rules as `Ok<T, E>`.

### Sealed Hierarchy

`Result<T, E>` uses a `private protected` constructor. External code cannot subclass it, making a third variant unrepresentable. The `Ok<T, E>` and `Err<T, E>` variants are the only permitted subtypes. Both are `internal sealed` and live in the same assembly as `Result<T, E>`.

### Extension API

The entire public-facing API surface is delivered via C# 14 `extension` blocks. Consumers interact exclusively through `Match`, `Map`, `MapErr`, `Bind`, `OrElse`, `Flatten`, `IsOk`, `IsErr`, `IsOkAnd`, `IsErrAnd`, etc. Direct construction via `new Ok<T, E>(...)` is not part of the public API.

### Factory Methods

A non-generic static class `Result` (distinct from `Result<T, E>` by arity — no naming conflict in C#) exposes two generic static methods: `Ok<T, E>(T value)` and `Err<T, E>(E error)`. Called as `Result.Ok<int, string>(42)` and `Result.Err<int, string>("boom")`. With `using static Monads.Results.Result;`, callers can write `Ok<int, string>(42)` directly. Replaces `ResultFactory` entirely. `ResultFactory` is removed. This is a plain static class, not a C# 14 `extension` block.

### Async Extensions

All async extensions use C# 14 `extension` blocks with constructed generic receivers: `extension<T, E>(Task<Result<T, E>> self)` and `extension<T, E>(ValueTask<Result<T, E>> self)`. Verified to compile on .NET 10 / C# 14. One file per feature, `partial static class`, mirroring the sync structure.

### Test Strategy

Tests use only the public API — no `InternalsVisibleTo`, no direct reference to `Ok<T, E>` or `Err<T, E>`. Construction via factory extensions (`Result<int, string>.Ok(42)`), assertions via `IsOk`, `IsErr`, and `Match`.

### Minimal Abstract Base

`Result<T, E>` contains exactly one member: the `private protected` constructor. No abstract methods, no properties, no behavior. It is a pure type-system construct whose only purpose is to anchor the sealed hierarchy.

### Match

The single primitive of `Result<T, E>`. Takes two callbacks — one for `Ok`, one for `Err` — and returns a value. Implemented as a C# 14 extension member in `Match.cs`. All other operations (`Map`, `Bind`, `IsOk`, `IsOkAnd`, `OrElse`, etc.) are implemented exclusively in terms of `Match`. Internally uses a `switch` expression with a `_ => throw new UnreachableException()` branch as a required compiler placeholder — this branch is never reachable by design.

### Unreachable Branch

The `_ => throw new UnreachableException()` (or `System.Diagnostics.Unreachable()`) arm present in every internal `switch` over `Result<T, E>`. Required by Roslyn — the compiler does not perform exhaustiveness analysis over sealed internal hierarchies of abstract types. The branch signals developer intent ("this cannot happen") while satisfying the compiler.

### Unit

A value type representing the absence of a meaningful return value. Used where a `void`-returning operation must be lifted into `Result<Unit, E>`.
