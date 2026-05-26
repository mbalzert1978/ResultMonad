// <copyright file="OrElse.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for lazy fallback on <see cref="Option{T}"/> instances.
/// </summary>
public static class OrElseExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns <paramref name="self"/> if it is Some, otherwise invokes
        /// <paramref name="operation"/> and returns its result.
        /// </summary>
        /// <param name="operation">The fallback factory invoked when <paramref name="self"/> is None.</param>
        /// <returns>
        /// <paramref name="self"/> when it is Some; otherwise the result of <paramref name="operation"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public Option<T> OrElse(Func<Option<T>> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(_ => self, operation);
        }
    }
}
