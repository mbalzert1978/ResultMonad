// <copyright file="UnwrapOrValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrValueTaskExtension"/> type.
/// </summary>
public sealed class UnwrapOrValueTaskExtensionTests
{
    private const int TestValue = 42;
    private const int Fallback = -1;

    [Fact]
    public async Task UnwrapOrAsync_WhenValueTaskSome_ShouldReturnWrappedValue()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        int result = await selfTask.UnwrapOrAsync(Fallback);

        result.Should().Be(TestValue);
    }

    [Fact]
    public async Task UnwrapOrAsync_WhenValueTaskNone_ShouldReturnFallback()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        int result = await selfTask.UnwrapOrAsync(Fallback);

        result.Should().Be(Fallback);
    }
}
