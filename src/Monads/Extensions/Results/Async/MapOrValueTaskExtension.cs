// <copyright file="MapOrValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for mapping the Ok value or returning a fallback on <see cref="Result{T, E}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class MapOrValueTaskExtension
{
    extension<T, E>(ValueTask<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result value task and returns the synchronously mapped Ok value; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The value returned when the awaited result is Err.</param>
        /// <param name="operation">The synchronous function applied to the Ok value.</param>
        /// <returns>A value task producing the mapped value when Ok; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<U> MapOrAsync<U>(U fallback, Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, _ => fallback).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the result value task and returns the asynchronously mapped Ok value; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The value returned when the awaited result is Err.</param>
        /// <param name="operation">The asynchronous function applied to the Ok value.</param>
        /// <returns>A value task producing the mapped value when Ok; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<U> MapOrAsync<U>(U fallback, Func<T, ValueTask<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, _ => ValueTask.FromResult(fallback))
                .ConfigureAwait(false);
        }
    }

    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns the asynchronously mapped Ok value when this result is Ok; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The value returned when this result is Err.</param>
        /// <param name="operation">The asynchronous function applied to the Ok value.</param>
        /// <returns>A value task producing the mapped value when Ok; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<U> MapOrAsync<U>(U fallback, Func<T, ValueTask<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, _ => ValueTask.FromResult(fallback))
                .ConfigureAwait(false);
        }
    }
}
