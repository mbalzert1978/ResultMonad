// <copyright file="Option{T}.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Monads.Strings;

namespace Monads.Options;

/// <summary>
/// Represents an optional value: either <c>Some(value)</c> or <c>None</c>.
/// </summary>
/// <typeparam name="T">The type of the wrapped value. Must be non-nullable.</typeparam>
public readonly record struct Option<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Option{T}"/> struct in the Some state.
    /// </summary>
    /// <param name="value">The non-null value to wrap.</param>
    internal Option(T value)
    {
        IsSome = value is not null;
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this option holds a value.
    /// When <c>true</c>, <see cref="Value"/> is guaranteed non-null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSome { get; private init; }

    /// <summary>
    /// Gets a value indicating whether this option is empty.
    /// </summary>
    public bool IsNone => !IsSome;

    /// <summary>
    /// Gets the wrapped value when <see cref="IsSome"/> is <c>true</c>; otherwise <c>default</c>.
    /// </summary>
    public T? Value
    {
        get => IsSome ? field : default;
        private init;
    }

    /// <summary>
    /// Invokes <paramref name="onSome"/> with the wrapped value when this option is Some,
    /// otherwise invokes <paramref name="onNone"/>.
    /// </summary>
    /// <typeparam name="U">The result type returned by both branches.</typeparam>
    /// <param name="onSome">Function invoked when this option is Some.</param>
    /// <param name="onNone">Function invoked when this option is None.</param>
    /// <returns>The value produced by the invoked branch.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either callback is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the invoked branch returns a null result.</exception>
    public U Match<U>(Func<T, U> onSome, Func<U> onNone)
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);

        U result = IsSome ? onSome(Value) : onNone();

        return result.OrThrowIfNull();
    }

    /// <inheritdoc/>
    public override string ToString() => IsSome ? $"Some({Value})" : "None";
}
