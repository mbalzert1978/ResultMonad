// <copyright file="Ok.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace ResultMonad;

/// <summary>
/// Represents a successful result containing a value.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error (not used in this variant).</typeparam>
/// <remarks>
/// This record is part of a discriminated union representing a successful outcome.
/// The <typeparamref name="TError"/> type parameter is constrained but not used in this variant.
/// </remarks>
public sealed record Ok<TValue, TError>(TValue Value) : Result<TValue, TError>
    where TError : notnull
    where TValue : notnull
{
    public TValue Value { get; } = Value ?? throw new ArgumentNullException(nameof(Value));

    // <inheritdoc/>
    public override bool IsOk => true;
}
