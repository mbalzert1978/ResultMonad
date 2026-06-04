# Inspect

> Observe the Ok value via a side-effecting action, then pass the result through unchanged.

**Namespace:** `Monads.Results.Extensions.Sync`  
**Class:** `InspectExtension`

## Signature

```csharp
public Result<T, E> Inspect(Action<T> action);
```

## Description

`Inspect` invokes `action` with the Ok value when this result is Ok, then returns this result unchanged. When this result is Err, `action` is never called and the result passes through as-is. It is intended for inserting side effects — logging, metrics, debugging — into a pipeline without interrupting the chain. Because the result is returned unchanged, `Inspect` can be inserted or removed without altering the pipeline's type or behaviour.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `action` | `Action<T>` | The side-effecting action to invoke with the Ok value. |

## Returns

This result unchanged, regardless of whether it is Ok or Err.

## Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | When `self` or `action` is null. |

## Examples

```csharp
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

// Ok path — action fires, result continues
Result<int, string> result = Ok<int, string>(42)
    .Inspect(v => Console.WriteLine($"Got: {v}"))  // prints "Got: 42"
    .Map(v => v * 2);                              // Ok(84)

// Err path — action is skipped, Err propagates
Result<int, string> err = Err<int, string>("bad input")
    .Inspect(v => Console.WriteLine($"Got: {v}"))  // nothing printed
    .Map(v => v * 2);                              // Err("bad input")

// Logging in a pipeline without modifying types
Result<Order, string> order = FindOrder(id)
    .Inspect(o => logger.LogInformation("Order found: {Id}", o.Id))
    .Bind(ValidateOrder)
    .Inspect(o => logger.LogInformation("Order validated: {Id}", o.Id));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Results Sync](./) | [Results Async](../async/) | [Options](../../options/)
