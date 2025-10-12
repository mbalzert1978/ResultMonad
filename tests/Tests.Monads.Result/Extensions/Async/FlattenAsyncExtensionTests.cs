// <copyright file="FlattenAsyncExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using Monads.Results.Extensions.Sync;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="FlattenAsyncExtension"/> type.
/// </summary>
public sealed class FlattenAsyncExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const string InnerErrorMessage = "Inner error";

    [Fact]
    public async Task FlattenAsync_WhenCalledWithTaskOkOkResult_ShouldReturnInnerOkValue()
    {
        Result<int, string> innerResult = Success<int, string>(SuccessValue);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );
        Task<Result<Result<int, string>, string>> taskResult = Task.FromResult(nestedResult);

        Result<int, string> flattened = await taskResult.FlattenAsync();

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithTaskOkErrResult_ShouldReturnInnerErrValue()
    {
        Result<int, string> innerResult = Failure<int, string>(InnerErrorMessage);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );
        Task<Result<Result<int, string>, string>> taskResult = Task.FromResult(nestedResult);

        Result<int, string> flattened = await taskResult.FlattenAsync();

        flattened.IsErr.Should().BeTrue();
        flattened.Match(value => string.Empty, error => error).Should().Be(InnerErrorMessage);
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithTaskErrResult_ShouldReturnOuterErrValue()
    {
        Result<Result<int, string>, string> nestedResult = Failure<Result<int, string>, string>(
            ErrorMessage
        );
        Task<Result<Result<int, string>, string>> taskResult = Task.FromResult(nestedResult);

        Result<int, string> flattened = await taskResult.FlattenAsync();

        flattened.IsErr.Should().BeTrue();
        flattened.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task FlattenAsync_WhenTaskIsNull_ShouldThrowArgumentNullException()
    {
        Task<Result<Result<int, string>, string>> taskResult = null!;

        Func<Task<Result<int, string>>> act = async () => await taskResult.FlattenAsync();

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithValueTaskOkOkResult_ShouldReturnInnerOkValue()
    {
        Result<int, string> innerResult = Success<int, string>(SuccessValue);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );
        ValueTask<Result<Result<int, string>, string>> valueTaskResult = ValueTask.FromResult(
            nestedResult
        );

        Result<int, string> flattened = await valueTaskResult.FlattenAsync();

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithValueTaskOkErrResult_ShouldReturnInnerErrValue()
    {
        Result<int, string> innerResult = Failure<int, string>(InnerErrorMessage);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );
        ValueTask<Result<Result<int, string>, string>> valueTaskResult = ValueTask.FromResult(
            nestedResult
        );

        Result<int, string> flattened = await valueTaskResult.FlattenAsync();

        flattened.IsErr.Should().BeTrue();
        flattened.Match(value => string.Empty, error => error).Should().Be(InnerErrorMessage);
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithValueTaskErrResult_ShouldReturnOuterErrValue()
    {
        Result<Result<int, string>, string> nestedResult = Failure<Result<int, string>, string>(
            ErrorMessage
        );
        ValueTask<Result<Result<int, string>, string>> valueTaskResult = ValueTask.FromResult(
            nestedResult
        );

        Result<int, string> flattened = await valueTaskResult.FlattenAsync();

        flattened.IsErr.Should().BeTrue();
        flattened.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task FlattenAsync_WhenChainedWithMapAsync_ShouldWorkCorrectly()
    {
        Result<int, string> innerResult = Success<int, string>(10);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );
        Task<Result<Result<int, string>, string>> taskResult = Task.FromResult(nestedResult);

        Result<int, string> result = await taskResult.FlattenAsync().MapAsync(value => value * 2);

        result.IsOk.Should().BeTrue();
        result.Match(value => value, error => 0).Should().Be(20);
    }

    [Fact]
    public async Task FlattenAsync_WhenChainedWithBindAsync_ShouldWorkCorrectly()
    {
        Result<int, string> innerResult = Success<int, string>(10);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );
        Task<Result<Result<int, string>, string>> taskResult = Task.FromResult(nestedResult);

        Result<int, string> result = await taskResult
            .FlattenAsync()
            .BindAsync(value => Task.FromResult(Success<int, string>(value + 5)));

        result.IsOk.Should().BeTrue();
        result.Match(value => value, error => 0).Should().Be(15);
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithComplexNestedOkOk_ShouldWorkCorrectly()
    {
        Result<string, string> innerResult = Success<string, string>("inner value");
        Result<Result<string, string>, string> nestedResult = Success<
            Result<string, string>,
            string
        >(innerResult);
        Task<Result<Result<string, string>, string>> taskResult = Task.FromResult(nestedResult);

        Result<string, string> flattened = await taskResult.FlattenAsync();

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => string.Empty).Should().Be("inner value");
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithDifferentTypes_ShouldWorkCorrectly()
    {
        Result<bool, int> innerResult = Success<bool, int>(true);
        Result<Result<bool, int>, int> nestedResult = Success<Result<bool, int>, int>(innerResult);
        Task<Result<Result<bool, int>, int>> taskResult = Task.FromResult(nestedResult);

        Result<bool, int> flattened = await taskResult.FlattenAsync();

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => false).Should().BeTrue();
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithDifferentTypesAndError_ShouldPropagateError()
    {
        Result<Result<bool, int>, int> nestedResult = Failure<Result<bool, int>, int>(404);
        Task<Result<Result<bool, int>, int>> taskResult = Task.FromResult(nestedResult);

        Result<bool, int> flattened = await taskResult.FlattenAsync();

        flattened.IsErr.Should().BeTrue();
        flattened.Match(value => 0, error => error).Should().Be(404);
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithValueTaskAndComplexType_ShouldWorkCorrectly()
    {
        Result<string, string> innerResult = Success<string, string>("test value");
        Result<Result<string, string>, string> nestedResult = Success<
            Result<string, string>,
            string
        >(innerResult);
        ValueTask<Result<Result<string, string>, string>> valueTaskResult = ValueTask.FromResult(
            nestedResult
        );

        Result<string, string> flattened = await valueTaskResult.FlattenAsync();

        flattened.IsOk.Should().BeTrue();
        flattened.Match(value => value, error => string.Empty).Should().Be("test value");
    }

    [Fact]
    public async Task FlattenAsync_WhenCalledWithValueTaskChainedWithMap_ShouldWorkCorrectly()
    {
        Result<int, string> innerResult = Success<int, string>(15);
        Result<Result<int, string>, string> nestedResult = Success<Result<int, string>, string>(
            innerResult
        );
        ValueTask<Result<Result<int, string>, string>> valueTaskResult = ValueTask.FromResult(
            nestedResult
        );

        Result<string, string> result = await valueTaskResult
            .FlattenAsync()
            .MapAsync(value => $"Value: {value}");

        result.IsOk.Should().BeTrue();
        result.Match(value => value, error => string.Empty).Should().Be("Value: 15");
    }
}
