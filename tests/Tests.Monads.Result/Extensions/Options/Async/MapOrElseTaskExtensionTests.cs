// <copyright file="MapOrElseTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="MapOrElseTaskExtension"/> type.
/// </summary>
public sealed class MapOrElseTaskExtensionTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public async Task MapOrElseAsync_WhenTaskSomeAndSync_ShouldReturnMappedValue()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        int result = await selfTask.MapOrElseAsync(() => FallbackValue, v => v * 2);

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenTaskNoneAndSync_ShouldReturnFallback()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        int result = await selfTask.MapOrElseAsync(() => FallbackValue, v => v * 2);

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenTaskSomeAndAsync_ShouldReturnMappedValue()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        int result = await selfTask.MapOrElseAsync(
            () => Task.FromResult(FallbackValue),
            v => Task.FromResult(v * 2)
        );

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenTaskNoneAndAsync_ShouldReturnFallback()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        int result = await selfTask.MapOrElseAsync(
            () => Task.FromResult(FallbackValue),
            v => Task.FromResult(v * 2)
        );

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenOptionSomeAndAsync_ShouldReturnMappedValue()
    {
        var self = Option.Some(TestValue);

        int result = await self.MapOrElseAsync(
            () => Task.FromResult(FallbackValue),
            v => Task.FromResult(v * 2)
        );

        result.Should().Be(84);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenOptionNoneAndAsync_ShouldReturnFallback()
    {
        var self = Option.None<int>();

        int result = await self.MapOrElseAsync(
            () => Task.FromResult(FallbackValue),
            v => Task.FromResult(v * 2)
        );

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public async Task MapOrElseAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = null!;

        Func<Task> act = async () => await selfTask.MapOrElseAsync(() => FallbackValue, v => v);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrElseAsync_WhenSyncFallbackIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Func<Task> act = async () => await selfTask.MapOrElseAsync<int, int>(null!, v => v);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapOrElseAsync_WhenAsyncOpOnOptionReceiverIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Task> act = async () =>
            await self.MapOrElseAsync(
                () => Task.FromResult(FallbackValue),
                (Func<int, Task<int>>)null!
            );

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
