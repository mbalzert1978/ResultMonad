// <copyright file="MapTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapTaskExtension"/> type.
/// </summary>
public sealed class MapTaskExtensionTests
{
    private const int TestValue = 42;

    [Fact]
    public async Task MapAsync_WhenTaskSomeAndSyncOp_ShouldReturnSomeOfMappedValue()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.MapAsync(v => v * 2);

        result.Should().Be(Option.Some(84));
    }

    [Fact]
    public async Task MapAsync_WhenTaskNoneAndSyncOp_ShouldReturnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Option<int> result = await selfTask.MapAsync(v => v * 2);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task MapAsync_WhenTaskSomeAndAsyncOp_ShouldReturnSomeOfMappedValue()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.MapAsync(v => Task.FromResult(v * 2));

        result.Should().Be(Option.Some(84));
    }

    [Fact]
    public async Task MapAsync_WhenTaskNoneAndAsyncOp_ShouldReturnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Option<int> result = await selfTask.MapAsync(v => Task.FromResult(v * 2));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task MapAsync_WhenSomeAndAsyncOp_ShouldReturnSomeOfMappedValue()
    {
        var self = Option.Some(TestValue);

        Option<int> result = await self.MapAsync(v => Task.FromResult(v * 2));

        result.Should().Be(Option.Some(84));
    }

    [Fact]
    public async Task MapAsync_WhenNoneAndAsyncOp_ShouldReturnNone()
    {
        var self = Option.None<int>();

        Option<int> result = await self.MapAsync(v => Task.FromResult(v * 2));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task MapAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = null!;

        Func<Task<Option<int>>> act = async () => await selfTask.MapAsync(v => v * 2);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapAsync_WhenSyncOpIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task<Option<int>>> act = async () =>
            await selfTask.MapAsync((Func<int, int>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapAsync_WhenAsyncOpOnTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task<Option<int>>> act = async () =>
            await selfTask.MapAsync((Func<int, Task<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task<Option<int>>> act = async () =>
            await self.MapAsync((Func<int, Task<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
