// <copyright file="Result.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results;

/// <summary>
/// Represents the base type for a discriminated union modeling either a success or an error.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error value.</typeparam>
/// <remarks>
/// This abstract record serves as the foundation for the Result monad pattern.
/// External code cannot subclass or directly construct this type; values are produced
/// exclusively through the <see cref="Result"/> factory and consumed via extension members.
/// </remarks>
public abstract record Result<T, E>
    where E : notnull
    where T : notnull
{
    private protected Result() { }
}
