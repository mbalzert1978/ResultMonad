// <copyright file="UnwrapOrTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrTaskExtension"/> type.
/// </summary>
public sealed class UnwrapOrTaskExtensionTests
{
    private const int TestValue = 42;
    private const int Fallback = -1;

    [Fact]
    public async Task UnwrapOrAsync_WhenTaskSome_ShouldReturnWrappedValue()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        int result = await selfTask.UnwrapOrAsync(Fallback);

        result.Should().Be(TestValue);
    }

    [Fact]
    public async Task UnwrapOrAsync_WhenTaskNone_ShouldReturnFallback()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        int result = await selfTask.UnwrapOrAsync(Fallback);

        result.Should().Be(Fallback);
    }

    [Fact]
    public async Task UnwrapOrAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = null!;

        Func<Task> act = async () => await selfTask.UnwrapOrAsync(Fallback);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
