// <copyright file="OrElseTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for error recovery operations on <see cref="Result{T, E}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class OrElseTaskExtension
{
    extension<T, E>(Task<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Asynchronously calls the operation if the result is Err, otherwise returns the Ok value unchanged.
        /// </summary>
        /// <typeparam name="F">The type of the new error value.</typeparam>
        /// <param name="operation">A synchronous function to call with the error value if the result is Err.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// calls `operation` if the result is [`Err`], otherwise returns the [`Ok`] value of `self`.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="operation"/> returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async Task<Result<T, F>> OrElseAsync<F>(Func<E, Result<T, F>> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(Result.Ok<T, F>, operation).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously calls the asynchronous operation if the result is Err, otherwise returns the Ok value unchanged.
        /// </summary>
        /// <typeparam name="F">The type of the new error value.</typeparam>
        /// <param name="operation">An asynchronous function to call with the error value if the result is Err.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// calls `operation` if the result is [`Err`], otherwise returns the [`Ok`] value of `self`.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="operation"/> returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async Task<Result<T, F>> OrElseAsync<F>(Func<E, Task<Result<T, F>>> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await (await self.ConfigureAwait(false))
                .MatchAsync(
                    value => Task.FromResult(Result.Ok<T, F>(value)),
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
        /// A <see cref="Task{TResult}"/> that produces a <see cref="Result{T, F}"/>,
        /// calls `operation` if the result is [`Err`], otherwise returns the [`Ok`] value of `self`.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="operation"/> returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async Task<Result<T, F>> OrElseAsync<F>(Func<E, Task<Result<T, F>>> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(
                    value => Task.FromResult(Result.Ok<T, F>(value)),
                    async error => await operation(error).ConfigureAwait(false)
                )
                .ConfigureAwait(false);
        }
    }
}
