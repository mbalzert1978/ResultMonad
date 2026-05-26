// <copyright file="OrElseTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="OrElseTaskExtension"/> type.
/// </summary>
public sealed class OrElseTaskExtensionTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public async Task OrElseAsync_WhenTaskSomeAndSyncOp_ShouldReturnSelfWithoutInvokingOperation()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));
        bool invoked = false;

        Option<int> result = await selfTask.OrElseAsync(() =>
        {
            invoked = true;
            return Option.Some(FallbackValue);
        });

        result.Should().Be(Option.Some(TestValue));
        invoked.Should().BeFalse();
    }

    [Fact]
    public async Task OrElseAsync_WhenTaskNoneAndSyncOp_ShouldReturnFallback()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Option<int> result = await selfTask.OrElseAsync(() => Option.Some(FallbackValue));

        result.Should().Be(Option.Some(FallbackValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenTaskSomeAndAsyncOp_ShouldReturnSelf()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.OrElseAsync(
            () => Task.FromResult(Option.Some(FallbackValue)));

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenTaskNoneAndAsyncOp_ShouldReturnFallback()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Option<int> result = await selfTask.OrElseAsync(
            () => Task.FromResult(Option.Some(FallbackValue)));

        result.Should().Be(Option.Some(FallbackValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenSomeAndAsyncOp_ShouldReturnSelf()
    {
        var self = Option.Some(TestValue);

        Option<int> result = await self.OrElseAsync(
            () => Task.FromResult(Option.Some(FallbackValue)));

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenNoneAndAsyncOp_ShouldReturnFallback()
    {
        var self = Option.None<int>();

        Option<int> result = await self.OrElseAsync(
            () => Task.FromResult(Option.Some(FallbackValue)));

        result.Should().Be(Option.Some(FallbackValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = null!;

        Func<Task> act = async () => await selfTask.OrElseAsync(() => Option.Some(FallbackValue));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_WhenSyncOpIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.OrElseAsync((Func<Option<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.OrElseAsync((Func<Task<Option<int>>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
