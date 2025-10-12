// <copyright file="Err.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace ResultMonad;

/// <summary>
/// Represents a failed result containing an error.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
/// <remarks>
/// This record is one variant of the <see cref="Result{TValue, TError}"/> discriminated union,
/// representing the failure state with an associated error value.
/// </remarks>
public sealed record Err<TValue, TError>(TError Error) : Result<TValue, TError>
    where TError : notnull
    where TValue : notnull
{
    public TError Error { get; } = Error ?? throw new ArgumentNullException(nameof(Error));

    // <inheritdoc/>
    public override bool IsOk => false;
}
