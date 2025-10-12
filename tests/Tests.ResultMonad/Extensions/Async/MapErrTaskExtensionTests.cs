// <copyright file="MapErrTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Globalization;
using ResultMonad;
using ResultMonad.Extensions.Async;
using ResultMonad.Extensions.Sync;
using static ResultMonad.ResultFactory;

namespace Tests.ResultMonad.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapErrTaskExtension"/> type.
/// </summary>
public sealed class MapErrTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";

    [Fact]
    public async Task MapErrAsync_WhenCalledWithTaskOkAndSyncFunction_ShouldPreserveValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Result<int, int> mapped = await resultTask.MapErrAsync(error => error.Length);

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task MapErrAsync_WhenCalledWithTaskErrAndSyncFunction_ShouldMapError()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> mapped = await resultTask.MapErrAsync(error => error.Length);

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task MapErrAsync_WhenCalledWithSyncOkAndAsyncFunction_ShouldPreserveValue()
    {
        Result<int, string> result = Success<int, string>(SuccessValue);

        Result<int, int> mapped = await result.MapErrAsync(error => Task.FromResult(error.Length));

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task MapErrAsync_WhenCalledWithSyncErrAndAsyncFunction_ShouldMapError()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);

        Result<int, int> mapped = await result.MapErrAsync(error => Task.FromResult(error.Length));

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task MapErrAsync_WhenCalledWithTaskOkAndAsyncFunction_ShouldPreserveValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Success<int, string>(SuccessValue));

        Result<int, int> mapped = await resultTask.MapErrAsync(error =>
            Task.FromResult(error.Length)
        );

        mapped.IsOk.Should().BeTrue();
        mapped.Match(value => value, error => 0).Should().Be(SuccessValue);
    }

    [Fact]
    public async Task MapErrAsync_WhenCalledWithTaskErrAndAsyncFunction_ShouldMapError()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, int> mapped = await resultTask.MapErrAsync(error =>
            Task.FromResult(error.Length)
        );

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => 0, error => error).Should().Be(ErrorMessage.Length);
    }

    [Fact]
    public async Task MapErrAsync_WhenTaskIsNull_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = null!;

        Func<Task<Result<int, int>>> act = async () =>
            await resultTask.MapErrAsync(error => error.Length);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapErrAsync_WhenOperationIsNullWithTask_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));
        Func<string, int> nullOperation = null!;

        Func<Task<Result<int, int>>> act = async () => await resultTask.MapErrAsync(nullOperation);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapErrAsync_WhenOperationIsNullWithSyncResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Failure<int, string>(ErrorMessage);
        Func<string, Task<int>> nullOperation = null!;

        Func<Task<Result<int, int>>> act = async () => await result.MapErrAsync(nullOperation);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapErrAsync_WhenMappingErrToString_ShouldReturnCorrectString()
    {
        Task<Result<int, int>> resultTask = Task.FromResult(Failure<int, int>(404));

        Result<int, string> mapped = await resultTask.MapErrAsync(error => $"Error code: {error}");

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be("Error code: 404");
    }

    [Fact]
    public async Task MapErrAsync_WhenMappingErrToComplexType_ShouldReturnCorrectType()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>(ErrorMessage));

        Result<int, (bool IsError, string Message)> mapped = await resultTask.MapErrAsync(error =>
            (true, error)
        );

        mapped.IsErr.Should().BeTrue();
        (bool IsError, string Message) tuple = mapped.Match(
            value => (false, string.Empty),
            error => error
        );
        tuple.IsError.Should().BeTrue();
        tuple.Message.Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task MapErrAsync_WhenChainedWithMultipleOperations_ShouldWorkCorrectly()
    {
        Task<Result<int, int>> resultTask = Task.FromResult(Failure<int, int>(10));

        Result<int, int> mapped = await resultTask
            .MapErrAsync(error => error + 5)
            .MapErrAsync(error => error * 2);

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => 0, error => error).Should().Be(30);
    }

    [Fact]
    public async Task MapErrAsync_WhenMappingWithCultureSpecificOperation_ShouldWorkCorrectly()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Failure<int, string>("error"));

        Result<int, string> mapped = await resultTask.MapErrAsync(error =>
            error.ToUpper(CultureInfo.InvariantCulture)
        );

        mapped.IsErr.Should().BeTrue();
        mapped.Match(value => string.Empty, error => error).Should().Be("ERROR");
    }
}
