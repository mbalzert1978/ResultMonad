// <copyright file="OkTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests;

/// <summary>
/// Contains unit tests for successful <see cref="Result{T, E}"/> instances created via <see cref="Result.Ok{T, E}(T)"/>.
/// </summary>
public sealed class OkTests
{
    private const int TestValue = 42;

    [Fact]
    public void Ok_WhenConstructedWithValidValue_ShouldExposeValueThroughMatch()
    {
        Result<int, string> result = Ok<int, string>(TestValue);

        result.Should().NotBeNull();
        result.Match(value => value, _ => 0).Should().Be(TestValue);
    }

    [Fact]
    public void Ok_WhenConstructedWithNullValue_ShouldThrowArgumentNullException()
    {
        Func<Result<string, string>> act = () => Ok<string, string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ok_WhenCheckedForIsOk_ShouldReturnTrue()
    {
        Result<int, string> result = Ok<int, string>(TestValue);

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public void Ok_WhenCheckedForIsErr_ShouldReturnFalse()
    {
        Result<int, string> result = Ok<int, string>(TestValue);

        result.IsErr.Should().BeFalse();
    }

    [Fact]
    public void Ok_WhenMatched_ShouldInvokeOnOkBranch()
    {
        Result<int, string> result = Ok<int, string>(TestValue);

        string matched = result.Match(value => $"value:{value}", err => $"error:{err}");

        matched.Should().Be("value:42");
    }

    [Fact]
    public void Ok_WhenComparedWithSameValue_ShouldBeEqual()
    {
        Result<int, string> result1 = Ok<int, string>(TestValue);
        Result<int, string> result2 = Ok<int, string>(TestValue);

        result1.Should().Be(result2);
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Ok_WhenComparedWithDifferentValue_ShouldNotBeEqual()
    {
        Result<int, string> result1 = Ok<int, string>(10);
        Result<int, string> result2 = Ok<int, string>(20);

        result1.Should().NotBe(result2);
        (result1 != result2).Should().BeTrue();
    }

    [Fact]
    public void Ok_WhenComparedWithEqualValues_ShouldBeEqualAndHashCodesMatch()
    {
        Result<string, string> result1 = Ok<string, string>(new string("Same value"));
        Result<string, string> result2 = Ok<string, string>(new string("Same value"));

        result1.Should().Be(result2);
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void Ok_WhenComparedAgainstErr_ShouldNotBeEqual()
    {
        Result<int, string> ok = Ok<int, string>(TestValue);
        Result<int, string> err = Err<int, string>("error");

        ok.Should().NotBe(err);
    }

    [Fact]
    public void Ok_WhenUsedWithDifferentValueTypes_ShouldMaintainTypeInformation()
    {
        Result<string, string> stringResult = Ok<string, string>("String value");
        Result<int, string> intResult = Ok<int, string>(TestValue);

        stringResult.Match(value => value, _ => string.Empty).Should().Be("String value");
        intResult.Match(value => value, _ => 0).Should().Be(TestValue);
    }
}
