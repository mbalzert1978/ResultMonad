// <copyright file="FlattenAsyncExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Async;

/// <summary>
/// Provides extension methods for flattening nested <see cref="Result{T, E}"/> structures.
/// </summary>
public static class FlattenAsyncExtension
{
    /// <summary>
    /// Flattens a task that results in a <see cref="Result{T, E}"/> where the Ok value is itself a <see cref="Result{T, E}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value in the inner result.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <param name="self">The task producing a result to flatten.</param>
    /// <returns>
    /// A task that results in a <see cref="Result{T, E}"/>, which is Ok with the inner result's value if both results are Ok,
    /// or Err with the first encountered error.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async Task<Result<T, E>> FlattenAsync<T, E>(
        this Task<Result<Result<T, E>, E>> self
    )
        where T : notnull
        where E : notnull
    {
        ArgumentNullException.ThrowIfNull(self);

        return await self.MatchAsync(ok => ok, Failure<T, E>).ConfigureAwait(false);
    }

    /// <summary>
    /// Flattens a ValueTask that results in a <see cref="Result{T, E}"/> where the Ok value is itself a <see cref="Result{T, E}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value in the inner result.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <param name="self">The ValueTask producing a result to flatten.</param>
    /// <returns>
    /// A ValueTask that results in a <see cref="Result{T, E}"/>, which is Ok with the inner result's value if both results are Ok,
    /// or Err with the first encountered error.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static async ValueTask<Result<T, E>> FlattenAsync<T, E>(
        this ValueTask<Result<Result<T, E>, E>> self
    )
        where T : notnull
        where E : notnull => await self.MatchAsync(ok => ok, Failure<T, E>).ConfigureAwait(false);
}
