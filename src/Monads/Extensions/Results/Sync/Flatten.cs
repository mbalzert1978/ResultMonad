// <copyright file="FlattenExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>
using System.Diagnostics;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension methods for flattening nested <see cref="Result{T, E}"/> structures.
/// </summary>
public static class FlattenExtension
{
    /// <summary>
    /// Flattens a nested <see cref="Result{T, E}"/> structure into a single <see cref="Result{T, E}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <param name="self">The nested result to flatten.</param>
    /// <returns>Converts from `Result&lt;Result&lt;T, E&gt;, E&gt;` to `Result&lt;T, E&gt;`.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    /// <remarks>
    /// This method is useful for simplifying nested result structures, allowing for easier chaining of operations
    /// that may fail.
    /// </remarks>
    public static Result<T, E> Flatten<T, E>(this Result<Result<T, E>, E> self)
        where T : notnull
        where E : notnull
    {
        ArgumentNullException.ThrowIfNull(self);

        return self.Match(ok => ok, Failure<T, E>);
    }
}
