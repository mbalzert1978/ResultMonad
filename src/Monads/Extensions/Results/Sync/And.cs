// <copyright file="And.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for short-circuiting <see cref="Result{T, E}"/> combinations.
/// </summary>
public static class AndExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns <paramref name="res"/> when this result is Ok; otherwise propagates this result's Err with the new value type.
        /// </summary>
        /// <typeparam name="U">The success value type of <paramref name="res"/>.</typeparam>
        /// <param name="res">The result to return when this result is Ok.</param>
        /// <returns><paramref name="res"/> when this result is Ok; otherwise an Err carrying this result's error.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="res"/> is <see langword="null"/>.</exception>
        public Result<U, E> And<U>(Result<U, E> res)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(res);

            return self.Match(_ => res, Result.Err<U, E>);
        }
    }
}
