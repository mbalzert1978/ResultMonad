// <copyright file="Result.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results;

/// <summary>
/// Provides factory methods for constructing <see cref="Result{T, E}"/> values.
/// </summary>
/// <remarks>
/// Import statically (<c>using static Monads.Results.Result;</c>) to call
/// <see cref="Ok{T, E}(T)"/> and <see cref="Err{T, E}(E)"/> without qualification.
/// </remarks>
public static class Result
{
    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error.</typeparam>
    /// <param name="value">The value to wrap in the result.</param>
    /// <returns>A new successful <see cref="Result{T, E}"/> containing the specified value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    public static Result<T, E> Ok<T, E>(T value)
        where T : notnull
        where E : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Ok<T, E>(value);
    }

    /// <summary>
    /// Creates a failed result containing the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error.</typeparam>
    /// <param name="error">The error to wrap in the result.</param>
    /// <returns>A new failed <see cref="Result{T, E}"/> containing the specified error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <c>null</c>.</exception>
    public static Result<T, E> Err<T, E>(E error)
        where T : notnull
        where E : notnull
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Err<T, E>(error);
    }
}
