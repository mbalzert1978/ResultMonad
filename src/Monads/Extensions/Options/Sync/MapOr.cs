// <copyright file="MapOr.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for mapping a value or returning a fallback on <see cref="Option{T}"/> instances.
/// </summary>
public static class MapOrExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns the result of applying <paramref name="operation"/> to the wrapped value when this option is Some;
        /// otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The value to return when this option is None.</param>
        /// <param name="operation">The function to apply to the wrapped value when this option is Some.</param>
        /// <returns>
        /// <c>operation(value)</c> when this option is Some; otherwise <paramref name="fallback"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public U MapOr<U>(U fallback, Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(operation, () => fallback);
        }
    }
}
