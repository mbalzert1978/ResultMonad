// <copyright file="MapTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Async;
using ResultMonad.Extensions.Sync;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapTaskExtension"/> type.
/// </summary>
public sealed class MapTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public async Task MapAsync_WhenCalledWithTaskOkAndSyncFunction_ShouldMapValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Result<int, string> mapped = await resultTask.MapAsync(value => value * 2);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithTaskErrAndSyncFunction_ShouldPropagateError()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, string> mapped = await resultTask.MapAsync(value => value * 2);

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithSyncOkAndAsyncFunction_ShouldMapValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, string> mapped = await result.MapAsync(value => Task.FromResult(value * 2));

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithSyncErrAndAsyncFunction_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, string> mapped = await result.MapAsync(value => Task.FromResult(value * 2));

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithTaskOkAndAsyncFunction_ShouldMapValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Result<int, string> mapped = await resultTask.MapAsync(value => Task.FromResult(value * 2));

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithTaskErrAndAsyncFunction_ShouldPropagateError()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, string> mapped = await resultTask.MapAsync(value => Task.FromResult(value * 2));

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void MapAsync_WhenOperationIsNullWithSyncFunction_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, int> nullFunc = null!;
            await resultTask.MapAsync(nullFunc);
        };

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MapAsync_WhenOperationIsNullWithAsyncFunctionOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Func<Task> act = async () =>
        {
            Func<int, Task<int>> nullFunc = null!;
            await result.MapAsync(nullFunc);
        };

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MapAsync_WhenOperationIsNullWithAsyncFunctionOnTask_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, Task<int>> nullFunc = null!;
            await resultTask.MapAsync(nullFunc);
        };

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapAsync_WhenMappingOkToComplexTypeWithAsync_ShouldReturnCorrectType()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<(bool Success, int Value), string> mapped = await result.MapAsync(value =>
            Task.FromResult((true, value))
        );

        mapped.IsOk.Should().BeTrue();
        (bool Success, int Value) tuple = mapped.Match(value => value, error => (false, 0));
        tuple.Success.Should().BeTrue();
        tuple.Value.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task MapAsync_WhenMappingErrToComplexTypeWithAsync_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<(bool Success, int Value), string> mapped = await result.MapAsync(value =>
            Task.FromResult((true, value))
        );

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task MapAsync_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> mapped = await result
            .MapAsync(value => Task.FromResult(value + 5))
            .MapAsync(value => value * 2);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(30);
    }

    [Fact]
    public async Task MapAsync_WhenMappingWithCultureSpecificOperation_ShouldWorkCorrectly()
    {
        Result<string, string> result = Success<string, string>("test");

        Result<string, string> mapped = await result.MapAsync(value =>
            Task.FromResult(value.ToUpper(CultureInfo.InvariantCulture))
        );

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => string.Empty).Should().Be("TEST");
    }
}
