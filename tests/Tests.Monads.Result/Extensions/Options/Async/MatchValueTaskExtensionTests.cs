// <copyright file="MatchValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MatchValueTaskExtension"/> type.
/// </summary>
public sealed class MatchValueTaskExtensionTests
{
    private const int TestValue = 42;
    private const int NoneValue = -1;

    [Fact]
    public async Task MatchAsync_WhenValueTaskOptionIsSomeAndSyncHandlers_ShouldInvokeOnSome()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int matched = await selfTask.MatchAsync(value => value * 2, () => NoneValue);

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenValueTaskOptionIsNoneAndSyncHandlers_ShouldInvokeOnNone()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int matched = await selfTask.MatchAsync(value => value, () => NoneValue);

        matched.Should().Be(NoneValue);
    }

    [Fact]
    public async Task MatchAsync_WhenValueTaskOptionIsSomeAndAsyncHandlers_ShouldInvokeOnSome()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int matched = await selfTask.MatchAsync(
            value => new ValueTask<int>(value * 2),
            () => new ValueTask<int>(NoneValue)
        );

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenValueTaskOptionIsNoneAndAsyncHandlers_ShouldInvokeOnNone()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int matched = await selfTask.MatchAsync(
            value => new ValueTask<int>(value),
            () => new ValueTask<int>(NoneValue)
        );

        matched.Should().Be(NoneValue);
    }

    [Fact]
    public async Task MatchAsync_WhenOptionIsSomeAndAsyncHandlers_ShouldInvokeOnSome()
    {
        var self = Option.Some(TestValue);

        int matched = await self.MatchAsync(
            value => new ValueTask<int>(value * 2),
            () => new ValueTask<int>(NoneValue)
        );

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenOptionIsNoneAndAsyncHandlers_ShouldInvokeOnNone()
    {
        var self = Option.None<int>();

        int matched = await self.MatchAsync(
            value => new ValueTask<int>(value),
            () => new ValueTask<int>(NoneValue)
        );

        matched.Should().Be(NoneValue);
    }

    [Fact]
    public async Task MatchAsync_WhenSyncOnSomeIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task<int>> act = async () => await selfTask.MatchAsync(null!, () => NoneValue);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenSyncOnNoneIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task<int>> act = async () => await selfTask.MatchAsync(value => value, null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenAsyncOnSomeOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task<int>> act = async () =>
            await self.MatchAsync(null!, () => new ValueTask<int>(NoneValue));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
