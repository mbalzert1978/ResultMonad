// <copyright file="Map.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for mapping values within <see cref="Result{T, E}"/> instances.
/// </summary>
public static class MapExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Transforms the success value of the result using the provided mapping function.
        /// </summary>
        /// <typeparam name="U">The type of the new success value after transformation.</typeparam>
        /// <param name="operation">The mapping function to apply to the success value.</param>
        /// <returns>A new result with the transformed success value or the original error.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        /// <remarks>
        /// This method applies the <paramref name="operation"/> function to the Ok value if the result is successful.
        /// If the result is Err, the error is propagated without invoking the mapping function.
        /// </remarks>
        public Result<U, E> Map<U>(Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(value => Result.Ok<U, E>(operation(value)), Result.Err<U, E>);
        }
    }
}
