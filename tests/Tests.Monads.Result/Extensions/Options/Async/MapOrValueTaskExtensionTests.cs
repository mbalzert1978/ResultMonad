// <copyright file="MapOrValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapOrValueTaskExtension"/> type.
/// </summary>
public sealed class MapOrValueTaskExtensionTests
{
    private const int TestValue = 42;
    private const int Fallback = -1;

    [Fact]
    public async Task MapOrAsync_WhenValueTaskSomeAndSyncOp_ShouldReturnMappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int result = await selfTask.MapOrAsync(Fallback, v => v * 2);

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenValueTaskNoneAndSyncOp_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int result = await selfTask.MapOrAsync(Fallback, v => v * 2);

        result.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenValueTaskSomeAndAsyncOp_ShouldReturnMappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int result = await selfTask.MapOrAsync(Fallback, v => new(v * 2));

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenValueTaskNoneAndAsyncOp_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int result = await selfTask.MapOrAsync(Fallback, v => new(v * 2));

        result.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenSomeAndAsyncOp_ShouldReturnMappedValue()
    {
        var self = Option.Some(TestValue);

        int result = await self.MapOrAsync(Fallback, v => new(v * 2));

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrAsync_WhenNoneAndAsyncOp_ShouldReturnFallback()
    {
        var self = Option.None<int>();

        int result = await self.MapOrAsync(Fallback, v => new(v * 2));

        result.Should().Be(Fallback);
    }

    [Fact]
    public async Task MapOrAsync_WhenSyncOpIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.MapOrAsync(Fallback, (Func<int, int>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () =>
            await self.MapOrAsync(Fallback, (Func<int, ValueTask<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
