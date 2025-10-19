using System.Diagnostics;

namespace Monads.Models.Options;

/// <summary>
/// Represents the base type for a discriminated union modeling either a value or absence of value.
/// </summary>
public abstract record Option<T>
    where T : notnull
{
    /// <summary>
    /// Gets a value indicating whether the option has a value.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the option is <see cref="Some{T}"/>; otherwise, <c>false</c>.
    /// </returns>
    public abstract bool HasValue { get; }

    /// <summary>
    /// Gets a value indicating whether the option has no value.
    /// </summary>
    public bool IsNone => !HasValue;
}

/// <summary>
/// Provides factory methods for creating <see cref="Option{T}"/> instances.
/// </summary>
public static class OptionFactory
{
    /// <summary>
    /// Creates an <see cref="Option{T}"/> representing a value.
    /// </summary>
    /// <param name="value">The value to wrap in the option.</param>
    /// <returns>An instance of <see cref="Some{T}"/> containing the provided value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided value is <c>null</c>.</exception>
    public static Option<T> Some<T>(T value)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(value);

        Some<T> some = new(value);

        Debug.Assert(some.HasValue, "Some option should have a value.");

        return some;
    }

    /// <summary>
    /// Creates an <see cref="Option{T}"/> representing no value.
    /// </summary>
    /// <returns>An instance of <see cref="None{T}"/>.</returns>
    public static Option<T> None<T>()
        where T : notnull
    {
        None<T> none = new();

        Debug.Assert(none.IsNone, "None option should not have a value.");

        return none;
    }
}
