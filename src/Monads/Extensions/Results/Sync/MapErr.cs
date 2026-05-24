// <copyright file="MapErr.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for mapping error values within <see cref="Result{T, E}"/> instances.
/// </summary>
public static class MapErrExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Maps the error value of a result to a new error type using the specified operation.
        /// If the result is Ok, the value is preserved unchanged.
        /// If the result is Err, the operation is applied to transform the error.
        /// </summary>
        /// <typeparam name="F">The type of the new error value.</typeparam>
        /// <param name="operation">The function to apply to the error value.</param>
        /// <returns>
        /// A new result with the transformed error type, containing either the original Ok value or the mapped error.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public Result<T, F> MapErr<F>(Func<E, F> operation)
            where F : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(Result.Ok<T, F>, err => Result.Err<T, F>(operation(err)));
        }
    }
}
