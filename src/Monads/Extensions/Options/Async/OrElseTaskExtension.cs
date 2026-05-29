// <copyright file="OrElseTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for lazy fallback on <see cref="Option{T}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class OrElseTaskExtension
{
    extension<T>(Task<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option task and returns it when Some; otherwise invokes the synchronous fallback.
        /// </summary>
        /// <param name="operation">The synchronous fallback invoked when the awaited option is None.</param>
        /// <returns>A task producing the awaited option when Some; otherwise the option produced by <paramref name="operation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<Option<T>> OrElseAsync(Func<Option<T>> operation)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(Option.Some, operation).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option task and returns it when Some; otherwise invokes the asynchronous fallback.
        /// </summary>
        /// <param name="operation">The asynchronous fallback invoked when the awaited option is None.</param>
        /// <returns>A task producing the awaited option when Some; otherwise the option produced by <paramref name="operation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<Option<T>> OrElseAsync(Func<Task<Option<T>>> operation)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(value => Task.FromResult(Option.Some(value)), operation)
                .ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Returns this option when Some; otherwise invokes the asynchronous fallback.
        /// </summary>
        /// <param name="operation">The asynchronous fallback invoked when this option is None.</param>
        /// <returns>A task producing this option when Some; otherwise the option produced by <paramref name="operation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async Task<Option<T>> OrElseAsync(Func<Task<Option<T>>> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(value => Task.FromResult(Option.Some(value)), operation)
                .ConfigureAwait(false);
        }
    }
}
