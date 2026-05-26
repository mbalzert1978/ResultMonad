// <copyright file="MapOrTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapOrTaskExtension"/> type.
/// </summary>
public sealed class MapOrTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int Fallback = -1;

    [Fact]
    public async Task MapOrAsync_WhenTaskOkAndSyncOp_ShouldReturnMappedValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        int mapped = await resultTask.MapOrAsync(Fallback, value => value * 2);

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenTaskErrAndSyncOp_ShouldReturnFallback()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        int mapped = await resultTask.MapOrAsync(Fallback, value => value * 2);

        mapped.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenTaskOkAndAsyncOp_ShouldReturnMappedValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        int mapped = await resultTask.MapOrAsync(Fallback, value => Task.FromResult(value * 2));

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenTaskErrAndAsyncOp_ShouldReturnFallback()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        int mapped = await resultTask.MapOrAsync(Fallback, value => Task.FromResult(value * 2));

        mapped.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenSyncOkAndAsyncOp_ShouldReturnMappedValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int mapped = await result.MapOrAsync(Fallback, value => Task.FromResult(value * 2));

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenSyncErrAndAsyncOp_ShouldReturnFallback()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int mapped = await result.MapOrAsync(Fallback, value => Task.FromResult(value * 2));

        mapped.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenOperationIsNullSyncOp_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, int> nullFunc = null!;
            await resultTask.MapOrAsync(Fallback, nullFunc);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrAsync_WhenOperationIsNullAsyncOpOnTask_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, Task<int>> nullFunc = null!;
            await resultTask.MapOrAsync(Fallback, nullFunc);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrAsync_WhenOperationIsNullAsyncOpOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<Task> act = async () =>
        {
            Func<int, Task<int>> nullFunc = null!;
            await result.MapOrAsync(Fallback, nullFunc);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
