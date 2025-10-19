// <copyright file="OrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="OrExtension"/> type.
/// </summary>
public sealed class OrTests
{
    [Fact]
    public void Or_WhenFirstResultIsOkAndSecondResultIsErr_ShouldReturnFirstOkResult()
    {
        // Arrange
        Result<int, string> x = Success<int, string>(2);
        Result<int, string> y = Failure<int, string>("late error");


        Result<int, string> result = x.Or(y);

        // Assert
        result.Should().Be(x);
    }

    [Fact]
    public void Or_WhenFirstResultIsErrAndSecondResultIsOk_ShouldReturnSecondOkResult()
    {
        // Arrange
        Result<int, string> x = Failure<int, string>("early error");
        Result<int, string> y = Success<int, string>(2);

        // Act
        Result<int, string> result = x.Or(y);

        // Assert
        result.Should().Be(y);
    }

    [Fact]
    public void Or_WhenBothResultsAreErr_ShouldReturnSecondErrResult()
    {
        // Arrange
        Result<int, string> x = Failure<int, string>("not a 2");
        Result<int, string> y = Failure<int, string>("late error");

        // Act
        Result<int, string> result = x.Or(y);

        // Assert
        result.Should().Be(y);
    }

    [Fact]
    public void Or_WhenBothResultsAreOk_ShouldReturnFirstOkResult()
    {
        // Arrange
        Result<int, string> x = Success<int, string>(2);
        Result<int, string> y = Success<int, string>(100);

        // Act
        Result<int, string> result = x.Or(y);

        // Assert
        result.Should().Be(x);
    }
}
