// <copyright file="UnwrapOrElse.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for unwrapping <see cref="Option{T}"/> values with a lazy fallback.
/// </summary>
public static class UnwrapOrElseExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns the wrapped value when this option is Some; otherwise invokes <paramref name="fallback"/>
        /// and returns its result.
        /// </summary>
        /// <param name="fallback">The function invoked when this option is None.</param>
        /// <returns>The wrapped value when Some; otherwise <c>fallback()</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fallback"/> is <see langword="null"/>.</exception>
        public T UnwrapOrElse(Func<T> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            return self.Match(static value => value, fallback);
        }
    }
}
