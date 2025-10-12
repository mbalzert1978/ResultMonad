// <copyright file="Result.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results;

/// <summary>
/// Represents the base type for a discriminated union modeling either a success or an error.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error value.</typeparam>
/// <remarks>
/// This abstract record serves as the foundation for the Result monad pattern.
/// Use derived types <see cref="Ok{TValue, TError}"/> and <see cref="Err{TValue, TError}"/>
/// to represent success and failure states respectively.
/// </remarks>
public abstract record Result<T, E>
    where E : notnull
    where T : notnull
{
    /// <summary>
    /// Gets a value indicating whether the result represents a success.
    /// </summary>
    /// <value>
    /// <c>true</c> if the result is <see cref="Ok{TValue, TError}"/>; otherwise, <c>false</c>.
    /// </value>
    public abstract bool IsOk { get; }

    /// <summary>
    /// Determines whether the result is <see cref="Ok{TValue, TError}"/> and satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">The function to test the success value.</param>
    /// <returns>
    /// <c>true</c> if the result is <see cref="Ok{TValue, TError}"/> and the predicate returns <c>true</c>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <c>null</c>.</exception>
    public abstract bool IsOkAnd(Func<T, bool> predicate);

    /// <summary>
    /// Determines whether the result is <see cref="Err{TValue, TError}"/> and satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">The function to test the error value.</param>
    /// <returns>
    /// <c>true</c> if the result is <see cref="Err{TValue, TError}"/> and the predicate returns <c>true</c>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <c>null</c>.</exception>
    public abstract bool IsErrAnd(Func<E, bool> predicate);

    /// <summary>
    /// Gets a value indicating whether the result represents an error.
    /// </summary>
    /// <value>
    /// <c>true</c> if the result is <see cref="Err{TValue, TError}"/>; otherwise, <c>false</c>.
    /// </value>
    public bool IsErr => !IsOk;
}

/// <summary>
/// Provides factory methods for creating <see cref="Result{TValue, TError}"/> instances.
/// </summary>
public static class ResultFactory
{
    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error.</typeparam>
    /// <param name="value">The value to wrap in the result.</param>
    /// <returns>A new <see cref="Ok{TValue, TError}"/> instance containing the specified value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static Result<T, E> Success<T, E>(T value)
        where E : notnull
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(value);

        Ok<T, E> result = new(value);

        Debug.Assert(result.IsOk, "Created Ok result must have IsOk == true.");

        return result;
    }

    /// <summary>
    /// Creates a failed result containing the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error.</typeparam>
    /// <param name="error">The error to wrap in the result.</param>
    /// <returns>A new <see cref="Err{TValue, TError}"/> instance containing the specified error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is null.</exception>
    public static Result<T, E> Failure<T, E>(E error)
        where E : notnull
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(error);

        Err<T, E> result = new(error);

        Debug.Assert(result.IsErr, "Created Err result must have IsErr == true.");

        return result;
    }
}
