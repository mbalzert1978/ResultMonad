// <copyright file="Err.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results;

/// <summary>
/// Represents a failed result containing an error.
/// </summary>
/// <typeparam name="T">The type of the success value (not used in this variant).</typeparam>
/// <typeparam name="E">The type of the error.</typeparam>
internal sealed record Err<T, E>(E Error) : Result<T, E>
    where E : notnull
    where T : notnull
{
    /// <summary>
    /// Gets the error value contained in this result.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown during construction if the provided error is <c>null</c>.</exception>
    public E Error { get; } = Error ?? throw new ArgumentNullException(nameof(Error));
}
