// <copyright file="MapOrElseTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for mapping the Ok value or lazily computing a fallback on <see cref="Result{T, E}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class MapOrElseTaskExtension
{
    extension<T, E>(Task<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result task and returns the synchronously mapped Ok value; otherwise returns the synchronous fallback applied to the error.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The function invoked with the error when the awaited result is Err.</param>
        /// <param name="operation">The function applied to the Ok value.</param>
        /// <returns>A task producing <c>operation(value)</c> when Ok; otherwise <c>fallback(error)</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/>, <paramref name="fallback"/>, or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<U> MapOrElseAsync<U>(Func<E, U> fallback, Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(fallback);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, fallback).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the result task and returns the asynchronously mapped Ok value; otherwise returns the asynchronous fallback applied to the error.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The asynchronous function invoked with the error when the awaited result is Err.</param>
        /// <param name="operation">The asynchronous function applied to the Ok value.</param>
        /// <returns>A task producing <c>operation(value)</c> when Ok; otherwise <c>fallback(error)</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/>, <paramref name="fallback"/>, or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<U> MapOrElseAsync<U>(
            Func<E, Task<U>> fallback,
            Func<T, Task<U>> operation
        )
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(fallback);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, fallback).ConfigureAwait(false);
        }
    }

    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns the asynchronously mapped Ok value when this result is Ok; otherwise returns the asynchronous fallback applied to the error.
        /// </summary>
        /// <typeparam name="U">The type of the returned value.</typeparam>
        /// <param name="fallback">The asynchronous function invoked with the error when this result is Err.</param>
        /// <param name="operation">The asynchronous function applied to the Ok value.</param>
        /// <returns>A task producing <c>operation(value)</c> when Ok; otherwise <c>fallback(error)</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fallback"/> or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<U> MapOrElseAsync<U>(
            Func<E, Task<U>> fallback,
            Func<T, Task<U>> operation
        )
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(fallback);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, fallback).ConfigureAwait(false);
        }
    }
}
