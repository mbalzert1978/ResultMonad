// <copyright file="MapErrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="MapErrExtension"/> type.
/// </summary>
public sealed class MapErrTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public void MapErr_WhenCalledWithOkResult_ShouldPreserveValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, int> mapped = result.MapErr(error => error.Length);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public void MapErr_WhenCalledWithErrResult_ShouldMapError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, int> mapped = result.MapErr(error => error.Length);

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public void MapErr_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<Result<int, int>> act = () => result.MapErr(error => error.Length);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapErr_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Func<Result<int, int>> act = () => result.MapErr<int, string, int>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapErr_WhenMappingErrToString_ShouldReturnCorrectString()
    {
        Result<int, int> result = Failure<int, int>(404);

        Result<int, string> mapped = result.MapErr(error => $"Error code: {error}");

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be("Error code: 404");
    }

    [Fact]
    public void MapErr_WhenMappingOkToString_ShouldPreserveValue()
    {
        Result<int, int> result = Success<int, int>(SuccessValue);

        Result<int, string> mapped = result.MapErr(error => $"Error code: {error}");

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public void MapErr_WhenMappingErrToComplexType_ShouldReturnCorrectType()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, (bool IsError, string Message)> mapped = result.MapErr(error => (true, error));

        mapped.IsErr.Should().BeTrue();
        (bool IsError, string Message) tuple = mapped.Match(
            value => (false, string.Empty),
            error => error
        );
        tuple.IsError.Should().BeTrue();
        tuple.Message.Should().Be(ErrorMessage);
    }

    [Fact]
    public void MapErr_WhenMappingOkToComplexType_ShouldPreserveValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, (bool IsError, string Message)> mapped = result.MapErr(error => (true, error));

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public void MapErr_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Result<int, int> result = Failure<int, int>(10);

        Result<int, int> mapped = result.MapErr(error => error + 5).MapErr(error => error * 2);

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => 0, error => error).Should().Be(30);
    }

    [Fact]
    public void MapErr_WhenChainedAndHasOkValue_ShouldPreserveValue()
    {
        Result<int, int> result = Success<int, int>(SuccessValue);

        Result<int, int> mapped = result.MapErr(error => error + 5).MapErr(error => error * 2);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public void MapErr_WhenMappingWithCultureSpecificOperation_ShouldWorkCorrectly()
    {
        Result<int, string> result = Failure<int, string>("error");

        Result<int, string> mapped = result.MapErr(error =>
            error.ToUpper(CultureInfo.InvariantCulture)
        );

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be("ERROR");
    }
}
