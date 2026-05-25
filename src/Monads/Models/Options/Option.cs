// <copyright file="Option.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options;

/// <summary>
/// Provides factory methods for constructing <see cref="Option{T}"/> values.
/// </summary>
/// <remarks>
/// Import statically (<c>using static Monads.Options.Option;</c>) to call
/// <see cref="Some{T}(T)"/> and <see cref="None{T}"/> without qualification.
/// </remarks>
public static class Option
{
    /// <summary>
    /// Creates an <see cref="Option{T}"/> in the Some state wrapping the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value.</typeparam>
    /// <param name="value">The non-null value to wrap.</param>
    /// <returns>A new Some <see cref="Option{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    public static Option<T> Some<T>(T value)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Option<T>(value);
    }

    /// <summary>
    /// Returns an empty <see cref="Option{T}"/> (None).
    /// </summary>
    /// <typeparam name="T">The type parameter of the option.</typeparam>
    /// <returns>An <see cref="Option{T}"/> in the None state.</returns>
    public static Option<T> None<T>()
        where T : notnull
        => default;
}
