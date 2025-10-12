// <copyright file="MapTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Sync;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="MapExtension"/> type.
/// </summary>
public sealed class MapTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public void Map_WhenCalledWithOkResult_ShouldMapValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, string> mapped = result.Map(value => value * 2);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public void Map_WhenCalledWithErrResult_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, string> mapped = result.Map(value => value * 2);

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Map_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<Result<int, string>> act = () => result.Map(value => value * 2);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Map_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Func<Result<int, string>> act = () => result.Map<int, string, int>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Map_WhenMappingOkToString_ShouldReturnCorrectString()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<string, string> mapped = result.Map(value => $"Value: {value}");

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => string.Empty).Should().Be("Value: 42");
    }

    [Fact]
    public void Map_WhenMappingErrToString_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<string, string> mapped = result.Map(value => $"Value: {value}");

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Map_WhenMappingOkToComplexType_ShouldReturnCorrectType()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<(bool Success, int Value), string> mapped = result.Map(value => (true, value));

        mapped.IsOk.Should().BeTrue();
        (bool Success, int Value) tuple = mapped.Match(value => value, error => (false, 0));
        tuple.Success.Should().BeTrue();
        tuple.Value.Should().Be(SuccessValue);
    }

    [Fact]
    public void Map_WhenMappingErrToComplexType_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<(bool Success, int Value), string> mapped = result.Map(value => (true, value));

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Map_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> mapped = result.Map(value => value + 5).Map(value => value * 2);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(30);
    }

    [Fact]
    public void Map_WhenChainedAndEncountersError_ShouldStopPropagation()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, string> mapped = result.Map(value => value + 5).Map(value => value * 2);

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Map_WhenMappingWithCultureSpecificOperation_ShouldWorkCorrectly()
    {
        Result<string, string> result = Success<string, string>("test");

        Result<string, string> mapped = result.Map(value =>
            value.ToUpper(CultureInfo.InvariantCulture)
        );

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => string.Empty).Should().Be("TEST");
    }
}
