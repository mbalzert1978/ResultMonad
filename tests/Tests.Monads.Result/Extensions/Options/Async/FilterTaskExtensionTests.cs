// <copyright file="FilterTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="FilterTaskExtension"/> type.
/// </summary>
public sealed class FilterTaskExtensionTests
{
    private const int TestValue = 42;

    [Fact]
    public async Task FilterAsync_WhenTaskSomeAndSyncPredicateMatches_ShouldReturnSelf()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.FilterAsync(v => v > 0);

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task FilterAsync_WhenTaskSomeAndSyncPredicateRejects_ShouldReturnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.FilterAsync(v => v < 0);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_WhenTaskNoneAndSyncPredicate_ShouldReturnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Option<int> result = await selfTask.FilterAsync(_ => true);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_WhenTaskSomeAndAsyncPredicateMatches_ShouldReturnSelf()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.FilterAsync(v => Task.FromResult(v > 0));

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task FilterAsync_WhenTaskSomeAndAsyncPredicateRejects_ShouldReturnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Option<int> result = await selfTask.FilterAsync(v => Task.FromResult(v < 0));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_WhenOptionSomeAndAsyncPredicateMatches_ShouldReturnSelf()
    {
        var self = Option.Some(TestValue);

        Option<int> result = await self.FilterAsync(v => Task.FromResult(v > 0));

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task FilterAsync_WhenOptionNoneAndAsyncPredicate_ShouldReturnNone()
    {
        var self = Option.None<int>();

        Option<int> result = await self.FilterAsync(v => Task.FromResult(v > 0));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = null!;

        Func<Task> act = async () => await selfTask.FilterAsync(_ => true);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task FilterAsync_WhenSyncPredicateIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.FilterAsync((Func<int, bool>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task FilterAsync_WhenAsyncPredicateOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.FilterAsync((Func<int, Task<bool>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
