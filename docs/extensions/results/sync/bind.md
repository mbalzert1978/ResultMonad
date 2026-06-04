# Bind

> Chain a fallible operation — the core railway-oriented flatMap.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `BindExtension`

## Signature

```csharp
public Result<U, E> Bind<U>(Func<T, Result<U, E>> operation) where U : notnull;
```

## Description

`Bind` (also known as flatMap or `and_then` in Rust) chains a fallible operation onto a result. When this result is Ok, `operation` is invoked with the value and its `Result<U, E>` is returned directly. When this result is Err, the error is propagated and `operation` is never called. This is the primary tool for sequencing steps where each step can fail — errors short-circuit the chain automatically.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `operation` | `Func<T, Result<U, E>>` | The fallible operation to run when this result is Ok. |

## Returns

The `Result<U, E>` produced by `operation` when this result is Ok; otherwise the original `Err<U, E>` carrying the propagated error.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` or `operation` is null. |
| `InvalidOperationException` | When `operation` returns null. |
| `UnreachableException` | Defensive Roslyn-required guard — never reached in practice. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

static Result<int, string> Halve(int n) =>
    n % 2 == 0 ? Ok<int, string>(n / 2) : Err<int, string>("odd number");

// Chain succeeds — 8 → 4 → 2
Result<int, string> success = Ok<int, string>(8).Bind(Halve).Bind(Halve);
// success == Ok(2)

// Chain short-circuits at first failure — 10 → 5 → Err (5 is odd)
Result<int, string> failure = Ok<int, string>(10).Bind(Halve).Bind(Halve);
// failure == Err("odd number")

// Err at the start — operation never called
Result<int, string> initial = Err<int, string>("already failed").Bind(Halve);
// initial == Err("already failed")

// Real-world: parse then validate
static Result<int, string> Parse(string s) =>
    int.TryParse(s, out var n) ? Ok<int, string>(n) : Err<int, string>($"'{s}' is not a number");

static Result<int, string> ValidatePositive(int n) =>
    n > 0 ? Ok<int, string>(n) : Err<int, string>("must be positive");

Result<int, string> pipeline = Parse("42").Bind(ValidatePositive);
// pipeline == Ok(42)
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
