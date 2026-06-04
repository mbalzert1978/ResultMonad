# Predicates

> Inspect whether a result is Ok or Err, with optional predicate conditions.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `PredicateExtension`

## Signatures

```csharp
public bool IsOk { get; }
public bool IsErr { get; }
public bool IsOkAnd(Func<T, bool> predicate);
public bool IsErrAnd(Func<E, bool> predicate);
```

## Description

`PredicateExtension` provides four members for inspecting the state of a `Result<T, E>`. All four are implemented exclusively via `Match`. `IsOk` and `IsErr` are computed properties that simply test which variant the result is. `IsOkAnd` and `IsErrAnd` combine a variant check with a user-supplied predicate, returning `true` only when the result is the expected variant and the predicate also returns `true`.

## Members

| Member | Type | Description |
|--------|------|-------------|
| `IsOk` | `bool` property | `true` when this result is Ok. |
| `IsErr` | `bool` property | `true` when this result is Err. |
| `IsOkAnd(predicate)` | `bool` method | `true` when Ok and `predicate(value)` is `true`. |
| `IsErrAnd(predicate)` | `bool` method | `true` when Err and `predicate(error)` is `true`. |

## Parameters

### `IsOkAnd`

| Parameter | Type | Description |
|-----------|------|-------------|
| `predicate` | `Func<T, bool>` | The condition to test against the Ok value. |

### `IsErrAnd`

| Parameter | Type | Description |
|-----------|------|-------------|
| `predicate` | `Func<E, bool>` | The condition to test against the Err value. |

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | `IsOkAnd`: when `predicate` is null. |
| `ArgumentNullException` | `IsErrAnd`: when `predicate` is null. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

Result<int, string> ok  = Ok<int, string>(10);
Result<int, string> err = Err<int, string>("timeout");

// IsOk / IsErr
bool isOk  = ok.IsOk;    // true
bool isErr = ok.IsErr;   // false
bool errIsErr = err.IsErr; // true

// IsOkAnd — true only when Ok AND predicate holds
bool okAndPositive    = ok.IsOkAnd(v => v > 0);   // true
bool okAndNegative    = ok.IsOkAnd(v => v < 0);   // false
bool errAndPositive   = err.IsOkAnd(v => v > 0);  // false (Err, predicate not called)

// IsErrAnd — true only when Err AND predicate holds
bool errAndTimeout    = err.IsErrAnd(e => e == "timeout");  // true
bool errAndNotFound   = err.IsErrAnd(e => e == "not found"); // false
bool okAndErrCheck    = ok.IsErrAnd(e => e.Length > 0);    // false (Ok, predicate not called)

// Guard pattern — only process if value meets a condition
if (result.IsOkAnd(v => v % 2 == 0))
{
    Console.WriteLine("Even Ok value");
}
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
