// <copyright file="Err.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results;

/// <summary>
/// Represents a failed result containing an error.
/// </summary>
/// <typeparam name="T">The type of the success value (not used in this variant).</typeparam>
/// <typeparam name="E">The type of the error.</typeparam>
/// <remarks>
/// This record is one variant of the <see cref="Result{TValue, TError}"/> discriminated union,
/// representing the failure state with an associated error value.
/// Use the <see cref="Error"/> property to access the contained error value.
/// </remarks>
/// <example>
/// <code>
/// var result = new Err&lt;int, string&gt;("Operation failed");
/// Console.WriteLine(result.Error); // Output: Operation failed
/// Console.WriteLine(result.IsErr); // Output: True
/// </code>
/// </example>
public sealed record Err<T, E>(E Error) : Result<T, E>
    where E : notnull
    where T : notnull
{
    /// <summary>
    /// Gets the error value contained in this result.
    /// </summary>
    /// <value>
    /// The error value of type <typeparamref name="E"/>.
    /// </value>
    /// <exception cref="ArgumentNullException">Thrown during construction if the provided error is <c>null</c>.</exception>
    public E Error { get; } = Error ?? throw new ArgumentNullException(nameof(Error));

    /// <inheritdoc/>
    public override bool IsOk => false;

    /// <inheritdoc/>
    public override bool IsErrAnd(Func<E, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return predicate(Error);
    }

    /// <inheritdoc/>
    public override bool IsOkAnd(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return false;
    }
}
