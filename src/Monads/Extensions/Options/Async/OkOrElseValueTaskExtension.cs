// <copyright file="OkOrElseValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for converting <see cref="Option{T}"/> instances into
/// <see cref="Result{T, E}"/> values with a lazily-computed error using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class OkOrElseValueTaskExtension
{
    extension<T>(ValueTask<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and transforms it into a <see cref="Result{T, E}"/>:
        /// Some becomes Ok, None becomes Err with <c>error()</c>.
        /// </summary>
        /// <typeparam name="E">The error type carried by the resulting Err variant.</typeparam>
        /// <param name="error">The synchronous function invoked to produce the error when None.</param>
        /// <returns>A value task producing the corresponding <see cref="Result{T, E}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
        public async ValueTask<Result<T, E>> OkOrElseAsync<E>(Func<E> error)
            where E : notnull
        {
            ArgumentNullException.ThrowIfNull(error);

            return await self.MatchAsync(Result.Ok<T, E>, () => Result.Err<T, E>(error()))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option value task and transforms it into a <see cref="Result{T, E}"/>:
        /// Some becomes Ok, None becomes Err with the value produced by the asynchronous error factory.
        /// </summary>
        /// <typeparam name="E">The error type carried by the resulting Err variant.</typeparam>
        /// <param name="error">The asynchronous function invoked to produce the error when None.</param>
        /// <returns>A value task producing the corresponding <see cref="Result{T, E}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
        public async ValueTask<Result<T, E>> OkOrElseAsync<E>(Func<ValueTask<E>> error)
            where E : notnull
        {
            ArgumentNullException.ThrowIfNull(error);

            return await self.MatchAsync(
                    value => new ValueTask<Result<T, E>>(Result.Ok<T, E>(value)),
                    async () => Result.Err<T, E>(await error().ConfigureAwait(false)))
                .ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Transforms this option into a <see cref="Result{T, E}"/>:
        /// Some becomes Ok, None becomes Err with the value produced by the asynchronous error factory.
        /// </summary>
        /// <typeparam name="E">The error type carried by the resulting Err variant.</typeparam>
        /// <param name="error">The asynchronous function invoked to produce the error when None.</param>
        /// <returns>A value task producing the corresponding <see cref="Result{T, E}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
        public async ValueTask<Result<T, E>> OkOrElseAsync<E>(Func<ValueTask<E>> error)
            where E : notnull
        {
            ArgumentNullException.ThrowIfNull(error);

            return await self.MatchAsync(
                    value => new ValueTask<Result<T, E>>(Result.Ok<T, E>(value)),
                    async () => Result.Err<T, E>(await error().ConfigureAwait(false)))
                .ConfigureAwait(false);
        }
    }
}
