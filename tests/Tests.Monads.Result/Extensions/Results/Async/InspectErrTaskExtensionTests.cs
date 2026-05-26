// <copyright file="InspectErrTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="InspectErrTaskExtension"/> type.
/// </summary>
public sealed class InspectErrTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task InspectErrAsync_WhenTaskErrAndSyncAction_ShouldInvokeActionAndReturnResult()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));
        string seen = string.Empty;

        Result<int, string> inspected = await resultTask.InspectErrAsync(err => seen = err);

        seen.Should().Be(ErrorMessage);
        inspected.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task InspectErrAsync_WhenTaskOkAndSyncAction_ShouldNotInvokeAction()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));
        bool invoked = false;

        Result<int, string> inspected = await resultTask.InspectErrAsync(_ => invoked = true);

        invoked.Should().BeFalse();
        inspected.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task InspectErrAsync_WhenTaskErrAndAsyncAction_ShouldAwaitActionAndReturnResult()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));
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
    public async Task InspectErrAsync_WhenSyncErrAndAsyncAction_ShouldAwaitActionAndReturnResult()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);
        string seen = string.Empty;

        Func<string, Task> action = async err =>
        {
            await Task.Yield();
            seen = err;
        };

        Result<int, string> inspected = await result.InspectErrAsync(action);

        seen.Should().Be(ErrorMessage);
        inspected.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task InspectErrAsync_WhenActionIsNullSyncAction_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        Func<Task> act = async () =>
        {
            Action<string> nullAction = null!;
            await resultTask.InspectErrAsync(nullAction);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
