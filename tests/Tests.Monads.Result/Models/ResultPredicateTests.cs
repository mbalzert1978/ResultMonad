// <copyright file="ResultPredicateTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Tests;

/// <summary>
/// Contains unit tests for the IsOkAnd and IsErrAnd predicate methods on <see cref="Result{T, E}"/> type.
/// </summary>
public sealed class ResultPredicateTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error message";

    [Fact]
    public void IsOkAnd_WhenCalledOnOkWithTruePredicate_ShouldReturnTrue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        bool isOkAnd = result.IsOkAnd(value => value == SuccessValue);

        isOkAnd.Should().BeTrue();
    }

    [Fact]
    public void IsOkAnd_WhenCalledOnOkWithFalsePredicate_ShouldReturnFalse()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        bool isOkAnd = result.IsOkAnd(value => value > 100);

        isOkAnd.Should().BeFalse();
    }

    [Fact]
    public void IsOkAnd_WhenCalledOnErr_ShouldReturnFalse()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        bool isOkAnd = result.IsOkAnd(value => value == SuccessValue);

        isOkAnd.Should().BeFalse();
    }

    [Fact]
    public void IsOkAnd_WhenCalledOnErrWithTruePredicate_ShouldReturnFalse()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        bool isOkAnd = result.IsOkAnd(value => true);

        isOkAnd.Should().BeFalse();
    }

    [Fact]
    public void IsOkAnd_WhenCalledWithComplexPredicate_ShouldEvaluateCorrectly()
    {
        Result<int, string> result = Success<int, string>(50);

        bool isOkAnd = result.IsOkAnd(value => value > 10 && value < 100);

        isOkAnd.Should().BeTrue();
    }

    [Fact]
    public void IsOkAnd_WhenCalledOnOkWithStringType_ShouldWorkCorrectly()
    {
        Result<string, string> result = Success<string, string>("test");

        bool isOkAnd = result.IsOkAnd(value => value.StartsWith('t'));

        isOkAnd.Should().BeTrue();
    }

    [Fact]
    public void IsErrAnd_WhenCalledOnErrWithTruePredicate_ShouldReturnTrue()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        bool isErrAnd = result.IsErrAnd(error => error == ErrorMessage);

        isErrAnd.Should().BeTrue();
    }

    [Fact]
    public void IsErrAnd_WhenCalledOnErrWithFalsePredicate_ShouldReturnFalse()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        bool isErrAnd = result.IsErrAnd(error => error.Contains("Different"));

        isErrAnd.Should().BeFalse();
    }

    [Fact]
    public void IsErrAnd_WhenCalledOnOk_ShouldReturnFalse()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        bool isErrAnd = result.IsErrAnd(error => error == ErrorMessage);

        isErrAnd.Should().BeFalse();
    }

    [Fact]
    public void IsErrAnd_WhenCalledOnOkWithTruePredicate_ShouldReturnFalse()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        bool isErrAnd = result.IsErrAnd(error => true);

        isErrAnd.Should().BeFalse();
    }

    [Fact]
    public void IsErrAnd_WhenCalledWithComplexPredicate_ShouldEvaluateCorrectly()
    {
        Result<int, string> result = Failure<int, string>("Error: Something went wrong");

        bool isErrAnd = result.IsErrAnd(error =>
            error.StartsWith("Error:", StringComparison.Ordinal) && error.Length > 5
        );

        isErrAnd.Should().BeTrue();
    }

    [Fact]
    public void IsErrAnd_WhenCalledOnErrWithIntType_ShouldWorkCorrectly()
    {
        Result<string, int> result = Failure<string, int>(404);

        bool isErrAnd = result.IsErrAnd(error => error >= 400 && error < 500);

        isErrAnd.Should().BeTrue();
    }

    [Fact]
    public void IsOkAnd_WhenChainedWithIsOkCheck_ShouldWorkCorrectly()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        bool isValid = result.IsOk && result.IsOkAnd(value => value > 0);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsErrAnd_WhenChainedWithIsErrCheck_ShouldWorkCorrectly()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        bool isValid = result.IsErr && result.IsErrAnd(error => !string.IsNullOrEmpty(error));

        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsOkAnd_WhenUsedForValidation_ShouldProvideCorrectResult()
    {
        Result<int, string> result = Success<int, string>(15);

        bool isValidRange = result.IsOkAnd(value => value >= 10 && value <= 20);
        bool isOutOfRange = result.IsOkAnd(value => value < 10);

        isValidRange.Should().BeTrue();
        isOutOfRange.Should().BeFalse();
    }

    [Fact]
    public void IsErrAnd_WhenUsedForErrorTypeChecking_ShouldProvideCorrectResult()
    {
        Result<int, string> result = Failure<int, string>("ValidationError: Invalid input");

        bool isValidationError = result.IsErrAnd(error =>
            error.StartsWith("ValidationError:", StringComparison.Ordinal)
        );
        bool isSystemError = result.IsErrAnd(error =>
            error.StartsWith("SystemError:", StringComparison.Ordinal)
        );

        isValidationError.Should().BeTrue();
        isSystemError.Should().BeFalse();
    }

    [Fact]
    public void IsOkAnd_WhenPredicateIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Action act = () => result.IsOkAnd(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("predicate");
    }

    [Fact]
    public void IsErrAnd_WhenPredicateIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Action act = () => result.IsErrAnd(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("predicate");
    }
}
