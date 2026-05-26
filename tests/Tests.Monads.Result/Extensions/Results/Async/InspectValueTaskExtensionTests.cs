// <copyright file="InspectValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="InspectValueTaskExtension"/> type.
/// </summary>
public sealed class InspectValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task InspectAsync_WhenValueTaskOkAndSyncAction_ShouldInvokeActionAndReturnResult()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));
        int seen = 0;

        Result<int, string> inspected = await resultTask.InspectAsync(value => seen = value);

        seen.Should().Be(SuccessValue);
        inspected.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task InspectAsync_WhenValueTaskErrAndSyncAction_ShouldNotInvokeAction()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));
        bool invoked = false;

        Result<int, string> inspected = await resultTask.InspectAsync(_ => invoked = true);

        invoked.Should().BeFalse();
        inspected.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task InspectAsync_WhenValueTaskOkAndAsyncAction_ShouldAwaitActionAndReturnResult()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));
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
    public async Task InspectAsync_WhenSyncOkAndValueTaskAction_ShouldAwaitActionAndReturnResult()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);
        int seen = 0;

        Func<int, ValueTask> action = async value =>
        {
            await Task.Yield();
            seen = value;
        };

        Result<int, string> inspected = await result.InspectAsync(action);

        seen.Should().Be(SuccessValue);
        inspected.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task InspectAsync_WhenActionIsNullValueTaskSyncAction_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Action<int> nullAction = null!;
            await resultTask.InspectAsync(nullAction);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task InspectAsync_WhenActionIsNullValueTaskAsyncAction_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        Func<Task> act = async () =>
        {
            Func<int, ValueTask> nullAction = null!;
            await resultTask.InspectAsync(nullAction);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
