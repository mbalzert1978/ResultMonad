// <copyright file="Bind.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for binding operations on <see cref="Result{T, E}"/> instances.
/// </summary>
public static class BindExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Binds the result to a new result using the specified operation.
        /// If the result is Ok, the operation is invoked with the value and its result is returned.
        /// If the result is Err, the error is propagated.
        /// </summary>
        /// <typeparam name="U">The type of the value in the output result.</typeparam>
        /// <param name="operation">The operation to invoke if the result is Ok.</param>
        /// <returns>
        /// A new result containing the value returned by the operation or the propagated error.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="operation"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
        public Result<U, E> Bind<U>(Func<T, Result<U, E>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(operation, Result.Err<U, E>);
        }
    }
}
