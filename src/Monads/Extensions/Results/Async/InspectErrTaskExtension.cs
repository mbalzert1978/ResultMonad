// <copyright file="InspectErrTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results.Extensions.Sync;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for observing the Err value of a <see cref="Result{T, E}"/> via a side-effecting action, using <see cref="Task{TResult}"/>.
/// </summary>
public static class InspectErrTaskExtension
{
    extension<T, E>(Task<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result task and invokes <paramref name="action"/> when the result is Err, then returns it unchanged.
        /// </summary>
        /// <param name="action">The synchronous action invoked with the error value.</param>
        /// <returns>A task producing the original result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
        public async Task<Result<T, E>> InspectErrAsync(Action<E> action)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(action);

            Result<T, E> result = await self.ConfigureAwait(false);
            return result.InspectErr(action);
        }

        /// <summary>
        /// Awaits the result task and awaits <paramref name="action"/> when the result is Err, then returns it unchanged.
        /// </summary>
        /// <param name="action">The asynchronous action invoked with the error value.</param>
        /// <returns>A task producing the original result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
        public async Task<Result<T, E>> InspectErrAsync(Func<E, Task> action)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(action);

            Result<T, E> result = await self.ConfigureAwait(false);
            return await result.InspectErrAsync(action).ConfigureAwait(false);
        }
    }

    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits <paramref name="action"/> with the error value when this result is Err, then returns it unchanged.
        /// </summary>
        /// <param name="action">The asynchronous action invoked with the error value.</param>
        /// <returns>A task producing this result unchanged.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        public async Task<Result<T, E>> InspectErrAsync(Func<E, Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            await self.MatchAsync(
                    _ => Task.FromResult(Unit.Default),
                    async error =>
                    {
                        await action(error).ConfigureAwait(false);
                        return Unit.Default;
                    })
                .ConfigureAwait(false);

            return self;
        }
    }
}
