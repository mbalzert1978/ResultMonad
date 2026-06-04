# Unit

`Unit` is a `readonly struct` that represents the absence of a meaningful value — the functional equivalent of `void`. It is used as the success type in `Result<Unit, E>` when an operation either succeeds (with nothing to return) or fails with an error.

## Namespace

```csharp
using Monads.Common;
```

## Type definition

```csharp
public readonly struct Unit : IEquatable<Unit>, ISpanFormattable
```

`Unit` has exactly one possible state. All instances are structurally identical and always compare equal.

## Static members

| Member | Kind | Description |
|--------|------|-------------|
| `Unit.Default` | `static readonly Unit` | The canonical singleton value |

`Unit.Default` is the recommended way to obtain a `Unit` value:

```csharp
using Monads.Common;

Unit u = Unit.Default;
```

## Equality

All `Unit` instances are equal. The equality contract is:

- `Equals(Unit other)` always returns `true`
- `GetHashCode()` always returns `0`
- `==` always returns `true`
- `!=` always returns `false`

```csharp
Unit a = Unit.Default;
Unit b = default;

Console.WriteLine(a == b);          // True
Console.WriteLine(a.Equals(b));     // True
Console.WriteLine(a != b);          // False
```

## String representation

`Unit` formats as `"()"` — the empty tuple notation from Rust and F#.

```csharp
Console.WriteLine(Unit.Default.ToString()); // ()
```

`ISpanFormattable` is implemented for allocation-free formatting in hot paths:

```csharp
Unit u = Unit.Default;
Span<char> buffer = stackalloc char[4];
u.TryFormat(buffer, out int written, ReadOnlySpan<char>.Empty, null);
// buffer[..written] == "()"
```

## Implicit conversions

`Unit` converts implicitly to and from `ValueTuple` (`()`):

```csharp
Unit u = ();          // implicit from ValueTuple
ValueTuple t = Unit.Default;  // implicit to ValueTuple
```

This allows interop with APIs that use `()` as a unit token.

## Primary use case: Result&lt;Unit, E&gt;

Use `Result<Unit, E>` for operations that either succeed (with no value) or fail with a typed error. This avoids returning `bool` or throwing exceptions.

```csharp
using Monads.Common;
using Monads.Results;
using static Monads.Results.Result;

static Result<Unit, string> DeleteFile(string path)
{
    try
    {
        File.Delete(path);
        return Ok<Unit, string>(Unit.Default);
    }
    catch (IOException ex)
    {
        return Err<Unit, string>(ex.Message);
    }
}

// Callers chain or inspect the result without caring about the Unit value
Result<Unit, string> r = DeleteFile("/tmp/scratch.txt");

string message = r.Match(
    onOk:  _ => "deleted",
    onErr: e => $"failed: {e}");
```

## Relationship to void

| Concept | C# | This library |
|---------|-----|--------------|
| No return value | `void` method | `Result<Unit, E>` |
| Async no return | `Task` | `Task<Result<Unit, E>>` |
| Optional nothing | `null` | `Option<T>` with `None` |

`Unit` makes "no value" an explicit, typed, composable concept rather than a language-level special case.

---

**Navigation:** [← README](../../README.md) | [↑ Models](./) | [← Option](./option.md)
