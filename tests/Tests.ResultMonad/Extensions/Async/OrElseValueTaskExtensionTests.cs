// <copyright file="OrElseValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Async;
using ResultMonad.Extensions.Sync;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="OrElseValueTaskExtension"/> type.
/// </summary>
public sealed class OrElseValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int FallbackValue = 99;

    [Fact]
    public async Task OrElseAsync_WhenCalledWithValueTaskOkAndSyncFunction_ShouldReturnOriginalOkValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Success<int, string>(SuccessValue));

        Result<int, int> recovered = await resultTask.OrElseAsync(error => Failure<int, int>(error.Length));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithValueTaskErrAndSyncFunction_ShouldCallOperation()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> recovered = await resultTask.OrElseAsync(error => Failure<int, int>(error.Length));

        recovered.IsErr.Should().BeTrue();
        recovered.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithValueTaskErrAndSyncFunctionReturnsOk_ShouldRecoverWithOkValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> recovered = await resultTask.OrElseAsync(error => Success<int, int>(FallbackValue));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(FallbackValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithSyncOkAndAsyncFunction_ShouldReturnOriginalOkValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, int> recovered = await result.OrElseAsync(error => ValueTask.FromResult(Failure<int, int>(error.Length)));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithSyncErrAndAsyncFunction_ShouldCallOperation()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, int> recovered = await result.OrElseAsync(error => ValueTask.FromResult(Failure<int, int>(error.Length)));

        recovered.IsErr.Should().BeTrue();
        recovered.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithSyncErrAndAsyncFunctionReturnsOk_ShouldRecoverWithOkValue()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, int> recovered = await result.OrElseAsync(error => ValueTask.FromResult(Success<int, int>(FallbackValue)));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(FallbackValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithValueTaskOkAndAsyncFunction_ShouldReturnOriginalOkValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Success<int, string>(SuccessValue));

        Result<int, int> recovered = await resultTask.OrElseAsync(error => ValueTask.FromResult(Failure<int, int>(error.Length)));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithValueTaskErrAndAsyncFunction_ShouldCallOperation()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> recovered = await resultTask.OrElseAsync(error => ValueTask.FromResult(Failure<int, int>(error.Length)));

        recovered.IsErr.Should().BeTrue();
        recovered.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithValueTaskErrAndAsyncFunctionReturnsOk_ShouldRecoverWithOkValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> recovered = await resultTask.OrElseAsync(error => ValueTask.FromResult(Success<int, int>(FallbackValue)));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(FallbackValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenOperationIsNullWithValueTask_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Failure<int, string>(ErrorMessage));
        Func<string, Result<int, int>> nullOperation = null!;

        Func<Task<Result<int, int>>> act = async () => await resultTask.OrElseAsync(nullOperation);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_WhenOperationIsNullWithSyncResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);
        Func<string, ValueTask<Result<int, int>>> nullOperation = null!;

        Func<Task<Result<int, int>>> act = async () => await result.OrElseAsync(nullOperation);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_WhenRecoveringErrToOkWithString_ShouldReturnCorrectValue()
    {
        ValueTask<Result<string, int>> resultTask = ValueTask.FromResult(Failure<string, int>(404));

        Result<string, string> recovered = await resultTask.OrElseAsync(error => Success<string, string>("Recovered"));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => string.Empty).Should().Be("Recovered");
    }

    [Fact]
    public async Task OrElseAsync_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        ValueTask<Result<int, int>> resultTask = ValueTask.FromResult(Failure<int, int>(10));

        Result<int, int> recovered = await resultTask
            .OrElseAsync(error => error < 20 ? Failure<int, int>(error * 2) : Success<int, int>(FallbackValue))
            .OrElseAsync(error => Success<int, int>(error + 5));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(25);
    }

    [Fact]
    public async Task OrElseAsync_WhenRecoveringWithCultureSpecificOperation_ShouldWorkCorrectly()
    {
        ValueTask<Result<string, string>> resultTask = ValueTask.FromResult(Failure<string, string>("error"));

        Result<string, string> recovered = await resultTask.OrElseAsync(error =>
            Success<string, string>(error.ToUpper(CultureInfo.InvariantCulture))
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => string.Empty).Should().Be("ERROR");
    }

    [Fact]
    public async Task OrElseAsync_WhenRecoveringBasedOnErrorCondition_ShouldWorkCorrectly()
    {
        ValueTask<Result<int, int>> resultTask = ValueTask.FromResult(Failure<int, int>(404));

        Result<int, string> recovered = await resultTask.OrElseAsync(error =>
            error == 404
                ? Success<int, string>(-1)
                : Failure<int, string>("Unknown error")
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(-1);
    }
}
