// <copyright file="AndTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="AndExtension"/> type.
/// </summary>
public sealed class AndTests
{
    private const int SuccessValue = 42;
    private const string OtherValue = "next";
    private const string ErrorMessage = "Test error";

    [Fact]
    public void And_WhenSelfOkAndOtherOk_ShouldReturnOther()
    {
        Result<int, string> self = Ok<int, string>(SuccessValue);
        Result<string, string> other = Ok<string, string>(OtherValue);

        Result<string, string> combined = self.And(other);

        combined.IsOk.Should().BeTrue();
        combined.Match(value => value, error => string.Empty).Should().Be(OtherValue);
    }

    [Fact]
    public void And_WhenSelfOkAndOtherErr_ShouldReturnOtherErr()
    {
        Result<int, string> self = Ok<int, string>(SuccessValue);
        Result<string, string> other = Err<string, string>("other-err");

        Result<string, string> combined = self.And(other);

        combined.IsErr.Should().BeTrue();
        combined.Match(value => string.Empty, error => error).Should().Be("other-err");
    }

    [Fact]
    public void And_WhenSelfErr_ShouldReturnSelfErrWithNewValueType()
    {
        Result<int, string> self = Err<int, string>(ErrorMessage);
        Result<string, string> other = Ok<string, string>(OtherValue);

        Result<string, string> combined = self.And(other);

        combined.IsErr.Should().BeTrue();
        combined.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void And_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> self = null!;
        Result<string, string> other = Ok<string, string>(OtherValue);

        Func<Result<string, string>> act = () => self.And(other);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void And_WhenOtherIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> self = Ok<int, string>(SuccessValue);
        Result<string, string> other = null!;

        Func<Result<string, string>> act = () => self.And(other);

        act.Should().Throw<ArgumentNullException>();
    }
}
