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
    /// Gets the error message used when an operation function returns null.
    /// </summary>
    /// <value>
    /// The error message: "The operation function returned null, which is not allowed."
    /// </value>
    public const string OperationNullError =
        "The operation function returned null, which is not allowed.";
}
