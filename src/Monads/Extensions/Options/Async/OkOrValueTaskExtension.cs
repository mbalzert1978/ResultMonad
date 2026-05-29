// <copyright file="OkOrValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for converting <see cref="Option{T}"/> instances into
/// <see cref="Result{T, E}"/> values with an eager error using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class OkOrValueTaskExtension
{
    extension<T>(ValueTask<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and transforms it into a <see cref="Result{T, E}"/>:
        /// Some becomes Ok, None becomes Err with <paramref name="error"/>.
        /// </summary>
        /// <typeparam name="E">The error type carried by the resulting Err variant.</typeparam>
        /// <param name="error">The error value used when the awaited option is None.</param>
        /// <returns>A value task producing the corresponding <see cref="Result{T, E}"/>.</returns>
        public async ValueTask<Result<T, E>> OkOrAsync<E>(E error)
            where E : notnull =>
            await self.MatchAsync(Result.Ok<T, E>, () => Result.Err<T, E>(error))
                .ConfigureAwait(false);
    }
}
