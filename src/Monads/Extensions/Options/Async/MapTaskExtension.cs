// <copyright file="MapTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for mapping the wrapped value of <see cref="Option{T}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class MapTaskExtension
{
    extension<T>(Task<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option task and transforms its Some value using a synchronous mapping function.
        /// </summary>
        /// <typeparam name="U">The result type of the mapped value.</typeparam>
        /// <param name="operation">The synchronous function applied to the wrapped value when Some.</param>
        /// <returns>A task producing Some of the mapped value when Some; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<Option<U>> MapAsync<U>(Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(value => Option.Some(operation(value)), Option.None<U>)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option task and transforms its Some value using an asynchronous mapping function.
        /// </summary>
        /// <typeparam name="U">The result type of the mapped value.</typeparam>
        /// <param name="operation">The asynchronous function applied to the wrapped value when Some.</param>
        /// <returns>A task producing Some of the mapped value when Some; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<Option<U>> MapAsync<U>(Func<T, Task<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(
                    async value => Option.Some(await operation(value).ConfigureAwait(false)),
                    () => Task.FromResult(Option.None<U>()))
                .ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Transforms the wrapped value of this synchronous option using an asynchronous mapping function.
        /// </summary>
        /// <typeparam name="U">The result type of the mapped value.</typeparam>
        /// <param name="operation">The asynchronous function applied to the wrapped value when Some.</param>
        /// <returns>A task producing Some of the mapped value when Some; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<Option<U>> MapAsync<U>(Func<T, Task<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(
                    async value => Option.Some(await operation(value).ConfigureAwait(false)),
                    () => Task.FromResult(Option.None<U>()))
                .ConfigureAwait(false);
        }
    }
}
