// <copyright file="Err.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace ResultMonad;

/// <summary>
/// Represents a failed result containing an error.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error.</typeparam>
/// <remarks>
/// This record is one variant of the <see cref="Result{TValue, TError}"/> discriminated union,
/// representing the failure state with an associated error value.
/// </remarks>
public sealed record Err<T, E>(E Error) : Result<T, E>
    where E : notnull
    where T : notnull
{
    public E Error { get; } = Error ?? throw new ArgumentNullException(nameof(Error));

    // <inheritdoc/>
    public override bool IsOk => false;
}
