// <copyright file="FlattenAsyncExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="FlattenAsyncExtension"/> type.
/// </summary>
public sealed class FlattenAsyncExtensionTests
{
    private const int TestValue = 42;

    [Fact]
    public async Task FlattenAsync_WhenTaskSomeOfSome_ShouldReturnInnerSome()
    {
        Task<Option<Option<int>>> selfTask = Task.FromResult(Option.Some(Option.Some(TestValue)));

        Option<int> result = await selfTask.FlattenAsync();

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task FlattenAsync_WhenTaskSomeOfNone_ShouldReturnNone()
    {
        Task<Option<Option<int>>> selfTask = Task.FromResult(Option.Some(Option.None<int>()));

        Option<int> result = await selfTask.FlattenAsync();

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FlattenAsync_WhenTaskNone_ShouldReturnNone()
    {
        Task<Option<Option<int>>> selfTask = Task.FromResult(Option.None<Option<int>>());

        Option<int> result = await selfTask.FlattenAsync();

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FlattenAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<Option<int>>> selfTask = null!;

        Func<Task> act = async () => await selfTask.FlattenAsync();

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task FlattenAsync_WhenValueTaskSomeOfSome_ShouldReturnInnerSome()
    {
        ValueTask<Option<Option<int>>> selfTask = new(Option.Some(Option.Some(TestValue)));

        Option<int> result = await selfTask.FlattenAsync();

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public async Task FlattenAsync_WhenValueTaskSomeOfNone_ShouldReturnNone()
    {
        ValueTask<Option<Option<int>>> selfTask = new(Option.Some(Option.None<int>()));

        Option<int> result = await selfTask.FlattenAsync();

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task FlattenAsync_WhenValueTaskNone_ShouldReturnNone()
    {
        ValueTask<Option<Option<int>>> selfTask = new(Option.None<Option<int>>());

        Option<int> result = await selfTask.FlattenAsync();

        result.IsNone.Should().BeTrue();
    }
}
