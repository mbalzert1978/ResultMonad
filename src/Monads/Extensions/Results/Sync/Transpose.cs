// <copyright file="Transpose.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;

namespace Monads.Results.Extensions.Sync;

/// <summary>
/// Provides an extension member for transposing a <see cref="Result{T, E}"/> of an <see cref="Option{T}"/>
/// into an <see cref="Option{T}"/> of a <see cref="Result{T, E}"/>.
/// </summary>
public static class TransposeExtension
{
    extension<T, E>(Result<Option<T>, E> self)
        where T : notnull
        where E : notnull
    {
        /// <summary>
        /// Transposes a result of an option into an option of a result.
        /// <c>Ok(None)</c> becomes <c>None</c>; <c>Ok(Some(x))</c> becomes <c>Some(Ok(x))</c>; <c>Err(e)</c> becomes <c>Some(Err(e))</c>.
        /// </summary>
        /// <returns>The transposed value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="self"/> is <see langword="null"/>.</exception>
        public Option<Result<T, E>> Transpose()
        {
            ArgumentNullException.ThrowIfNull(self);

            return self.Match(
                option =>
                    option.Match(
                        value => Option.Some(Result.Ok<T, E>(value)),
                        Option.None<Result<T, E>>
                    ),
                err => Option.Some(Result.Err<T, E>(err))
            );
        }
    }
}
