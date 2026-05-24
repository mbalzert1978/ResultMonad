# PRD: Result<T, E> — .NET 10 / C# 14 Modernisierung & Sealed Hierarchy

## Problem Statement

`Result<T, E>` ist heute eine offene Typenhierarchie. `Ok<T, E>` und `Err<T, E>` sind öffentliche, direkt konstruierbare Typen — externer Code kann `Result<T, E>` subclassen und eine dritte Variante einführen. Das bedeutet: "invalid state" ist zur Compilezeit nicht unrepresentable, jedes interne `switch` braucht einen `_ =>`-Zweig als Laufzeitschutz. Gleichzeitig nutzt die Library noch C# 10-Style Extension-Klassen statt C# 14 `extension`-Blöcke, und `ResultFactory` ist ein separates Factory-Konstrukt, das konzeptuell nicht mit dem Typ verbunden ist.

## Solution

Die `Result<T, E>`-Hierarchie wird versiegelt: `Ok<T, E>` und `Err<T, E>` werden `internal sealed`, `Result<T, E>` bekommt einen `private protected`-Konstruktor. Kein externer Code kann mehr eine dritte Variante bauen. Die gesamte öffentliche API wird als C# 14 `extension`-Blöcke neu geschrieben. `ResultFactory` fällt weg und wird durch eine nicht-generische statische Klasse `Result` ersetzt, die `Result.Ok<int, string>(42)` und `Result.Err<int, string>("boom")` ermöglicht — inklusive `using static`-Support. Der abstrakte Record selbst enthält keinerlei Verhalten — er ist ein reines Typsystem-Konstrukt.

## User Stories

1. Als Library-Konsument möchte ich, dass es unmöglich ist, `Result<T, E>` zu subclassen, damit ich sicher bin, dass nur `Ok` und `Err` als Zustände existieren können.
2. Als Library-Konsument möchte ich `Result.Ok<int, string>(42)` schreiben, damit die Ok-Konstruktion kurz, typsicher und ohne direkte Referenz auf interne Typen möglich ist.
3. Als Library-Konsument möchte ich `Result.Err<int, string>("boom")` schreiben, damit die Err-Konstruktion konsistent mit `Ok` und ohne interne Typen möglich ist.
4. Als Library-Konsument möchte ich `using static Monads.Results.Result;` setzen und dann `Ok<int, string>(42)` und `Err<int, string>("boom")` direkt aufrufen, damit ich in dichten Pipelines auf den Qualifier verzichten kann.
5. Als Library-Konsument möchte ich nicht mehr `new Ok<int, string>(42)` schreiben müssen, damit ich nicht direkt von internen Implementierungstypen abhänge.
6. Als Library-Konsument möchte ich `result.Match(ok: v => v * 2, err: e => 0)` aufrufen, damit ich exhaustiv über beide Zustände matchen kann ohne `switch`-Syntax zu brauchen.
7. Als Library-Konsument möchte ich `result.IsOk` und `result.IsErr` als Properties aufrufen, damit ich einfache Zustandsprüfungen ohne Lambda machen kann.
8. Als Library-Konsument möchte ich `result.IsOkAnd(v => v > 0)` aufrufen, damit ich Zustand und Wertbedingung in einem Ausdruck prüfen kann.
9. Als Library-Konsument möchte ich `result.IsErrAnd(e => e.Contains("timeout"))` aufrufen, damit ich Fehlertyp und Fehlerbedingung kombinieren kann.
10. Als Library-Konsument möchte ich `result.Map(v => v.ToString())` aufrufen, damit ich Ok-Werte transformieren kann ohne den Fehler-Pfad zu berühren.
11. Als Library-Konsument möchte ich `result.MapErr(e => new AppError(e))` aufrufen, damit ich Fehlertypen transformieren kann ohne den Ok-Pfad zu berühren.
12. Als Library-Konsument möchte ich `result.Bind(v => Divide(v, 2))` aufrufen, damit ich mehrere fehlerhafte Operationen sequenziell verketten kann.
13. Als Library-Konsument möchte ich `result.OrElse(e => Result.Ok<int, string>(0))` aufrufen, damit ich Fehler mit einem Fallback-Wert behandeln kann.
14. Als Library-Konsument möchte ich `result.Or(fallback)` aufrufen, damit ich im Fehlerfall direkt einen alternativen `Result`-Wert zurückgeben kann.
15. Als Library-Konsument möchte ich `result.Flatten()` aufrufen, damit ich verschachtelte `Result<Result<T,E>, E>` auflösen kann.
16. Als Library-Konsument möchte ich alle Sync-Operationen auch auf `Task<Result<T, E>>` aufrufen, damit ich asynchrone Pipelines ohne manuelle `await`-Unterbrechungen bauen kann.
17. Als Library-Konsument möchte ich alle Sync-Operationen auch auf `ValueTask<Result<T, E>>` aufrufen, damit ich allokationsarme asynchrone Pipelines schreiben kann.
18. Als Library-Konsument möchte ich synchrone und asynchrone Operationen frei kombinieren (`Task<Result>` + sync Mapper, `Result` + async Mapper), damit ich nicht zwischen Sync- und Async-Pfaden umbauen muss.
19. Als Library-Autor möchte ich, dass alle internen `switch`-Ausdrücke über `Result<T, E>` den `_`-Zweig als `UnreachableException` dokumentieren, damit der Intent klar ist ohne die Compiler-Anforderung zu umgehen.

