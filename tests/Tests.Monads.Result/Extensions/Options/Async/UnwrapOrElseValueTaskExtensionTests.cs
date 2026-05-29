// <copyright file="UnwrapOrElseValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrElseValueTaskExtension"/> type.
/// </summary>
public sealed class UnwrapOrElseValueTaskExtensionTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public async Task UnwrapOrElseAsync_WhenValueTaskSomeAndSync_ShouldReturnWrappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int result = await selfTask.UnwrapOrElseAsync(() => FallbackValue);

        result.Should().Be(TestValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenValueTaskNoneAndSync_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int result = await selfTask.UnwrapOrElseAsync(() => FallbackValue);

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenValueTaskSomeAndAsync_ShouldReturnWrappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int result = await selfTask.UnwrapOrElseAsync(() => new(FallbackValue));

        result.Should().Be(TestValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenValueTaskNoneAndAsync_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int result = await selfTask.UnwrapOrElseAsync(() => new(FallbackValue));

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenOptionSomeAndAsync_ShouldReturnWrappedValue()
    {
        var self = Option.Some(TestValue);

        int result = await self.UnwrapOrElseAsync(() => new(FallbackValue));

        result.Should().Be(TestValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenOptionNoneAndAsync_ShouldReturnFallback()
    {
        var self = Option.None<int>();

        int result = await self.UnwrapOrElseAsync(() => new(FallbackValue));

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenSyncFallbackIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.UnwrapOrElseAsync((Func<int>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UnwrapOrElseAsync_WhenAsyncFallbackOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.UnwrapOrElseAsync((Func<ValueTask<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
