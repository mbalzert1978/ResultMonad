// <copyright file="UnwrapOrElseTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for unwrapping <see cref="Result{T, E}"/> values with a lazy fallback computed from the error, using <see cref="Task{TResult}"/>.
/// </summary>
public static class UnwrapOrElseTaskExtension
{
    extension<T, E>(Task<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result task and returns the Ok value; otherwise returns the synchronous fallback applied to the error.
        /// </summary>
        /// <param name="fallback">The synchronous function invoked with the error when the awaited result is Err.</param>
        /// <returns>A task producing the Ok value when Ok; otherwise <c>fallback(error)</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="fallback"/> is <see langword="null"/>.</exception>
        public async Task<T> UnwrapOrElseAsync(Func<E, T> fallback)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(fallback);

            return await self.MatchAsync(static value => value, fallback).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the result task and returns the Ok value; otherwise returns the asynchronous fallback applied to the error.
        /// </summary>
        /// <param name="fallback">The asynchronous function invoked with the error when the awaited result is Err.</param>
        /// <returns>A task producing the Ok value when Ok; otherwise <c>fallback(error)</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="fallback"/> is <see langword="null"/>.</exception>
        public async Task<T> UnwrapOrElseAsync(Func<E, Task<T>> fallback)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(fallback);

            return await self.MatchAsync(Task.FromResult, fallback).ConfigureAwait(false);
        }
    }

    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Returns the Ok value when this result is Ok; otherwise returns the asynchronous fallback applied to the error.
        /// </summary>
        /// <param name="fallback">The asynchronous function invoked with the error when this result is Err.</param>
        /// <returns>A task producing the Ok value when Ok; otherwise <c>fallback(error)</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fallback"/> is <see langword="null"/>.</exception>
        public async Task<T> UnwrapOrElseAsync(Func<E, Task<T>> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            return await self.MatchAsync(Task.FromResult, fallback).ConfigureAwait(false);
        }
    }
}
