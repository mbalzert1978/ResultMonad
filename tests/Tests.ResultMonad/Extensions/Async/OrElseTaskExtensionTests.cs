// <copyright file="OrElseTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Async;
using ResultMonad.Extensions.Sync;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="OrElseTaskExtension"/> type.
/// </summary>
public sealed class OrElseTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int FallbackValue = 99;

    [Fact]
    public async Task OrElseAsync_WhenCalledWithTaskOkAndSyncFunction_ShouldReturnOriginalOkValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Result<int, int> recovered = await resultTask.OrElseAsync(error =>
            Failure<int, int>(error.Length)
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithTaskErrAndSyncFunction_ShouldCallOperation()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> recovered = await resultTask.OrElseAsync(error =>
            Failure<int, int>(error.Length)
        );

        recovered.IsErr.Should().BeTrue();
        recovered.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithTaskErrAndSyncFunctionReturnsOk_ShouldRecoverWithOkValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> recovered = await resultTask.OrElseAsync(error =>
            Success<int, int>(FallbackValue)
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(FallbackValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithSyncOkAndAsyncFunction_ShouldReturnOriginalOkValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, int> recovered = await result.OrElseAsync(error =>
            Task.FromResult(Failure<int, int>(error.Length))
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithSyncErrAndAsyncFunction_ShouldCallOperation()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, int> recovered = await result.OrElseAsync(error =>
            Task.FromResult(Failure<int, int>(error.Length))
        );

        recovered.IsErr.Should().BeTrue();
        recovered.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithSyncErrAndAsyncFunctionReturnsOk_ShouldRecoverWithOkValue()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, int> recovered = await result.OrElseAsync(error =>
            Task.FromResult(Success<int, int>(FallbackValue))
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(FallbackValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithTaskOkAndAsyncFunction_ShouldReturnOriginalOkValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Result<int, int> recovered = await resultTask.OrElseAsync(error =>
            Task.FromResult(Failure<int, int>(error.Length))
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithTaskErrAndAsyncFunction_ShouldCallOperation()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> recovered = await resultTask.OrElseAsync(error =>
            Task.FromResult(Failure<int, int>(error.Length))
        );

        recovered.IsErr.Should().BeTrue();
        recovered.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task OrElseAsync_WhenCalledWithTaskErrAndAsyncFunctionReturnsOk_ShouldRecoverWithOkValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> recovered = await resultTask.OrElseAsync(error =>
            Task.FromResult(Success<int, int>(FallbackValue))
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(FallbackValue);
    }

    [Fact]
    public async Task OrElseAsync_WhenTaskIsNull_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = null!;

        Func<Task<Result<int, int>>> act = async () =>
            await resultTask.OrElseAsync(error => Failure<int, int>(error.Length));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_WhenOperationIsNullWithTask_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));
        Func<string, Result<int, int>> nullOperation = null!;

        Func<Task<Result<int, int>>> act = async () => await resultTask.OrElseAsync(nullOperation);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_WhenOperationIsNullWithSyncResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);
        Func<string, Task<Result<int, int>>> nullOperation = null!;

        Func<Task<Result<int, int>>> act = async () => await result.OrElseAsync(nullOperation);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_WhenRecoveringErrToOkWithString_ShouldReturnCorrectValue()
    {
        Task<Result<string, int>> resultTask = Task.FromResult(Failure<string, int>(404));

        Result<string, string> recovered = await resultTask.OrElseAsync(error =>
            Success<string, string>("Recovered")
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => string.Empty).Should().Be("Recovered");
    }

    [Fact]
    public async Task OrElseAsync_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Task<Result<int, int>> resultTask = Task.FromResult(Failure<int, int>(10));

        Result<int, int> recovered = await resultTask
            .OrElseAsync(error =>
                error < 20 ? Failure<int, int>(error * 2) : Success<int, int>(FallbackValue)
            )
            .OrElseAsync(error => Success<int, int>(error + 5));

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(25);
    }

    [Fact]
    public async Task OrElseAsync_WhenRecoveringWithCultureSpecificOperation_ShouldWorkCorrectly()
    {
        Task<Result<string, string>> resultTask = Task.FromResult(Failure<string, string>("error"));

        Result<string, string> recovered = await resultTask.OrElseAsync(error =>
            Success<string, string>(error.ToUpper(CultureInfo.InvariantCulture))
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => string.Empty).Should().Be("ERROR");
    }

    [Fact]
    public async Task OrElseAsync_WhenRecoveringBasedOnErrorCondition_ShouldWorkCorrectly()
    {
        Task<Result<int, int>> resultTask = Task.FromResult(Failure<int, int>(404));

        Result<int, string> recovered = await resultTask.OrElseAsync(error =>
            error == 404 ? Success<int, string>(-1) : Failure<int, string>("Unknown error")
        );

        recovered.IsOk.Should().BeTrue();
        recovered.Match(value => value, error => 0).Should().Be(-1);
    }
}
