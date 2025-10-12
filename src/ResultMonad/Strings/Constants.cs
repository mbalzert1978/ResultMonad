// <copyright file="Constants.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace ResultMonad.Strings;

/// <summary>
/// Contains constant string values used throughout the ResultMonad library.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Gets the error message used when a Result type is exhaustively matched but is neither Ok nor Err.
    /// </summary>
    /// <value>
    /// The error message: "Result must be either Ok or Err."
    /// </value>
    public const string ExhaustedError = "Result must be either Ok or Err.";
}
