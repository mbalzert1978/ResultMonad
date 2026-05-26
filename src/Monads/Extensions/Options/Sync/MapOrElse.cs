// <copyright file="MapOrElse.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for mapping a value or lazily computing a fallback on <see cref="Option{T}"/> instances.
/// </summary>
public static class MapOrElseExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns the result of applying <paramref name="operation"/> to the wrapped value when this option is Some;
        /// otherwise invokes <paramref name="fallback"/> and returns its result.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The function invoked when this option is None.</param>
        /// <param name="operation">The function to apply to the wrapped value when this option is Some.</param>
        /// <returns>
        /// <c>operation(value)</c> when this option is Some; otherwise <c>fallback()</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="fallback"/> or <paramref name="operation"/> is <see langword="null"/>.
        /// </exception>
        public U MapOrElse<U>(Func<U> fallback, Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(fallback);
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(operation, fallback);
        }
    }
}
