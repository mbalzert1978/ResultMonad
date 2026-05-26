// <copyright file="UnwrapOrElseTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrElseTaskExtension"/> type.
/// </summary>
public sealed class UnwrapOrElseTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task UnwrapOrElseAsync_WhenTaskOkAndSyncFn_ShouldReturnInnerValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        int unwrapped = await resultTask.UnwrapOrElseAsync(err => err.Length);

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenTaskErrAndSyncFn_ShouldReturnFallback()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        int unwrapped = await resultTask.UnwrapOrElseAsync(err => err.Length);

        unwrapped.Should().Be(4);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenTaskOkAndAsyncFn_ShouldReturnInnerValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        int unwrapped = await resultTask.UnwrapOrElseAsync(err => Task.FromResult(err.Length));

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenTaskErrAndAsyncFn_ShouldReturnFallback()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        int unwrapped = await resultTask.UnwrapOrElseAsync(err => Task.FromResult(err.Length));

        unwrapped.Should().Be(4);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenSyncOkAndAsyncFn_ShouldReturnInnerValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int unwrapped = await result.UnwrapOrElseAsync(err => Task.FromResult(err.Length));

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenSyncErrAndAsyncFn_ShouldReturnFallback()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int unwrapped = await result.UnwrapOrElseAsync(err => Task.FromResult(err.Length));

        unwrapped.Should().Be(4);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenFallbackIsNullSyncFn_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<string, int> nullFn = null!;
            await resultTask.UnwrapOrElseAsync(nullFn);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenFallbackIsNullAsyncFnOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<Task> act = async () =>
        {
            Func<string, Task<int>> nullFn = null!;
            await result.UnwrapOrElseAsync(nullFn);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
