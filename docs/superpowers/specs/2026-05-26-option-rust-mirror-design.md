# Option<T> — Full Rust API Mirror (Sync + Async)

**Issue:** [#13 — Update Option extension blocks for readonly record struct](https://github.com/Markus-Iorio/Monads/issues/13)
**Branch:** `13-update-option-extensions-rust-mirror`
**Date:** 2026-05-26

## Problem Statement

`Option<T>` currently exposes only `Or` and `OrElse` as sync extensions on top of the readonly record struct's built-in `IsSome`, `IsNone`, `Value`, and `Match` members. There is no `Map`, no `Bind`, no `Flatten`, no value-extraction helpers, no `Result`-interop, no combinators, and no async surface at all. Issue #13 calls out Map/Bind/Flatten/Predicates as the minimum bar, but to bring `Option<T>` to feature parity with the broader Rust `Option` API — and with the conventions already established by `Result<T, E>` — a wider set of methods and full async coverage are needed.

## Scope

In-scope sync extensions (14 new files under [src/Monads/Extensions/Options/Sync/](src/Monads/Extensions/Options/Sync/)):

| Group | File | Signature | Rust source |
|---|---|---|---|
| **A** | `Map.cs` | `Option<T>.Map<U>(Func<T,U>) → Option<U>` | `map` |
| A | `Bind.cs` | `Option<T>.Bind<U>(Func<T,Option<U>>) → Option<U>` | `and_then` |
| A | `Flatten.cs` | `Option<Option<T>>.Flatten() → Option<T>` | `flatten` |
| A | `Predicates.cs` | `IsSomeAnd(Func<T,bool>)`, `IsNoneOr(Func<T,bool>)` | `is_some_and`, `is_none_or` |
| **B** | `MapOr.cs` | `Option<T>.MapOr<U>(U fallback, Func<T,U>) → U` | `map_or` |
| B | `MapOrElse.cs` | `Option<T>.MapOrElse<U>(Func<U>, Func<T,U>) → U` | `map_or_else` |
| B | `UnwrapOr.cs` | `Option<T>.UnwrapOr(T fallback) → T` | `unwrap_or` |
| B | `UnwrapOrElse.cs` | `Option<T>.UnwrapOrElse(Func<T>) → T` | `unwrap_or_else` |
| **C** | `OkOr.cs` | `Option<T>.OkOr<E>(E error) → Result<T,E>` | `ok_or` |
| C | `OkOrElse.cs` | `Option<T>.OkOrElse<E>(Func<E>) → Result<T,E>` | `ok_or_else` |
| **D** | `Filter.cs` | `Option<T>.Filter(Func<T,bool>) → Option<T>` | `filter` |
| D | `And.cs` | `Option<T>.And<U>(Option<U>) → Option<U>` | `and` |
| D | `Xor.cs` | `Option<T>.Xor(Option<T>) → Option<T>` | `xor` |
| D | `Zip.cs` | `Option<T>.Zip<U>(Option<U>) → Option<(T,U)>` | `zip` |

In-scope async extensions (23 new files under [src/Monads/Extensions/Options/Async/](src/Monads/Extensions/Options/Async/)):

| Sync feature | Async files |
|---|---|
| Map | `MapTaskExtension.cs`, `MapValueTaskExtension.cs` |
| Bind | `BindTaskExtension.cs`, `BindValueTaskExtension.cs` |
| Flatten | `FlattenAsyncExtension.cs` (Task + ValueTask in one file, no operation arg) |
| Match (already on struct) | `MatchTaskExtension.cs`, `MatchValueTaskExtension.cs` |
| OrElse (already sync extension) | `OrElseTaskExtension.cs`, `OrElseValueTaskExtension.cs` |
| MapOr | `MapOrTaskExtension.cs`, `MapOrValueTaskExtension.cs` |
| MapOrElse | `MapOrElseTaskExtension.cs`, `MapOrElseValueTaskExtension.cs` |
| UnwrapOr | `UnwrapOrTaskExtension.cs`, `UnwrapOrValueTaskExtension.cs` |
| UnwrapOrElse | `UnwrapOrElseTaskExtension.cs`, `UnwrapOrElseValueTaskExtension.cs` |
| OkOr | `OkOrTaskExtension.cs`, `OkOrValueTaskExtension.cs` |
| OkOrElse | `OkOrElseTaskExtension.cs`, `OkOrElseValueTaskExtension.cs` |
| Filter | `FilterTaskExtension.cs`, `FilterValueTaskExtension.cs` |

**Out-of-scope** (no async variants — eager operations or pure predicates, consistent with how `Result<T,E>` handles `Or`/`Predicates`):
- `Predicates` (IsSomeAnd, IsNoneOr) — sync-only
- `Or` (already sync-only) — eager fallback value
- `And` — eager argument
- `Xor` — eager argument
- `Zip` — eager argument

**Explicitly not added** (Rust API methods that don't fit a readonly C# struct or are non-idiomatic):
- `as_ref`/`as_mut`/`as_deref`/`cloned`/`copied` — Rust reference/lifetime semantics, no C# analog
- `unwrap`/`expect`/`unwrap_unchecked` — Panic-style; safe path covered by `UnwrapOr`/`UnwrapOrElse`
- `insert`/`replace`/`take`/`take_if`/`get_or_insert*` — Require mutation; `Option<T>` is a `readonly record struct`
- `iter`/`iter_mut`/`into_iter` — Possible but unusual in this library's idiom; defer
- `transpose`/`unzip` — Defer to follow-up issue if demand arises
- `inspect` — `Result<T,E>` doesn't have it either; keep symmetry

## Conventions (apply to every new file)

1. **Copyright header** — `// <copyright file="X.cs" company="Markus - Iorio">` (matches existing files)
2. **C# 14 extension blocks** — one static class per file, named `[Feature]Extension` (sync) or `[Feature]TaskExtension` / `[Feature]ValueTaskExtension` (async)
3. **No `ArgumentNullException.ThrowIfNull(self)`** — `Option<T>` is a struct, cannot be null. (For async, the awaited `Task<Option<T>>`/`ValueTask<Option<T>>` receiver may be null and IS guarded with `ThrowIfNull(self)`, matching the Result async pattern.)
4. **`ArgumentNullException.ThrowIfNull(operation/predicate)`** for every callback parameter
5. **No `UnreachableException`** — Option's `Match` on the struct exhausts the cases without a `_ =>` arm; no need for the import or XML exception tag
6. **`.ConfigureAwait(false)`** on every `await` in async methods
7. **`MapAsync`-style naming** for async methods (matches existing Result async surface)
8. **Three-overload pattern** for transforming async extensions:
   - `Task<Option<T>>.MethodAsync(sync op)` — overload 1
   - `Task<Option<T>>.MethodAsync(async op)` — overload 2
   - `Option<T>.MethodAsync(async op)` — overload 3
9. **Expression-bodied members** where idiomatic; expanded bodies for methods that need guards
10. **`Match` as the only dispatch primitive** — every extension is implemented exclusively via `self.Match(...)`. No direct `IsSome`/`Value` access in extension bodies.

## Implementation notes per group

**Group D — `Zip`:** Uses C# tuple `(T, U)`. Constraint: tuples are notnull by definition, so `Zip` returns `Option<(T, U)>` directly. Implementation: `self.Match(t => other.Match(u => Some((t, u)), None<(T,U)>), None<(T,U)>)`.

**Group C — `OkOr` / `OkOrElse`:** Returns `Result<T, E>`. Requires `where E : notnull` to match `Result<T,E>`'s constraint. Implementation: `self.Match(Result.Ok<T,E>, () => Result.Err<T,E>(error))`. Note: live import of `Monads.Results` namespace.

**Group B — `MapOr` / `MapOrElse`:** Returns `U`, not `Option<U>`. Constraint `where U : notnull`. `MapOr` eagerly evaluates the fallback (matches Rust semantics).

**Group A — `Predicates`:** Both predicates take `Func<T, bool>`, follow the `IsErrAnd`/`IsOkAnd` pattern from Result's `PredicateExtension`.

**Async `Match`:** Mirrors Result's `MatchTaskExtension` / `MatchValueTaskExtension`. Provides three overloads taking `Func<T, U>` + `Func<U>` (sync arms), `Func<T, Task<U>>` + `Func<Task<U>>` (async arms), and the mixed combinations on `Option<T>` receiver. This is the dispatch primitive every other async extension depends on.

## Test Plan

Tests live under [tests/Tests.Monads.Result/Extensions/Options/](tests/Tests.Monads.Result/Extensions/Options/), mirroring the source layout (`Sync/` + new `Async/`). Existing convention: xunit v3 + AwesomeAssertions, `[Feature]_When[Condition]_Should[Outcome]` naming.

Per sync extension:
- **Some path** — operation invoked, returns expected
- **None path** — operation skipped, returns None / fallback / error
- **Null-callback guard** — `ArgumentNullException` when callback is null
- **Type transformation** (where applicable, e.g. Map/Bind: `Option<int> → Option<string>`)
- **Chaining** for monadic ops (Map/Bind/Flatten) — at least one chained-pipeline test

Per async extension (three overloads):
- All three overload paths × Some + None = 6 happy-path tests minimum
- Null-callback guard on each overload
- Null-receiver guard on `Task<Option<T>>` receiver (where applicable)

Total estimated new test classes: ~37 (14 sync + 23 async).

## Success Criteria

- [ ] All sync extension files compile and live under `src/Monads/Extensions/Options/Sync/`
- [ ] All async extension files compile and live under `src/Monads/Extensions/Options/Async/`
- [ ] Every extension uses `self.Match(...)` exclusively — no direct `Value`/`IsSome` access
- [ ] No `UnreachableException` import or XML tag in any Option extension file
- [ ] `dotnet build` — zero warnings, zero errors (`TreatWarningsAsErrors=true`)
- [ ] `dotnet test` — all tests green (including existing Or/OrElse and Option<T> struct tests)
- [ ] Issue #13's stated acceptance criteria all met
- [ ] PR opened linking #13

## Process

Strict vertical TDD: one test → one impl → next test. No horizontal slicing (no batch-write-all-tests-first). Work the 7 groups in roughly this order:

1. Spec self-review + user approval (this doc)
2. Group A sync (4 files) — establishes the pattern
3. Group B sync (4 files)
4. Group C sync (2 files)
5. Group D sync (4 files)
6. Async surface (23 files) — `Match` async first, since others depend on it
7. Final verification + PR

Build/test runs after each file or small batch — fast feedback loop.

## Out of Scope

- Adding async variants of `Or`, `And`, `Xor`, `Zip`, or `Predicates` (eager / pure-predicate; matches Result's choice)
- Rust methods listed under "Explicitly not added" above (`unwrap`, `iter`, `transpose`, etc.) — separate follow-up issues
- Updating any `Result<T, E>` code
- Documentation outside XML doc comments
- NuGet/packaging changes
