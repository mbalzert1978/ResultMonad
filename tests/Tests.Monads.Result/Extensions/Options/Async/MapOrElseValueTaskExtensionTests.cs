// <copyright file="MapOrElseValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapOrElseValueTaskExtension"/> type.
/// </summary>
public sealed class MapOrElseValueTaskExtensionTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public async Task MapOrElseAsync_WhenValueTaskSomeAndSync_ShouldReturnMappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int result = await selfTask.MapOrElseAsync(() => FallbackValue, v => v * 2);

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenValueTaskNoneAndSync_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int result = await selfTask.MapOrElseAsync(() => FallbackValue, v => v * 2);

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenValueTaskSomeAndAsync_ShouldReturnMappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int result = await selfTask.MapOrElseAsync(
            () => new ValueTask<int>(FallbackValue),
            v => new ValueTask<int>(v * 2));

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenValueTaskNoneAndAsync_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int result = await selfTask.MapOrElseAsync(
            () => new ValueTask<int>(FallbackValue),
            v => new ValueTask<int>(v * 2));

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenOptionSomeAndAsync_ShouldReturnMappedValue()
    {
        var self = Option.Some(TestValue);

        int result = await self.MapOrElseAsync(
            () => new ValueTask<int>(FallbackValue),
            v => new ValueTask<int>(v * 2));

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenOptionNoneAndAsync_ShouldReturnFallback()
    {
        var self = Option.None<int>();

        int result = await self.MapOrElseAsync(
            () => new ValueTask<int>(FallbackValue),
            v => new ValueTask<int>(v * 2));

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenSyncFallbackIsNull_ShouldThrowArgumentNullException()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Func<Task> act = async () =>
            await selfTask.MapOrElseAsync<int, int>(null!, v => v);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrElseAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () => await self.MapOrElseAsync(
            () => new ValueTask<int>(FallbackValue),
            (Func<int, ValueTask<int>>)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
