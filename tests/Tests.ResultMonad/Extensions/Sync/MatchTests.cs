// <copyright file="MatchTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Sync;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="MatchExtension"/> type.
/// </summary>
public sealed class MatchTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public void Match_WhenCalledWithOkResult_ShouldInvokeOnOkFunction()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        int matched = result.Match(value => value * 2, error => 0);

        matched.Should().Be(84);
    }

    [Fact]
    public void Match_WhenCalledWithErrResult_ShouldInvokeOnErrFunction()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        string matched = result.Match(
            value => "success",
            error => error.ToUpper(CultureInfo.InvariantCulture)
        );

        matched.Should().Be("TEST ERROR");
    }

    [Fact]
    public void Match_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<int> act = () => result.Match(value => value, error => 0);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_WhenOnOkIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Func<int> act = () => result.Match(null!, error => 0);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_WhenOnErrIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Func<int> act = () => result.Match(value => value, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_WhenOkResultMatchedToString_ShouldReturnCorrectString()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        string matched = result.Match(value => $"Value: {value}", error => $"Error: {error}");

        matched.Should().Be("Value: 42");
    }

    [Fact]
    public void Match_WhenErrResultMatchedToString_ShouldReturnCorrectString()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        string matched = result.Match(value => $"Value: {value}", error => $"Error: {error}");

        matched.Should().Be("Error: Test error");
    }

    [Fact]
    public void Match_WhenMatchingOkToComplexType_ShouldReturnCorrectType()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        (bool Success, int Value) = result.Match(value => (true, value), error => (false, 0));

        Success.Should().BeTrue();
        Value.Should().Be(SuccessValue);
    }

    [Fact]
    public void Match_WhenMatchingErrToComplexType_ShouldReturnCorrectType()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        (bool Success, string Message) = result.Match(
            value => (true, "OK"),
            error => (false, error)
        );

        Success.Should().BeFalse();
        Message.Should().Be(ErrorMessage);
    }

    [Fact]
    public void Match_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> intermediate = result.Match(
            value => Success<int, string>(value + 5),
            Failure<int, string>
        );

        int matched = intermediate.Match(value => value * 2, error => 0);

        matched.Should().Be(30);
    }
}
