// <copyright file="FlattenTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="FlattenExtension"/> type.
/// </summary>
public sealed class FlattenTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const string InnerErrorMessage = "Inner error";

    [Fact]
    public void Flatten_WhenCalledWithOkOkResult_ShouldReturnInnerOkValue()
    {
        Result<int, string> innerResult = Success<int, string>(SuccessValue);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );

        Result<int, string> flattened = nestedResult.Flatten();

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public void Flatten_WhenCalledWithOkErrResult_ShouldReturnInnerErrValue()
    {
        Result<int, string> innerResult = Failure<int, string>(InnerErrorMessage);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );

        Result<int, string> flattened = nestedResult.Flatten();

        flattened.IsErr.Should().BeTrue();
        flattened.Match(value => string.Empty, error => error).Should().Be(InnerErrorMessage);
    }

    [Fact]
    public void Flatten_WhenCalledWithErrResult_ShouldReturnOuterErrValue()
    {
        Result<Result<int, string>, string> nestedResult = Failure<Result<int, string>, string>(
            ErrorMessage
        );

        Result<int, string> flattened = nestedResult.Flatten();

        flattened.IsErr.Should().BeTrue();
        flattened.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void Flatten_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        Result<Result<int, string>, string> nestedResult = null!;

        Func<Result<int, string>> act = () => nestedResult.Flatten();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Flatten_WhenCalledWithComplexNestedOkOk_ShouldWorkCorrectly()
    {
        Result<string, string> innerResult = Success<string, string>("inner value");
        Result<Result<string, string>, string> nestedResult = Success<
            Result<string, string>,
            string
        >(innerResult);

        Result<string, string> flattened = nestedResult.Flatten();

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => string.Empty).Should().Be("inner value");
    }

    [Fact]
    public void Flatten_WhenChainedWithMap_ShouldWorkCorrectly()
    {
        Result<int, string> innerResult = Success<int, string>(10);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );

        Result<int, string> flattened = nestedResult.Flatten().Map(value => value * 2);

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => 0).Should().Be(20);
    }

    [Fact]
    public void Flatten_WhenChainedWithBind_ShouldWorkCorrectly()
    {
        Result<int, string> innerResult = Success<int, string>(10);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );

        Result<int, string> result = nestedResult
            .Flatten()
            .Bind(value => Success<int, string>(value + 5));

        result.IsOk.Should().BeTrue();
        result.Match(value => value, error => 0).Should().Be(15);
    }

    [Fact]
    public void Flatten_WhenCalledWithDifferentTypes_ShouldWorkCorrectly()
    {
        Result<bool, int> innerResult = Success<bool, int>(true);
        Result<Result<bool, int>, int> nestedResult = Success<Result<bool, int>, int>(innerResult);

        Result<bool, int> flattened = nestedResult.Flatten();

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => false).Should().BeTrue();
    }

    [Fact]
    public void Flatten_WhenCalledWithDifferentTypesAndError_ShouldPropagateError()
    {
        Result<Result<bool, int>, int> nestedResult = Failure<Result<bool, int>, int>(404);

        Result<bool, int> flattened = nestedResult.Flatten();

        flattened.IsErr.Should().BeTrue();
        flattened.Match(value => 0, error => error).Should().Be(404);
    }
}
