// <copyright file="MapValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using Monads.Results;
using Monads.Results.Extensions.Async;
using Monads.Results.Extensions.Sync;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapValueTaskExtension"/> type.
/// </summary>
public sealed class MapValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public async Task MapAsync_WhenCalledWithValueTaskOkAndSyncFunction_ShouldMapValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Success<int, string>(SuccessValue)
        );

        Result<int, string> mapped = await resultTask.MapAsync(value => value * 2);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithValueTaskErrAndSyncFunction_ShouldPropagateError()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Failure<int, string>(ErrorMessage)
        );

        Result<int, string> mapped = await resultTask.MapAsync(value => value * 2);

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithSyncOkAndAsyncFunction_ShouldMapValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, string> mapped = await result.MapAsync(value =>
            ValueTask.FromResult(value * 2)
        );

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithSyncErrAndAsyncFunction_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, string> mapped = await result.MapAsync(value =>
            ValueTask.FromResult(value * 2)
        );

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithValueTaskOkAndAsyncFunction_ShouldMapValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Success<int, string>(SuccessValue)
        );

        Result<int, string> mapped = await resultTask.MapAsync(value =>
            ValueTask.FromResult(value * 2)
        );

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task MapAsync_WhenCalledWithValueTaskErrAndAsyncFunction_ShouldPropagateError()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Failure<int, string>(ErrorMessage)
        );

        Result<int, string> mapped = await resultTask.MapAsync(value =>
            ValueTask.FromResult(value * 2)
        );

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void MapAsync_WhenOperationIsNullWithSyncFunction_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Success<int, string>(SuccessValue)
        );

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
            Func<int, ValueTask<int>> nullFunc = null!;
            await result.MapAsync(nullFunc);
        };

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MapAsync_WhenOperationIsNullWithAsyncFunctionOnValueTask_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Success<int, string>(SuccessValue)
        );

        Func<Task> act = async () =>
        {
            Func<int, ValueTask<int>> nullFunc = null!;
            await resultTask.MapAsync(nullFunc);
        };

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapAsync_WhenMappingOkToComplexTypeWithAsync_ShouldReturnCorrectType()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<(bool Success, int Value), string> mapped = await result.MapAsync(value =>
            ValueTask.FromResult((true, value))
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
            ValueTask.FromResult((true, value))
        );

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task MapAsync_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> mapped = await result
            .MapAsync(value => ValueTask.FromResult(value + 5))
            .MapAsync(value => value * 2);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(30);
    }

    [Fact]
    public async Task MapAsync_WhenMappingWithCultureSpecificOperation_ShouldWorkCorrectly()
    {
        Result<string, string> result = Success<string, string>("test");

        Result<string, string> mapped = await result.MapAsync(value =>
            ValueTask.FromResult(value.ToUpper(CultureInfo.InvariantCulture))
        );

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => string.Empty).Should().Be("TEST");
    }
}
