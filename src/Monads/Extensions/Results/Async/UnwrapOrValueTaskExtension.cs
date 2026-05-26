// <copyright file="UnwrapOrValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for unwrapping <see cref="Result{T, E}"/> values with a fallback using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class UnwrapOrValueTaskExtension
{
    extension<T, E>(ValueTask<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result value task and returns the Ok value; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <param name="fallback">The value to return when the awaited result is Err.</param>
        /// <returns>A value task producing the Ok value when Ok; otherwise <paramref name="fallback"/>.</returns>
        public async ValueTask<T> UnwrapOrAsync(T fallback)
            => await self.MatchAsync(static value => value, _ => fallback)
                .ConfigureAwait(false);
    }
}
