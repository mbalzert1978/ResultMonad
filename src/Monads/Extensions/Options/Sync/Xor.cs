// <copyright file="Xor.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for exclusive-or combination of two <see cref="Option{T}"/> instances.
/// </summary>
public static class XorExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns the option that is Some when exactly one of <paramref name="self"/> or
        /// <paramref name="other"/> is Some; otherwise returns <see cref="Option.None{T}"/>.
        /// </summary>
        /// <param name="other">The other option to combine with this one.</param>
        /// <returns>
        /// The Some option when exactly one is Some; otherwise None.
        /// </returns>
        public Option<T> Xor(Option<T> other)
            => self.Match(
                _ => other.Match(_ => Option.None<T>(), () => self),
                () => other);
    }
}
