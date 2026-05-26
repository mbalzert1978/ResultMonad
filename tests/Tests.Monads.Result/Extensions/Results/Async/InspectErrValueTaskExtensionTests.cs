// <copyright file="InspectErrValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="InspectErrValueTaskExtension"/> type.
/// </summary>
public sealed class InspectErrValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task InspectErrAsync_WhenValueTaskErrAndSyncAction_ShouldInvokeActionAndReturnResult()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));
        string seen = string.Empty;

        Result<int, string> inspected = await resultTask.InspectErrAsync(err => seen = err);

        seen.Should().Be(ErrorMessage);
        inspected.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task InspectErrAsync_WhenValueTaskOkAndSyncAction_ShouldNotInvokeAction()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));
        bool invoked = false;

        Result<int, string> inspected = await resultTask.InspectErrAsync(_ => invoked = true);

        invoked.Should().BeFalse();
        inspected.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task InspectErrAsync_WhenValueTaskErrAndAsyncAction_ShouldAwaitActionAndReturnResult()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));
        string seen = string.Empty;

        Result<int, string> inspected = await resultTask.InspectErrAsync(async err =>
        {
            await Task.Yield();
            seen = err;
        });

        seen.Should().Be(ErrorMessage);
        inspected.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task InspectErrAsync_WhenSyncErrAndValueTaskAction_ShouldAwaitActionAndReturnResult()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);
        string seen = string.Empty;

        Func<string, ValueTask> action = async err =>
        {
            await Task.Yield();
            seen = err;
        };

        Result<int, string> inspected = await result.InspectErrAsync(action);

        seen.Should().Be(ErrorMessage);
        inspected.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task InspectErrAsync_WhenActionIsNullValueTaskAsyncAction_ShouldThrowArgumentNullException()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));

        Func<Task> act = async () =>
        {
            Func<string, ValueTask> nullAction = null!;
            await resultTask.InspectErrAsync(nullAction);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
