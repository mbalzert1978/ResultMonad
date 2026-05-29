// <copyright file="InspectValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results.Extensions.Sync;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for observing the Ok value of a <see cref="Result{T, E}"/> via a side-effecting action, using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class InspectValueTaskExtension
{
    extension<T, E>(ValueTask<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result value task and invokes <paramref name="action"/> when the result is Ok, then returns it unchanged.
        /// </summary>
        /// <param name="action">The synchronous action invoked with the Ok value.</param>
        /// <returns>A value task producing the original result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        public async ValueTask<Result<T, E>> InspectAsync(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            Result<T, E> result = await self.ConfigureAwait(false);
            return result.Inspect(action);
        }

        /// <summary>
        /// Awaits the result value task and awaits <paramref name="action"/> when the result is Ok, then returns it unchanged.
        /// </summary>
        /// <param name="action">The asynchronous action invoked with the Ok value.</param>
        /// <returns>A value task producing the original result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        public async ValueTask<Result<T, E>> InspectAsync(Func<T, ValueTask> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            Result<T, E> result = await self.ConfigureAwait(false);
            return await result.InspectAsync(action).ConfigureAwait(false);
        }
    }

    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits <paramref name="action"/> with the Ok value when this result is Ok, then returns it unchanged.
        /// </summary>
        /// <param name="action">The asynchronous action invoked with the Ok value.</param>
        /// <returns>A value task producing this result unchanged.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        public async ValueTask<Result<T, E>> InspectAsync(Func<T, ValueTask> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            await self.MatchAsync(
                    async value =>
                    {
                        await action(value).ConfigureAwait(false);
                        return Unit.Default;
                    },
                    _ => ValueTask.FromResult(Unit.Default)
                )
                .ConfigureAwait(false);

            return self;
        }
    }
}
