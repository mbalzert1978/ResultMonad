// <copyright file="Map.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension methods for mapping values within <see cref="Result{T, E}"/> instances.
/// </summary>
public static class MapExtension
{
    /// <summary>
    /// Transforms the success value of the result using the provided mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <typeparam name="U">The type of the new success value after transformation.</typeparam>
    /// <param name="self">The result to transform.</param>
    /// <param name="operation">The mapping function to apply to the success value.</param>
    /// <returns>A new result with the transformed success value or the original error.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    /// <remarks>
    /// This method applies the <paramref name="operation"/> function to the Ok value if the result is successful.
    /// If the result is Err, the error is propagated without invoking the mapping function.
    /// </remarks>
    public static Result<U, E> Map<T, E, U>(this Result<T, E> self, Func<T, U> operation)
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(operation);

        return self.Match(value => Success<U, E>(operation(value)), Failure<U, E>);
    }
}
