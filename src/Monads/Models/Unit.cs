// <copyright file="Unit.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Monads.Results;

/// <summary>
/// Represents a type that signifies the absence of a meaningful value, similar to <c>void</c> in methods.
/// </summary>
/// <remarks>
/// The <see cref="Unit"/> type is a singleton, meaning there is only one instance of this type.
/// It is often used in functional programming to indicate that a function does not return a value.
/// </remarks>
public readonly struct Unit : IEquatable<Unit>, ISpanFormattable
{
    private const string VoidReturnValue = "()";

    /// <summary>
    /// Gets the default <see cref="Unit"/> instance.
    /// </summary>
    /// <value>
    /// The singleton instance of <see cref="Unit"/>.
    /// </value>
    public static readonly Unit Default;

    /// <summary>
    /// Determines whether the current instance is equal to another <see cref="Unit"/> instance.
    /// </summary>
    /// <param name="other">The <see cref="Unit"/> instance to compare with.</param>
    /// <returns>Always returns <see langword="true"/> since all <see cref="Unit"/> instances are equal.</returns>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the specified object is a <see cref="Unit"/>; otherwise, <see langword="false"/>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Unit;

    /// <summary>
    /// Returns the hash code for the current instance.
    /// </summary>
    /// <returns>Always returns <c>0</c> since all <see cref="Unit"/> instances are equal.</returns>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Returns the string representation of the current instance.
    /// </summary>
    /// <returns>Always returns <c>"()"</c>.</returns>
    public override string ToString() => VoidReturnValue;

    /// <summary>
    /// Formats the value of the current instance using the specified format.
    /// </summary>
    /// <param name="format">The format to use (ignored).</param>
    /// <param name="formatProvider">The format provider to use (ignored).</param>
    /// <returns>Always returns <c>"()"</c>.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider) => VoidReturnValue;

    /// <summary>
    /// Tries to format the value of the current instance into the provided span of characters.
    /// </summary>
    /// <param name="destination">The span to write the formatted value into.</param>
    /// <param name="charsWritten">The number of characters written to the destination.</param>
    /// <param name="format">The format to use (ignored).</param>
    /// <param name="provider">The format provider to use (ignored).</param>
    /// <returns><see langword="true"/> if formatting was successful; otherwise, <see langword="false"/>.</returns>
    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider
    )
    {
        if (VoidReturnValue.AsSpan().TryCopyTo(destination))
        {
            charsWritten = VoidReturnValue.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }

#pragma warning disable IDE0060 // Remove unused parameter

    /// <summary>
    /// Determines whether two <see cref="Unit"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="Unit"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Unit"/> instance to compare.</param>
    /// <returns>Always returns <see langword="true"/>.</returns>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// Determines whether two <see cref="Unit"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="Unit"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Unit"/> instance to compare.</param>
    /// <returns>Always returns <see langword="false"/>.</returns>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>
    /// Implicitly converts a <see cref="Unit"/> to a <see cref="ValueTuple"/>.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/> instance to convert.</param>
    /// <returns>A <see cref="ValueTuple"/>.</returns>
    public static implicit operator ValueTuple(Unit unit) => default;

    /// <summary>
    /// Implicitly converts a <see cref="ValueTuple"/> to a <see cref="Unit"/>.
    /// </summary>
    /// <param name="tuple">The <see cref="ValueTuple"/> to convert.</param>
    /// <returns>A <see cref="Unit"/> instance.</returns>
    public static implicit operator Unit(ValueTuple tuple) => default;

#pragma warning restore IDE0060 // Remove unused parameter
}
