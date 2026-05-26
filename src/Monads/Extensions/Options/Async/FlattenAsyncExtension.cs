// <copyright file="FlattenAsyncExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options.Extensions.Sync;

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides extension members for asynchronously flattening nested <see cref="Option{T}"/> structures.
/// </summary>
public static class FlattenAsyncExtension
{
    extension<T>(Task<Option<Option<T>>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option task and collapses the nested option into a single-level option.
        /// </summary>
        /// <returns>A task producing the inner option when Some; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public async Task<Option<T>> FlattenAsync()
        {
            ArgumentNullException.ThrowIfNull(self);

            return (await self.ConfigureAwait(false)).Flatten();
        }
    }

    extension<T>(ValueTask<Option<Option<T>>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and collapses the nested option into a single-level option.
        /// </summary>
        /// <returns>A value task producing the inner option when Some; otherwise None.</returns>
        public async ValueTask<Option<T>> FlattenAsync()
            => (await self.ConfigureAwait(false)).Flatten();
    }
}
