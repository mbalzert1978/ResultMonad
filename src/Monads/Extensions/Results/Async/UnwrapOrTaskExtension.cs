// <copyright file="UnwrapOrTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for unwrapping <see cref="Result{T, E}"/> values with a fallback using <see cref="Task{TResult}"/>.
/// </summary>
public static class UnwrapOrTaskExtension
{
    extension<T, E>(Task<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result task and returns the Ok value; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <param name="fallback">The value to return when the awaited result is Err.</param>
        /// <returns>A task producing the Ok value when Ok; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public async Task<T> UnwrapOrAsync(T fallback)
        {
            ArgumentNullException.ThrowIfNull(self);

            return await self.MatchAsync(static value => value, _ => fallback)
                .ConfigureAwait(false);
        }
    }
}
