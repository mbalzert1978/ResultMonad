// <copyright file="Zip.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for combining two <see cref="Option{T}"/> instances into a single tupled option.
/// </summary>
public static class ZipExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns <c>Some((self, other))</c> when both options are Some;
        /// otherwise returns <see cref="Option.None{T}"/> of the tuple type.
        /// </summary>
        /// <typeparam name="U">The value type of <paramref name="other"/>.</typeparam>
        /// <param name="other">The option to combine with this one.</param>
        /// <returns>
        /// Some of the tuple <c>(T, U)</c> when both are Some; otherwise None.
        /// </returns>
        public Option<(T, U)> Zip<U>(Option<U> other)
            where U : notnull
            => self.Match(
                t => other.Match(u => Option.Some((t, u)), Option.None<(T, U)>),
                Option.None<(T, U)>);
    }
}
