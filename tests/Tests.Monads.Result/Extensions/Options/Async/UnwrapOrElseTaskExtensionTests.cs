// <copyright file="UnwrapOrElseTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrElseTaskExtension"/> type.
/// </summary>
public sealed class UnwrapOrElseTaskExtensionTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public async Task UnwrapOrElseAsync_WhenTaskSomeAndSync_ShouldReturnWrappedValue()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        int result = await selfTask.UnwrapOrElseAsync(() => FallbackValue);

        result.Should().Be(TestValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenTaskNoneAndSync_ShouldReturnFallback()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        int result = await selfTask.UnwrapOrElseAsync(() => FallbackValue);

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenTaskSomeAndAsync_ShouldReturnWrappedValue()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        int result = await selfTask.UnwrapOrElseAsync(() => Task.FromResult(FallbackValue));

        result.Should().Be(TestValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenTaskNoneAndAsync_ShouldReturnFallback()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        int result = await selfTask.UnwrapOrElseAsync(() => Task.FromResult(FallbackValue));

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenOptionSomeAndAsync_ShouldReturnWrappedValue()
    {
        var self = Option.Some(TestValue);

        int result = await self.UnwrapOrElseAsync(() => Task.FromResult(FallbackValue));

        result.Should().Be(TestValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenOptionNoneAndAsync_ShouldReturnFallback()
    {
        var self = Option.None<int>();

        int result = await self.UnwrapOrElseAsync(() => Task.FromResult(FallbackValue));

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenSyncFallbackIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.UnwrapOrElseAsync((Func<int>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenAsyncFallbackOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.UnwrapOrElseAsync((Func<Task<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
