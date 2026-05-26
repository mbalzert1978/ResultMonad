// <copyright file="MapOrElseTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for mapping a value or lazily computing a fallback on <see cref="Option{T}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class MapOrElseTaskExtension
{
    extension<T>(Task<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option task and returns the synchronously mapped value when Some; otherwise returns the result of the synchronous fallback.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The synchronous function invoked when the awaited option is None.</param>
        /// <param name="operation">The synchronous function applied when Some.</param>
        /// <returns>A task producing the mapped value when Some; otherwise <c>fallback()</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/>, <paramref name="fallback"/>, or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<U> MapOrElseAsync<U>(Func<U> fallback, Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(fallback);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, fallback).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option task and returns the asynchronously mapped value when Some; otherwise returns the result of the asynchronous fallback.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The asynchronous function invoked when the awaited option is None.</param>
        /// <param name="operation">The asynchronous function applied when Some.</param>
        /// <returns>A task producing the mapped value when Some; otherwise <c>fallback()</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/>, <paramref name="fallback"/>, or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<U> MapOrElseAsync<U>(Func<Task<U>> fallback, Func<T, Task<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(fallback);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, fallback).ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns the asynchronously mapped value when this option is Some; otherwise returns the result of the asynchronous fallback.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The asynchronous function invoked when this option is None.</param>
        /// <param name="operation">The asynchronous function applied when Some.</param>
        /// <returns>A task producing the mapped value when Some; otherwise <c>fallback()</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fallback"/> or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<U> MapOrElseAsync<U>(Func<Task<U>> fallback, Func<T, Task<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(fallback);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, fallback).ConfigureAwait(false);
        }
    }
}
