// <copyright file="ErrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests;

/// <summary>
/// Contains unit tests for failed <see cref="Result{T, E}"/> instances created via <see cref="Result.Err{T, E}(E)"/>.
/// </summary>
public sealed class ErrTests
{
    private const string ErrorMessage = "Test error message";

    [Fact]
    public void Err_WhenConstructedWithValidError_ShouldExposeErrorThroughMatch()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        result.Should().NotBeNull();
        result.Match(_ => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Err_WhenConstructedWithNullError_ShouldThrowArgumentNullException()
    {
        Func<Result<int, string>> act = () => Err<int, string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Err_WhenCheckedForIsOk_ShouldReturnFalse()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        result.IsOk.Should().BeFalse();
    }

    [Fact]
    public void Err_WhenCheckedForIsErr_ShouldReturnTrue()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        result.IsErr.Should().BeTrue();
    }

    [Fact]
    public void Err_WhenMatched_ShouldInvokeOnErrBranch()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        string matched = result.Match(value => $"value:{value}", err => $"error:{err}");

        matched.Should().Be("error:Test error message");
    }

    [Fact]
    public void Err_WhenComparedWithSameError_ShouldBeEqual()
    {
        Result<int, string> result1 = Err<int, string>(ErrorMessage);
        Result<int, string> result2 = Err<int, string>(ErrorMessage);

        result1.Should().Be(result2);
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Err_WhenComparedWithDifferentError_ShouldNotBeEqual()
    {
        Result<int, string> result1 = Err<int, string>("Error 1");
        Result<int, string> result2 = Err<int, string>("Error 2");

        result1.Should().NotBe(result2);
        (result1 != result2).Should().BeTrue();
    }

    [Fact]
    public void Err_WhenComparedWithEqualErrorValues_ShouldBeEqualAndHashCodesMatch()
    {
        Result<int, string> result1 = Err<int, string>(new string("Same message"));
        Result<int, string> result2 = Err<int, string>(new string("Same message"));

        result1.Should().Be(result2);
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void Err_WhenComparedAgainstOk_ShouldNotBeEqual()
    {
        Result<int, string> err = Err<int, string>(ErrorMessage);
        Result<int, string> ok = Ok<int, string>(42);

        err.Should().NotBe(ok);
    }

    [Fact]
    public void Err_WhenUsedWithDifferentErrorTypes_ShouldMaintainTypeInformation()
    {
        Result<int, string> stringResult = Err<int, string>("String error");
        Result<int, int> intResult = Err<int, int>(42);

        stringResult.Match(_ => string.Empty, err => err).Should().Be("String error");
        intResult.Match(_ => 0, err => err).Should().Be(42);
    }
}
