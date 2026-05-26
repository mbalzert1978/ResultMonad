// <copyright file="OrElseValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="OrElseValueTaskExtension"/> type.
/// </summary>
public sealed class OrElseValueTaskExtensionTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public async Task OrElseAsync_WhenValueTaskSomeAndSyncOp_ShouldReturnSelf()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.OrElseAsync(() => Option.Some(FallbackValue));

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenValueTaskNoneAndSyncOp_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Option<int> result = await selfTask.OrElseAsync(() => Option.Some(FallbackValue));

        result.Should().Be(Option.Some(FallbackValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenValueTaskSomeAndAsyncOp_ShouldReturnSelf()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.OrElseAsync(
            () => new ValueTask<Option<int>>(Option.Some(FallbackValue)));

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenValueTaskNoneAndAsyncOp_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Option<int> result = await selfTask.OrElseAsync(
            () => new ValueTask<Option<int>>(Option.Some(FallbackValue)));

        result.Should().Be(Option.Some(FallbackValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenSomeAndAsyncOp_ShouldReturnSelf()
    {
        var self = Option.Some(TestValue);

        Option<int> result = await self.OrElseAsync(
            () => new ValueTask<Option<int>>(Option.Some(FallbackValue)));

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenNoneAndAsyncOp_ShouldReturnFallback()
    {
        var self = Option.None<int>();

        Option<int> result = await self.OrElseAsync(
            () => new ValueTask<Option<int>>(Option.Some(FallbackValue)));

        result.Should().Be(Option.Some(FallbackValue));
    }

    [Fact]
    public async Task OrElseAsync_WhenSyncOpIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.OrElseAsync((Func<Option<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.OrElseAsync((Func<ValueTask<Option<int>>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
