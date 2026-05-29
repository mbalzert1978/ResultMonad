// <copyright file="Constants.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Strings;

/// <summary>
/// Contains constant string values used throughout the Monads library.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Gets the error message used when a Result type is exhaustively matched but is neither Ok nor Err.
    /// </summary>
    /// <value>
    /// The error message: "Result must be either Ok or Err."
    /// </value>
    public const string ExhaustedResultError = "Result must be either Ok or Err.";

    /// <summary>
    /// Gets the error message used when an Option type is exhaustively matched but is neither Some nor None.
    /// </summary>
    /// <value>
    /// The error message: "Option must be either Some or None."
    /// </value>
    public const string ExhaustedOptionError = "Option must be either Some or None.";

    /// <summary>
    /// Gets the error message used when an operation function returns null.
    /// </summary>
    /// <value>
    /// The error message: "The operation function returned null, which is not allowed."
    /// </value>
    public const string OperationNullError =
        "The operation function returned null, which is not allowed.";

    /// <summary>
    /// Extension method for any non-nullable type that throws an InvalidOperationException with a predefined error message if the value is null.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="value"></param>
    extension<U>(U value)
        where U : notnull
    {
        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> with a predefined error message if the provided value is null.
        /// </summary>
        /// <returns>The original value if it is not null.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the provided value is null.</exception>
        internal U OrThrowIfNull() =>
            value ?? throw new InvalidOperationException(OperationNullError);
    }
}
