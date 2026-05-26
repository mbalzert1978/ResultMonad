// <copyright file="BindTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="BindTaskExtension"/> type.
/// </summary>
public sealed class BindTaskExtensionTests
{
    private const int TestValue = 42;

    [Fact]
    public async Task BindAsync_WhenTaskSomeAndSyncOp_ShouldReturnOperationResult()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.BindAsync(v => Option.Some(v + 1));

        result.Should().Be(Option.Some(43));
    }

    [Fact]
    public async Task BindAsync_WhenTaskSomeAndSyncOpReturnsNone_ShouldReturnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.BindAsync(_ => Option.None<int>());

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_WhenTaskNoneAndSyncOp_ShouldReturnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Option<int> result = await selfTask.BindAsync(v => Option.Some(v + 1));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_WhenTaskSomeAndAsyncOp_ShouldReturnOperationResult()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.BindAsync(v => Task.FromResult(Option.Some(v + 1)));

        result.Should().Be(Option.Some(43));
    }

    [Fact]
    public async Task BindAsync_WhenTaskNoneAndAsyncOp_ShouldReturnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Option<int> result = await selfTask.BindAsync(v => Task.FromResult(Option.Some(v + 1)));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_WhenSomeAndAsyncOp_ShouldReturnOperationResult()
    {
        var self = Option.Some(TestValue);

        Option<int> result = await self.BindAsync(v => Task.FromResult(Option.Some(v + 1)));

        result.Should().Be(Option.Some(43));
    }

    [Fact]
    public async Task BindAsync_WhenNoneAndAsyncOp_ShouldReturnNone()
    {
        var self = Option.None<int>();

        Option<int> result = await self.BindAsync(v => Task.FromResult(Option.Some(v + 1)));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = null!;

        Func<Task> act = async () => await selfTask.BindAsync(Option.Some);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task BindAsync_WhenSyncOpIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.BindAsync((Func<int, Option<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task BindAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.BindAsync((Func<int, Task<Option<int>>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
