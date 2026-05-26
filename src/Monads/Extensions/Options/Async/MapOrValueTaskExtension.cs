// <copyright file="MapOrValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for mapping a value or returning a fallback on <see cref="Option{T}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class MapOrValueTaskExtension
{
    extension<T>(ValueTask<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and returns the synchronously mapped value when Some; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The value to return when the awaited option is None.</param>
        /// <param name="operation">The synchronous function applied when Some.</param>
        /// <returns>A value task producing the mapped value when Some; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<U> MapOrAsync<U>(U fallback, Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, () => fallback).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option value task and returns the asynchronously mapped value when Some; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The value to return when the awaited option is None.</param>
        /// <param name="operation">The asynchronous function applied when Some.</param>
        /// <returns>A value task producing the mapped value when Some; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<U> MapOrAsync<U>(U fallback, Func<T, ValueTask<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, () => new ValueTask<U>(fallback))
                .ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns the asynchronously mapped value when this option is Some; otherwise returns <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The value to return when this option is None.</param>
        /// <param name="operation">The asynchronous function applied when Some.</param>
        /// <returns>A value task producing the mapped value when Some; otherwise <paramref name="fallback"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<U> MapOrAsync<U>(U fallback, Func<T, ValueTask<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, () => new ValueTask<U>(fallback))
                .ConfigureAwait(false);
        }
    }
}
