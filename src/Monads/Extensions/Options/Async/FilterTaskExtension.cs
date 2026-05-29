// <copyright file="FilterTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for filtering <see cref="Option{T}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class FilterTaskExtension
{
    extension<T>(Task<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option task and returns it when Some and the synchronous predicate matches; otherwise returns None.
        /// </summary>
        /// <param name="predicate">The synchronous predicate applied to the wrapped value when Some.</param>
        /// <returns>A task producing the awaited option when Some and the predicate matches; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public async Task<Option<T>> FilterAsync(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(predicate);

            return await self.MatchAsync(
                    value => predicate(value) ? Option.Some(value) : Option.None<T>(),
                    Option.None<T>
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option task and returns it when Some and the asynchronous predicate matches; otherwise returns None.
        /// </summary>
        /// <param name="predicate">The asynchronous predicate applied to the wrapped value when Some.</param>
        /// <returns>A task producing the awaited option when Some and the predicate matches; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public async Task<Option<T>> FilterAsync(Func<T, Task<bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(predicate);

            return await self.MatchAsync(
                    async value =>
                        await predicate(value).ConfigureAwait(false)
                            ? Option.Some(value)
                            : Option.None<T>(),
                    () => Task.FromResult(Option.None<T>())
                )
                .ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns this option when Some and the asynchronous predicate matches; otherwise returns None.
        /// </summary>
        /// <param name="predicate">The asynchronous predicate applied to the wrapped value when Some.</param>
        /// <returns>A task producing this option when Some and the predicate matches; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public async Task<Option<T>> FilterAsync(Func<T, Task<bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return await self.MatchAsync(
                    async value =>
                        await predicate(value).ConfigureAwait(false)
                            ? Option.Some(value)
                            : Option.None<T>(),
                    () => Task.FromResult(Option.None<T>())
                )
                .ConfigureAwait(false);
        }
    }
}
