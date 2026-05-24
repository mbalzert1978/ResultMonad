// <copyright file="OrElseValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for error recovery operations on <see cref="Result{T, E}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class OrElseValueTaskExtension
{
    extension<T, E>(ValueTask<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Asynchronously calls the operation if the result is Err, otherwise returns the Ok value unchanged.
        /// </summary>
        /// <typeparam name="F">The type of the new error value.</typeparam>
        /// <param name="operation">A synchronous function to call with the error value if the result is Err.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// calls <paramref name="operation"/> if the result is Err, otherwise returns the Ok value of <paramref name="self"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the result is Err and the operation returns an Err.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<T, F>> OrElseAsync<F>(Func<E, Result<T, F>> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(Result.Ok<T, F>, operation).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously calls the asynchronous operation if the result is Err, otherwise returns the Ok value unchanged.
        /// </summary>
        /// <typeparam name="F">The type of the new error value.</typeparam>
        /// <param name="operation">An asynchronous function to call with the error value if the result is Err.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// calls <paramref name="operation"/> if the result is Err, otherwise returns the Ok value of <paramref name="self"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the result is Err and the operation returns an Err.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<T, F>> OrElseAsync<F>(Func<E, ValueTask<Result<T, F>>> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await (await self.ConfigureAwait(false))
                .MatchAsync(
                    value => ValueTask.FromResult(Result.Ok<T, F>(value)),
                    async error => await operation(error).ConfigureAwait(false)
                )
                .ConfigureAwait(false);
        }
    }

    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Calls the asynchronous operation if the result is Err, otherwise returns the Ok value unchanged.
        /// </summary>
        /// <typeparam name="F">The type of the new error value.</typeparam>
        /// <param name="operation">An asynchronous function to call with the error value if the result is Err.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// calls <paramref name="operation"/> if the result is Err, otherwise returns the Ok value of <paramref name="self"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the result is Err and the operation returns an Err.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<T, F>> OrElseAsync<F>(Func<E, ValueTask<Result<T, F>>> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(
                    value => ValueTask.FromResult(Result.Ok<T, F>(value)),
                    async error => await operation(error).ConfigureAwait(false)
                )
                .ConfigureAwait(false);
        }
    }
}
