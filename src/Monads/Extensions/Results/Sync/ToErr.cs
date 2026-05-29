// <copyright file="ToErr.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for converting a <see cref="Result{T, E}"/> into an <see cref="Option{T}"/> over its Err value.
/// </summary>
public static class ToErrExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Converts this result into an <see cref="Option{T}"/>: Err becomes Some carrying the error, Ok becomes None.
        /// </summary>
        /// <returns>Some carrying the error when this result is Err; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public Option<E> ToErr()
        {
            ArgumentNullException.ThrowIfNull(self);

            return self.Match(_ => Option.None<E>(), Option.Some);
        }
    }
}
