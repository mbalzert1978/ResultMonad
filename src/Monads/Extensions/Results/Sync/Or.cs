// <copyright file="Or.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for combining <see cref="Result{T, E}"/> instances.
/// </summary>
public static class OrExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns the first result if it is Ok, otherwise returns the second result.
        /// This function can be used for providing fallback results or changing the error type.
        /// </summary>
        /// <typeparam name="F">The type of the fallback error value.</typeparam>
        /// <param name="res">The fallback result to return if the first result is Err.</param>
        /// <returns>
        /// The original Ok value if the result is Ok, or the fallback result if the result is Err.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="res"/> is <see langword="null"/>.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public Result<T, F> Or<F>(Result<T, F> res)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(res);

            return self.Match(Result.Ok<T, F>, _ => res);
        }
    }
}
