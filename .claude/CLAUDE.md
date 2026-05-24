# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**Monads** is a zero-dependency C# functional programming library providing `Result<T, E>` (railway-oriented programming) and a partial `Option<T>` implementation. Target: .NET 10, C# 14. Solution file: `Monads.slnx`.

## Commands

```bash
dotnet build          # build — TreatWarningsAsErrors=true, zero warnings required
dotnet test           # run all tests
dotnet test --filter "FullyQualifiedName~BindTests"   # run a single test class
dotnet pack           # produce NuGet package
```

## Architecture

### Type hierarchy

```text
Result<T, E>           ← abstract record, T/E : notnull
├── Ok<T, E>           ← sealed record, holds Value: T
└── Err<T, E>          ← sealed record, holds Error: E
```

`Ok` and `Err` are currently public. A planned refactor (`refactor/AI-grilled` branch, see `.claude/prds/result-modernization-dotnet10-csharp14.md`) will make them `internal sealed` and add a `private protected` constructor to `Result<T, E>`. See `CONTEXT.md` for the canonical domain glossary.

### Extension method layout

```text
src/Monads/Extensions/Results/
├── Sync/      Map, MapErr, Bind, Match, Or, OrElse, Flatten
└── Async/     *TaskExtension, *ValueTaskExtension — same features, three overloads each:
               Task<Result> + sync op | Result + async op | Task<Result> + async op
```

`Match` is the single dispatch primitive — all other extensions are implemented via `Match`. Every internal `switch` on `Result<T, E>` has a `_ => throw new UnreachableException(...)` arm; this is required by Roslyn and is never reached.

`ResultFactory.Success<T, E>(value)` / `ResultFactory.Failure<T, E>(error)` are the current construction API, imported via `using static Monads.Results.ResultFactory;`.

### Key conventions

- **One static class per file** for extensions, named `[Feature]Extension`
- **Every public method** validates parameters with `ArgumentNullException.ThrowIfNull`
- **Async methods** always call `.ConfigureAwait(false)` on every `await`
- **Records over classes** — all core types use `abstract record` with sealed variants
- **Expression-bodied members** for properties/operators/accessors; expanded bodies for methods and constructors
- Copyright header on every file: `// <copyright file="X.cs" company="Markus - Iorio">`

### Tests

Framework: xunit v3 + AwesomeAssertions. Naming: `[Feature]_When[Condition]_Should[Outcome]`. Mirror the source folder structure under `tests/Tests.Monads.Result/`. Every extension needs: Ok path, Err path, null-parameter guards, chaining, type transformations, and (for async) all Task/ValueTask combinations.

---

Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:

- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:

- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:

- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:

- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:

```text
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.
