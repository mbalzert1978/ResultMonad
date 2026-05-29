// <copyright file="FilterValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="FilterValueTaskExtension"/> type.
/// </summary>
public sealed class FilterValueTaskExtensionTests
{
    private const int TestValue = 42;

    [Fact]
    public async Task FilterAsync_WhenValueTaskSomeAndSyncPredicateMatches_ShouldReturnSelf()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.FilterAsync(v => v > 0);

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task FilterAsync_WhenValueTaskSomeAndSyncPredicateRejects_ShouldReturnNone()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.FilterAsync(v => v < 0);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_WhenValueTaskNoneAndSyncPredicate_ShouldReturnNone()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Option<int> result = await selfTask.FilterAsync(_ => true);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_WhenValueTaskSomeAndAsyncPredicateMatches_ShouldReturnSelf()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Option<int> result = await selfTask.FilterAsync(v => new(v > 0));

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task FilterAsync_WhenOptionSomeAndAsyncPredicateRejects_ShouldReturnNone()
    {
        var self = Option.Some(TestValue);

        Option<int> result = await self.FilterAsync(v => new(v < 0));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_WhenOptionNoneAndAsyncPredicate_ShouldReturnNone()
    {
        var self = Option.None<int>();

        Option<int> result = await self.FilterAsync(_ => new(true));

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_WhenSyncPredicateIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.FilterAsync((Func<int, bool>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task FilterAsync_WhenAsyncPredicateOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.FilterAsync((Func<int, ValueTask<bool>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
