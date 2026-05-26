// <copyright file="MapOrElseValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapOrElseValueTaskExtension"/> type.
/// </summary>
public sealed class MapOrElseValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task MapOrElseAsync_WhenValueTaskOkAndSyncFns_ShouldReturnMappedValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        int mapped = await resultTask.MapOrElseAsync(err => err.Length, value => value * 2);

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenValueTaskErrAndSyncFns_ShouldReturnFallback()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));

        int mapped = await resultTask.MapOrElseAsync(err => err.Length, value => value * 2);

        mapped.Should().Be(4);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenValueTaskOkAndAsyncFns_ShouldReturnMappedValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        int mapped = await resultTask.MapOrElseAsync(
            err => ValueTask.FromResult(err.Length),
            value => ValueTask.FromResult(value * 2));

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenValueTaskErrAndAsyncFns_ShouldReturnFallback()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));

        int mapped = await resultTask.MapOrElseAsync(
            err => ValueTask.FromResult(err.Length),
            value => ValueTask.FromResult(value * 2));

        mapped.Should().Be(4);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenSyncOkAndValueTaskFns_ShouldReturnMappedValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int mapped = await result.MapOrElseAsync(
            err => ValueTask.FromResult(err.Length),
            value => ValueTask.FromResult(value * 2));

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenSyncErrAndValueTaskFns_ShouldReturnFallback()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int mapped = await result.MapOrElseAsync(
            err => ValueTask.FromResult(err.Length),
            value => ValueTask.FromResult(value * 2));

        mapped.Should().Be(4);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenFallbackIsNullValueTaskAsyncFns_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<string, ValueTask<int>> nullFn = null!;
            await resultTask.MapOrElseAsync(nullFn, value => ValueTask.FromResult(value * 2));
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrElseAsync_WhenOperationIsNullValueTaskAsyncFns_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, ValueTask<int>> nullFn = null!;
            await resultTask.MapOrElseAsync(err => ValueTask.FromResult(err.Length), nullFn);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
