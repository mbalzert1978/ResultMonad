// <copyright file="UnwrapOrElseValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrElseValueTaskExtension"/> type.
/// </summary>
public sealed class UnwrapOrElseValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task UnwrapOrElseAsync_WhenValueTaskOkAndSyncFn_ShouldReturnInnerValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        int unwrapped = await resultTask.UnwrapOrElseAsync(err => err.Length);

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenValueTaskErrAndSyncFn_ShouldReturnFallback()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));

        int unwrapped = await resultTask.UnwrapOrElseAsync(err => err.Length);

        unwrapped.Should().Be(4);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenValueTaskOkAndAsyncFn_ShouldReturnInnerValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        int unwrapped = await resultTask.UnwrapOrElseAsync(err => ValueTask.FromResult(err.Length));

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenValueTaskErrAndAsyncFn_ShouldReturnFallback()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));

        int unwrapped = await resultTask.UnwrapOrElseAsync(err => ValueTask.FromResult(err.Length));

        unwrapped.Should().Be(4);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenSyncOkAndValueTaskFn_ShouldReturnInnerValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int unwrapped = await result.UnwrapOrElseAsync(err => ValueTask.FromResult(err.Length));

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenSyncErrAndValueTaskFn_ShouldReturnFallback()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int unwrapped = await result.UnwrapOrElseAsync(err => ValueTask.FromResult(err.Length));

        unwrapped.Should().Be(4);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenFallbackIsNullValueTaskFn_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<string, ValueTask<int>> nullFn = null!;
            await resultTask.UnwrapOrElseAsync(nullFn);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
