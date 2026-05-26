// <copyright file="MapOrElse.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for mapping the Ok value of a <see cref="Result{T, E}"/> or lazily computing a fallback from the error.
/// </summary>
public static class MapOrElseExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns the result of applying <paramref name="operation"/> to the Ok value when this result is Ok;
        /// otherwise invokes <paramref name="fallback"/> with the Err value and returns its result.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The function invoked with the error when this result is Err.</param>
        /// <param name="operation">The function applied to the Ok value when this result is Ok.</param>
        /// <returns><c>operation(value)</c> when Ok; otherwise <c>fallback(error)</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/>, <paramref name="fallback"/>, or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public U MapOrElse<U>(Func<E, U> fallback, Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(fallback);
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(operation, fallback);
        }
    }
}
