# Xor

> Return the Some option when exactly one of two options is Some; return None when both or neither are Some.

**Namespace:** `Monads.Options.Extensions.Sync`
**Class:** `XorExtension`

## Signature

```csharp
public Option<T> Xor(Option<T> other);
```

## Description

`Xor` implements exclusive-or semantics over two options of the same type. It returns the Some option when exactly one of the two is Some. When both options are Some, the result is `None` — the ambiguity is discarded. When both options are None, the result is also `None`. Use `Xor` when a value must come from exactly one of two sources; having both be present is considered an error condition.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `other` | `Option<T>` | The second option to compare against. |

## Returns

The single Some option when exactly one is Some; `None<T>()` when both or neither are Some.

## Examples

```csharp
using Monads.Options;
using Monads.Options.Extensions.Sync;
using static Monads.Options.Option;

Option<int> a = Some(1);
Option<int> b = Some(2);
Option<int> n = None<int>();

Option<int> onlyA     = a.Xor(n);  // Some(1)
Option<int> onlyB     = n.Xor(b);  // Some(2)
Option<int> both      = a.Xor(b);  // None  — ambiguous
Option<int> neither   = n.Xor(n);  // None

// Ensure a config value comes from exactly one source
Option<string> value = FromEnvVar("MY_VAR").Xor(FromConfigFile("MY_VAR"));
```

---

**Navigation:** [← README](../../../../README.md) | [↑ Options Sync](./) | [Options Async](../async/) | [Results](../../results/)
