// <copyright file="ToOk.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for converting a <see cref="Result{T, E}"/> into an <see cref="Option{T}"/> over its Ok value.
/// </summary>
public static class ToOkExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Converts this result into an <see cref="Option{T}"/>: Ok becomes Some, Err becomes None.
        /// </summary>
        /// <returns>Some carrying the Ok value when this result is Ok; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public Option<T> ToOk()
        {
            ArgumentNullException.ThrowIfNull(self);

            return self.Match(Option.Some, _ => Option.None<T>());
        }
    }
}
