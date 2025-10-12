// <copyright file="Unit.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Monads.Results;

/// <summary>
/// Represents a type that signifies the absence of a meaningful value, similar to <c>void</c> in methods.
/// </summary>
/// <remarks>
/// The <see cref="Unit"/> type is a singleton, meaning there is only one instance of this type.
/// It is often used in functional programming to indicate that a function does not return a value.
/// </remarks>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, ISpanFormattable
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
    /// Compares the current instance with another <see cref="Unit"/> instance.
    /// </summary>
    /// <param name="other">The <see cref="Unit"/> instance to compare with.</param>
    /// <returns>Always returns <c>0</c> since all <see cref="Unit"/> instances are equal.</returns>
    public int CompareTo(Unit other)
    {
        Debug.Assert(Equals(other), "All Unit instances must be equal.");
        return 0;
    }

    /// <summary>
    /// Determines whether the current instance is equal to another <see cref="Unit"/> instance.
    /// </summary>
    /// <param name="other">The <see cref="Unit"/> instance to compare with.</param>
    /// <returns>Always returns <see langword="true"/> since all <see cref="Unit"/> instances are equal.</returns>
    public bool Equals(Unit other)
    {
        Debug.Assert(
            GetHashCode() == other.GetHashCode(),
            "Equal Unit instances must have equal hash codes."
        );
        return true;
    }

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
        Debug.Assert(VoidReturnValue.Length == 2, "VoidReturnValue must have length 2.");

        if (VoidReturnValue.AsSpan().TryCopyTo(destination))
        {
            charsWritten = VoidReturnValue.Length;
            Debug.Assert(charsWritten > 0, "Chars written must be positive when copy succeeds.");
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
    /// Determines whether one <see cref="Unit"/> instance is less than another.
    /// </summary>
    /// <param name="left">The first <see cref="Unit"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Unit"/> instance to compare.</param>
    /// <returns>Always returns <see langword="false"/>.</returns>
    public static bool operator <(Unit left, Unit right)
    {
        Debug.Assert(left.CompareTo(right) == 0, "CompareTo must return 0 for all Unit instances.");
        return false;
    }

    /// <summary>
    /// Determines whether one <see cref="Unit"/> instance is less than or equal to another.
    /// </summary>
    /// <param name="left">The first <see cref="Unit"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Unit"/> instance to compare.</param>
    /// <returns>Always returns <see langword="true"/>.</returns>
    public static bool operator <=(Unit left, Unit right)
    {
        Debug.Assert(!(left > right), "Operator <= must be opposite of >.");
        return true;
    }

    /// <summary>
    /// Determines whether one <see cref="Unit"/> instance is greater than another.
    /// </summary>
    /// <param name="left">The first <see cref="Unit"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Unit"/> instance to compare.</param>
    /// <returns>Always returns <see langword="false"/>.</returns>
    public static bool operator >(Unit left, Unit right)
    {
        Debug.Assert(left.CompareTo(right) == 0, "CompareTo must return 0 for all Unit instances.");
        return false;
    }

    /// <summary>
    /// Determines whether one <see cref="Unit"/> instance is greater than or equal to another.
    /// </summary>
    /// <param name="left">The first <see cref="Unit"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Unit"/> instance to compare.</param>
    /// <returns>Always returns <see langword="true"/>.</returns>
    public static bool operator >=(Unit left, Unit right)
    {
        Debug.Assert(!(left < right), "Operator >= must be opposite of <.");
        return true;
    }

    /// <summary>
    /// Adds two <see cref="Unit"/> instances.
    /// </summary>
    /// <param name="left">The first <see cref="Unit"/> instance.</param>
    /// <param name="right">The second <see cref="Unit"/> instance.</param>
    /// <returns>A <see cref="Unit"/> instance.</returns>
    public static Unit operator +(Unit left, Unit right)
    {
        Debug.Assert(left == right, "All Unit instances must be equal.");
        Debug.Assert(default == left, "Default Unit must equal any Unit instance.");
        return default;
    }

    /// <summary>
    /// Implicitly converts a <see cref="Unit"/> to a <see cref="ValueTuple"/>.
    /// </summary>
    /// <param name="unit">The <see cref="Unit"/> instance to convert.</param>
    /// <returns>A <see cref="ValueTuple"/>.</returns>
    public static implicit operator ValueTuple(Unit unit)
    {
        Debug.Assert(unit == Default, "Unit instance must equal Default.");
        return default;
    }

    /// <summary>
    /// Implicitly converts a <see cref="ValueTuple"/> to a <see cref="Unit"/>.
    /// </summary>
    /// <param name="tuple">The <see cref="ValueTuple"/> to convert.</param>
    /// <returns>A <see cref="Unit"/> instance.</returns>
    public static implicit operator Unit(ValueTuple tuple)
    {
        Debug.Assert(default == Default, "Default Unit must equal static Default field.");
        return default;
    }

#pragma warning restore IDE0060 // Remove unused parameter
}
