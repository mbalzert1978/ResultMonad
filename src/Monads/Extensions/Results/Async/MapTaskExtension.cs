// <copyright file="MapTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension methods for mapping values within <see cref="Result{T, E}"/> instances using <see cref="Task{TResult}"/>.
/// </summary>
public static class MapTaskExtension
{
    /// <summary>
    /// Asynchronously maps the value of a <see cref="Result{T, E}"/> contained within a <see cref="Task{TResult}"/>
    /// to a new value using the specified synchronous mapping function if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value in the Ok case.</typeparam>
    /// <typeparam name="E">The type of the error in the Err case.</typeparam>
    /// <typeparam name="U">The type of the new value after mapping.</typeparam>
    /// <param name="self">A <see cref="Task{TResult}"/> that produces a <see cref="Result{T, E}"/>.</param>
    /// <param name="operation">A synchronous function to map the value from <typeparamref name="T"/> to <typeparamref name="U"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that produces a <see cref="Result{U, E}"/>,
    /// applying the mapping function to the Ok value if the original result was Ok, or propagating the original error if it was Err.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    /// <remarks>
    /// This overload awaits the <paramref name="self"/> task, then applies the synchronous <paramref name="operation"/> to the Ok value.
    /// If the result is Err, the error is propagated without invoking the mapping function.
    /// </remarks>
    public static async Task<Result<U, E>> MapAsync<T, E, U>(
        this Task<Result<T, E>> self,
        Func<T, U> operation
    )
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(value => Success<U, E>(operation(value)), Failure<U, E>)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Maps the value of a synchronous <see cref="Result{T, E}"/> to a new value using an asynchronous mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the value in the Ok case.</typeparam>
    /// <typeparam name="E">The type of the error in the Err case.</typeparam>
    /// <typeparam name="U">The type of the new value after mapping.</typeparam>
    /// <param name="self">The <see cref="Result{T, E}"/> to map.</param>
    /// <param name="operation">An asynchronous function to map the value from <typeparamref name="T"/> to <typeparamref name="U"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that produces a <see cref="Result{U, E}"/>,
    /// applying the mapping function to the Ok value if the original result was Ok, or propagating the original error if it was Err.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    /// <remarks>
    /// This overload takes a synchronous result and applies an asynchronous <paramref name="operation"/> to the Ok value.
    /// The operation is only invoked if the result is Ok; otherwise, the error is propagated.
    /// </remarks>
    public static async Task<Result<U, E>> MapAsync<T, E, U>(
        this Result<T, E> self,
        Func<T, Task<U>> operation
    )
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(
                async value => Success<U, E>(await operation(value).ConfigureAwait(false)),
                err => Task.FromResult(Failure<U, E>(err))
            )
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously maps the value of a <see cref="Result{T, E}"/> contained within a <see cref="Task{TResult}"/>
    /// to a new value using an asynchronous mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the value in the Ok case.</typeparam>
    /// <typeparam name="E">The type of the error in the Err case.</typeparam>
    /// <typeparam name="U">The type of the new value after mapping.</typeparam>
    /// <param name="self">A <see cref="Task{TResult}"/> that produces a <see cref="Result{T, E}"/>.</param>
    /// <param name="operation">An asynchronous function to map the value from <typeparamref name="T"/> to <typeparamref name="U"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that produces a <see cref="Result{U, E}"/>,
    /// applying the mapping function to the Ok value if the original result was Ok, or propagating the original error if it was Err.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    /// <remarks>
    /// This overload awaits both the <paramref name="self"/> task and the asynchronous <paramref name="operation"/>.
    /// The operation is only invoked if the awaited result is Ok; otherwise, the error is propagated.
    /// </remarks>
    public static async Task<Result<U, E>> MapAsync<T, E, U>(
        this Task<Result<T, E>> self,
        Func<T, Task<U>> operation
    )
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(operation);

        return await self.MatchAsync(
                async value => Success<U, E>(await operation(value).ConfigureAwait(false)),
                err => Task.FromResult(Failure<U, E>(err))
            )
            .ConfigureAwait(false);
    }
}
