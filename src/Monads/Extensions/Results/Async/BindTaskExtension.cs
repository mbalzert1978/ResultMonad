// <copyright file="BindTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension methods for binding operations on <see cref="Result{T, E}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class BindTaskExtension
{
    /// <summary>
    /// Asynchronously binds a result wrapped in a <see cref="Task{TResult}"/> to a new result using a synchronous operation.
    /// If the result is Ok, the operation is invoked with the value and its result is returned.
    /// If the result is Err, the error is propagated.
    /// </summary>
    /// <typeparam name="T">The type of the value in the input result.</typeparam>
    /// <typeparam name="U">The type of the value in the output result.</typeparam>
    /// <typeparam name="E">The type of the error.</typeparam>
    /// <param name="self">The asynchronous result to bind.</param>
    /// <param name="operation">The synchronous operation to invoke if the result is Ok.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> containing a new result with the value returned by the operation or the propagated error.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async Task<Result<U, E>> BindAsync<T, U, E>(
        this Task<Result<T, E>> self,
        Func<T, Result<U, E>> operation
    )
        where T : notnull
        where U : notnull
        where E : notnull
    {
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(operation, Failure<U, E>).ConfigureAwait(false);
    }

    /// <summary>
    /// Binds a synchronous result to a new result using an asynchronous operation.
    /// If the result is Ok, the operation is invoked with the value and its result is returned.
    /// If the result is Err, the error is propagated.
    /// </summary>
    /// <typeparam name="T">The type of the value in the input result.</typeparam>
    /// <typeparam name="U">The type of the value in the output result.</typeparam>
    /// <typeparam name="E">The type of the error.</typeparam>
    /// <param name="self">The result to bind.</param>
    /// <param name="operation">The asynchronous operation to invoke if the result is Ok.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> containing a new result with the value returned by the operation or the propagated error.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async Task<Result<U, E>> BindAsync<T, U, E>(
        this Result<T, E> self,
        Func<T, Task<Result<U, E>>> operation
    )
        where T : notnull
        where U : notnull
        where E : notnull
    {
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(operation, err => Task.FromResult(Failure<U, E>(err)))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously binds a result wrapped in a <see cref="Task{TResult}"/> to a new result using an asynchronous operation.
    /// If the result is Ok, the operation is invoked with the value and its result is returned.
    /// If the result is Err, the error is propagated.
    /// </summary>
    /// <typeparam name="T">The type of the value in the input result.</typeparam>
    /// <typeparam name="U">The type of the value in the output result.</typeparam>
    /// <typeparam name="E">The type of the error.</typeparam>
    /// <param name="self">The asynchronous result to bind.</param>
    /// <param name="operation">The asynchronous operation to invoke if the result is Ok.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> containing a new result with the value returned by the operation or the propagated error.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async Task<Result<U, E>> BindAsync<T, U, E>(
        this Task<Result<T, E>> self,
        Func<T, Task<Result<U, E>>> operation
    )
        where T : notnull
        where U : notnull
        where E : notnull
    {
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(operation, err => Task.FromResult(Failure<U, E>(err)))
            .ConfigureAwait(false);
    }
}
