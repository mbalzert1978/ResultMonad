// <copyright file="Flatten.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for flattening nested <see cref="Option{T}"/> structures.
/// </summary>
public static class FlattenExtension
{
    extension<T>(Option<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Collapses a nested option into a single-level option.
        /// </summary>
        /// <returns>
        /// The inner option when this option is Some; otherwise <see cref="Option.None{T}"/>.
        /// </returns>
        public Option<T> Flatten() => self.Match(inner => inner, Option.None<T>);
    }
}
