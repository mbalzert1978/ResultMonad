// <copyright file="MapOr.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for mapping the Ok value of a <see cref="Result{T, E}"/> or returning an eager fallback.
/// </summary>
public static class MapOrExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns the result of applying <paramref name="operation"/> to the Ok value when this result is Ok;
        /// otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The value to return when this result is Err.</param>
        /// <param name="operation">The function to apply to the Ok value when this result is Ok.</param>
        /// <returns><c>operation(value)</c> when this result is Ok; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public U MapOr<U>(U fallback, Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(operation, _ => fallback);
        }
    }
}
