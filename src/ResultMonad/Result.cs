// <copyright file="Result.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace ResultMonad;

/// <summary>
/// Represents the base type for a discriminated union modeling either a success or an error.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
/// <remarks>
/// This abstract record serves as the foundation for the Result monad pattern.
/// Use derived types <see cref="Ok{TValue, TError}"/> and <see cref="Err{TValue, TError}"/>
/// to represent success and failure states respectively.
/// </remarks>
public abstract record Result<TValue, TError>
    where TError : notnull
    where TValue : notnull
{
    /// <summary>
    /// Gets a value indicating whether the result represents a success.
    /// </summary>
    /// <value>
    /// <c>true</c> if the result is <see cref="Ok{TValue, TError}"/>; otherwise, <c>false</c>.
    /// </value>
    public abstract bool IsOk { get; }

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
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value to wrap in the result.</param>
    /// <returns>A new <see cref="ResultMonad.Ok{TValue, TError}"/> instance containing the specified value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static Result<TValue, TError> Ok<TValue, TError>(TValue value)
        where TError : notnull
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(value);

        Ok<TValue, TError> result = new(value);

        Debug.Assert(result.IsOk, "Created Ok result must have IsOk == true.");

        return result;
    }

    /// <summary>
    /// Creates a failed result containing the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error to wrap in the result.</param>
    /// <returns>A new <see cref="ResultMonad.Err{TValue, TError}"/> instance containing the specified error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is null.</exception>
    public static Result<TValue, TError> Err<TValue, TError>(TError error)
        where TError : notnull
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(error);

        Err<TValue, TError> result = new(error);

        Debug.Assert(result.IsErr, "Created Err result must have IsErr == true.");

        return result;
    }
}
