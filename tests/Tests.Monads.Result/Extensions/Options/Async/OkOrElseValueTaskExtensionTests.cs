// <copyright file="OkOrElseValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;
using Monads.Results;
using Monads.Results.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="OkOrElseValueTaskExtension"/> type.
/// </summary>
public sealed class OkOrElseValueTaskExtensionTests
{
    private const int TestValue = 42;
    private const string ErrorValue = "missing";

    [Fact]
    public async Task OkOrElseAsync_WhenValueTaskSomeAndSyncError_ShouldReturnOk()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Result<int, string> result = await selfTask.OkOrElseAsync(() => ErrorValue);

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenValueTaskNoneAndSyncError_ShouldReturnErr()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Result<int, string> result = await selfTask.OkOrElseAsync(() => ErrorValue);

        result.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenValueTaskSomeAndAsyncError_ShouldReturnOk()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Result<int, string> result = await selfTask.OkOrElseAsync(() =>
            new ValueTask<string>(ErrorValue)
        );

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenValueTaskNoneAndAsyncError_ShouldReturnErr()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Result<int, string> result = await selfTask.OkOrElseAsync(() =>
            new ValueTask<string>(ErrorValue)
        );

        result.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenOptionSomeAndAsyncError_ShouldReturnOk()
    {
        var self = Option.Some(TestValue);

        Result<int, string> result = await self.OkOrElseAsync(() =>
            new ValueTask<string>(ErrorValue)
        );

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenOptionNoneAndAsyncError_ShouldReturnErr()
    {
        var self = Option.None<int>();

        Result<int, string> result = await self.OkOrElseAsync(() =>
            new ValueTask<string>(ErrorValue)
        );

        result.IsErr.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenSyncErrorIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.OkOrElseAsync((Func<string>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OkOrElseAsync_WhenAsyncErrorOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.OkOrElseAsync((Func<ValueTask<string>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
