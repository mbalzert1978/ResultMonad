// <copyright file="Or.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension methods for combining <see cref="Result{T, E}"/> instances.
/// </summary>
public static class OrExtension
{
    /// <summary>
    /// Returns the first result if it is Ok, otherwise returns the second result.
    /// This function can be used for providing fallback results or changing the error type.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the original error value.</typeparam>
    /// <typeparam name="F">The type of the fallback error value.</typeparam>
    /// <param name="self">The result to operate on.</param>
    /// <param name="res">The fallback result to return if the first result is Err.</param>
    /// <returns>
    /// The original Ok value if the result is Ok, or the fallback result if the result is Err.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="res"/> is <see langword="null"/>.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static Result<T, F> Or<T, E, F>(this Result<T, E> self, Result<T, F> res)
        where T : notnull
        where E : notnull
        where F : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(res);

        return self.Match(Success<T, F>, _ => res);
    }
}
