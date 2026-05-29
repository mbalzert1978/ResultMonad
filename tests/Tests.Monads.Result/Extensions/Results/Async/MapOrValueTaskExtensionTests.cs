// <copyright file="MapOrValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapOrValueTaskExtension"/> type.
/// </summary>
public sealed class MapOrValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int Fallback = -1;

    [Fact]
    public async Task MapOrAsync_WhenValueTaskOkAndSyncOp_ShouldReturnMappedValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Ok<int, string>(SuccessValue)
        );

        int mapped = await resultTask.MapOrAsync(Fallback, value => value * 2);

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenValueTaskErrAndSyncOp_ShouldReturnFallback()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Err<int, string>(ErrorMessage)
        );

        int mapped = await resultTask.MapOrAsync(Fallback, value => value * 2);

        mapped.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenValueTaskOkAndAsyncOp_ShouldReturnMappedValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Ok<int, string>(SuccessValue)
        );

        int mapped = await resultTask.MapOrAsync(
            Fallback,
            value => ValueTask.FromResult(value * 2)
        );

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenValueTaskErrAndAsyncOp_ShouldReturnFallback()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Err<int, string>(ErrorMessage)
        );

        int mapped = await resultTask.MapOrAsync(
            Fallback,
            value => ValueTask.FromResult(value * 2)
        );

        mapped.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenSyncOkAndValueTaskOp_ShouldReturnMappedValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int mapped = await result.MapOrAsync(Fallback, value => ValueTask.FromResult(value * 2));

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenSyncErrAndValueTaskOp_ShouldReturnFallback()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int mapped = await result.MapOrAsync(Fallback, value => ValueTask.FromResult(value * 2));

        mapped.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenOperationIsNullValueTaskSyncOp_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Ok<int, string>(SuccessValue)
        );

        Func<Task> act = async () =>
        {
            Func<int, int> nullFunc = null!;
            await resultTask.MapOrAsync(Fallback, nullFunc);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrAsync_WhenOperationIsNullValueTaskAsyncOp_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Ok<int, string>(SuccessValue)
        );

        Func<Task> act = async () =>
        {
            Func<int, ValueTask<int>> nullFunc = null!;
            await resultTask.MapOrAsync(Fallback, nullFunc);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrAsync_WhenOperationIsNullValueTaskOpOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<Task> act = async () =>
        {
            Func<int, ValueTask<int>> nullFunc = null!;
            await result.MapOrAsync(Fallback, nullFunc);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
