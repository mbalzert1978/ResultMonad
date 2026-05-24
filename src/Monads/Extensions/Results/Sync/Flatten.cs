// <copyright file="Flatten.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for flattening nested <see cref="Result{T, E}"/> structures.
/// </summary>
public static class FlattenExtension
{
    extension<T, E>(Result<Result<T, E>, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Flattens a nested <see cref="Result{T, E}"/> structure into a single <see cref="Result{T, E}"/>.
        /// </summary>
        /// <returns>Converts from <c>Result&lt;Result&lt;T, E&gt;, E&gt;</c> to <c>Result&lt;T, E&gt;</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the result is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        /// <remarks>
        /// This method is useful for simplifying nested result structures, allowing for easier chaining of operations
        /// that may fail.
        /// </remarks>
        public Result<T, E> Flatten()
        {
            ArgumentNullException.ThrowIfNull(self);

            return self.Match(ok => ok, Result.Err<T, E>);
        }
    }
}
