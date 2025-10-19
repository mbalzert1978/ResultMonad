// <copyright file="Match.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Models.Options;

namespace Monads.Extensions.Options.Sync;

/// <summary>
/// Provides extension methods for pattern matching on <see cref="Option{T}"/> instances.
/// </summary>
public static class MatchExtension
{
    /// <summary>
    /// Matches the option and invokes the appropriate function based on whether it is a <see cref="Some{T}"/> or a <see cref="None{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the option.</typeparam>
    /// <typeparam name="U">The type of the result returned by the match functions.</typeparam>
    /// <param name="self">The option to match.</param>
    /// <param name="onSome">The function to invoke if the option is a <see cref="Some{T}"/>.</param>
    /// <param name="onNone">The function to invoke if the option is a <see cref="None{T}"/>.</param>
    /// <returns>The result of invoking either <paramref name="onSome"/> or <paramref name="onNone"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if either <paramref name="onSome"/> or <paramref name="onNone"/> returns null.</exception>
    /// <exception cref="UnreachableException">Thrown if the option is neither <see cref="Some{T}"/> nor <see cref="None{T}"/>.</exception>
    public static U Match<T, U>(this Option<T> self, Func<T, U> onSome, Func<U> onNone)
        where T : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);

        U result = self switch
        {
            Some<T>(var value) => onSome(value)
                ?? throw new InvalidOperationException(Strings.Constants.OperationNullError),
            None<T> => onNone()
                ?? throw new InvalidOperationException(Strings.Constants.OperationNullError),
            _ => throw new UnreachableException(Strings.Constants.ExhaustedOptionError),
        };

        return result;
    }
}
