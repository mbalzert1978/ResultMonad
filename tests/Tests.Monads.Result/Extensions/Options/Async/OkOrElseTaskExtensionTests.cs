// <copyright file="OkOrElseTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;
using Monads.Results;
using Monads.Results.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="OkOrElseTaskExtension"/> type.
/// </summary>
public sealed class OkOrElseTaskExtensionTests
{
    private const int TestValue = 42;
    private const string ErrorValue = "missing";

    [Fact]
    public async Task OkOrElseAsync_WhenTaskSomeAndSyncError_ShouldReturnOk()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));
        bool invoked = false;

        Result<int, string> result = await selfTask.OkOrElseAsync(() =>
        {
            invoked = true;
            return ErrorValue;
        });

        result.IsOk.Should().BeTrue();
        invoked.Should().BeFalse();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenTaskNoneAndSyncError_ShouldReturnErr()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Result<int, string> result = await selfTask.OkOrElseAsync(() => ErrorValue);

        result.IsErr.Should().BeTrue();
        result.Match(_ => string.Empty, e => e).Should().Be(ErrorValue);
    }

    [Fact]
    public async Task OkOrElseAsync_WhenTaskSomeAndAsyncError_ShouldReturnOk()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Result<int, string> result = await selfTask.OkOrElseAsync(() =>
            Task.FromResult(ErrorValue)
        );

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenTaskNoneAndAsyncError_ShouldReturnErr()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Result<int, string> result = await selfTask.OkOrElseAsync(() =>
            Task.FromResult(ErrorValue)
        );

        result.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenOptionSomeAndAsyncError_ShouldReturnOk()
    {
        var self = Option.Some(TestValue);

        Result<int, string> result = await self.OkOrElseAsync(() => Task.FromResult(ErrorValue));

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenOptionNoneAndAsyncError_ShouldReturnErr()
    {
        var self = Option.None<int>();

        Result<int, string> result = await self.OkOrElseAsync(() => Task.FromResult(ErrorValue));

        result.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenSyncErrorIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.OkOrElseAsync((Func<string>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenAsyncErrorOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.OkOrElseAsync((Func<Task<string>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
