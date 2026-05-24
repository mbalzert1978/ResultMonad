// <copyright file="Match.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension members for pattern matching on <see cref="Result{T, E}"/> instances.
/// </summary>
public static partial class MatchExtension
{
    extension<T, E>(Result<T, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Matches the result and invokes the appropriate function based on whether it is a success or a failure.
        /// </summary>
        /// <typeparam name="U">The type of the result returned by the match functions.</typeparam>
        /// <param name="onOk">The function to invoke if the result is a success.</param>
        /// <param name="onErr">The function to invoke if the result is a failure.</param>
        /// <returns>The result of invoking either <paramref name="onOk"/> or <paramref name="onErr"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if either <paramref name="onOk"/> or <paramref name="onErr"/> returns null.</exception>
        /// <exception cref="UnreachableException">Thrown if the result is neither a success nor a failure (defensive guard required by Roslyn; never reachable in practice).</exception>
        public U Match<U>(Func<T, U> onOk, Func<E, U> onErr)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(onOk);
            ArgumentNullException.ThrowIfNull(onErr);

            U result = self switch
            {
                Ok<T, E>(var value) => onOk(value)
                    ?? throw new InvalidOperationException(Strings.Constants.OperationNullError),
                Err<T, E>(var error) => onErr(error)
                    ?? throw new InvalidOperationException(Strings.Constants.OperationNullError),
                _ => throw new UnreachableException(Strings.Constants.ExhaustedResultError),
            };

            return result;
        }
    }
}
