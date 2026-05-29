// <copyright file="TransposeAsyncExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results.Extensions.Sync;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for transposing a <see cref="Result{T, E}"/> of an <see cref="Option{T}"/>
/// into an <see cref="Option{T}"/> of a <see cref="Result{T, E}"/>.
/// </summary>
public static class TransposeAsyncExtension
{
    extension<T, E>(Task<Result<Option<T>, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the task and transposes the result of an option into an option of a result.
        /// </summary>
        /// <returns>A task producing the transposed value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public async Task<Option<Result<T, E>>> TransposeAsync()
        {
            ArgumentNullException.ThrowIfNull(self);

            Result<Option<T>, E> result = await self.ConfigureAwait(false);
            return result.Transpose();
        }
    }

    extension<T, E>(ValueTask<Result<Option<T>, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Awaits the value task and transposes the result of an option into an option of a result.
        /// </summary>
        /// <returns>A value task producing the transposed value.</returns>
        public async ValueTask<Option<Result<T, E>>> TransposeAsync()
        {
            Result<Option<T>, E> result = await self.ConfigureAwait(false);
            return result.Transpose();
        }
    }
}
