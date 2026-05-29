// <copyright file="ToOkValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results.Extensions.Sync;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for converting <see cref="Result{T, E}"/> into an <see cref="Option{T}"/> over its Ok value, using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class ToOkValueTaskExtension
{
    extension<T, E>(ValueTask<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result value task and converts it into an <see cref="Option{T}"/>: Ok becomes Some, Err becomes None.
        /// </summary>
        /// <returns>A value task producing Some carrying the Ok value when the result is Ok; otherwise None.</returns>
        public async ValueTask<Option<T>> ToOkAsync()
        {
            Result<T, E> result = await self.ConfigureAwait(false);
            return result.ToOk();
        }
    }
}
