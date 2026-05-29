// <copyright file="FlattenAsyncExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides extension members for flattening nested <see cref="Result{T, E}"/> structures asynchronously.
/// </summary>
public static class FlattenAsyncExtension
{
    extension<T, E>(Task<Result<Result<T, E>, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Flattens a task that results in a <see cref="Result{T, E}"/> where the Ok value is itself a <see cref="Result{T, E}"/>.
        /// </summary>
        /// <returns>
        /// A task that results in a <see cref="Result{T, E}"/>, which is Ok with the inner result's value if both results are Ok,
        /// or Err with the first encountered error.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if the task is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async Task<Result<T, E>> FlattenAsync()
        {
            ArgumentNullException.ThrowIfNull(self);

            return await self.MatchAsync(ok => ok, Result.Err<T, E>).ConfigureAwait(false);
        }
    }

    extension<T, E>(ValueTask<Result<Result<T, E>, E>> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Flattens a ValueTask that results in a <see cref="Result{T, E}"/> where the Ok value is itself a <see cref="Result{T, E}"/>.
        /// </summary>
        /// <returns>
        /// A ValueTask that results in a <see cref="Result{T, E}"/>, which is Ok with the inner result's value if both results are Ok,
        /// or Err with the first encountered error.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public async ValueTask<Result<T, E>> FlattenAsync() =>
            await self.MatchAsync(ok => ok, Result.Err<T, E>).ConfigureAwait(false);
    }
}
