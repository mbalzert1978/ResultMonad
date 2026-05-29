// <copyright file="ToErrTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results.Extensions.Sync;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for converting <see cref="Result{T, E}"/> into an <see cref="Option{T}"/> over its Err value, using <see cref="Task{TResult}"/>.
/// </summary>
public static class ToErrTaskExtension
{
    extension<T, E>(Task<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result task and converts it into an <see cref="Option{T}"/>: Err becomes Some carrying the error, Ok becomes None.
        /// </summary>
        /// <returns>A task producing Some carrying the error when the result is Err; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public async Task<Option<E>> ToErrAsync()
        {
            ArgumentNullException.ThrowIfNull(self);

            Result<T, E> result = await self.ConfigureAwait(false);
            return result.ToErr();
        }
    }
}
