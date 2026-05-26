// <copyright file="OrElseTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="OrElseExtension"/> type.
/// </summary>
public sealed class OrElseTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int FallbackValue = 99;

    [Fact]
    public void OrElse_WhenCalledWithOkResult_ShouldReturnOriginalOkValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Result<int, int> recovered = result.OrElse(error => Err<int, int>(error.Length));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public void OrElse_WhenCalledWithErrResult_ShouldCallOperation()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        Result<int, int> recovered = result.OrElse(error => Err<int, int>(error.Length));

        recovered.IsErr.Should().BeTrue();
        recovered.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public void OrElse_WhenCalledWithErrAndOperationReturnsOk_ShouldRecoverWithOkValue()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        Result<int, int> recovered = result.OrElse(error => Ok<int, int>(FallbackValue));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(FallbackValue);
    }

    [Fact]
    public void OrElse_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<Result<int, int>> act = () => result.OrElse(error => Err<int, int>(error.Length));

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void OrElse_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        Func<Result<int, int>> act = () => result.OrElse<int, string, int>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void OrElse_WhenRecoveringErrToOkWithString_ShouldReturnCorrectValue()
    {
        Result<string, int> result = Err<string, int>(404);

        Result<string, string> recovered = result.OrElse(error =>
            Ok<string, string>("Recovered")
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => string.Empty).Should().Be("Recovered");
    }

    [Fact]
    public void OrElse_WhenRecoveringErrToErrWithDifferentType_ShouldReturnNewError()
    {
        Result<string, int> result = Err<string, int>(404);

        Result<string, string> recovered = result.OrElse(error =>
            Err<string, string>($"Error: {error}")
        );

        recovered.IsErr.Should().BeTrue();
        recovered.Match(value => string.Empty, error => error).Should().Be("Error: 404");
    }

    [Fact]
    public void OrElse_WhenOkResultWithComplexType_ShouldPreserveValue()
    {
        Result<(bool Success, int Value), string> result = Ok<
            (bool Success, int Value),
            string
        >((true, SuccessValue));

        Result<(bool Success, int Value), int> recovered = result.OrElse(error =>
            Err<(bool Success, int Value), int>(0)
        );

        recovered.IsOk.Should().BeTrue();
        (bool Success, int Value) tuple = recovered.Match(value => value, error => (false, 0));
        tuple.Success.Should().BeTrue();
        tuple.Value.Should().Be(SuccessValue);
    }

    [Fact]
    public void OrElse_WhenErrResultWithComplexTypeRecovery_ShouldReturnRecoveredValue()
    {
        Result<(bool Success, int Value), string> result = Err<
            (bool Success, int Value),
            string
        >(ErrorMessage);

        Result<(bool Success, int Value), int> recovered = result.OrElse(error =>
            Ok<(bool Success, int Value), int>((false, FallbackValue))
        );

        recovered.IsOk.Should().BeTrue();
        (bool Success, int Value) tuple = recovered.Match(value => value, error => (false, 0));
        tuple.Success.Should().BeFalse();
        tuple.Value.Should().Be(FallbackValue);
    }

    [Fact]
    public void OrElse_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Result<int, int> result = Err<int, int>(10);

        Result<int, int> recovered = result
            .OrElse(error =>
                error < 20 ? Err<int, int>(error * 2) : Ok<int, int>(FallbackValue)
            )
            .OrElse(error => Ok<int, int>(error + 5));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(25);
    }

    [Fact]
    public void OrElse_WhenChainedAndFirstSucceeds_ShouldSkipSecondOperation()
    {
        Result<int, int> result = Err<int, int>(10);

        Result<int, int> recovered = result
            .OrElse(error => Ok<int, int>(FallbackValue))
            .OrElse(error => Ok<int, int>(0));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(FallbackValue);
    }

    [Fact]
    public void OrElse_WhenRecoveringWithCultureSpecificOperation_ShouldWorkCorrectly()
    {
        Result<string, string> result = Err<string, string>("error");

        Result<string, string> recovered = result.OrElse(error =>
            Ok<string, string>(error.ToUpper(CultureInfo.InvariantCulture))
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => string.Empty).Should().Be("ERROR");
    }

    [Fact]
    public void OrElse_WhenRecoveringBasedOnErrorCondition_ShouldWorkCorrectly()
    {
        Result<int, int> result = Err<int, int>(404);

        Result<int, string> recovered = result.OrElse(error =>
            error == 404 ? Ok<int, string>(-1) : Err<int, string>("Unknown error")
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(-1);
    }
}
