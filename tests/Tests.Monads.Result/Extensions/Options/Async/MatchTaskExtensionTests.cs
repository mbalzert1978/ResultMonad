// <copyright file="MatchTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MatchTaskExtension"/> type.
/// </summary>
public sealed class MatchTaskExtensionTests
{
    private const int TestValue = 42;
    private const int NoneValue = -1;

    [Fact]
    public async Task MatchAsync_WhenTaskOptionIsSomeAndSyncHandlers_ShouldInvokeOnSome()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        int matched = await selfTask.MatchAsync(value => value * 2, () => NoneValue);

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenTaskOptionIsNoneAndSyncHandlers_ShouldInvokeOnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        int matched = await selfTask.MatchAsync(value => value, () => NoneValue);

        matched.Should().Be(NoneValue);
    }

    [Fact]
    public async Task MatchAsync_WhenTaskOptionIsSomeAndAsyncHandlers_ShouldInvokeOnSome()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        int matched = await selfTask.MatchAsync(
            value => Task.FromResult(value * 2),
            () => Task.FromResult(NoneValue)
        );

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenTaskOptionIsNoneAndAsyncHandlers_ShouldInvokeOnNone()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        int matched = await selfTask.MatchAsync(
            value => Task.FromResult(value),
            () => Task.FromResult(NoneValue)
        );

        matched.Should().Be(NoneValue);
    }

    [Fact]
    public async Task MatchAsync_WhenOptionIsSomeAndAsyncHandlers_ShouldInvokeOnSome()
    {
        var self = Option.Some(TestValue);

        int matched = await self.MatchAsync(
            value => Task.FromResult(value * 2),
            () => Task.FromResult(NoneValue)
        );

        matched.Should().Be(84);
    }

    [Fact]
    public async Task MatchAsync_WhenOptionIsNoneAndAsyncHandlers_ShouldInvokeOnNone()
    {
        var self = Option.None<int>();

        int matched = await self.MatchAsync(
            value => Task.FromResult(value),
            () => Task.FromResult(NoneValue)
        );

        matched.Should().Be(NoneValue);
    }

    [Fact]
    public async Task MatchAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = null!;

        Func<Task<int>> act = async () =>
            await selfTask.MatchAsync(value => value, () => NoneValue);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenSyncOnSomeIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task<int>> act = async () => await selfTask.MatchAsync(null!, () => NoneValue);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenSyncOnNoneIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task<int>> act = async () => await selfTask.MatchAsync(value => value, null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenAsyncOnSomeOnTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task<int>> act = async () =>
            await selfTask.MatchAsync(null!, () => Task.FromResult(NoneValue));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenAsyncOnSomeOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task<int>> act = async () =>
            await self.MatchAsync(null!, () => Task.FromResult(NoneValue));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_WhenAsyncOnNoneOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task<int>> act = async () =>
            await self.MatchAsync(value => Task.FromResult(value), null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
