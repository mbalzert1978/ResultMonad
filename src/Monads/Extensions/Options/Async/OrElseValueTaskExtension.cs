// <copyright file="OrElseValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for lazy fallback on <see cref="Option{T}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class OrElseValueTaskExtension
{
    extension<T>(ValueTask<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and returns it when Some; otherwise invokes the synchronous fallback.
        /// </summary>
        /// <param name="operation">The synchronous fallback invoked when the awaited option is None.</param>
        /// <returns>A value task producing the awaited option when Some; otherwise the option produced by <paramref name="operation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<Option<T>> OrElseAsync(Func<Option<T>> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(Option.Some, operation).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option value task and returns it when Some; otherwise invokes the asynchronous fallback.
        /// </summary>
        /// <param name="operation">The asynchronous fallback invoked when the awaited option is None.</param>
        /// <returns>A value task producing the awaited option when Some; otherwise the option produced by <paramref name="operation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<Option<T>> OrElseAsync(Func<ValueTask<Option<T>>> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(
                    value => new ValueTask<Option<T>>(Option.Some(value)),
                    operation)
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
        /// <returns>A value task producing this option when Some; otherwise the option produced by <paramref name="operation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<Option<T>> OrElseAsync(Func<ValueTask<Option<T>>> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(
                    value => new ValueTask<Option<T>>(Option.Some(value)),
                    operation)
                .ConfigureAwait(false);
        }
    }
}
