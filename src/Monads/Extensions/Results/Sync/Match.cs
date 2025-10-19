// <copyright file="Match.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides extension methods for pattern matching on <see cref="Result{T, E}"/> instances.
/// </summary>
public static class MatchExtension
{
    /// <summary>
    /// Matches the result and invokes the appropriate function based on whether it is an <see cref="Ok{T, E}"/> or an <see cref="Err{T, E}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <typeparam name="U">The type of the result returned by the match functions.</typeparam>
    /// <param name="self">The result to match.</param>
    /// <param name="onOk">The function to invoke if the result is an <see cref="Ok{T, E}"/>.</param>
    /// <param name="onErr">The function to invoke if the result is an <see cref="Err{T, E}"/>.</param>
    /// <returns>The result of invoking either <paramref name="onOk"/> or <paramref name="onErr"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if either <paramref name="onOk"/> or <paramref name="onErr"/> returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the result is neither <see cref="Ok{T, E}"/> nor <see cref="Err{T, E}"/>.</exception>
    public static U Match<T, E, U>(this Result<T, E> self, Func<T, U> onOk, Func<E, U> onErr)
        where T : notnull
        where E : notnull
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
