// <copyright file="Or.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for combining <see cref="Option{T}"/> instances.
/// </summary>
public static class OrExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns <paramref name="self"/> if it is Some, otherwise returns <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The fallback option to return if <paramref name="self"/> is None.</param>
        /// <returns>
        /// <paramref name="self"/> when it is Some; otherwise <paramref name="other"/>.
        /// </returns>
        public Option<T> Or(Option<T> other)
        {
            return self.Match(_ => self, () => other);
        }
    }
}
