// <copyright file="BindTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Async;
using ResultMonad.Extensions.Sync;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="BindTaskExtension"/> type.
/// </summary>
public sealed class BindTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public async Task BindAsync_WhenCalledWithTaskOkAndSyncFunction_ShouldBindValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Result<int, string> bound = await resultTask.BindAsync(value =>
            Success<int, string>(value * 2)
        );

        bound.IsOk.Should().BeTrue();
        bound.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task BindAsync_WhenCalledWithTaskErrAndSyncFunction_ShouldPropagateError()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, string> bound = await resultTask.BindAsync(value =>
            Success<int, string>(value * 2)
        );

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task BindAsync_WhenCalledWithSyncOkAndAsyncFunction_ShouldBindValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, string> bound = await result.BindAsync(value =>
            Task.FromResult(Success<int, string>(value * 2))
        );

        bound.IsOk.Should().BeTrue();
        bound.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task BindAsync_WhenCalledWithSyncErrAndAsyncFunction_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, string> bound = await result.BindAsync(value =>
            Task.FromResult(Success<int, string>(value * 2))
        );

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task BindAsync_WhenCalledWithTaskOkAndAsyncFunction_ShouldBindValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Result<int, string> bound = await resultTask.BindAsync(value =>
            Task.FromResult(Success<int, string>(value * 2))
        );

        bound.IsOk.Should().BeTrue();
        bound.Match(value => value, error => 0).Should().Be(84);
    }

    [Fact]
    public async Task BindAsync_WhenCalledWithTaskErrAndAsyncFunction_ShouldPropagateError()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, string> bound = await resultTask.BindAsync(value =>
            Task.FromResult(Success<int, string>(value * 2))
        );

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public void BindAsync_WhenOperationIsNullWithSyncFunction_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, Result<int, string>> nullFunc = null!;
            await resultTask.BindAsync(nullFunc);
        };

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void BindAsync_WhenOperationIsNullWithAsyncFunctionOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Func<Task> act = async () =>
        {
            Func<int, Task<Result<int, string>>> nullFunc = null!;
            await result.BindAsync(nullFunc);
        };

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void BindAsync_WhenOperationIsNullWithAsyncFunctionOnTask_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, Task<Result<int, string>>> nullFunc = null!;
            await resultTask.BindAsync(nullFunc);
        };

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task BindAsync_WhenOperationReturnsErr_ShouldReturnErr()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, string> bound = await result.BindAsync(value =>
            Task.FromResult(Failure<int, string>("Operation error"))
        );

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be("Operation error");
    }

    [Fact]
    public async Task BindAsync_WhenBindingOkToComplexTypeWithAsync_ShouldReturnCorrectType()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<(bool Success, int Value), string> bound = await result.BindAsync(value =>
            Task.FromResult(Success<(bool, int), string>((true, value)))
        );

        bound.IsOk.Should().BeTrue();
        (bool Success, int Value) tuple = bound.Match(value => value, error => (false, 0));
        tuple.Success.Should().BeTrue();
        tuple.Value.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task BindAsync_WhenBindingErrToComplexTypeWithAsync_ShouldPropagateError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<(bool Success, int Value), string> bound = await result.BindAsync(value =>
            Task.FromResult(Success<(bool, int), string>((true, value)))
        );

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task BindAsync_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> bound = await result
            .BindAsync(value => Task.FromResult(Success<int, string>(value + 5)))
            .BindAsync(value => Success<int, string>(value * 2));

        bound.IsOk.Should().BeTrue();
        bound.Match(value => value, error => 0).Should().Be(30);
    }

    [Fact]
    public async Task BindAsync_WhenChainedAndEncountersError_ShouldStopPropagation()
    {
        Result<int, string> result = Success<int, string>(10);

        Result<int, string> bound = await result
            .BindAsync(value => Task.FromResult(Failure<int, string>("First error")))
            .BindAsync(value => Success<int, string>(value * 2));

        bound.IsErr.Should().BeTrue();
        bound.Match(value => string.Empty, error => error).Should().Be("First error");
    }
}
