# Or

> Return this result if Ok, otherwise return a fallback result — with an eagerly-provided alternative.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `OrExtension`

## Signature

```csharp
public Result<T, F> Or<F>(Result<T, F> res) where F : notnull;
```

## Description

`Or` returns this result re-typed with the new error type `F` when it is Ok, and returns `res` when it is Err. The fallback `res` is always constructed before the call (eager). Because `Or` changes the error type from `E` to `F`, it is particularly useful when merging result branches that disagree on error type, or when providing a typed fallback value.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `res` | `Result<T, F>` | The fallback result to return when this result is Err. Always evaluated. |

## Returns

This result (re-wrapped as `Result<T, F>`) when Ok; otherwise `res`.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` or `res` is null. |
| `UnreachableException` | Defensive Roslyn-required guard — never reached in practice. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Ok path — original value kept, fallback ignored
Result<int, int> fromOk = Ok<int, string>(5).Or(Err<int, int>(1));
// fromOk == Ok(5)

// Err path — fallback returned
Result<int, int> fromErr = Err<int, string>("fail").Or(Ok<int, int>(0));
// fromErr == Ok(0)

// Providing a typed default when parsing fails
Result<int, string> parsed = Err<int, string>("bad input");
Result<int, string> withDefault = parsed.Or(Ok<int, string>(0));
// withDefault == Ok(0)
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
