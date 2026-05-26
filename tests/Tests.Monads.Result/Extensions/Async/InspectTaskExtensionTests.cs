// <copyright file="InspectTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="InspectTaskExtension"/> type.
/// </summary>
public sealed class InspectTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task InspectAsync_WhenTaskOkAndSyncAction_ShouldInvokeActionAndReturnResult()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));
        int seen = 0;

        Result<int, string> inspected = await resultTask.InspectAsync(value => seen = value);

        seen.Should().Be(SuccessValue);
        inspected.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task InspectAsync_WhenTaskErrAndSyncAction_ShouldNotInvokeAction()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));
        bool invoked = false;

        Result<int, string> inspected = await resultTask.InspectAsync(_ => invoked = true);

        invoked.Should().BeFalse();
        inspected.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task InspectAsync_WhenTaskOkAndAsyncAction_ShouldAwaitActionAndReturnResult()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));
        int seen = 0;

        Result<int, string> inspected = await resultTask.InspectAsync(async value =>
        {
            await Task.Yield();
            seen = value;
        });

        seen.Should().Be(SuccessValue);
        inspected.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task InspectAsync_WhenTaskErrAndAsyncAction_ShouldNotInvokeAction()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));
        bool invoked = false;

        Result<int, string> inspected = await resultTask.InspectAsync(async _ =>
        {
            await Task.Yield();
            invoked = true;
        });

        invoked.Should().BeFalse();
        inspected.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task InspectAsync_WhenSyncOkAndAsyncAction_ShouldAwaitActionAndReturnResult()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);
        int seen = 0;

        Func<int, Task> action = async value =>
        {
            await Task.Yield();
            seen = value;
        };

        Result<int, string> inspected = await result.InspectAsync(action);

        seen.Should().Be(SuccessValue);
        inspected.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task InspectAsync_WhenActionIsNullSyncAction_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Action<int> nullAction = null!;
            await resultTask.InspectAsync(nullAction);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task InspectAsync_WhenActionIsNullAsyncActionOnResult_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<Task> act = async () =>
        {
            Func<int, Task> nullAction = null!;
            await result.InspectAsync(nullAction);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
