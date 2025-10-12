// <copyright file="Ok.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Results;

/// <summary>
/// Represents a successful result containing a value.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error (not used in this variant).</typeparam>
/// <remarks>
/// This record is part of a discriminated union representing a successful outcome.
/// The <typeparamref name="E"/> type parameter is constrained but not used in this variant.
/// Use the <see cref="Value"/> property to access the contained success value.
/// </remarks>
/// <example>
/// <code>
/// var result = new Ok&lt;int, string&gt;(42);
/// Console.WriteLine(result.Value); // Output: 42
/// Console.WriteLine(result.IsOk);  // Output: True
/// </code>
/// </example>
public sealed record Ok<T, E>(T Value) : Result<T, E>
    where E : notnull
    where T : notnull
{
    /// <summary>
    /// Gets the success value contained in this result.
    /// </summary>
    /// <value>
    /// The success value of type <typeparamref name="T"/>.
    /// </value>
    /// <exception cref="ArgumentNullException">Thrown during construction if the provided value is <c>null</c>.</exception>
    public T Value { get; } = Value ?? throw new ArgumentNullException(nameof(Value));

    /// <inheritdoc/>
    public override bool IsOk => true;

    /// <inheritdoc/>
    public override bool IsErrAnd(Func<E, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return false;
    }

    /// <inheritdoc/>
    public override bool IsOkAnd(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return predicate(Value);
    }
}
