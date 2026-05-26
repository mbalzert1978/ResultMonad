// <copyright file="BindValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="BindValueTaskExtension"/> type.
/// </summary>
public sealed class BindValueTaskExtensionTests
{
    private const int TestValue = 42;

    [Fact]
    public async Task BindAsync_WhenValueTaskSomeAndSyncOp_ShouldReturnOperationResult()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.BindAsync(v => Option.Some(v + 1));

        result.Should().Be(Option.Some(43));
    }

    [Fact]
    public async Task BindAsync_WhenValueTaskNoneAndSyncOp_ShouldReturnNone()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Option<int> result = await selfTask.BindAsync(v => Option.Some(v + 1));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_WhenValueTaskSomeAndAsyncOp_ShouldReturnOperationResult()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.BindAsync(v => new ValueTask<Option<int>>(Option.Some(v + 1)));

        result.Should().Be(Option.Some(43));
    }

    [Fact]
    public async Task BindAsync_WhenValueTaskNoneAndAsyncOp_ShouldReturnNone()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Option<int> result = await selfTask.BindAsync(v => new ValueTask<Option<int>>(Option.Some(v)));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_WhenSomeAndAsyncOp_ShouldReturnOperationResult()
    {
        var self = Option.Some(TestValue);

        Option<int> result = await self.BindAsync(v => new ValueTask<Option<int>>(Option.Some(v + 1)));

        result.Should().Be(Option.Some(43));
    }

    [Fact]
    public async Task BindAsync_WhenNoneAndAsyncOp_ShouldReturnNone()
    {
        var self = Option.None<int>();

        Option<int> result = await self.BindAsync(v => new ValueTask<Option<int>>(Option.Some(v)));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_WhenSyncOpIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.BindAsync((Func<int, Option<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task BindAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.BindAsync((Func<int, ValueTask<Option<int>>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
