// <copyright file="MapOrElseTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapOrElseTaskExtension"/> type.
/// </summary>
public sealed class MapOrElseTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task MapOrElseAsync_WhenTaskOkAndSyncFns_ShouldReturnMappedValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        int mapped = await resultTask.MapOrElseAsync(err => err.Length, value => value * 2);

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenTaskErrAndSyncFns_ShouldReturnFallback()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        int mapped = await resultTask.MapOrElseAsync(err => err.Length, value => value * 2);

        mapped.Should().Be(4);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenTaskOkAndAsyncFns_ShouldReturnMappedValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        int mapped = await resultTask.MapOrElseAsync(
            err => Task.FromResult(err.Length),
            value => Task.FromResult(value * 2));

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenTaskErrAndAsyncFns_ShouldReturnFallback()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        int mapped = await resultTask.MapOrElseAsync(
            err => Task.FromResult(err.Length),
            value => Task.FromResult(value * 2));

        mapped.Should().Be(4);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenSyncOkAndAsyncFns_ShouldReturnMappedValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int mapped = await result.MapOrElseAsync(
            err => Task.FromResult(err.Length),
            value => Task.FromResult(value * 2));

        mapped.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenSyncErrAndAsyncFns_ShouldReturnFallback()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int mapped = await result.MapOrElseAsync(
            err => Task.FromResult(err.Length),
            value => Task.FromResult(value * 2));

        mapped.Should().Be(4);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenFallbackIsNullSyncFns_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<string, int> nullFn = null!;
            await resultTask.MapOrElseAsync(nullFn, value => value * 2);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrElseAsync_WhenOperationIsNullSyncFns_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, int> nullFn = null!;
            await resultTask.MapOrElseAsync(err => err.Length, nullFn);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrElseAsync_WhenFallbackIsNullAsyncFnsOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<Task> act = async () =>
        {
            Func<string, Task<int>> nullFn = null!;
            await result.MapOrElseAsync(nullFn, value => Task.FromResult(value * 2));
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
