// <copyright file="MatchTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Async;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MatchTaskExtension"/> type.
/// </summary>
public sealed class MatchTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public async Task MatchAsync_WhenCalledWithTaskOkAndSyncFunctions_ShouldInvokeOnOk()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        int matched = await resultTask.MatchAsync(value => value * 2, error => 0);

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithTaskErrAndSyncFunctions_ShouldInvokeOnErr()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        string matched = await resultTask.MatchAsync(
            value => "success",
            error => error.ToUpper(CultureInfo.InvariantCulture)
        );

        matched.Should().Be("TEST ERROR");
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithSyncOkAndAsyncFunctions_ShouldInvokeOnOk()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        int matched = await result.MatchAsync(
            value => Task.FromResult(value * 2),
            error => Task.FromResult(0)
        );

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithSyncErrAndAsyncFunctions_ShouldInvokeOnErr()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        string matched = await result.MatchAsync(
            value => Task.FromResult("success"),
            error => Task.FromResult(error.ToUpper(CultureInfo.InvariantCulture))
        );

        matched.Should().Be("TEST ERROR");
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithTaskOkAndAsyncFunctions_ShouldInvokeOnOk()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        int matched = await resultTask.MatchAsync(
            value => Task.FromResult(value * 2),
            error => Task.FromResult(0)
        );

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithTaskErrAndAsyncFunctions_ShouldInvokeOnErr()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        string matched = await resultTask.MatchAsync(
            value => Task.FromResult("success"),
            error => Task.FromResult(error.ToUpper(CultureInfo.InvariantCulture))
        );

        matched.Should().Be("TEST ERROR");
    }

    [Fact]
    public void MatchAsync_WhenOnOkIsNullWithSyncFunctions_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Func<Task> act = async () => await resultTask.MatchAsync(null!, error => 0);

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MatchAsync_WhenOnErrIsNullWithSyncFunctions_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Func<Task> act = async () => await resultTask.MatchAsync(value => value, null!);

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MatchAsync_WhenOnOkIsNullWithAsyncFunctionsOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Func<Task> act = async () => await result.MatchAsync(null!, error => Task.FromResult(0));

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MatchAsync_WhenOnErrIsNullWithAsyncFunctionsOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Func<Task> act = async () =>
            await result.MatchAsync(value => Task.FromResult(value), null!);

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenMatchingOkToComplexTypeWithAsync_ShouldReturnCorrectType()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        (bool Success, int Value) = await result.MatchAsync(
            value => Task.FromResult((true, value)),
            error => Task.FromResult((false, 0))
        );

        Success.Should().BeTrue();
        Value.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task MatchAsync_WhenMatchingErrToComplexTypeWithAsync_ShouldReturnCorrectType()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        (bool Success, string Message) = await result.MatchAsync(
            value => Task.FromResult((true, "OK")),
            error => Task.FromResult((false, error))
        );

        Success.Should().BeFalse();
        Message.Should().Be(ErrorMessage);
    }
}
