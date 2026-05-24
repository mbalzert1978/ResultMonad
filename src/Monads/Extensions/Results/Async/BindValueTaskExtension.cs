// <copyright file="BindValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for binding operations on <see cref="Result{T, E}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class BindValueTaskExtension
{
    extension<T, E>(ValueTask<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Asynchronously binds a result wrapped in a <see cref="ValueTask{TResult}"/> to a new result using a synchronous operation.
        /// If the result is Ok, the operation is invoked with the value and its result is returned.
        /// If the result is Err, the error is propagated.
        /// </summary>
        /// <typeparam name="U">The type of the value in the output result.</typeparam>
        /// <param name="operation">The synchronous operation to invoke if the result is Ok.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> containing a new result with the value returned by the operation or the propagated error.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<U, E>> BindAsync<U>(Func<T, Result<U, E>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, Result.Err<U, E>).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously binds a result wrapped in a <see cref="ValueTask{TResult}"/> to a new result using an asynchronous operation.
        /// If the result is Ok, the operation is invoked with the value and its result is returned.
        /// If the result is Err, the error is propagated.
        /// </summary>
        /// <typeparam name="U">The type of the value in the output result.</typeparam>
        /// <param name="operation">The asynchronous operation to invoke if the result is Ok.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> containing a new result with the value returned by the operation or the propagated error.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<U, E>> BindAsync<U>(Func<T, ValueTask<Result<U, E>>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, err => ValueTask.FromResult(Result.Err<U, E>(err)))
                .ConfigureAwait(false);
        }
    }

    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Binds a synchronous result to a new result using an asynchronous operation.
        /// If the result is Ok, the operation is invoked with the value and its result is returned.
        /// If the result is Err, the error is propagated.
        /// </summary>
        /// <typeparam name="U">The type of the value in the output result.</typeparam>
        /// <param name="operation">The asynchronous operation to invoke if the result is Ok.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> containing a new result with the value returned by the operation or the propagated error.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<U, E>> BindAsync<U>(Func<T, ValueTask<Result<U, E>>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, err => ValueTask.FromResult(Result.Err<U, E>(err)))
                .ConfigureAwait(false);
        }
    }
}
