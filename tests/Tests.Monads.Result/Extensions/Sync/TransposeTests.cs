// <copyright file="TransposeTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="TransposeExtension"/> type.
/// </summary>
public sealed class TransposeTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public void Transpose_WhenOkSome_ShouldReturnSomeOk()
    {
        Result<Option<int>, string> self = Ok<Option<int>, string>(Option.Some(SuccessValue));

        Option<Result<int, string>> transposed = self.Transpose();

        transposed.IsSome.Should().BeTrue();
        transposed.Value!.IsOk.Should().BeTrue();
        transposed.Value!.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public void Transpose_WhenOkNone_ShouldReturnNone()
    {
        Result<Option<int>, string> self = Ok<Option<int>, string>(Option.None<int>());

        Option<Result<int, string>> transposed = self.Transpose();

        transposed.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Transpose_WhenErr_ShouldReturnSomeErr()
    {
        Result<Option<int>, string> self = Err<Option<int>, string>(ErrorMessage);

        Option<Result<int, string>> transposed = self.Transpose();

        transposed.IsSome.Should().BeTrue();
        transposed.Value!.IsErr.Should().BeTrue();
        transposed.Value!.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Transpose_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<Option<int>, string> self = null!;

        Func<Option<Result<int, string>>> act = () => self.Transpose();

        act.Should().Throw<ArgumentNullException>();
    }
}
