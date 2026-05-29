// <copyright file="Filter.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for keeping or discarding the wrapped value of an <see cref="Option{T}"/>
/// based on a predicate.
/// </summary>
public static class FilterExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns this option unchanged when it is Some and <paramref name="predicate"/> returns
        /// <see langword="true"/>; otherwise returns <see cref="Option.None{T}"/>.
        /// </summary>
        /// <param name="predicate">The function used to test the wrapped value.</param>
        /// <returns>
        /// This option when Some and the predicate matches; otherwise None.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public Option<T> Filter(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return self.Match(value => predicate(value) ? self : Option.None<T>(), Option.None<T>);
        }
    }
}
