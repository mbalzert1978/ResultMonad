# Flatten

> Collapse a nested `Result<Result<T, E>, E>` into a single `Result<T, E>`.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `FlattenExtension`

## Signature

```csharp
// Receiver: Result<Result<T, E>, E>
public Result<T, E> Flatten();
```

## Description

`Flatten` removes one level of `Result` nesting. It is equivalent to `Bind(identity)` and is useful when a function that already returns a `Result<T, E>` is wrapped in another `Result` — for example, when using `Map` with a function that itself returns a `Result`. The three cases are: `Ok(Ok(x))` → `Ok(x)`, `Ok(Err(e))` → `Err(e)`, and `Err(e)` → `Err(e)`.

## Parameters

_None._ The receiver is `Result<Result<T, E>, E>`.

## Returns

The inner result when the outer result is Ok; the outer `Err` unchanged when the outer result is Err.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` is null. |
| `InvalidOperationException` | When the inner result (returned by the Ok branch) is null. |
| `UnreachableException` | Defensive Roslyn-required guard — never reached in practice. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Ok(Ok(x)) → Ok(x)
Result<Result<int, string>, string> nested =
    Ok<Result<int, string>, string>(Ok<int, string>(42));
Result<int, string> flat = nested.Flatten();
// flat == Ok(42)

// Ok(Err(e)) → Err(e)
Result<Result<int, string>, string> okOfErr =
    Ok<Result<int, string>, string>(Err<int, string>("inner error"));
Result<int, string> flatErr = okOfErr.Flatten();
// flatErr == Err("inner error")

// Err(e) → Err(e)
Result<Result<int, string>, string> outerErr =
    Err<Result<int, string>, string>("outer error");
Result<int, string> passThrough = outerErr.Flatten();
// passThrough == Err("outer error")

// Practical: Map + Flatten is equivalent to Bind
static Result<int, string> Parse(string s) =>
    int.TryParse(s, out var n) ? Ok<int, string>(n) : Err<int, string>("not a number");

Result<Result<int, string>, string> mapped = Ok<string, string>("42").Map(Parse);
Result<int, string> result = mapped.Flatten();
// result == Ok(42)
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
