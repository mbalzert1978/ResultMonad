// <copyright file="Inspect.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for observing the Ok value of a <see cref="Result{T, E}"/> via a side-effecting action.
/// </summary>
public static class InspectExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Invokes <paramref name="action"/> with the Ok value when this result is Ok, then returns this result unchanged.
        /// Does nothing when this result is Err.
        /// </summary>
        /// <param name="action">The action to invoke with the Ok value.</param>
        /// <returns>This result unchanged.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
        public Result<T, E> Inspect(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(action);

            return self.Match(
                value =>
                {
                    action(value);
                    return self;
                },
                _ => self);
        }
    }
}
