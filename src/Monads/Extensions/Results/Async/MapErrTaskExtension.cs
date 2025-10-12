// <copyright file="MapErrTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension methods for mapping error values within <see cref="Result{T, E}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class MapErrTaskExtension
{
    /// <summary>
    /// Asynchronously maps the error value of a <see cref="Result{T, E}"/> contained within a <see cref="Task{TResult}"/>
    /// to a new error type using the specified synchronous mapping function if the result is an error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the original error value.</typeparam>
    /// <typeparam name="F">The type of the new error value after mapping.</typeparam>
    /// <param name="self">A <see cref="Task{TResult}"/> that produces a <see cref="Result{T, E}"/>.</param>
    /// <param name="operation">A synchronous function to map the error from <typeparamref name="E"/> to <typeparamref name="F"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that yields a <see cref="Result{T, F}"/>,
    /// which contains the original Ok value or the result of applying the mapping function to the original Err value.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async Task<Result<T, F>> MapErrAsync<T, E, F>(
        this Task<Result<T, E>> self,
        Func<E, F> operation
    )
        where T : notnull
        where E : notnull
        where F : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(Success<T, F>, err => Failure<T, F>(operation(err)))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Maps the error value of a synchronous <see cref="Result{T, E}"/> to a new error type using an asynchronous mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the original error value.</typeparam>
    /// <typeparam name="F">The type of the new error value after mapping.</typeparam>
    /// <param name="self">The <see cref="Result{T, E}"/> to map.</param>
    /// <param name="operation">An asynchronous function to map the error from <typeparamref name="E"/> to <typeparamref name="F"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that yields a <see cref="Result{T, F}"/>,
    /// which contains the original Ok value or the result of applying the mapping function to the original Err value.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async Task<Result<T, F>> MapErrAsync<T, E, F>(
        this Result<T, E> self,
        Func<E, Task<F>> operation
    )
        where T : notnull
        where E : notnull
        where F : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(
                value => Task.FromResult(Success<T, F>(value)),
                async err => Failure<T, F>(await operation(err).ConfigureAwait(false))
            )
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously maps the error value of a <see cref="Result{T, E}"/> contained within a <see cref="Task{TResult}"/>
    /// to a new error type using an asynchronous mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the original error value.</typeparam>
    /// <typeparam name="F">The type of the new error value after mapping.</typeparam>
    /// <param name="self">A <see cref="Task{TResult}"/> that produces a <see cref="Result{T, E}"/>.</param>
    /// <param name="operation">An asynchronous function to map the error from <typeparamref name="E"/> to <typeparamref name="F"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that yields a <see cref="Result{T, F}"/>,
    /// which contains the original Ok value or the result of applying the mapping function to the original Err value.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async Task<Result<T, F>> MapErrAsync<T, E, F>(
        this Task<Result<T, E>> self,
        Func<E, Task<F>> operation
    )
        where T : notnull
        where E : notnull
        where F : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(
                value => Task.FromResult(Success<T, F>(value)),
                async err => Failure<T, F>(await operation(err).ConfigureAwait(false))
            )
            .ConfigureAwait(false);
    }
}
