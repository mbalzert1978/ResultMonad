// <copyright file="Predicates.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides predicate extension members for <see cref="Option{T}"/> instances,
/// all implemented exclusively in terms of <c>Match</c>.
/// </summary>
public static class PredicateExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Determines whether this option is Some and its value satisfies the specified predicate.
        /// </summary>
        /// <param name="predicate">The function to test the wrapped value.</param>
        /// <returns>
        /// <see langword="true"/> when this option is Some and <paramref name="predicate"/> returns
        /// <see langword="true"/>; otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public bool IsSomeAnd(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return self.Match(predicate, static () => false);
        }

        /// <summary>
        /// Determines whether this option is None, or its value satisfies the specified predicate.
        /// </summary>
        /// <param name="predicate">The function to test the wrapped value.</param>
        /// <returns>
        /// <see langword="true"/> when this option is None, or when this option is Some and
        /// <paramref name="predicate"/> returns <see langword="true"/>; otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public bool IsNoneOr(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return self.Match(predicate, static () => true);
        }
    }
}
