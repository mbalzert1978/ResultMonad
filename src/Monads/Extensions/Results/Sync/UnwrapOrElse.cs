// <copyright file="UnwrapOrElse.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for unwrapping <see cref="Result{T, E}"/> values with a lazy fallback computed from the error.
/// </summary>
public static class UnwrapOrElseExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns the Ok value when this result is Ok; otherwise invokes <paramref name="fallback"/> with the error and returns its result.
        /// </summary>
        /// <param name="fallback">The function invoked with the error when this result is Err.</param>
        /// <returns>The Ok value when Ok; otherwise <c>fallback(error)</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="fallback"/> is <see langword="null"/>.</exception>
        public T UnwrapOrElse(Func<E, T> fallback)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(fallback);

            return self.Match(static value => value, fallback);
        }
    }
}
