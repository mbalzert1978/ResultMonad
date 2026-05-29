// <copyright file="Ok.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results;

/// <summary>
/// Represents a successful result containing a value.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error (not used in this variant).</typeparam>
internal sealed record Ok<T, E>(T Value) : Result<T, E>
    where E : notnull
    where T : notnull
{
    /// <summary>
    /// Gets the success value contained in this result.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown during construction if the provided value is <c>null</c>.</exception>
    public T Value { get; } = Value ?? throw new ArgumentNullException(nameof(Value));
}
