// <copyright file="OkOrElse.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for converting <see cref="Option{T}"/> instances into
/// <see cref="Result{T, E}"/> values with a lazily-computed error.
/// </summary>
public static class OkOrElseExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Transforms this option into a <see cref="Result{T, E}"/>:
        /// Some becomes Ok, None becomes Err with the value produced by <paramref name="error"/>.
        /// </summary>
        /// <typeparam name="E">The error type carried by the resulting Err variant.</typeparam>
        /// <param name="error">The function invoked to produce the error value when this option is None.</param>
        /// <returns>
        /// An Ok carrying the wrapped value when this option is Some;
        /// otherwise an Err carrying <c>error()</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
        public Result<T, E> OkOrElse<E>(Func<E> error)
            where E : notnull
        {
            ArgumentNullException.ThrowIfNull(error);

            return self.Match(Result.Ok<T, E>, () => Result.Err<T, E>(error()));
        }
    }
}
