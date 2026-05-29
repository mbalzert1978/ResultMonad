// <copyright file="MatchValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for pattern matching on <see cref="Option{T}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class MatchValueTaskExtension
{
    extension<T>(ValueTask<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and dispatches to the synchronous Some/None handlers.
        /// </summary>
        /// <typeparam name="U">The result type returned by both branches.</typeparam>
        /// <param name="onSome">Function invoked when the awaited option is Some.</param>
        /// <param name="onNone">Function invoked when the awaited option is None.</param>
        /// <returns>A value task that completes with the value produced by the invoked branch.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="onSome"/> or <paramref name="onNone"/> is <see langword="null"/>.</exception>
        public async ValueTask<U> MatchAsync<U>(Func<T, U> onSome, Func<U> onNone)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(onSome);
            ArgumentNullException.ThrowIfNull(onNone);

            return (await self.ConfigureAwait(false)).Match(onSome, onNone);
        }

        /// <summary>
        /// Awaits the option value task and dispatches to the asynchronous Some/None handlers.
        /// </summary>
        /// <typeparam name="U">The result type returned by both branches.</typeparam>
        /// <param name="onSome">Asynchronous function invoked when the awaited option is Some.</param>
        /// <param name="onNone">Asynchronous function invoked when the awaited option is None.</param>
        /// <returns>A value task that completes with the value produced by the invoked branch.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="onSome"/> or <paramref name="onNone"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the invoked branch returns a null result.</exception>
        public async ValueTask<U> MatchAsync<U>(
            Func<T, ValueTask<U>> onSome,
            Func<ValueTask<U>> onNone
        )
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(onSome);
            ArgumentNullException.ThrowIfNull(onNone);

            return await (await self.ConfigureAwait(false))
                .MatchAsync(onSome, onNone)
                .ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Dispatches to the asynchronous Some/None handlers without awaiting the option itself.
        /// </summary>
        /// <typeparam name="U">The result type returned by both branches.</typeparam>
        /// <param name="onSome">Asynchronous function invoked when this option is Some.</param>
        /// <param name="onNone">Asynchronous function invoked when this option is None.</param>
        /// <returns>A value task that completes with the value produced by the invoked branch.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="onSome"/> or <paramref name="onNone"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the invoked branch returns a null result.</exception>
        public async ValueTask<U> MatchAsync<U>(
            Func<T, ValueTask<U>> onSome,
            Func<ValueTask<U>> onNone
        )
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(onSome);
            ArgumentNullException.ThrowIfNull(onNone);

            return await self.Match(onSome, onNone).ConfigureAwait(false);
        }
    }
}
