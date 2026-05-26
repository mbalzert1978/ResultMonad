// <copyright file="UnwrapOrTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for unwrapping <see cref="Option{T}"/> values with a fallback using <see cref="Task{TResult}"/>.
/// </summary>
public static class UnwrapOrTaskExtension
{
    extension<T>(Task<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option task and returns its wrapped value when Some; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <param name="fallback">The value to return when the awaited option is None.</param>
        /// <returns>A task producing the wrapped value when Some; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public async Task<T> UnwrapOrAsync(T fallback)
        {
            ArgumentNullException.ThrowIfNull(self);

            return await self.MatchAsync(static value => value, () => fallback)
                .ConfigureAwait(false);
        }
    }
}
