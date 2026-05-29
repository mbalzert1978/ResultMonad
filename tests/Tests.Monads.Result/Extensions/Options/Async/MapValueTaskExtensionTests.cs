// <copyright file="MapValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapValueTaskExtension"/> type.
/// </summary>
public sealed class MapValueTaskExtensionTests
{
    private const int TestValue = 42;

    [Fact]
    public async Task MapAsync_WhenValueTaskSomeAndSyncOp_ShouldReturnSomeOfMappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.MapAsync(v => v * 2);

        result.Should().Be(Option.Some(84));
    }

    [Fact]
    public async Task MapAsync_WhenValueTaskNoneAndSyncOp_ShouldReturnNone()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Option<int> result = await selfTask.MapAsync(v => v * 2);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task MapAsync_WhenValueTaskSomeAndAsyncOp_ShouldReturnSomeOfMappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.MapAsync(v => new ValueTask<int>(v * 2));

        result.Should().Be(Option.Some(84));
    }

    [Fact]
    public async Task MapAsync_WhenValueTaskNoneAndAsyncOp_ShouldReturnNone()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Option<int> result = await selfTask.MapAsync(v => new ValueTask<int>(v * 2));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task MapAsync_WhenSomeAndAsyncOp_ShouldReturnSomeOfMappedValue()
    {
        var self = Option.Some(TestValue);

        Option<int> result = await self.MapAsync(v => new ValueTask<int>(v * 2));

        result.Should().Be(Option.Some(84));
    }

    [Fact]
    public async Task MapAsync_WhenNoneAndAsyncOp_ShouldReturnNone()
    {
        var self = Option.None<int>();

        Option<int> result = await self.MapAsync(v => new ValueTask<int>(v * 2));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task MapAsync_WhenSyncOpIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.MapAsync((Func<int, int>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.MapAsync((Func<int, ValueTask<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
