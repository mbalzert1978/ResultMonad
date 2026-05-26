// <copyright file="InspectErr.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for observing the Err value of a <see cref="Result{T, E}"/> via a side-effecting action.
/// </summary>
public static class InspectErrExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Invokes <paramref name="action"/> with the Err value when this result is Err, then returns this result unchanged.
        /// Does nothing when this result is Ok.
        /// </summary>
        /// <param name="action">The action to invoke with the error value.</param>
        /// <returns>This result unchanged.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
        public Result<T, E> InspectErr(Action<E> action)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(action);

            return self.Match(
                _ => self,
                error =>
                {
                    action(error);
                    return self;
                });
        }
    }
}
