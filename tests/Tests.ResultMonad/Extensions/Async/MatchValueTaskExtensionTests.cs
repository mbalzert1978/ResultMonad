// <copyright file="MatchValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Async;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MatchValueTaskExtension"/> type.
/// </summary>
public sealed class MatchValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public async Task MatchAsync_WhenCalledWithValueTaskOkAndSyncFunctions_ShouldInvokeOnOk()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Success<int, string>(SuccessValue)
        );

        int matched = await resultTask.MatchAsync(value => value * 2, error => 0);

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithValueTaskErrAndSyncFunctions_ShouldInvokeOnErr()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Failure<int, string>(ErrorMessage)
        );

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
            value => ValueTask.FromResult(value * 2),
            error => ValueTask.FromResult(0)
        );

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithSyncErrAndAsyncFunctions_ShouldInvokeOnErr()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        string matched = await result.MatchAsync(
            value => ValueTask.FromResult("success"),
            error => ValueTask.FromResult(error.ToUpper(CultureInfo.InvariantCulture))
        );

        matched.Should().Be("TEST ERROR");
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithValueTaskOkAndAsyncFunctions_ShouldInvokeOnOk()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Success<int, string>(SuccessValue)
        );

        int matched = await resultTask.MatchAsync(
            value => ValueTask.FromResult(value * 2),
            error => ValueTask.FromResult(0)
        );

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenCalledWithValueTaskErrAndAsyncFunctions_ShouldInvokeOnErr()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Failure<int, string>(ErrorMessage)
        );

        string matched = await resultTask.MatchAsync(
            value => ValueTask.FromResult("success"),
            error => ValueTask.FromResult(error.ToUpper(CultureInfo.InvariantCulture))
        );

        matched.Should().Be("TEST ERROR");
    }

    [Fact]
    public void MatchAsync_WhenOnOkIsNullWithSyncFunctions_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Success<int, string>(SuccessValue)
        );

        Func<Task> act = async () => await resultTask.MatchAsync(null!, error => 0);

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MatchAsync_WhenOnErrIsNullWithSyncFunctions_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Failure<int, string>(ErrorMessage)
        );

        Func<Task> act = async () => await resultTask.MatchAsync(value => value, null!);

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MatchAsync_WhenOnOkIsNullWithAsyncFunctionsOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Func<Task> act = async () =>
            await result.MatchAsync(null!, error => ValueTask.FromResult(0));

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MatchAsync_WhenOnErrIsNullWithAsyncFunctionsOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Func<Task> act = async () =>
            await result.MatchAsync(value => ValueTask.FromResult(value), null!);

        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenMatchingOkToComplexTypeWithAsync_ShouldReturnCorrectType()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        (bool Success, int Value) = await result.MatchAsync(
            value => ValueTask.FromResult((true, value)),
            error => ValueTask.FromResult((false, 0))
        );

        Success.Should().BeTrue();
        Value.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task MatchAsync_WhenMatchingErrToComplexTypeWithAsync_ShouldReturnCorrectType()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        (bool Success, string Message) = await result.MatchAsync(
            value => ValueTask.FromResult((true, "OK")),
            error => ValueTask.FromResult((false, error))
        );

        Success.Should().BeFalse();
        Message.Should().Be(ErrorMessage);
    }
}
