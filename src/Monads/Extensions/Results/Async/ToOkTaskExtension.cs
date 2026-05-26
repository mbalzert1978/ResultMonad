// <copyright file="ToOkTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results.Extensions.Sync;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for converting <see cref="Result{T, E}"/> into an <see cref="Option{T}"/> over its Ok value, using <see cref="Task{TResult}"/>.
/// </summary>
public static class ToOkTaskExtension
{
    extension<T, E>(Task<Result<T, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the result task and converts it into an <see cref="Option{T}"/>: Ok becomes Some, Err becomes None.
        /// </summary>
        /// <returns>A task producing Some carrying the Ok value when the result is Ok; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public async Task<Option<T>> ToOkAsync()
        {
            ArgumentNullException.ThrowIfNull(self);

            Result<T, E> result = await self.ConfigureAwait(false);
            return result.ToOk();
        }
    }
}
