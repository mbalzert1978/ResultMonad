// <copyright file="MapErrValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for mapping error values within <see cref="Result{T, E}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class MapErrValueTaskExtension
{
    extension<T, E>(ValueTask<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Asynchronously maps the error value of a <see cref="Result{T, E}"/> contained within a <see cref="ValueTask{TResult}"/>
        /// to a new error type using the specified synchronous mapping function if the result is an error.
        /// </summary>
        /// <typeparam name="F">The type of the new error value after mapping.</typeparam>
        /// <param name="operation">A synchronous function to map the error from <typeparamref name="E"/> to <typeparamref name="F"/>.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// which contains the original Ok value or the result of applying the mapping function to the original Err value.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="ValueTask{TResult}"/> is not completed successfully.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<T, F>> MapErrAsync<F>(Func<E, F> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(Result.Ok<T, F>, err => Result.Err<T, F>(operation(err)))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously maps the error value of a <see cref="Result{T, E}"/> contained within a <see cref="ValueTask{TResult}"/>
        /// to a new error type using an asynchronous mapping function.
        /// </summary>
        /// <typeparam name="F">The type of the new error value after mapping.</typeparam>
        /// <param name="operation">An asynchronous function to map the error from <typeparamref name="E"/> to <typeparamref name="F"/>.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// which contains the original Ok value or the result of applying the mapping function to the original Err value.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<T, F>> MapErrAsync<F>(Func<E, ValueTask<F>> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(
                    value => ValueTask.FromResult(Result.Ok<T, F>(value)),
                    async err => Result.Err<T, F>(await operation(err).ConfigureAwait(false))
                )
                .ConfigureAwait(false);
        }
    }

    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Maps the error value of a synchronous <see cref="Result{T, E}"/> to a new error type using an asynchronous mapping function.
        /// </summary>
        /// <typeparam name="F">The type of the new error value after mapping.</typeparam>
        /// <param name="operation">An asynchronous function to map the error from <typeparamref name="E"/> to <typeparamref name="F"/>.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// which contains the original Ok value or the result of applying the mapping function to the original Err value.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<T, F>> MapErrAsync<F>(Func<E, ValueTask<F>> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(
                    value => ValueTask.FromResult(Result.Ok<T, F>(value)),
                    async err => Result.Err<T, F>(await operation(err).ConfigureAwait(false))
                )
                .ConfigureAwait(false);
        }
    }
}
