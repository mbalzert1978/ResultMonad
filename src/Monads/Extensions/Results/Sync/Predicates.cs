// <copyright file="Predicates.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides predicate extension members for <see cref="Result{T, E}"/> instances,
/// all implemented exclusively in terms of <see cref="MatchExtension.Match"/>.
/// </summary>
public static class PredicateExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Gets a value indicating whether the result represents a success.
        /// </summary>
        public bool IsOk => self.Match(static _ => true, static _ => false);

        /// <summary>
        /// Gets a value indicating whether the result represents a failure.
        /// </summary>
        public bool IsErr => self.Match(static _ => false, static _ => true);

        /// <summary>
        /// Determines whether the result is a success and its value satisfies the specified predicate.
        /// </summary>
        /// <param name="predicate">The function to test the success value.</param>
        /// <returns><c>true</c> if the result is a success and <paramref name="predicate"/> returns <c>true</c>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <c>null</c>.</exception>
        public bool IsOkAnd(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return self.Match(predicate, static _ => false);
        }

        /// <summary>
        /// Determines whether the result is a failure and its error satisfies the specified predicate.
        /// </summary>
        /// <param name="predicate">The function to test the error value.</param>
        /// <returns><c>true</c> if the result is a failure and <paramref name="predicate"/> returns <c>true</c>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <c>null</c>.</exception>
        public bool IsErrAnd(Func<E, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return self.Match(static _ => false, predicate);
        }
    }
}
