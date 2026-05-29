// <copyright file="ToErrValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results.Extensions.Sync;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for converting <see cref="Result{T, E}"/> into an <see cref="Option{T}"/> over its Err value, using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class ToErrValueTaskExtension
{
    extension<T, E>(ValueTask<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result value task and converts it into an <see cref="Option{T}"/>: Err becomes Some carrying the error, Ok becomes None.
        /// </summary>
        /// <returns>A value task producing Some carrying the error when the result is Err; otherwise None.</returns>
        public async ValueTask<Option<E>> ToErrAsync()
        {
            Result<T, E> result = await self.ConfigureAwait(false);
            return result.ToErr();
        }
    }
}
