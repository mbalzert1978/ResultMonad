// <copyright file="Ok.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results;

/// <summary>
/// Represents a successful result containing a value.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error (not used in this variant).</typeparam>
/// <remarks>
/// This record is part of a discriminated union representing a successful outcome.
/// The <typeparamref name="E"/> type parameter is constrained but not used in this variant.
/// </remarks>
public sealed record Ok<T, E>(T Value) : Result<T, E>
    where E : notnull
    where T : notnull
{
    public T Value { get; } = Value ?? throw new ArgumentNullException(nameof(Value));

    // <inheritdoc/>
    public override bool IsOk => true;

    // <inheritdoc/>
    public override bool IsErrAnd(Func<E, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return false;
    }

    // <inheritdoc/>
    public override bool IsOkAnd(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return predicate(Value);
    }
}
