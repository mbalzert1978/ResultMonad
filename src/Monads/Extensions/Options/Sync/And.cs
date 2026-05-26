// <copyright file="And.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for short-circuiting <see cref="Option{T}"/> combinations.
/// </summary>
public static class AndExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns <paramref name="other"/> when this option is Some;
        /// otherwise returns <see cref="Option.None{U}"/>.
        /// </summary>
        /// <typeparam name="U">The value type of <paramref name="other"/>.</typeparam>
        /// <param name="other">The option to return when this option is Some.</param>
        /// <returns>
        /// <paramref name="other"/> when this option is Some; otherwise <see cref="Option.None{U}"/>.
        /// </returns>
        public Option<U> And<U>(Option<U> other)
            where U : notnull
            => self.Match(_ => other, Option.None<U>);
    }
}