## Implementation Decisions

**Sealed Hierarchy**
`Result<T, E>` erhält einen `private protected`-Konstruktor. Es hat ausser diesem Konstruktor keine weiteren Member — keine abstrakten Methoden, keine Properties, kein Verhalten. Es ist ein reines Typsystem-Konstrukt.

**Interne Varianten**
`Ok<T, E>` und `Err<T, E>` bleiben separate Typen im gleichen Assembly, werden aber auf `internal sealed record` geändert. Sie behalten ihre `Value`- bzw. `Error`-Properties mit Null-Guards. Sie implementieren keinerlei abstrakte Member mehr, da die Basis keine definiert.

**Match als einziges Primitiv**
`Match` ist die einzige Operation, die intern auf `Ok<T, E>` und `Err<T, E>` per `switch` dispatcht. Alle anderen Operationen (`Map`, `Bind`, `IsOk`, `IsOkAnd`, `OrElse`, etc.) sind ausschliesslich in Termen von `Match` implementiert. Der interne `switch` in `Match` enthält einen `_ => throw new UnreachableException()`-Zweig — Compilerpflicht, nie erreichbar.

**Factory**
`ResultFactory` wird entfernt. An seine Stelle tritt eine nicht-generische statische Klasse `Result` — separater Typ von `Result<T, E>`, kein Namenskonflikt durch unterschiedliche Arität. Sie enthält zwei generische statische Methoden:

- `Ok<T, E>(T value)` — erzeugt einen erfolgreichen `Result<T, E>`
- `Err<T, E>(E error)` — erzeugt einen fehlerhaften `Result<T, E>`

Aufruf: `Result.Ok<int, string>(42)` bzw. `Result.Err<int, string>("boom")`. Mit `using static Monads.Results.Result;` kann der Caller den Qualifier weglassen und direkt `Ok<int, string>(42)` schreiben. Diese Klasse ist eine gewöhnliche statische Klasse — kein C# 14 `extension`-Block.

**C# 14 Extension-Block-Syntax**
Alle Instanz-Extensions (`Match`, `Map`, `MapErr`, `Bind`, `Or`, `OrElse`, `Flatten`, `IsOk`, `IsErr`, `IsOkAnd`, `IsErrAnd`) werden auf C# 14 `extension`-Block-Syntax migriert. Die Dateistruktur bleibt: eine `partial static class` pro Feature-Datei mit einem `extension`-Block.

**`IsOk`, `IsErr`, `IsOkAnd`, `IsErrAnd`**
Alle vier werden aus dem abstrakten Record entfernt und als Extension-Properties bzw. -Methoden im `extension`-Block neu implementiert — ausschliesslich in Termen von `Match`.

**Async Extensions**
Alle Async-Extensions verwenden `extension`-Blöcke mit konstruierten Generics als Receiver — `extension<T, E>(Task<Result<T, E>>)` und `extension<T, E>(ValueTask<Result<T, E>>)`. Auf .NET 10 / C# 14 verifiziert und kompilierbar. Eine `partial static class` pro Feature-Datei, analog zur Sync-Struktur.

## Testing Decisions

**Was einen guten Test ausmacht**
Tests verifizieren ausschliesslich beobachtbares Verhalten durch die öffentliche API. Kein Test referenziert `Ok<T, E>` oder `Err<T, E>` direkt. Kein `InternalsVisibleTo`. Konstruktion immer via `Result.Ok<T,E>()` / `Result.Err<T,E>()`, Assertions via `IsOk`, `IsErr`, `Match`.

**Getestete Module**
- Factory: Konstruktion via `Result.Ok<T,E>()` / `Result.Err<T,E>()`, Null-Guards
- Match: Ok-Pfad, Err-Pfad, Null-Parameter
- Map / MapErr: Transformation, Fehler-Propagation, Null-Guards
- Bind: Verkettung, Fehler-Propagation, Null-Guards
- Or / OrElse: Fallback-Verhalten für Err
- Flatten: Auflösen verschachtelter Results
- IsOk / IsErr / IsOkAnd / IsErrAnd: Alle vier Kombinationen
- Alle Async-Varianten (Task + ValueTask): identische Abdeckung wie Sync

**Prior Art**
Bestehende Tests in `tests/Tests.Monads.Result/` folgen dem Muster `[Feature]_When[Condition]_Should[Outcome]` mit xunit + AwesomeAssertions. Dieses Muster wird beibehalten.

## Out of Scope

- `Option<T>` — bekommt in einem separaten Refactor dieselbe Behandlung
- Neue Extension-Methoden jenseits des bestehenden Funktionsumfangs
- NuGet-Packaging-Änderungen oder Versioning-Strategie
- Dokumentation ausserhalb von XML-Docstrings

## Further Notes

Dies ist ein Breaking Change — `new Ok<int, string>(42)` kompiliert nach der Migration nicht mehr. Ein Major-Version-Bump (v2.0) ist angebracht.

Die `CONTEXT.md` im Repository-Root dokumentiert alle getroffenen Designentscheidungen und dient als kanonische Referenz für die Implementierung.
