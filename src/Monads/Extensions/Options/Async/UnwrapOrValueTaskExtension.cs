// <copyright file="UnwrapOrValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for unwrapping <see cref="Option{T}"/> values with a fallback using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class UnwrapOrValueTaskExtension
{
    extension<T>(ValueTask<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and returns its wrapped value when Some; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <param name="fallback">The value to return when the awaited option is None.</param>
        /// <returns>A value task producing the wrapped value when Some; otherwise <paramref name="fallback"/>.</returns>
        public async ValueTask<T> UnwrapOrAsync(T fallback) =>
            await self.MatchAsync(static value => value, () => fallback).ConfigureAwait(false);
    }
}
