// <copyright file="UnwrapOr.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for unwrapping <see cref="Option{T}"/> values with a fallback.
/// </summary>
public static class UnwrapOrExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns the wrapped value when this option is Some; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <param name="fallback">The value to return when this option is None.</param>
        /// <returns>The wrapped value when Some; otherwise <paramref name="fallback"/>.</returns>
        public T UnwrapOr(T fallback) => self.Match(static value => value, () => fallback);
    }
}
