# Match

> Dispatch on Ok or Err and transform the result into a single value — the primitive all other extensions build on.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `MatchExtension` (partial)

## Signature

```csharp
public U Match<U>(Func<T, U> onOk, Func<E, U> onErr) where U : notnull;
```

## Description

`Match` is the single dispatch primitive for `Result<T, E>`. It invokes `onOk` when the result is Ok and `onErr` when it is Err, returning the value produced by whichever branch ran. Every other sync extension in this library is implemented exclusively in terms of `Match`. Use it whenever you need to fold a result down to a concrete value without losing exhaustiveness checking.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `onOk` | `Func<T, U>` | Invoked with the Ok value when this result is Ok. |
| `onErr` | `Func<E, U>` | Invoked with the Err value when this result is Err. |

## Returns

The value returned by `onOk(value)` or `onErr(error)`, depending on which variant this result is.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self`, `onOk`, or `onErr` is null. |
| `InvalidOperationException` | When `onOk` or `onErr` returns null (violates the `U : notnull` constraint). |
| `UnreachableException` | Defensive Roslyn-required guard — never reached in practice. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Transform to a display string
Result<int, string> ok  = Ok<int, string>(42);
Result<int, string> err = Err<int, string>("something went wrong");

string okMsg  = ok.Match(v => $"Value: {v}", e => $"Error: {e}");   // "Value: 42"
string errMsg = err.Match(v => $"Value: {v}", e => $"Error: {e}");  // "Error: something went wrong"

// Use as a pipeline step — Match collapses the Result into an HTTP status code
Result<string, int> response = Ok<string, int>("payload");
int status = response.Match(_ => 200, code => code);  // 200
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
