// <copyright file="UnwrapOr.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for unwrapping <see cref="Result{T, E}"/> values with an eager fallback.
/// </summary>
public static class UnwrapOrExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns the Ok value when this result is Ok; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <param name="fallback">The value to return when this result is Err.</param>
        /// <returns>The Ok value when Ok; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public T UnwrapOr(T fallback)
        {
            ArgumentNullException.ThrowIfNull(self);

            return self.Match(static value => value, _ => fallback);
        }
    }
}
