// <copyright file="OkOr.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for converting <see cref="Option{T}"/> instances into
/// <see cref="Result{T, E}"/> values with an eager error.
/// </summary>
public static class OkOrExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Transforms this option into a <see cref="Result{T, E}"/>:
        /// Some becomes Ok, None becomes Err with <paramref name="error"/>.
        /// </summary>
        /// <typeparam name="E">The error type carried by the resulting Err variant.</typeparam>
        /// <param name="error">The error value used when this option is None.</param>
        /// <returns>
        /// An Ok carrying the wrapped value when this option is Some;
        /// otherwise an Err carrying <paramref name="error"/>.
        /// </returns>
        public Result<T, E> OkOr<E>(E error)
            where E : notnull => self.Match(Result.Ok<T, E>, () => Result.Err<T, E>(error));
    }
}
