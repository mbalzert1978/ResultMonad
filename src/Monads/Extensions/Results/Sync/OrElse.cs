// <copyright file="OrElse.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension methods for error recovery operations on <see cref="Result{T, E}"/> instances.
/// </summary>
public static class OrElseExtension
{
    /// <summary>
    /// Calls the operation if the result is Err, otherwise returns the Ok value unchanged.
    /// This function can be used for error recovery or providing fallback values.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the original error value.</typeparam>
    /// <typeparam name="F">The type of the new error value.</typeparam>
    /// <param name="self">The result to operate on.</param>
    /// <param name="operation">The function to call with the error value if the result is Err.</param>
    /// <returns>
    /// The original Ok value if the result is Ok, or the result of calling the operation if the result is Err.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="operation"/> returns <see langword="null"/>.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static Result<T, F> OrElse<T, E, F>(
        this Result<T, E> self,
        Func<E, Result<T, F>> operation
    )
        where T : notnull
        where E : notnull
        where F : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(operation);

        return self.Match(Success<T, F>, operation);
    }
}
