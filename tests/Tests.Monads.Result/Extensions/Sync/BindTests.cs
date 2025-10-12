// <copyright file="BindTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="BindExtension"/> type.
/// </summary>
public sealed class BindTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public void Bind_WhenCalledWithOkResult_ShouldBindValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, string> bound = result.Bind(value => Success<int, string>(value * 2));

        bound.IsOk.Should().BeTrue();
        bound.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public void Bind_WhenCalledWithErrResult_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, string> bound = result.Bind(value => Success<int, string>(value * 2));

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Bind_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<Result<int, string>> act = () => result.Bind(value => Success<int, string>(value * 2));

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Bind_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Func<Result<int, string>> act = () => result.Bind<int, string, int>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Bind_WhenOperationReturnsErr_ShouldReturnErr()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, string> bound = result.Bind(value => Failure<int, string>("Operation error"));

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be("Operation error");
    }

    [Fact]
    public void Bind_WhenBindingOkToString_ShouldReturnCorrectString()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<string, string> bound = result.Bind(value =>
            Success<string, string>($"Value: {value}")
        );

        bound.IsOk.Should().BeTrue();
        bound.Match(value => value, error => string.Empty).Should().Be("Value: 42");
    }

    [Fact]
    public void Bind_WhenBindingErrToString_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<string, string> bound = result.Bind(value =>
            Success<string, string>($"Value: {value}")
        );

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Bind_WhenBindingOkToComplexType_ShouldReturnCorrectType()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<(bool Success, int Value), string> bound = result.Bind(value =>
            Success<(bool, int), string>((true, value))
        );

        bound.IsOk.Should().BeTrue();
        (bool Success, int Value) tuple = bound.Match(value => value, error => (false, 0));
        tuple.Success.Should().BeTrue();
        tuple.Value.Should().Be(SuccessValue);
    }

    [Fact]
    public void Bind_WhenBindingErrToComplexType_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<(bool Success, int Value), string> bound = result.Bind(value =>
            Success<(bool, int), string>((true, value))
        );

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Bind_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> bound = result
            .Bind(value => Success<int, string>(value + 5))
            .Bind(value => Success<int, string>(value * 2));

        bound.IsOk.Should().BeTrue();
        bound.Match(value => value, error => 0).Should().Be(30);
    }

    [Fact]
    public void Bind_WhenChainedAndEncountersError_ShouldStopPropagation()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> bound = result
            .Bind(value => Failure<int, string>("First error"))
            .Bind(value => Success<int, string>(value * 2));

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be("First error");
    }

    [Fact]
    public void Bind_WhenBindingWithValidation_ShouldWorkCorrectly()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> bound = result.Bind(value =>
            value > 5 ? Success<int, string>(value) : Failure<int, string>("Value too small")
        );

        bound.IsOk.Should().BeTrue();
        bound.Match(value => value, error => 0).Should().Be(10);
    }
}
