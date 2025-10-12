// <copyright file="OrElseValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension methods for error recovery operations on <see cref="Result{T, E}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class OrElseValueTaskExtension
{
    /// <summary>
    /// Asynchronously calls the operation if the result is Err, otherwise returns the Ok value unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the original error value.</typeparam>
    /// <typeparam name="F">The type of the new error value.</typeparam>
    /// <param name="self">A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, E}"/>.</param>
    /// <param name="operation">A synchronous function to call with the error value if the result is Err.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
    /// calls `operation` if the result is [`Err`], otherwise returns the [`Ok`] value of `self`.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result is Err and the operation returns an Err.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async ValueTask<Result<T, F>> OrElseAsync<T, E, F>(
        this ValueTask<Result<T, E>> self,
        Func<E, Result<T, F>> operation
    )
        where T : notnull
        where E : notnull
        where F : notnull
    {
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(Success<T, F>, operation).ConfigureAwait(false);
    }

    /// <summary>
    /// Calls the asynchronous operation if the result is Err, otherwise returns the Ok value unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the original error value.</typeparam>
    /// <typeparam name="F">The type of the new error value.</typeparam>
    /// <param name="self">The <see cref="Result{T, E}"/> to operate on.</param>
    /// <param name="operation">An asynchronous function to call with the error value if the result is Err.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
    /// calls `operation` if the result is [`Err`], otherwise returns the [`Ok`] value of `self`.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result is Err and the operation returns an Err.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async ValueTask<Result<T, F>> OrElseAsync<T, E, F>(
        this Result<T, E> self,
        Func<E, ValueTask<Result<T, F>>> operation
    )
        where T : notnull
        where E : notnull
        where F : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(
                value => ValueTask.FromResult(Success<T, F>(value)),
                async error => await operation(error).ConfigureAwait(false)
            )
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously calls the asynchronous operation if the result is Err, otherwise returns the Ok value unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the original error value.</typeparam>
    /// <typeparam name="F">The type of the new error value.</typeparam>
    /// <param name="self">A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, E}"/>.</param>
    /// <param name="operation">An asynchronous function to call with the error value if the result is Err.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that produces a <see cref="Result{T, F}"/>,
    ///
    /// calls `operation` if the result is [`Err`], otherwise returns the [`Ok`] value of `self`.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result is Err and the operation returns an Err.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async ValueTask<Result<T, F>> OrElseAsync<T, E, F>(
        this ValueTask<Result<T, E>> self,
        Func<E, ValueTask<Result<T, F>>> operation
    )
        where T : notnull
        where E : notnull
        where F : notnull
    {
        ArgumentNullException.ThrowIfNull(operation);

        return await (await self.ConfigureAwait(false))
            .MatchAsync(
                value => ValueTask.FromResult(Success<T, F>(value)),
                async error => await operation(error).ConfigureAwait(false)
            )
            .ConfigureAwait(false);
    }
}
