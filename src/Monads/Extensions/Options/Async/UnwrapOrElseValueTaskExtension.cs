// <copyright file="UnwrapOrElseValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for unwrapping <see cref="Option{T}"/> values with a lazy fallback using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class UnwrapOrElseValueTaskExtension
{
    extension<T>(ValueTask<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and returns its wrapped value when Some; otherwise returns the result of the synchronous fallback.
        /// </summary>
        /// <param name="fallback">The synchronous function invoked when the awaited option is None.</param>
        /// <returns>A value task producing the wrapped value when Some; otherwise <c>fallback()</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fallback"/> is <see langword="null"/>.</exception>
        public async ValueTask<T> UnwrapOrElseAsync(Func<T> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            return await self.MatchAsync(static value => value, fallback).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option value task and returns its wrapped value when Some; otherwise returns the result of the asynchronous fallback.
        /// </summary>
        /// <param name="fallback">The asynchronous function invoked when the awaited option is None.</param>
        /// <returns>A value task producing the wrapped value when Some; otherwise <c>fallback()</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fallback"/> is <see langword="null"/>.</exception>
        public async ValueTask<T> UnwrapOrElseAsync(Func<ValueTask<T>> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            return await self.MatchAsync(value => new ValueTask<T>(value), fallback)
                .ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns the wrapped value when this option is Some; otherwise returns the result of the asynchronous fallback.
        /// </summary>
        /// <param name="fallback">The asynchronous function invoked when this option is None.</param>
        /// <returns>A value task producing the wrapped value when Some; otherwise <c>fallback()</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fallback"/> is <see langword="null"/>.</exception>
        public async ValueTask<T> UnwrapOrElseAsync(Func<ValueTask<T>> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            return await self.MatchAsync(value => new ValueTask<T>(value), fallback)
                .ConfigureAwait(false);
        }
    }
}
