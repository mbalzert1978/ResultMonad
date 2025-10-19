// <copyright file="MatchTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results.Extensions.Sync;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension methods for pattern matching on <see cref="Result{T, E}"/> instances.
/// </summary>
public static class MatchTaskExtension
{
    /// <summary>
    /// Asynchronously matches a result wrapped in a <see cref="Task{TResult}"/> and invokes the appropriate synchronous function.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <typeparam name="U">The type of the result returned by the match functions.</typeparam>
    /// <param name="self">The asynchronous result to match.</param>
    /// <param name="onOk">The function to invoke if the result is an <see cref="Ok{T, E}"/>.</param>
    /// <param name="onErr">The function to invoke if the result is an <see cref="Err{T, E}"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the result of invoking either <paramref name="onOk"/> or <paramref name="onErr"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/>, <paramref name="onOk"/>, or <paramref name="onErr"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if either <paramref name="onOk"/> or <paramref name="onErr"/> returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    /// <remarks>
    /// This overload awaits the <paramref name="self"/> task and then delegates to the synchronous <see cref="MatchExtension.Match{T, E, U}(Result{T, E}, Func{T, U}, Func{E, U})"/> method.
    /// Both match functions are executed synchronously after the result is awaited.
    /// </remarks>
    public static async Task<U> MatchAsync<T, E, U>(
        this Task<Result<T, E>> self,
        Func<T, U> onOk,
        Func<E, U> onErr
    )
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(onOk);
        ArgumentNullException.ThrowIfNull(onErr);

        return (await self.ConfigureAwait(false)).Match(onOk, onErr);
    }

    /// <summary>
    /// Matches a synchronous result and invokes the appropriate asynchronous function returning a <see cref="Task{TResult}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <typeparam name="U">The type of the result returned by the match functions.</typeparam>
    /// <param name="self">The result to match.</param>
    /// <param name="onOk">The asynchronous function to invoke if the result is an <see cref="Ok{T, E}"/>.</param>
    /// <param name="onErr">The asynchronous function to invoke if the result is an <see cref="Err{T, E}"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the result of invoking either <paramref name="onOk"/> or <paramref name="onErr"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onOk"/> or <paramref name="onErr"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if either <paramref name="onOk"/> or <paramref name="onErr"/> returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    /// <remarks>
    /// This overload takes a synchronous result but invokes asynchronous match functions that return <see cref="Task{TResult}"/>.
    /// The appropriate function is selected based on the result type and then awaited.
    /// </remarks>
    public static async Task<U> MatchAsync<T, E, U>(
        this Result<T, E> self,
        Func<T, Task<U>> onOk,
        Func<E, Task<U>> onErr
    )
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(onOk);
        ArgumentNullException.ThrowIfNull(onErr);

        U result = self switch
        {
            Ok<T, E>(var value) => await onOk(value).ConfigureAwait(false)
                ?? throw new InvalidOperationException(Strings.Constants.OperationNullError),
            Err<T, E>(var error) => await onErr(error).ConfigureAwait(false)
                ?? throw new InvalidOperationException(Strings.Constants.OperationNullError),
            _ => throw new UnreachableException(Strings.Constants.ExhaustedResultError),
        };

        return result;
    }

    /// <summary>
    /// Asynchronously matches a result wrapped in a <see cref="Task{TResult}"/> and invokes the appropriate asynchronous function.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <typeparam name="U">The type of the result returned by the match functions.</typeparam>
    /// <param name="self">The asynchronous result to match.</param>
    /// <param name="onOk">The asynchronous function to invoke if the result is an <see cref="Ok{T, E}"/>.</param>
    /// <param name="onErr">The asynchronous function to invoke if the result is an <see cref="Err{T, E}"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the result of invoking either <paramref name="onOk"/> or <paramref name="onErr"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/>, <paramref name="onOk"/>, or <paramref name="onErr"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if either <paramref name="onOk"/> or <paramref name="onErr"/> returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    /// <remarks>
    /// This overload awaits the <paramref name="self"/> task and then delegates to the <see cref="MatchAsync{T, E, U}(Result{T, E}, Func{T, Task{U}}, Func{E, Task{U}})"/> overload.
    /// Both the result and the selected match function are awaited asynchronously.
    /// </remarks>
    public static async Task<U> MatchAsync<T, E, U>(
        this Task<Result<T, E>> self,
        Func<T, Task<U>> onOk,
        Func<E, Task<U>> onErr
    )
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(onOk);
        ArgumentNullException.ThrowIfNull(onErr);

        return await (await self.ConfigureAwait(false))
            .MatchAsync(onOk, onErr)
            .ConfigureAwait(false);
    }
}
